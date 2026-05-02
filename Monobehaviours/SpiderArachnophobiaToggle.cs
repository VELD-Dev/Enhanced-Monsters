namespace EnhancedMonsters.Monobehaviours;

/// <summary>
/// Companion <see cref="MonoBehaviour"/> attached only to Bunker Spider scrap prefabs.
/// Keeps the two mesh variants (normal vs arachnophobia-safe) in sync with the player's
/// <c>IngamePlayerSettings.spiderSafeMode</c> setting.
///
/// We re-apply the renderer state every <see cref="LateUpdate"/> rather than caching it,
/// because LC's GrabbableObject pipeline (GrabItem / PlayDropSFX / equipping the held item)
/// internally re-enables renderers as part of its state transitions. Without re-applying,
/// the wrong mesh would briefly become visible when the corpse is picked up.
/// </summary>
public class SpiderArachnophobiaToggle : MonoBehaviour
{
    private MeshRenderer _safeMesh;
    private SkinnedMeshRenderer _normalMesh;
    private bool _renderersResolved;

    private void Start()
    {
        ResolveRenderers();
        ApplyArachnophobeMode(IngamePlayerSettings.Instance.unsavedSettings.spiderSafeMode);
    }

    private void LateUpdate()
    {
        ApplyArachnophobeMode(IngamePlayerSettings.Instance.unsavedSettings.spiderSafeMode);
    }

    public void ApplyArachnophobeMode(bool arachnophobeEnabled)
    {
        if (!_renderersResolved)
            ResolveRenderers();

        if (_safeMesh == null || _normalMesh == null)
            return;

        if (_safeMesh.enabled != arachnophobeEnabled)
            _safeMesh.enabled = arachnophobeEnabled;
        if (_normalMesh.enabled != !arachnophobeEnabled)
            _normalMesh.enabled = !arachnophobeEnabled;
    }

    private void ResolveRenderers()
    {
        if (transform.childCount == 0) return;

        var root = transform.GetChild(0);
        var safeTransform = root.Find("MeshContainer")?.Find("AnimContainer")?.Find("Armature")?.Find("Abdomen")?.Find("SpiderText");
        var normalTransform = root.Find("MeshContainer")?.Find("MeshRenderer");

        _safeMesh = safeTransform != null ? safeTransform.GetComponent<MeshRenderer>() : null;
        _normalMesh = normalTransform != null ? normalTransform.GetComponent<SkinnedMeshRenderer>() : null;

        if (_safeMesh == null || _normalMesh == null)
        {
            Plugin.logger.LogError("This Spider mesh doesn't have a normal mesh or an arachnophobe mesh.");
            return;
        }

        _renderersResolved = true;
    }
}
