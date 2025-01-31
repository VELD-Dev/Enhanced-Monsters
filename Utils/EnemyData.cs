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
        public EnemyMetadata(Vec3 meshOffset = new(), Vec3 meshRotation = new(), bool animateOnDeath = true, bool twoHanded = true, Dictionary<string, float>? loots = null)
        {
            MeshOffset = meshOffset;
            MeshRotation = meshRotation;
            AnimateOnDeath = animateOnDeath;
            TwoHanded = twoHanded;
            LootTable = loots ?? [];
        }

        public Vec3 MeshOffset;
        public Vec3 MeshRotation;
        public bool AnimateOnDeath;
        public bool TwoHanded;
        public Dictionary<string, float> LootTable;
    }

    /// <summary>
    /// LCMass will convert the Mass inputted to the weird mass shit LC has.<br/>This way, the mass in the mass counter on the top left corner should be the one inputted !
    /// </summary>
    [JsonIgnore]
    public readonly float LCMass => (Mass / 105f) + 1;
}

[Serializable]
[JsonObject(Description = "A simple and flexible Vector3 structure.")]
public record struct Vec3
{
    public float X, Y, Z = 0;

    [JsonConstructor]
    public Vec3(float x, float y, float z) { X = x; Y = y; Z = z; }

    public static implicit operator System.Numerics.Vector3(Vec3 v) => new(v.X, v.Y, v.Z);
    public static implicit operator UnityEngine.Vector3(Vec3 v) => new(v.X, v.Y, v.Z);
    public static implicit operator Vec3(System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);
    public static implicit operator Vec3(UnityEngine.Vector3 v) => new(v.x, v.y, v.z);

    public static Vec3 operator *(Vec3 l, float r) => new(l.X * r, l.Y * r, l.Z * r);
    public static Vec3 operator *(Vec3 l, Vec3 r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
    public static Vec3 operator /(Vec3 l, float r) => new(l.X / r, l.Y / r, l.Z / r);
    public static Vec3 operator /(Vec3 l, Vec3 r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z);
    public static Vec3 operator +(Vec3 l, Vec3 r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
    public static Vec3 operator -(Vec3 l, Vec3 r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
    public static Vec3 operator -(Vec3 s) => new(-s.X, -s.Y, -s.Z);
    public static float operator ~(Vec3 l) => l.Length();

    public readonly float Length() => (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

    public readonly void Deconstruct(out float x, out float y, out float z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public readonly void Deconstruct(out float x, out float y)
    {
        x = X;
        y = Y;
    }
}