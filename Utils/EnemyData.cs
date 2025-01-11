namespace EnhancedMonsters.Utils;

[JsonObject(Description = "The value of a moby once killed, if it can be killed.")]
[method: JsonConstructor]
[Serializable]
public record struct EnemyData(bool Pickupable = true, int MinValue = 80, int MaxValue = 250, int Mass = 50, string Rank = "E");
