using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatPlus.Core.Netcode;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.Uploads
{
    // Streams image files between clients so [u:key] draws in multiplayer.
    internal sealed class UploadNetHandler : BasePacketHandler
    {
        public const byte HandlerId = 1;
        public static UploadNetHandler Instance { get; } = new UploadNetHandler();

        private const int ChunkBytes = 60 * 1024;
        private const int MaxBytes = 2 * 1024 * 1024;

        private enum Msg : byte { Request = 1, Forward = 2, Chunk = 3 }

        private static readonly Dictionary<string, MemoryStream> Incoming = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, int> Expected = new(StringComparer.OrdinalIgnoreCase);

        private UploadNetHandler() : base(HandlerId) { }

        public static void Request(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            if (Main.netMode != NetmodeID.MultiplayerClient) return;

            var p = Instance.GetPacket((byte)Msg.Request);
            p.Write(key);
            p.Send();
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            var msg = (Msg)reader.ReadByte();

            if (Main.netMode == NetmodeID.Server)
            {
                switch (msg)
                {
                    case Msg.Request:
                        {
                            string key = reader.ReadString();
                            // Ask every other client to serve this key, if they can.
                            var p = GetPacket((byte)Msg.Forward);
                            p.Write(key);
                            p.Write(fromWho);
                            p.Send(ignoreClient: fromWho);
                            break;
                        }
                    case Msg.Chunk:
                        {
                            string key = reader.ReadString();
                            int total = reader.ReadInt32();
                            int index = reader.ReadInt32();
                            int count = reader.ReadInt32();
                            int target = reader.ReadInt32();
                            int len = reader.ReadInt32();
                            byte[] data = reader.ReadBytes(len);

                            // Forward chunk to target only.
                            var p = GetPacket((byte)Msg.Chunk);
                            p.Write(key);
                            p.Write(total);
                            p.Write(index);
                            p.Write(count);
                            p.Write(target);
                            p.Write(len);
                            p.Write(data);
                            p.Send(target);
                            break;
                        }
                }
                return;
            }

            // Client side
            switch (msg)
            {
                case Msg.Forward:
                    {
                        string key = reader.ReadString();
                        int requester = reader.ReadInt32();
                        TryServeKeyTo(key, requester);
                        break;
                    }
                case Msg.Chunk:
                    {
                        string key = reader.ReadString();
                        int total = reader.ReadInt32();
                        int index = reader.ReadInt32();
                        int count = reader.ReadInt32();
                        int target = reader.ReadInt32(); // already routed
                        int len = reader.ReadInt32();
                        byte[] payload = reader.ReadBytes(len);

                        if (total <= 0 || total > MaxBytes) return;

                        if (!Incoming.TryGetValue(key, out var ms))
                        {
                            ms = new MemoryStream(total);
                            Incoming[key] = ms;
                            Expected[key] = total;
                        }

                        ms.Write(payload, 0, payload.Length);

                        // When complete, build texture on main thread and register it
                        if (ms.Length >= Expected[key])
                        {
                            byte[] bytes = ms.ToArray();
                            Incoming.Remove(key);
                            Expected.Remove(key);
                            ms.Dispose();

                            Main.QueueMainThreadAction(() =>
                            {
                                try
                                {
                                    using var s = new MemoryStream(bytes, writable: false);
                                    Texture2D tex = Texture2D.FromStream(Main.instance.GraphicsDevice, s);
                                    UploadTagHandler.Register(key, tex);
                                }
                                catch { /* ignore bad data */ }
                            });
                        }
                        break;
                    }
            }
        }

        private static void TryServeKeyTo(string key, int requester)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            if (!TryGetLocalBytes(key, out var bytes)) return;
            if (bytes.Length == 0 || bytes.Length > MaxBytes) return;

            int chunkCount = (bytes.Length + ChunkBytes - 1) / ChunkBytes;

            for (int i = 0; i < chunkCount; i++)
            {
                int offset = i * ChunkBytes;
                int size = Math.Min(ChunkBytes, bytes.Length - offset);

                var p = Instance.GetPacket((byte)Msg.Chunk);
                p.Write(key);
                p.Write(bytes.Length);
                p.Write(i);
                p.Write(chunkCount);
                p.Write(requester);
                p.Write(size);
                p.Write(bytes, offset, size);
                p.Send();
            }
        }

        private static bool TryGetLocalBytes(string key, out byte[] bytes)
        {
            bytes = null;

            Upload upload = UploadInitializer.Uploads
                .FirstOrDefault(u =>
                    string.Equals(Path.GetFileNameWithoutExtension(u.FileName), key, StringComparison.OrdinalIgnoreCase));

            if (!File.Exists(upload.FullFilePath)) return false;

            try
            {
                bytes = File.ReadAllBytes(upload.FullFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
