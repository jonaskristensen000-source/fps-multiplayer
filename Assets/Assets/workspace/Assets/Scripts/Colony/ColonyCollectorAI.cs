using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ColonyCollectorAI : MonoBehaviour
{
    public float moveSpeed = 4.1f;
    public float gravity = -18f;
    public float gatherRange = 1.15f;

    CharacterController body;
    ColonyHealth health;
    ColonyAntMotion motion;
    ColonyClimber climber;
    ColonyNavigation nav;
    ColonyFoodPickup target;
    float vy;

    public string PositionWriter => "controller";

    void Awake()
    {
        body = GetComponent<CharacterController>();
        health = GetComponent<ColonyHealth>();
        motion = GetComponent<ColonyAntMotion>();
        climber = GetComponent<ColonyClimber>();
        nav = GetComponent<ColonyNavigation>();
    }

    void Update()
    {
        if (health != null && health.IsDead)
        {
            motion?.SetMoving(false, false);
            gameObject.SetActive(false);
            EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.CollectorCountChanged);
            return;
        }

        var director = ColonyGameDirector.Instance;
        if (director == null || !director.IsPlaying)
        {
            return;
        }

        if (target == null || !target.IsAvailable)
        {
            target = FindCrumb();
        }

        Vector3 dest;
        if (target != null)
        {
            dest = target.transform.position;
        }
        else if (director.Nest != null)
        {
            dest = director.Nest.RespawnPosition;
        }
        else
        {
            dest = transform.position;
        }

        Vector3 to = dest - transform.position;
        float dist = to.magnitude;
        bool gathering = false;
        if (target != null && dist < gatherRange)
        {
            gathering = target.Collect();
            target = null;
        }

        bool moving = false;
        float hold = gathering ? 0.2f : gatherRange;
        if (dist > hold)
        {
            if (climber != null && climber.MoveTowards(dest, moveSpeed, ref vy, gravity))
            {
                moving = true;
            }
            else
            {
                Vector3 dir = nav != null ? nav.Direction(dest) : new Vector3(to.x, 0f, to.z).normalized;
                Face(dir);
                if (body.isGrounded)
                {
                    vy = -2f;
                }
                else
                {
                    vy += gravity * Time.deltaTime;
                }

                body.Move((dir * moveSpeed + Vector3.up * vy) * Time.deltaTime);
                moving = true;
            }
        }
        else if (!body.isGrounded)
        {
            vy += gravity * Time.deltaTime;
            body.Move(Vector3.up * vy * Time.deltaTime);
        }

        motion?.SetMoving(moving, gathering);
    }

    ColonyFoodPickup FindCrumb()
    {
        ColonyFoodPickup best = null;
        float bestD = 90f * 90f;
        var crumbs = Object.FindObjectsByType<ColonyFoodPickup>(FindObjectsSortMode.None);
        for (int i = 0; i < crumbs.Length; i++)
        {
            var crumb = crumbs[i];
            if (crumb == null || !crumb.IsAvailable)
            {
                continue;
            }

            float d = (crumb.transform.position - transform.position).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = crumb;
            }
        }

        return best;
    }

    void Face(Vector3 dir)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized, Vector3.up), Time.deltaTime * 10f);
    }
}
