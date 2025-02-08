using EnhancedMonsters.Utils;
using Vector3 = UnityEngine.Vector3;

namespace EnhancedMonsters.Monobehaviours;

public class EnemyScrap : GrabbableObject
{
    public Animator EnemyAnimator { get; private set; }
    public ScanNodeProperties ScanNode { get; private set; }
    public GameObject EnemyGameObject { get; set; }
    public EnemyType enemyType;
    public EnemyData EnemyData { get => LocalConfig.Singleton.synchronizeRanks.Value ? SyncedConfig.Instance.EnemiesData[enemyType.enemyName] : SyncedConfig.Default.EnemiesData[enemyType.enemyName]; }

    private readonly NetworkVariable<int> _syncedScrapValue = new();
    public int SyncedScrapValue { get => _syncedScrapValue.Value; set { _syncedScrapValue.Value = value; } }

    private void Awake()
    {
        EnemyAnimator = gameObject.GetComponentInChildren<Animator>();
        if (EnemyAnimator == null)
        {
            Plugin.logger.LogWarning($"So somehow, the monster {gameObject.name} does not have any Animator. Will work anyway but might have its animations glitched");
        }

        ScanNode = gameObject.GetComponentInChildren<ScanNodeProperties>();
        if (ScanNode == null)
        {
            Plugin.logger.LogWarning($"No ScanNode found in {gameObject.name}. That's a weird enemy...");
        }

        if(!itemProperties.saveItemVariable)
        {
            Plugin.logger.LogError($"Error: Enemy corpse saving was set to false for {enemyType.enemyName}");
        }
    }

    public override void Start()
    {
        base.Start();

        if (IsServer)
        {
            Plugin.logger.LogInfo("Synchronizing the mob data and scrap values and positions with clients...");
            var enemyData = SyncedConfig.Instance.EnemiesData[enemyType.enemyName];
            if(EnemyGameObject is not null)
            {
                EnemyGameObject.transform.localEulerAngles = enemyData.Metadata.FloorRotation * ((float)Math.PI / 180f);
                EnemyGameObject.transform.localPosition = enemyData.Metadata.MeshOffset;
            }
            else
            {
                Plugin.logger.LogWarning("whoops ! EnemyGameObject is not defined somehow !");
            }

            if(GameNetworkManager.Instance.gameHasStarted || scrapValue == 0)
            {
                // Save is already defining the scrap values here.
                int mobValue = new System.Random().Next(enemyData.MinValue, enemyData.MaxValue);
                SyncedScrapValue = mobValue;
            }
            else
            {
                // So we just need to make it synchronized
                SyncedScrapValue = scrapValue;
            }
        }

        Plugin.logger.LogInfo($"Mob scrap {enemyType.enemyName} has spawned !");
        if(EnemyAnimator != null)
        {
            if(!EnemyData.Metadata.AnimateOnDeath)
            {
                EnemyAnimator.enabled = false;
            }
            try
            {
                EnemyAnimator.SetLayerWeight(2, 0f);
            }
            catch(Exception e)
            {
                Plugin.logger.LogWarning($"Failed to set layer 2's weight to 0 (bracken fix?). Error: {e}");
                EnemyAnimator.enabled = false;
            }
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

            // I hate doing that but I have no choice here
            if(enemyType.enemyName == "Flowerman")
            {
                EnemyAnimator.gameObject.SetActive(false);
                EnemyAnimator.gameObject.SetActive(true);
                EnemyAnimator.enabled = false;
            }
        }
        
        if(ScanNode)
        {
            var enemyData = LocalConfig.Singleton.synchronizeRanks.Value ? SyncedConfig.Instance.EnemiesData[enemyType.enemyName] : SyncedConfig.Default.EnemiesData[enemyType.enemyName];
            var scanNode = ScanNode.GetComponent<ScanNodeProperties>();
            scanNode.scrapValue = SyncedScrapValue;
            scanNode.subText = $"Rank:{enemyData.Rank}\nValue: ${SyncedScrapValue}";
        }
        else
        {
            Plugin.logger.LogError($"Enemy corpse {itemProperties.itemName} is missing a ScanNode !");
        }
    }

    public new void SetScrapValue(int value)
    {
        Plugin.logger.LogInfo($"Setting scrap value for {itemProperties.itemName}: {value}");
        scrapValue = value;
        ScanNode.scrapValue = value;
        var enemyData = LocalConfig.Singleton.synchronizeRanks.Value ? SyncedConfig.Instance.EnemiesData[enemyType.enemyName] : SyncedConfig.Default.EnemiesData[enemyType.enemyName];
        ScanNode.subText = $"Rank: {enemyData.Rank}\nValue: {value}";
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
        _syncedScrapValue.Initialize(this);
        _syncedScrapValue.OnValueChanged += (oldVal, newVal) => SetScrapValue(newVal);
    }

    public override int GetItemDataToSave()
    {
        Plugin.logger.LogInfo($"GetItemData: Trying to save enemy scrap for {itemProperties.itemName}.");
        return 0;
    }
}
