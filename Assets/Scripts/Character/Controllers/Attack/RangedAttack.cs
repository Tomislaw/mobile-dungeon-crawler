using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class RangedAttack : AttackController
    {
        public int attackDamage = 2;
        public int specialAttackDamage = 2;
        public int specialAttackMaxProjectileCount = 3;

        [SerializeField]
        private List<ProjectileData> projectiles = new();
        [SerializeField]
        private List<ProjectileData> projectilesCharged = new();

        public override void Attack()
        {
            if (CanAttack)
            {
                if (IsOvercharged)
                {
                    int counter = 0;
                    foreach (var p in projectilesCharged)
                    {
                        StartCoroutine(p.Launch(this, specialAttackDamage));
                        counter++;
                        if (counter >= specialAttackMaxProjectileCount)
                            break;
                    }
                }
                else
                {

                    foreach (var p in projectiles)
                    {
                        StartCoroutine(p.Launch(this, attackDamage));
                    }
                }
                base.Attack();
            }

        }

        [Serializable]
        public struct ProjectileData
        {
            public Projectile projectile;
            public Vector3 offset;
            public float delay;
            public IEnumerator Launch(RangedAttack rangedAttack, int damage)
            {
                yield return new WaitForSeconds(delay);

                Projectile launchedProjectile = Instantiate(projectile);
                launchedProjectile.launcher = rangedAttack._character;
                launchedProjectile.damage = damage;

                if (rangedAttack._healthController)
                    launchedProjectile.group = rangedAttack._healthController.group;

                var faceLeft = launchedProjectile.launcher.transform.lossyScale.x < 0;
                launchedProjectile.transform.position = launchedProjectile.launcher.transform.position + new Vector3(offset.x * (faceLeft ? 1 : -1), offset.y, 0);
                if (faceLeft)
                    launchedProjectile.initialVelocity *= -1;
            }
        }
    }
}