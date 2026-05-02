namespace EnhancedMonsters.Utils;

internal static class FastResourcesManager
{
    public static AssetBundle Resources;

    public static Sprite EnemyScrapIcon { get; private set; }

    public static AudioClip EnemyDropDefaultSound { get; private set; }

    public static string CustomSoundsFolder => Path.Combine(BepInEx.Paths.ConfigPath, "EnhancedMonsters", "CustomSFX");

    public static readonly Dictionary<string, AudioClip> CustomAudioClips = [];

    static FastResourcesManager()
    {
        var streamName = Assembly.GetExecutingAssembly().GetManifestResourceNames()[0];
        var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
        Resources = AssetBundle.LoadFromStream(resStream);

        EnemyScrapIcon = Resources.LoadAsset<Sprite>("EnemyScrapItemIcon");
        EnemyDropDefaultSound = Resources.LoadAsset<AudioClip>("BodyDrop");

        LoadCustomSounds();
    }

    private static void LoadCustomSounds()
    {
        if (!Directory.Exists(CustomSoundsFolder))
        {
            Directory.CreateDirectory(CustomSoundsFolder);
            Plugin.logger.LogInfo($"CustomSFX folder didn't exist, created at {CustomSoundsFolder}");
        }

        var files = Directory.GetFiles(CustomSoundsFolder, "*.wav", SearchOption.TopDirectoryOnly);
        int loaded = 0;

        foreach (var file in files)
        {
            var key = Path.GetFileNameWithoutExtension(file);

            // "default" and "none" are reserved keys understood by EnemiesDataManager.ResolveSfx
            if (key == "default" || key == "none")
            {
                Plugin.logger.LogWarning($"Skipping '{Path.GetFileName(file)}': '{key}' is a reserved SFX key.");
                continue;
            }

            if (CustomAudioClips.ContainsKey(key))
            {
                Plugin.logger.LogWarning($"Skipping '{Path.GetFileName(file)}': key '{key}' already loaded.");
                continue;
            }

            try
            {
                var clip = LoadWav(file, key);
                CustomAudioClips[key] = clip;
                loaded++;
                Plugin.logger.LogInfo($"Loaded custom SFX '{key}' from {Path.GetFileName(file)}");
            }
            catch (Exception e)
            {
                Plugin.logger.LogError($"Failed to load '{Path.GetFileName(file)}': {e.Message}");
            }
        }

        Plugin.logger.LogInfo($"CustomSFX loading complete: {loaded} clip(s) loaded from {CustomSoundsFolder}");
    }

    // Warning: from here onwards it becomes particularly difficult to understand anything

    // Synchronous WAV decoder
    // Supports PCM 16/24/32-bit, IEEE float 32-bit and WAVE_FORMAT_EXTENSIBLE wrappers
    private static AudioClip LoadWav(string filePath, string key)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);

        if (FourCC(reader) != "RIFF")
            throw new InvalidDataException("Not a RIFF file");
        reader.ReadInt32();
        if (FourCC(reader) != "WAVE")
            throw new InvalidDataException("Not a WAVE file");

        short audioFormat = 0;
        short channels = 0;
        int sampleRate = 0;
        short bitsPerSample = 0;
        byte[] dataBytes = null!;

        // Chunk order is not guaranteed by the spec, so scanning continues until "data" is found
        while (stream.Position < stream.Length)
        {
            var chunkId = FourCC(reader);
            var chunkSize = reader.ReadInt32();

            if (chunkId == "fmt ")
            {
                audioFormat = reader.ReadInt16();
                channels = reader.ReadInt16();
                sampleRate = reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt16();
                bitsPerSample = reader.ReadInt16();
                int consumed = 16;

                // EXTENSIBLE (0xFFFE) hides the real format inside a SubFormat GUID
                // Common in multichannel and 24-bit WAVs from Windows tools
                // The first 4 bytes of the GUID identify the inner format: 0x01 = PCM, 0x03 = IEEE_FLOAT
                if ((ushort)audioFormat == 0xFFFE && chunkSize >= 40)
                {
                    reader.ReadInt16();
                    reader.ReadInt16();
                    reader.ReadInt32();
                    var guid = reader.ReadBytes(16);
                    consumed += 24;

                    var subFormat = BitConverter.ToInt32(guid, 0);
                    if (subFormat == 1) audioFormat = 1;
                    else if (subFormat == 3) audioFormat = 3;
                }

                if (chunkSize > consumed)
                    stream.Seek(chunkSize - consumed, SeekOrigin.Current);
            }
            else if (chunkId == "data")
            {
                dataBytes = reader.ReadBytes(chunkSize);
                break;
            }
            else
            {
                stream.Seek(chunkSize, SeekOrigin.Current);
            }
        }

        if (dataBytes == null)
            throw new InvalidDataException("No 'data' chunk found");

        // AudioClip.SetData expects samples normalized to [-1, +1]
        float[] samples;
        if (audioFormat == 1 && bitsPerSample == 16)
        {
            var count = dataBytes.Length / 2;
            samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                short s = (short)(dataBytes[i * 2] | (dataBytes[i * 2 + 1] << 8));
                samples[i] = s / 32768f;
            }
        }
        else if (audioFormat == 1 && bitsPerSample == 24)
        {
            // wav files can also be 24 bit XDD
            // C# has no native int24. Bit 23 must be replicated into bits 24-31
            var count = dataBytes.Length / 3;
            samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                int s = dataBytes[i * 3] | (dataBytes[i * 3 + 1] << 8) | (dataBytes[i * 3 + 2] << 16);
                if ((s & 0x800000) != 0) s |= unchecked((int)0xFF000000);
                samples[i] = s / 8388608f;
            }
        }
        else if (audioFormat == 1 && bitsPerSample == 32)
        {
            var count = dataBytes.Length / 4;
            samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                int s = dataBytes[i * 4] | (dataBytes[i * 4 + 1] << 8) | (dataBytes[i * 4 + 2] << 16) | (dataBytes[i * 4 + 3] << 24);
                samples[i] = s / 2147483648f;
            }
        }
        else if (audioFormat == 3 && bitsPerSample == 32)
        {
            // On-disk byte layout is identical to a managed float[]
            var count = dataBytes.Length / 4;
            samples = new float[count];
            Buffer.BlockCopy(dataBytes, 0, samples, 0, dataBytes.Length);
        }
        else
        {
            throw new InvalidDataException($"Unsupported WAV format (format={audioFormat}, bits={bitsPerSample}). Use 16/24/32-bit PCM or 32-bit IEEE float.");
        }

        // AudioClip.Create takes the per-channel sample count, not the total
        var lengthSamples = samples.Length / channels;
        var clip = AudioClip.Create($"EM_CSFX_{key}", lengthSamples, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static string FourCC(BinaryReader reader) => Encoding.ASCII.GetString(reader.ReadBytes(4));
}
