using UnityEngine;

public class ColonyQA : MonoBehaviour
{
    public bool qaMode;

    void Awake()
    {
        qaMode = Application.isEditor || qaMode;
    }

    void Update()
    {
        if (!qaMode)
        {
            return;
        }

        var d = ColonyGameDirector.Instance;
        if (d == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            d.AddFood(3);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            d.TrySummon();
        }

        if (Input.GetKeyDown(KeyCode.F7) && d.Anthill != null)
        {
            d.Anthill.GetComponent<ColonyHealth>()?.ApplyDamage(80f, gameObject);
        }

        if (Input.GetKeyDown(KeyCode.F8) && d.PlayerHealth != null)
        {
            d.PlayerHealth.ApplyDamage(999f, gameObject);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            Dump();
        }
    }

    public object Snapshot()
    {
        var d = ColonyGameDirector.Instance;
        return new
        {
            flow = d != null ? d.flow.ToString() : "none",
            food = d != null ? d.food : 0,
            soldiers = d != null ? d.SoldierCount : 0,
            raiders = d != null ? d.AliveRaiders : 0,
            playerHp = d != null && d.PlayerHealth != null ? d.PlayerHealth.current : 0,
            hillHp = d != null && d.Anthill != null ? d.Anthill.GetComponent<ColonyHealth>().current : 0,
            score = d != null ? d.score : 0
        };
    }

    void Dump()
    {
        Debug.Log("COLONY_QA " + JsonUtility.ToJson(new QASnap
        {
            flow = ColonyGameDirector.Instance != null ? ColonyGameDirector.Instance.flow.ToString() : "none",
            food = ColonyGameDirector.Instance != null ? ColonyGameDirector.Instance.food : 0,
            soldiers = ColonyGameDirector.Instance != null ? ColonyGameDirector.Instance.SoldierCount : 0,
            raiders = ColonyGameDirector.Instance != null ? ColonyGameDirector.Instance.AliveRaiders : 0
        }));
    }

    [System.Serializable]
    class QASnap
    {
        public string flow;
        public int food;
        public int soldiers;
        public int raiders;
    }
}
