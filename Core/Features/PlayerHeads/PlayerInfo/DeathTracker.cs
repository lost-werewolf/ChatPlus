using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo;

/// <summary>
/// Tracks the number of deaths for every player in this world
/// /alldeath is pretty good at this
/// </summary>
public static class DeathTracker
{
    /// <summary>
    /// Key: player index, Value: death count
    /// </summary>
    public static Dictionary<int, int> PlayerIndexToDeathCount;
}
