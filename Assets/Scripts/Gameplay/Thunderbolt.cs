using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuinsRaiders {
    public class Thunderbolt : MonoBehaviour
    {
        public HealthController.Group group;
        public ThunderboltVisualisation visualisationPrefab;

        public int damage = 2;
        public int minDamage = 1;
        public int damageReductionWhenJumping = 1;
        public int maxJumps = int.MaxValue;

        public float seekDistance = 4;
        public float seekDistanceReductionWhenJumping = 1;

        public float timeToLive = 0.5f;
        public float delayBetweenJumps = 0.1f;

        private List<GameObject> _targets = new List<GameObject>();
        private GameObject _launcher;
        private List<ThunderboltVisualisation> _arcs = new List<ThunderboltVisualisation>();

        private GameObject _dummy;

        private float _timeLeftTolive = float.MaxValue;

        public void Launch(GameObject launcher, GameObject start, GameObject target)
        {
            _arcs.ForEach(it=>Destroy(it));
            _arcs.Clear();
            _targets.Clear();

            _launcher = launcher;
            _targets.Add(target);
            CreateArc(start, target);
            StartCoroutine(JumpToNextTarget());
        }

        public void Launch(GameObject launcher, GameObject start, Vector3 position)
        {
            if (_dummy != null)
                Destroy(_dummy);
            _dummy = new GameObject();

            var collision = Physics2D.Linecast(launcher.transform.position, position, Physics2D.GetLayerCollisionMask(gameObject.layer));
            if (collision)
                _dummy.transform.position = collision.point;
            else
                _dummy.transform.position = position;
            _dummy.name = "DummyArcTarget";
            Launch(launcher, start, _dummy);

        }

        private void OnDestroy()
        {
           if(_dummy!= null) 
                DestroyImmediate(_dummy);
        }

        private void CreateArc(GameObject start, GameObject end)
        {
            var arc = Instantiate(visualisationPrefab);
            arc.start = start;
            arc.end = end;
            arc.name = "Arc";
            arc.transform.SetParent(transform, false);
            _arcs.Add(arc);
        }

        IEnumerator JumpToNextTarget()
        {
            int currentDamage = damage;
            float currentSeekDistance = seekDistance;
            int iteration = 0;


            HealthController healthController = _targets.Last().GetComponent<HealthController>();
            if (healthController != null && healthController.group != group)
            {
                healthController.Damage(currentDamage, _launcher);
                currentSeekDistance -= seekDistanceReductionWhenJumping;
                currentDamage -= damageReductionWhenJumping;
                if (currentDamage < minDamage)
                    currentDamage = minDamage;
                iteration++;
            }

            while (currentDamage > 0 && currentSeekDistance > 0 && iteration <= maxJumps)
            {
                yield return new WaitForSeconds(delayBetweenJumps);

                var newTarget = GetNearestTarget(currentSeekDistance);
                if (newTarget == null)
                    break;

                CreateArc(_targets.LastOrDefault(), newTarget.gameObject);
                _targets.Add(newTarget.gameObject);

                newTarget.Damage(currentDamage, _launcher);
                currentSeekDistance -= seekDistanceReductionWhenJumping;
                currentDamage -= damageReductionWhenJumping;
                if (currentDamage < minDamage)
                    currentDamage = minDamage;
                iteration++;
            }

            _timeLeftTolive = timeToLive;

        }

        HealthController GetNearestTarget(float maxDistance)
        {
            var overlaps = Physics2D.OverlapCircleAll(_targets.Last().transform.position, maxDistance);
            var healthControllers = new List<HealthController>();

            foreach (var overlap in overlaps)
            {
                if (_targets.Contains(overlap.gameObject))
                    continue;

                HealthController controller = overlap.GetComponent<HealthController>();
                if (controller == null || controller.group == group)
                    continue;


                var cast = Physics2D.Linecast(_targets.Last().transform.position, overlap.transform.position, Physics2D.GetLayerCollisionMask(gameObject.layer));
                if (!cast || cast.collider.gameObject != overlap.gameObject)
                    continue;

                healthControllers.Add(controller);
            }

            HealthController hMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (HealthController h in healthControllers)
            {
                float dist = Vector3.Distance(h.transform.position, currentPos);
                if (dist < minDist)
                {
                    hMin = h;
                    minDist = dist;
                }
            }
            return hMin;
        }


        void FixedUpdate()
        {
            if (_timeLeftTolive > timeToLive)
                return;

            _timeLeftTolive -= Time.fixedDeltaTime;

            float visibility = Mathf.Clamp(_timeLeftTolive, 0, timeToLive) / timeToLive;
            foreach (var arc in _arcs)
                arc.visibility = visibility;

            if (_timeLeftTolive < 0)
                Destroy(gameObject);

        }
    }
}