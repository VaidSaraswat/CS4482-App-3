using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CharacterAnimator : MonoBehaviour
{
    public AnimationClip replacableAttackAnimation;
    public AnimationClip[] defaultAnimationSet;
    protected AnimationClip[] currentAttackAnimationSet;
    const float locomotionAnimationSmoothTime = 0.1f;
    NavMeshAgent agent;
    Animator animator;
    protected CharacterCombat combat;
    public AnimatorOverrideController overrideController;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponentInChildren<CharacterCombat>();
        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        }
        animator.runtimeAnimatorController = overrideController;
        currentAttackAnimationSet = defaultAnimationSet;
        combat.OnAttack += OnAttack;

    }

    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
        animator.SetBool("inCombat", combat.InCombat);
    }

    protected virtual void OnAttack()
    {
        animator.SetTrigger("attack");
        int attackIndex = Random.Range(0, currentAttackAnimationSet.Length);
        overrideController[replacableAttackAnimation.name] = currentAttackAnimationSet[attackIndex];
    }
}
