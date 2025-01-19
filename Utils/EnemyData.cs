using Vector3 = System.Numerics.Vector3;

namespace EnhancedMonsters.Utils;

[JsonObject(Description = "The value of a mob once killed, if it can be killed.")]
[method: JsonConstructor]
[Serializable]
public record struct EnemyData(bool Pickupable = true, int MinValue = 80, int MaxValue = 250, float Mass = 50f, string Rank = "E", EnemyData.EnemyMetadata Metadata = new())
{
    [JsonObject(Description = "The metadata of a mob once killed, if it can be killed.")]
    [Serializable]
    public record struct EnemyMetadata
    {
        [JsonConstructor]
        public EnemyMetadata(Vector3 meshOffset = new(), Vector3 meshRotation = new(), bool animateOnDeath = true, Dictionary<string, float>? loots = null)
        {
            MeshOffset = meshOffset;
            MeshRotation = meshRotation;
            AnimateOnDeath = animateOnDeath;
            LootTable = loots ?? [];
        }

        public Vector3 MeshOffset;
        public Vector3 MeshRotation;
        public bool AnimateOnDeath;
        public Dictionary<string, float> LootTable;
    }

    /// <summary>
    /// LCMass will convert the Mass inputted to the weird mass shit LC has.<br/>This way, the mass in the mass counter on the top left corner should be the one inputted !
    /// </summary>
    [JsonIgnore]
    public readonly float LCMass => (Mass / 105f) + 1;
}
