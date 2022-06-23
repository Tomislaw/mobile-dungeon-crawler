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

        private Character _character;
        public override void Attack()
        {
            if (CanAttack)
            {
                if (IsOvercharged)
                    foreach (var p in projectilesCharged)
                        StartCoroutine(p.Launch(_character));
                else
                    foreach (var p in projectiles)
                        StartCoroutine(p.Launch(_character));
                base.Attack();
            }

        }

        // Start is called before the first frame update
        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        [Serializable]
        public struct ProjectileData
        {
            public Projectile projectile;
            public Vector3 offset;
            public float delay;
            public IEnumerator Launch(Character launcher)
            {
                yield return new WaitForSeconds(delay);

                GameObject go = Instantiate(projectile.gameObject);
                var p = go.GetComponent<Projectile>();

                p.launcher = launcher;

                var movementController = launcher.GetComponent<MovementController>();
                if (movementController == null)
                    yield break;

                go.transform.position = launcher.transform.position + new Vector3(offset.x * (movementController.faceLeft ? 1 : -1), offset.y, 0);
                if (movementController.faceLeft)
                    p.initialVelocity *= -1;
            }
        }
    }
}