using UnityEngine;

public class ColonyAntMotion : MonoBehaviour
{
    public Transform visual;
    public Animator animator;
    public string positionWriter = "controller";
    static readonly int Idle = Animator.StringToHash("Idle");
    static readonly int Walk = Animator.StringToHash("Walk");
    static readonly int Attack = Animator.StringToHash("Attack");
    int current = -1;
    float attackUntil;
    public string BoundClip { get; private set; } = "Idle";
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
    }
    public void SetMoving(bool moving, bool attacking)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        if (attacking) attackUntil = Time.time + .2f;
        int next = Time.time < attackUntil ? Attack : moving ? Walk : Idle;
        BoundClip = next == Attack ? "Attack" : next == Walk ? "Walk" : "Idle";
        if (next == current) return;
        current = next;
        animator.CrossFadeInFixedTime(next, .08f);
    }
}
