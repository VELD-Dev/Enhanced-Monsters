namespace EnhancedMonsters.Utils;

[JsonObject(Description = "The value of a mob once killed, if it can be killed.")]
[Serializable]
public record struct EnemyData
{
    [JsonConstructor]
    public EnemyData()
    {
        Pickupable = true;
        MinValue = 80;
        MaxValue = 110;
        Mass = 50f;
        Rank = "C";
        Metadata = new();
    }

    public EnemyData(bool pickupable = true, int minValue = 80, int maxValue = 250, float mass = 50f, string rank = "E", EnemyData.EnemyMetadata? metadata = null)
    {
        Pickupable = pickupable;
        MinValue = minValue;
        MaxValue = maxValue;
        Mass = mass;
        Rank = rank;
        Metadata = metadata ?? new();
    }

    public bool Pickupable;
    public int MinValue;
    public int MaxValue;
    public float Mass;
    public string Rank;
    public EnemyMetadata Metadata;

    /// <summary>
    /// LCMass will convert the Mass inputted to the weird mass shit LC has.<br/>This way, the mass in the mass counter on the top left corner should be the one inputted !
    /// </summary>
    [JsonIgnore]
    public readonly float LCMass => (Mass / 105f) + 1;

    [JsonObject(Description = "The metadata of a mob once killed, if it can be killed.")]
    [Serializable]
    public record struct EnemyMetadata
    {
        [JsonConstructor]
        public EnemyMetadata()
        {
            MeshOffset = new();
            HandRotation = new();
            FloorRotation = new();
            CollisionExtents = new(1.5f, 1.5f, 1.5f);
            AnimateOnDeath = false;
            TwoHanded = true;
            DropSFX = "default";
            GrabSFX = "none";
            PocketSFX = "none";
            LootTable = [];
        }

        public EnemyMetadata(Vec3 meshOffset = new(), Vec3 handRotation = new(), Vec3 floorRotation = new(), bool animateOnDeath = true, bool twoHanded = true, Dictionary<string, float>? loots = null, string dropsfx = "default", string grabsfx = "none", string pocketsfx = "none", Vec3? collisionSize = null)
        {
            MeshOffset = meshOffset;
            HandRotation = handRotation;
            FloorRotation = floorRotation;
            CollisionExtents = collisionSize ?? new(1.5f, 1.5f, 1.5f);
            AnimateOnDeath = animateOnDeath;
            TwoHanded = twoHanded;
            DropSFX = dropsfx;
            GrabSFX = grabsfx;
            PocketSFX = pocketsfx;
            LootTable = loots ?? [];
        }

        public Vec3 MeshOffset;
        public Vec3 HandRotation;
        public Vec3 FloorRotation;
        public Vec3 CollisionExtents;
        public bool AnimateOnDeath;
        public bool TwoHanded;
        public string DropSFX = "default";
        public string GrabSFX = "none";
        public string PocketSFX = "none";
        public Dictionary<string, float> LootTable;
    }
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