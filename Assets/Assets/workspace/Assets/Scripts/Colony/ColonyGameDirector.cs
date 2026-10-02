using System.Collections.Generic;
using UnityEngine;

public class ColonyGameDirector : MonoBehaviour
{
    public enum Flow
    {
        Loading,
        Menu,
        Playing,
        Paused,
        Won,
        Lost
    }

    public static ColonyGameDirector Instance { get; private set; }

    public PlayerController Player;
    public ColonyHealth PlayerHealth;
    public ColonyAnthill Anthill;
    public ColonyNest Nest;
    public GameObject soldierPrefab;
    public GameObject collectorPrefab;
    public ColonyAcidGlob globPrefab;
    public Transform soldierAnchor;
    public int food;
    public int foodMax = 12;
    public int summonCost = 3;
    public int collectorCost = 2;
    public int maxSoldiers = 4;
    public int maxCollectors = 3;
    public int score;
    public int foodCollected;
    public int enemiesKilled;
    public Flow flow = Flow.Loading;

    readonly List<ColonySoldierAI> soldiers = new List<ColonySoldierAI>();
    readonly List<ColonyCollectorAI> collectors = new List<ColonyCollectorAI>();
    readonly List<ColonyRaiderAI> raiders = new List<ColonyRaiderAI>();
    readonly List<ColonyHealth> enemies = new List<ColonyHealth>();

    public bool IsPlaying => flow == Flow.Playing;
    public int AliveRaiders
    {
        get
        {
            int n = 0;
            for (int i = 0; i < raiders.Count; i++)
            {
                if (raiders[i] != null && raiders[i].gameObject.activeInHierarchy)
                {
                    n++;
                }
            }

            return n;
        }
    }

    public int SoldierCount
    {
        get
        {
            int n = 0;
            for (int i = 0; i < soldiers.Count; i++)
            {
                if (soldiers[i] != null && soldiers[i].gameObject.activeInHierarchy)
                {
                    n++;
                }
            }

            return n;
        }
    }

    public int CollectorCount
    {
        get
        {
            int n = 0;
            for (int i = 0; i < collectors.Count; i++)
            {
                if (collectors[i] != null && collectors[i].gameObject.activeInHierarchy)
                {
                    n++;
                }
            }

            return n;
        }
    }

    Vector3 playerSpawn;
    Quaternion playerRotation;
    float respawnLock;

    void Awake()
    {
        Instance = this;
        CacheSpawn();
        if (globPrefab != null)
        {
            ColonyAcidGun.BindPrefab(globPrefab);
        }
    }

