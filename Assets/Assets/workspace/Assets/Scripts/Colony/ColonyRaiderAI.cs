using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ColonyRaiderAI : MonoBehaviour
{
    public float moveSpeed = 3.7f;
    public float gravity = -18f;
    public float shootRange = 12f;

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
            ColonyGameDirector.Instance?.NotifyRaiderDead(this);
            gameObject.SetActive(false);
            return;
        }

        var director = ColonyGameDirector.Instance;
        if (director == null || !director.IsPlaying)
        {
            return;
        }

        Transform prey = director.FindRaiderTarget(transform.position);
        if (prey == null)
        {
            motion?.SetMoving(false, false);
            return;
        }

        Vector3 to = prey.position - transform.position;
        to.y = 0f;
        float dist = to.magnitude;
        attacking = false;
        if (dist > 0.05f)
        {
            Face(to.normalized);
        }

        var preyHealth = prey.GetComponent<ColonyHealth>();
        if (dist < shootRange && gun != null)
        {
            Vector3 aimPoint = preyHealth != null ? preyHealth.AimPoint : prey.position + Vector3.up * 0.48f;
            attacking = gun.TryFire(aimPoint - gun.Origin);
        }

        bool moving = false;
        if (dist > 3.2f)
        {
            if (climber != null && climber.MoveTowards(prey.position, moveSpeed, ref vy, gravity))
            {
                moving = true;
            }
            else
            {
                if (body.isGrounded)
                {
                    vy = -2f;
                }
                else
                {
                    vy += gravity * Time.deltaTime;
                }

                Vector3 dir = nav != null ? nav.Direction(prey.position) : to.normalized;
                Face(dir);
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
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * 9f);
    }
}
