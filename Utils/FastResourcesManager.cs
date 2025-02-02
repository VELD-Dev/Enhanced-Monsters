using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EnhancedMonsters.Utils;

internal static class FastResourcesManager
{
    public static AssetBundle Resources;

    public static Sprite EnemyScrapIcon { get; private set; }

    public static AudioClip EnemyDropDefaultSound { get; private set; }

    public static string CustomSoundsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "configs", "EnhancedMonsters", "CustomSFX");

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
        if(!Directory.Exists(CustomSoundsFolder))
        {
            Directory.CreateDirectory(CustomSoundsFolder);
            Plugin.logger.LogInfo("CustomSFX folder didn't exist, created one.");
        }

        var files = Directory.GetFiles(CustomSoundsFolder, "*.ogg", SearchOption.TopDirectoryOnly);
        
        foreach(var file in files)
        {
            var soundName = Path.GetFileNameWithoutExtension(file);
            if (CustomAudioClips.ContainsKey(soundName))
                soundName += CustomAudioClips.Keys.Where(n => n == soundName).Count().ToString();

            var clip = AudioClip.Create($"EM_CSFX_{soundName}", 2 * 44100, 1, 44100, false);

            var stream = new FileStream(file, FileMode.Open);
            float[] data = new float[(int)(stream.Length / sizeof(float))];
            for(int i = 0; i < stream.Length; i+=sizeof(float))
            {
                Span<byte> span = stackalloc byte[4];
                stream.Read(span);
                data[i/sizeof(float)] = BitConverter.ToSingle(span);
            }
            stream.Dispose();
            clip.SetData([..data], 0);

            CustomAudioClips.Add(soundName, clip);
        }
    }
}
