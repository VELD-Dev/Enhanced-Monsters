using System;
using System.Collections.Generic;
using System.Text;

namespace LootableMonsters;

[JsonObject(Description = "The value of a moby once killed, if it can be killed.")]
[method: JsonConstructor]
public struct EnemyValue(bool canBePicked = true, int value = 100)
{
    public bool Pickupable = canBePicked;
    public int CreditsValue = value;
}
