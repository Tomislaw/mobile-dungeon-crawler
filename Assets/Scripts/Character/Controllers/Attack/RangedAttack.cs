using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : AttackController
{

    public List<ProjectileData> projectiles = new List<ProjectileData>();
    public List<ProjectileData> projectilesCharged = new List<ProjectileData>();

    private Character character;
    public override void Attack()
    {
        if (CanAttack)
        {
            if (IsOvercharged)
                foreach (var p in projectilesCharged)
                    StartCoroutine(p.Launch(character));
            else
                foreach (var p in projectiles)
                    StartCoroutine(p.Launch(character));
            base.Attack();
        }

    }

    // Start is called before the first frame update
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    [Serializable]
    public struct ProjectileData
    {
        public Projectile projectile;
        public Vector3 offset;
        public float delay;
        public IEnumerator Launch(Character launcher, GameObject target = null)
        {
            yield return new WaitForSeconds(delay);

            GameObject go = Instantiate(projectile.gameObject);
            var p = go.GetComponent<Projectile>();

            p.Launcher = launcher;

            var movementController = launcher.GetComponent<MovementController>();
            if (movementController == null)
                yield break;

            go.transform.position = launcher.transform.position + new Vector3(offset.x * (movementController.FaceLeft ? 1 : -1), offset.y, 0);
            if (movementController.FaceLeft)
                p.InitialVelocity *= -1;
        }
    }
}
