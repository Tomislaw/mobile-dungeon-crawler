using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static Character;

public class AttackController : MonoBehaviour
{
    // attack and damage group
    public float AttackSpeed = 0.2f;
    public float AttackAnimationTime = 0.15f;

    public float OverchargeTime = 1;


    public bool ChargeAttack = false;

    public UnityEvent OnAttack;

    // private fields
    private float timeToNextAttack = 0;
    private float timeToAnimationAttackFinish = 0;
    private float timeToOvercharge = 0;

    private Character character;

    public bool CanAttack { get => timeToNextAttack <= 0 && !character.IsDead && !character.holdUpdate; }
    public bool IsAttacking { get => timeToAnimationAttackFinish > 0; }
    public bool IsOvercharged{ get => timeToOvercharge <=0; }

    public void OnEnable()
    {
        timeToOvercharge = OverchargeTime;
        character = GetComponent<Character>();
      
    }
    public void FixedUpdate()
    {
        if (timeToAnimationAttackFinish > 0) timeToAnimationAttackFinish -= Time.deltaTime;
        if (timeToNextAttack > 0) timeToNextAttack -= Time.deltaTime;

        if (ChargeAttack)
            timeToOvercharge -= Time.fixedDeltaTime;
        else
            timeToOvercharge = OverchargeTime;

    }

    public virtual void Attack()
    {
        if (CanAttack)
        {
            timeToAnimationAttackFinish = AttackAnimationTime;
            timeToNextAttack = AttackSpeed;
            ChargeAttack = false;
        }
    }
}
 

