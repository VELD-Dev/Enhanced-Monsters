using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Utils;

[JsonObject(Description = "The value of a moby once killed, if it can be killed.")]
[method: JsonConstructor]
public struct EnemyData(bool canBePicked = true, int minval = 80, int maxval = 250, int mass = 50, string rank = "E")
{
    public bool Pickupable = canBePicked;
    public int MinValue = minval;
    public int MaxValue = maxval;
    public int Mass = mass;
    public string Rank = rank;
}
