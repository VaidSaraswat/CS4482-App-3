using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    public float attackSpeed = 1f;
    private float attackCoolDown = 0f;
    const float combatCoolDown = 5;
    float lastAttackTime;
    public float attackDelay = 0.6f;
    public bool InCombat { get; private set; }

    public event System.Action OnAttack;
    CharacterStats myStats;
    CharacterStats opponentStats;

    void Start()
    {
        myStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        attackCoolDown -= Time.deltaTime;

        if (Time.time - lastAttackTime > combatCoolDown)
        {
            InCombat = false;
        }
    }
    public void Attack(CharacterStats targetStats)
    {
        if (attackCoolDown <= 0f)
        {
            opponentStats = targetStats;

            if (OnAttack != null)
            {
                OnAttack();
            }
            targetStats.TakeDamage(myStats.damage.GetValue());
            attackCoolDown = 1f / attackSpeed;
            InCombat = true;
            lastAttackTime = Time.time;
        }
    }

    public void AttackHit_AnimationEvent()
    {
        opponentStats.TakeDamage(myStats.damage.GetValue());

        if (opponentStats.currHealth <= 0)
        {
            InCombat = false;
        }
    }
}
