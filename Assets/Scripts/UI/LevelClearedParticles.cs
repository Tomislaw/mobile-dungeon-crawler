using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders {
    public class LevelClearedParticles : MonoBehaviour
    {
        [SerializeField]
        private float sizeFactor = 1f;

        [SerializeField]
        public List<Vector2> positions;

        [SerializeField]
        public ParticleSystem equisiteChestPrefab;

        [SerializeField]
        public ParticleSystem normalChestPrefab;

        private List<ParticleSystem> _equisiteChestSystems = new List<ParticleSystem>();
        private List<ParticleSystem> _normalChestSystems = new List<ParticleSystem>();

        void Start()
        {
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;

            _normalChestSystems.ForEach(e =>Destroy(e.gameObject));
            _equisiteChestSystems.ForEach(e => Destroy(e.gameObject));

            foreach (var position in positions)
            { 

                ParticleSystem system1 = Instantiate(normalChestPrefab);
                system1.transform.parent = transform;
                system1.transform.localPosition = position;
                system1.gameObject.SetActive(true);
                system1.transform.localScale *= sizeFactor;
                _normalChestSystems.Add(system1);
                system1.name = "NormalParticle";

                ParticleSystem system2 = Instantiate(equisiteChestPrefab);
                system2.transform.parent = transform;
                system2.transform.localPosition = position;
                system2.gameObject.SetActive(true);
                system2.transform.localScale *= sizeFactor;
                _equisiteChestSystems.Add(system2);
                system2.name = "EquisiteParticle";
            }
        }

        public void StartParticleSystem(AdventureData.ChestData.Type chest, int id)
        {
            if(chest == AdventureData.ChestData.Type.Equisite)
            {
                _equisiteChestSystems[id].Play(true);
            } else
            {
                _normalChestSystems[id].Play(true);
            }
        }

    }
}