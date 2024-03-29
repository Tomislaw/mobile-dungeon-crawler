using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RuinsRaiders
{
    public class RaycastAttack : AttackController
    {
        public GameObject thunderboltStartPosition;

        [Header("Attack")]
        public int attackDamage = 2;
        public int attackDamageReduction = 1;

        public ColliderContainer colliderContainer;
        public Thunderbolt thunderboltPrefab;

        [Header("Special Attack")]
        public int specialAttackDamage = 1;
        public int specialAttackDamageReduction = 0;
        public ColliderContainer specialColliderContainer;
        public Thunderbolt specialThunderboltPrefab;

        public float specialRateOfFire = 0.2f;

        private float _timeToNextSpecialAttack;
        private float _attackDistance;

        void Start()
        {
            onAttack.AddListener(OnAttack);
            onChargedAttack.AddListener(OnChargedAttack);
            var collider = colliderContainer.GetComponent<Collider2D>();
            _attackDistance = collider.bounds.size.x;
        }

        private void OnAttack()
        {
            var targetPos = thunderboltStartPosition.transform.position + new Vector3(_attackDistance * transform.localScale.x, 0, 0);
            LaunchBolt(thunderboltPrefab, targetPos, attackDamage, attackDamageReduction);
        }

        private void OnChargedAttack()
        {

        }

        private void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsOvercharged)
                _timeToNextSpecialAttack -= Time.fixedDeltaTime;
            else
                _timeToNextSpecialAttack = specialRateOfFire;

            if (_timeToNextSpecialAttack <= 0)
            {
                _timeToNextSpecialAttack = specialRateOfFire;
                LaunchBolt(specialThunderboltPrefab, GetRandomPoint(colliderContainer), specialAttackDamage, specialAttackDamageReduction);
            }
        }

        private void LaunchBolt(Thunderbolt thunderbolt, Vector3 targerPos, int damage, int damageReduction)
        {
            var collider = colliderContainer.GetColliders().Where(it =>
            {
                var health = it.GetComponent<HealthController>();
                bool validGroup = health != null && health.group != _healthController.group;

                if (validGroup == false)
                    return false;

                var cast = Physics2D.Linecast(thunderboltStartPosition.transform.position, it.transform.position, Physics2D.GetLayerCollisionMask(gameObject.layer));
                if (!cast)
                    return false;

                return cast.collider.gameObject == it.gameObject;

            }).OrderBy(it => Vector2.Distance(thunderboltStartPosition.transform.position, it.transform.position)).FirstOrDefault();


            Thunderbolt bolt = Instantiate(thunderbolt);
            bolt.name = gameObject.name + "_bolt";
            bolt.damage = damage;
            bolt.damageReductionWhenJumping = damageReduction;
            bolt.group = _healthController.group;
            bolt.transform.position = thunderboltStartPosition.transform.position;

            if (collider != null)
                bolt.Launch(gameObject, thunderboltStartPosition, collider.gameObject);
            else
                bolt.Launch(gameObject, thunderboltStartPosition, targerPos);

        }

        private Vector3 GetRandomPoint(ColliderContainer container)
        {
            var nearestPoint = container.GetRandomPoint();

            var cast = Physics2D.Linecast(thunderboltStartPosition.transform.position, nearestPoint, Physics2D.GetLayerCollisionMask(gameObject.layer));
            if (cast)
                return cast.point;
            else return nearestPoint;
        }
    }
}