using Assets.Scripts.Character.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseAndAttackTargetAction", menuName = "RuinsRaiders/Ai/ChaseAndAttackTargetAction", order = 1)]
public class ChaseAndAttackAction : ChaseAction
{
    public float distanceForAttack;
    public float attackTime;
    public float timeBetweenAttack;
    public override BasicAiAction Create(ActivatorData trigger)
    {
        return new Action(this, trigger);
    }

    new public class Action : ChaseAction.Action
    {
        new ChaseAndAttackAction parent;
        private float _timeToAttack;
        private float _attackCooldown;
        private bool attacking = false;

        public Action(ChaseAndAttackAction parent, ActivatorData trigger) : base(parent,trigger)
        {
            this.parent = parent;
        }

        public override void Update(float dt)
        {
            if (IsFinished())
            {
                character.ChargeAttack = false;
                character.Move = new Vector2();
                return;
            }
            
            if (targetCharacter != null && targetCharacter.IsDead)
            {
                targetCharacter = null;
                target = null;
                return;
            }

            if (_attackCooldown > 0)
            {
                _attackCooldown -= dt;
                return;
            }
             

            if (attacking)
            {
                if(_timeToAttack == parent.attackTime)
                {
                    character.Move = new Vector2();
                }
                if(_timeToAttack < 0)
                {
                    character.Attack();
                    character.ChargeAttack = false;
                    attacking = false;
                    _attackCooldown = parent.timeBetweenAttack;
                }
                else
                {
                    character.ChargeAttack = true;
                    _timeToAttack -= dt;
                }
            }
            if(!attacking && parent.distanceForAttack > Vector2.Distance(character.transform.position,target.transform.position))
            {
                attacking = true;
                _timeToAttack = parent.attackTime;
            }

            timeToNextPathfinding -= dt;
            if (timeToNextPathfinding > 0)
                return;

            timeToNextPathfinding = parent.timeBetweenRetryingFindingPath;
            if (pathfinding.Target == pathfinding.GetTileId(target.transform.position))
                return;

            TryWalkingToTarget();

        }
    }
}