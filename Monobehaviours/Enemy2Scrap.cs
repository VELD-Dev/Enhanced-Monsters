using EnhancedMonsters.Utils;
using Vector3 = UnityEngine.Vector3;

namespace EnhancedMonsters.Monobehaviours;

public class EnemyScrap : GrabbableObject
{
    public Animator EnemyAnimator { get; private set; }
    public GameObject ScanNode { get; private set; }
    public EnemyType enemyType;

    private readonly NetworkVariable<int> _syncedScrapValue = new();
    private readonly NetworkVariable<Vector3> _syncedPosition = new();
    public int SyncedScrapValue { get => _syncedScrapValue.Value; set { _syncedScrapValue.Value = value; } }
    public Vector3 SyncedPosition { get => _syncedPosition.Value; set { _syncedPosition.Value = value; } }

    private void Awake()
    {
        EnemyAnimator = gameObject.GetComponentInChildren<Animator>();
        if (EnemyAnimator == null)
        {
            Plugin.logger.LogWarning($"So somehow, the monster {gameObject.name} does not have any Animator. Will work anyway but might have its animations glitched");
        }

        ScanNode = gameObject.transform.Find("ScanNode").gameObject;
        if (ScanNode == null)
        {
            Plugin.logger.LogWarning($"No ScanNode found in {gameObject.name}. That's a weird enemy...");
        }
    }

    public override void Start()
    {
        base.Start();

        Plugin.logger.LogInfo($"Mob scrap {enemyType.enemyName} has spawned !");
        if(EnemyAnimator != null)
        {
            try
            {
                EnemyAnimator.SetBool("Stunned", false);
                EnemyAnimator.SetBool("stunned", false);
                EnemyAnimator.SetBool("stun", false);
                EnemyAnimator.SetTrigger("KillEnemy");
                EnemyAnimator.SetBool("Dead", true);
                StartCoroutine(DisableAnimatorOnAnimationEnd());
            }
            catch (Exception e)
            {
                Plugin.logger.LogError($"Failed to set the enemy {gameObject.name} mob mesh animator to dead state. Maybe the animator doesn't have a dead state ? Error: {e}");
                EnemyAnimator.enabled = false;
            }
        }

        grabbable = SyncedConfig.Instance.EnemiesData[enemyType.enemyName].Pickupable;
        
        if(ScanNode)
        {
            var enemyData = LocalConfig.Singleton.synchronizeRanks.Value ? SyncedConfig.Instance.EnemiesData[enemyType.enemyName] : SyncedConfig.Default.EnemiesData[enemyType.enemyName];
            var scanNode = ScanNode.GetComponent<ScanNodeProperties>();
            scanNode.scrapValue = scrapValue;
            scanNode.subText = $"Rank:{enemyData.Rank}\nValue: ${scanNode.scrapValue}";
        }
    }

    public IEnumerator DisableAnimatorOnAnimationEnd()
    {
        var currentClip = EnemyAnimator.GetCurrentAnimatorClipInfo(0);
        if (currentClip.Length < 1) 
            yield break;

        var deathAnimDuration = currentClip[0].clip.length;
        var deathAnimProgress = EnemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime; // Happily this is a property

        while (deathAnimProgress < deathAnimDuration)
            yield return new WaitForEndOfFrame();

        EnemyAnimator.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _syncedScrapValue.OnValueChanged += (oldVal, newVal) => scrapValue = newVal;
        _syncedPosition.OnValueChanged += (oldVal, newVal) => transform.position = newVal;

        if(IsServer)
        {
            Plugin.logger.LogInfo("Synchronizing the mob data and scrap values and positions with clients...");
            var enemyData = SyncedConfig.Instance.EnemiesData[enemyType.enemyName];
            int mobValue = new System.Random().Next(enemyData.MinValue, enemyData.MaxValue);

            SyncedScrapValue = mobValue;
        }
    }
}
