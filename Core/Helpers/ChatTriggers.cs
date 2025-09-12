using System;

namespace ChatPlus.Core.Helpers;

public interface ITrigger
{
    bool ShouldOpen(string text, int caret);
}

public static class ChatTriggers
{
    private sealed class PredicateTrigger : ITrigger
    {
        private readonly Func<string, int, bool> predicate;

        public PredicateTrigger(Func<string, int, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool ShouldOpen(string text, int caret)
        {
            return predicate(text, caret);
        }
    }

    public static ITrigger When(Func<bool> condition)
    {
        return new PredicateTrigger((_, __) => condition());
    }

    public static ITrigger UnclosedTag(string prefix)
    {
        return new PredicateTrigger((text, caret) =>
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int start = text.LastIndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
                return false;

            int close = text.IndexOf(']', start + prefix.Length);
            if (close == -1)
                return true;

            return close > caret;
        });
    }

    public static ITrigger CharOutsideTags(char character)
    {
        return new PredicateTrigger((text, caret) =>
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int searchStart = Math.Min(Math.Max(caret - 1, 0), text.Length - 1);
            int pos = text.LastIndexOf(character, searchStart);
            if (pos < 0)
                return false;

            int lb = text.LastIndexOf('[', pos);
            int rb = text.LastIndexOf(']', pos);
            bool insideTag = lb > rb;

            return !insideTag;
        });
    }

    public static ITrigger AtMentionWord()
    {
        return new PredicateTrigger((text, caret) =>
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int searchStart = Math.Min(Math.Max(caret - 1, 0), text.Length - 1);
            int at = text.LastIndexOf('@', searchStart);
            if (at < 0)
                return false;

            int space = text.IndexOf(' ', at + 1);
            if (space == -1)
                return true;

            return space > caret;
        });
    }

    public static ITrigger CurrentWordStartsWith(char starter)
    {
        return new PredicateTrigger((text, caret) =>
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int lastSpace = text.LastIndexOf(' ', Math.Max(caret - 1, 0));
            int wordStart = lastSpace == -1 ? 0 : lastSpace + 1;
            if (wordStart >= text.Length)
                return false;

            return text[wordStart] == starter;
        });
    }
}
