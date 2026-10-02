using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ColonySoldierAI : MonoBehaviour
{
    public Transform followTarget;
    public float followDistance = 2.2f;
    public float moveSpeed = 4.4f;
    public float engageRange = 14f;
    public float gravity = -18f;

    CharacterController body;
    ColonyAcidGun gun;
    ColonyHealth health;
    ColonyAntMotion motion;
    ColonyClimber climber;
    ColonyNavigation nav;
    float vy;
    bool attacking;

    public string PositionWriter => "controller";

    void Awake()
    {
        body = GetComponent<CharacterController>();
        gun = GetComponent<ColonyAcidGun>();
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
            EventDispatcher.SendEvent(GameEventTypes.ColonyEventType.SoldierCountChanged);
            return;
        }

        var director = ColonyGameDirector.Instance;
        if (director == null || !director.IsPlaying)
        {
            return;
        }

        if (followTarget == null)
        {
            var player = director.Player;
            if (player != null)
            {
                followTarget = player.transform;
            }
        }

        var enemy = director.FindNearestEnemy(transform.position, engageRange);
        Vector3 dest;
        attacking = false;
        if (enemy != null)
        {
            dest = enemy.transform.position;
            Vector3 toEnemy = enemy.transform.position - transform.position;
            toEnemy.y = 0f;
            if (toEnemy.sqrMagnitude > 0.01f)
            {
                Face(toEnemy.normalized);
            }

            if (toEnemy.magnitude < 10f && gun != null)
            {
                Vector3 aim = enemy.AimPoint - gun.Origin;
                attacking = gun.TryFire(aim);
            }
        }
        else if (followTarget != null)
        {
            dest = followTarget.position;
        }
        else
        {
            dest = transform.position;
        }

        Vector3 to = dest - transform.position;
        to.y = 0f;
        float dist = to.magnitude;
        bool moving = false;
        float hold = enemy != null ? 2.4f : followDistance;
        if (dist > hold)
        {
            if (climber != null && climber.MoveTowards(dest, moveSpeed, ref vy, gravity))
            {
                moving = true;
            }
            else
            {
                Vector3 dir = nav != null ? nav.Direction(dest) : to.normalized;
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

        motion?.SetMoving(moving, attacking);
    }

    void Face(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 10f);
    }
}
