namespace EnhancedMonsters.Utils;

[JsonObject(Description = "The value of a moby once killed, if it can be killed.")]
[method: JsonConstructor]
[Serializable]
public record struct EnemyData(bool Pickupable = true, int MinValue = 80, int MaxValue = 250, float Mass = 50f, string Rank = "E")
{
    /// <summary>
    /// LCMass will convert the Mass inputted to the weird mass shit LC has.<br/>This way, the mass in the mass counter on the top left corner should be the one inputted !
    /// </summary>
    [JsonIgnore]
    public readonly float LCMass => (Mass / 105f) + 1;
}
