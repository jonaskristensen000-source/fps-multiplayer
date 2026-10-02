using System;
using UnityEngine;

public class ColonyAcidGlob : MonoBehaviour
{
    public Vector3 velocity;
    public float damage = 18f;
    public float lifetime = 1.4f;
    public float radius = 0.09f;
    public GameObject owner;
    public ColonyHealth.Team team;
    public TrailRenderer trail;
    float age;
    public void Launch(Vector3 origin, Vector3 dir, float speed, float dmg, GameObject src, ColonyHealth.Team srcTeam)
    {
        transform.position = origin;
        velocity = dir.normalized * speed;
        damage = dmg; owner = src; team = srcTeam; age = 0f;
        gameObject.SetActive(true);
        Color c = ColonyHealth.Friendly(team) ? new Color(.7f,1f,.18f) : new Color(1f,.18f,.08f);
        var renderer = GetComponent<Renderer>();
        var block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor",c); block.SetColor("_EmissionColor",c*2f);
        if (renderer != null) renderer.SetPropertyBlock(block);
        if (trail != null) { trail.Clear(); trail.startColor=c; trail.endColor=new Color(c.r,c.g,c.b,0); }
    }
    void Update()
    {
        var director = ColonyGameDirector.Instance;
        if (director == null || !director.IsPlaying) return;
        float step = Time.deltaTime;
        Vector3 delta = velocity * step;
        var hits = Physics.SphereCastAll(transform.position, radius, delta.normalized, delta.magnitude, ~0, QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (a,b) => a.distance.CompareTo(b.distance));
        foreach (var hit in hits)
        {
            if (owner != null && hit.transform.IsChildOf(owner.transform)) continue;
            var health = hit.collider.GetComponentInParent<ColonyHealth>();
            if (health != null && ColonyHealth.Friendly(health.team) == ColonyHealth.Friendly(team)) continue;
            if (health != null)
            {
                health.ApplyDamage(damage, owner);
                if (team == ColonyHealth.Team.Player)
                    EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.Feedback, health.team == ColonyHealth.Team.Base ? "Anthill hit! Keep firing." : "Hit!");
            }
            Destroy(gameObject);
            return;
        }
        transform.position += delta;
        age += step;
        if (age >= lifetime) Destroy(gameObject);
    }
}
