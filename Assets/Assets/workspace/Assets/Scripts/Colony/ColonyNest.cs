using UnityEngine;

public class ColonyNest : MonoBehaviour
{
    public Transform respawnPoint;
    ColonyHealth health;
    Vector3 originalScale;

    public ColonyHealth Health => health;
    public Vector3 RespawnPosition
    {
        get
        {
            if (respawnPoint != null)
            {
                return respawnPoint.position;
            }

            return transform.position + transform.forward * 1.8f + Vector3.up * 0.12f;
        }
    }

    public Quaternion RespawnRotation
    {
        get
        {
            if (respawnPoint != null)
            {
                return Quaternion.Euler(0f, respawnPoint.eulerAngles.y, 0f);
            }

            return Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
    }

    void Awake()
    {
        originalScale = transform.localScale;
        health = GetComponent<ColonyHealth>();
        if (respawnPoint == null)
        {
            var child = transform.Find("Respawn");
            respawnPoint = child != null ? child : transform;
        }
    }

    public void ResetNest()
    {
        transform.localScale = originalScale;
        health?.ResetHealth();
    }

    void Update()
    {
        if (health != null && health.IsDead)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalScale.x, originalScale.y * 0.18f, originalScale.z), Time.deltaTime * 3f);
        }
    }
}
