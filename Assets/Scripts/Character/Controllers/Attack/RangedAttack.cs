using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class RangedAttack : AttackController
    {
        [SerializeField]
        private List<ProjectileData> projectiles = new();
        [SerializeField]
        private List<ProjectileData> projectilesCharged = new();

        public override void Attack()
        {
            if (CanAttack)
            {
                if (IsOvercharged)
                    foreach (var p in projectilesCharged)
                        StartCoroutine(p.Launch(this));
                else
                    foreach (var p in projectiles)
                        StartCoroutine(p.Launch(this));
                base.Attack();
            }

        }

        [Serializable]
        public struct ProjectileData
        {
            public Projectile projectile;
            public Vector3 offset;
            public float delay;
            public IEnumerator Launch(RangedAttack rangedAttack)
            {
                yield return new WaitForSeconds(delay);

                Projectile launchedProjectile = Instantiate(projectile);
                launchedProjectile.launcher = rangedAttack._character;

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