    void CacheSpawn()
    {
        if (Nest != null)
        {
            playerSpawn = Nest.RespawnPosition;
            playerRotation = Nest.RespawnRotation;
        }
        else if (Player != null)
        {
            playerSpawn = Player.transform.position;
            playerRotation = Quaternion.Euler(0f, Player.transform.eulerAngles.y, 0f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (flow == Flow.Playing) PauseGame();
            else if (flow == Flow.Paused) ResumeGame();
            return;
        }

        if (flow == Flow.Playing && Input.GetKeyDown(KeyCode.Q))
        {
            TrySummonSoldier();
        }

        if (flow == Flow.Playing && Input.GetKeyDown(KeyCode.C))
        {
            TrySummonCollector();
        }

        if (flow == Flow.Playing)
        {
            if (Nest != null && Nest.Health != null && Nest.Health.IsDead)
            {
                Lose();
            }
            else if (PlayerHealth != null && PlayerHealth.IsDead)
            {
                TryRespawnPlayer();
            }
            else if (Anthill != null)
            {
                var hp = Anthill.GetComponent<ColonyHealth>();
                if (hp != null && hp.IsDead)
                {
                    Win();
                }
            }
        }
    }

    public void BeginPlay()
    {
        Time.timeScale = 1f;
        flow = Flow.Playing;
        food = 0;
        score = 0;
        foodCollected = 0;
        enemiesKilled = 0;
        respawnLock = 0f;
        CacheSpawn();
        ClearUnits();
        foreach (var glob in FindObjectsByType<ColonyAcidGlob>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Destroy(glob.gameObject);
        Player.ResetMotor(playerSpawn, playerRotation);
        if (PlayerHealth != null)
        {
            PlayerHealth.ResetHealth();
        }

        Nest?.ResetNest();

        if (Anthill != null)
        {
            var hp = Anthill.GetComponent<ColonyHealth>();
            hp?.ResetHealth();
            Anthill.ResetSpawner();
        }

        foreach (var pickup in FindObjectsByType<ColonyFoodPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            pickup.ResetPickup();
        }

        if (Player != null)
        {
            Player.InputEnabled = true;
        }

        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.GameStarted);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.FoodChanged, food, foodMax);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.SoldierCountChanged, SoldierCount, maxSoldiers);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.CollectorCountChanged, CollectorCount, maxCollectors);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.ScoreChanged, score);
        if (Nest != null && Nest.Health != null)
        {
            EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.NestHealthChanged, Nest.Health.current, Nest.Health.maxHealth);
        }

        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, "Hold Alt to command the nest. Spawn collectors and soldiers, then climb the walls.");
    }

    public void PauseGame()
    {
        if (flow != Flow.Playing)
        {
            return;
        }

        flow = Flow.Paused;
        Time.timeScale = 0f;
        if (Player != null)
        {
            Player.InputEnabled = false;
        }

        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.GamePaused);
    }

    public void ResumeGame()
    {
        if (flow != Flow.Paused)
        {
            return;
        }

        flow = Flow.Playing;
        Time.timeScale = 1f;
        if (Player != null)
        {
            Player.InputEnabled = true;
        }

        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.GameResumed);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        flow = Flow.Menu;
        if (Player != null)
        {
            Player.InputEnabled = false;
        }

        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.MenuShown);
    }

    public void AddFood(int amount)
    {
        food = Mathf.Clamp(food + amount, 0, foodMax);
        foodCollected += amount;
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.FoodChanged, food, foodMax);
    }

    public bool TrySummon() => TrySummonSoldier();

    public bool TrySummonSoldier()
    {
        if (!IsPlaying || soldierPrefab == null) return false;
        if (food < summonCost || SoldierCount >= maxSoldiers)
        {
            EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, food < summonCost ? "Need 3 food for a soldier." : "Squad full: four soldiers are already fighting.");
            return false;
        }

        food -= summonCost;
        Vector3 pos = SpawnNearNest(Player != null ? Player.transform.right * 0.8f + Player.transform.forward * 0.4f : Vector3.right);
        var go = Instantiate(soldierPrefab, pos, Quaternion.identity);
        go.SetActive(true);
        var ai = go.GetComponent<ColonySoldierAI>();
        if (ai != null)
        {
            ai.followTarget = Player != null ? Player.transform : null;
            soldiers.Add(ai);
        }

        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.FoodChanged, food, foodMax);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.SoldierCountChanged, SoldierCount, maxSoldiers);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, "Soldier spawned. Green ants attack the nearest enemy and the hill.");
        return true;
    }

    public bool TrySummonCollector()
    {
        if (!IsPlaying || collectorPrefab == null) return false;
        if (food < collectorCost || CollectorCount >= maxCollectors)
        {
            EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, food < collectorCost ? "Need 2 food for a collector." : "Foragers full: three collectors are already gathering.");
            return false;
        }

        food -= collectorCost;
        Vector3 pos = SpawnNearNest(Player != null ? -Player.transform.right * 0.8f + Player.transform.forward * 0.3f : -Vector3.right);
        var go = Instantiate(collectorPrefab, pos, Quaternion.identity);
        go.SetActive(true);
        var ai = go.GetComponent<ColonyCollectorAI>();
        if (ai != null)
        {
            collectors.Add(ai);
        }

        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.FoodChanged, food, foodMax);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.CollectorCountChanged, CollectorCount, maxCollectors);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, "Collector spawned. Gold ants haul crumbs back to the nest.");
        return true;
    }

    Vector3 SpawnNearNest(Vector3 offset)
    {
        Vector3 pos = Nest != null ? Nest.RespawnPosition + offset : (Player != null ? Player.transform.position + offset : transform.position);
        if (UnityEngine.AI.NavMesh.SamplePosition(pos, out var spawnHit, 2.4f, UnityEngine.AI.NavMesh.AllAreas)) pos = spawnHit.position;
        pos.y += 0.1f;
        return pos;
    }

    public bool TryRespawnPlayer()
    {
        if (!IsPlaying || Player == null || PlayerHealth == null)
        {
            return false;
        }

        if (Nest == null || Nest.Health == null || Nest.Health.IsDead)
        {
            Lose();
            return false;
        }

        if (Time.time < respawnLock)
        {
            return false;
        }

        CacheSpawn();
        PlayerHealth.ResetHealth();
        PlayerHealth.invulnerable = true;
        Player.ResetMotor(playerSpawn, playerRotation);
        Player.InputEnabled = true;
        respawnLock = Time.time + 1.4f;
        CancelInvoke(nameof(ClearRespawnGuard));
        Invoke(nameof(ClearRespawnGuard), 1.6f);
        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.PlayerRespawned, playerSpawn);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, "Respawned at the nest. Defend it!");
        return true;
    }

    void ClearRespawnGuard()
    {
        if (PlayerHealth != null)
        {
            PlayerHealth.invulnerable = false;
        }
    }

    public void RegisterRaider(ColonyRaiderAI raider)
    {
        if (raider == null)
        {
            return;
        }

        raiders.Add(raider);
        var hp = raider.GetComponent<ColonyHealth>();
        if (hp != null)
        {
            enemies.Add(hp);
        }
    }

    public void NotifyRaiderDead(ColonyRaiderAI raider)
    {
        enemiesKilled++;
        score = foodCollected + enemiesKilled * 10;
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.ScoreChanged, score);
    }

    public ColonyHealth FindNearestEnemy(Vector3 from, float range)
    {
        ColonyHealth best = null;
        float bestD = range * range;
        if (Anthill != null)
        {
            var hillHp = Anthill.GetComponent<ColonyHealth>();
            if (hillHp != null && !hillHp.IsDead)
            {
                float d = (Anthill.transform.position - from).sqrMagnitude;
                if (d < bestD)
                {
                    bestD = d;
                    best = hillHp;
                }
            }
        }

        for (int i = 0; i < raiders.Count; i++)
        {
            var r = raiders[i];
            if (r == null || !r.gameObject.activeInHierarchy)
            {
                continue;
            }

            var hp = r.GetComponent<ColonyHealth>();
            if (hp == null || hp.IsDead)
            {
                continue;
            }

            float d = (r.transform.position - from).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = hp;
            }
        }

        return best;
    }

    public Transform FindRaiderTarget(Vector3 from)
    {
        Transform nest = Nest != null && Nest.Health != null && !Nest.Health.IsDead ? Nest.transform : null;
        Transform unit = null;
        float unitD = float.MaxValue;
        if (Player != null && PlayerHealth != null && !PlayerHealth.IsDead)
        {
            unit = Player.transform;
            unitD = (unit.position - from).sqrMagnitude;
        }

        for (int i = 0; i < soldiers.Count; i++)
        {
            var s = soldiers[i];
            if (s == null || !s.gameObject.activeInHierarchy)
            {
                continue;
            }

            var hp = s.GetComponent<ColonyHealth>();
            if (hp != null && hp.IsDead)
            {
                continue;
            }

            float d = (s.transform.position - from).sqrMagnitude;
            if (d + 4f < unitD)
            {
                unitD = d;
                unit = s.transform;
            }
        }

        if (unit != null && unitD < 64f)
        {
            return unit;
        }

        return nest != null ? nest : unit;
    }

    void Win()
    {
        flow = Flow.Won;
        if (Player != null)
        {
            Player.InputEnabled = false;
        }

        int remainHp = PlayerHealth != null ? Mathf.RoundToInt(PlayerHealth.current) : 0;
        int nestHp = Nest != null && Nest.Health != null ? Mathf.RoundToInt(Nest.Health.current) : 0;
        score = foodCollected + enemiesKilled * 10 + remainHp + nestHp + SoldierCount * 25 + CollectorCount * 15;
        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.ScoreChanged, score);
        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.GameWon, score);
    }

    void Lose()
    {
        flow = Flow.Lost;
        if (Player != null)
        {
            Player.InputEnabled = false;
        }

        score = foodCollected + enemiesKilled * 10;
        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.ScoreChanged, score);
        EventDispatcher.SendEventImmediate(GameEventTypes.ColonyEventType.GameLost, score);
    }

    void ClearUnits()
    {
        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i] != null)
            {
                Destroy(soldiers[i].gameObject);
            }
        }

        soldiers.Clear();
        for (int i = 0; i < collectors.Count; i++)
        {
            if (collectors[i] != null)
            {
                Destroy(collectors[i].gameObject);
            }
        }

        collectors.Clear();
        for (int i = 0; i < raiders.Count; i++)
        {
            if (raiders[i] != null)
            {
                Destroy(raiders[i].gameObject);
            }
        }

        raiders.Clear();
        enemies.Clear();
    }
}
