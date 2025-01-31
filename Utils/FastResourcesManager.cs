using System;
using System.Collections.Generic;
using System.Text;

namespace EnhancedMonsters.Utils;

internal static class FastResourcesManager
{
    public static AssetBundle Resources;

    public static Sprite EnemyScrapIcon { get; private set; }

    static FastResourcesManager()
    {
        var streamName = Assembly.GetExecutingAssembly().GetManifestResourceNames()[0];
        var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
        Resources = AssetBundle.LoadFromStream(resStream);

        EnemyScrapIcon = Resources.LoadAsset<Sprite>("EnemyScrapItemIcon");
    }
}
