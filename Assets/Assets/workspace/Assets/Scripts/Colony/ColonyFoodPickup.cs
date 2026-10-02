using UnityEngine;

public class ColonyFoodPickup : MonoBehaviour
{
    public int amount = 1;
    public float respawnSeconds = 14f;
    public bool IsAvailable => !taken;
    bool taken;
    float respawn;
    Vector3 originalScale;
    Renderer[] visuals;
    void Awake()
    {
        originalScale = transform.localScale;
        visuals = GetComponentsInChildren<Renderer>();
    }
    public void ResetPickup()
    {
        taken = false;
        respawn = 0f;
        gameObject.SetActive(true);
        foreach (var r in visuals) r.enabled = true;
        transform.localScale = originalScale;
    }
    void Update()
    {
        var d = ColonyGameDirector.Instance;
        if (d == null || !d.IsPlaying) return;
        if (taken)
        {
            respawn -= Time.deltaTime;
            if (respawn <= 0f) ResetPickup();
            return;
        }
        transform.Rotate(Vector3.up, 28f * Time.deltaTime, Space.World);
        transform.localScale = originalScale * (1f + 0.055f * Mathf.Sin(Time.time * 3f));
        if (Input.GetKeyDown(KeyCode.E) && Vector3.Distance(d.Player.transform.position, transform.position) < 2f) Collect();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null) Collect();
    }
    public bool Collect()
    {
        var d = ColonyGameDirector.Instance;
        if (taken || d == null || !d.IsPlaying || d.food >= d.foodMax) return false;
        taken = true;
        respawn = respawnSeconds;
        foreach (var r in visuals) r.enabled = false;
        d.AddFood(amount);
        EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, "+1 food. Gather 3 to summon a soldier.");
        return true;
    }
}
