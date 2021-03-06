using System;
using UnityEngine;


namespace RuinsRaiders
{
    // Place where player spawn at the beginning of the level
    [ExecuteAlways]
    public class PlayerSpawn : MonoBehaviour
    {
        [SerializeField]
        private PlayerData data;


        // todo, store these in scriptable object instead
        [SerializeField]
        private Character knightPrefab;

        [SerializeField]
        private Character archerPrefab;

        [SerializeField]
        private Character magePrefab;

        [SerializeField]
        private Character spearmanPrefab;


        public void Spawn(CharacterType type)
        {
            Character player = null;
            switch (type)
            {
                case CharacterType.Spearman:
                    player = Instantiate(spearmanPrefab);
                    break;
                case CharacterType.Mage:
                    player = Instantiate(magePrefab);
                    break;
                case CharacterType.Knight:
                    player = Instantiate(knightPrefab);
                    break;
                case CharacterType.Bowman:
                    player = Instantiate(archerPrefab);
                    break;
            }
            player.transform.position = transform.position;
            transform.parent = player.gameObject.transform;

            var healthComponent = player.GetComponent<HealthController>();
            if(healthComponent)
                healthComponent.onDeath.AddListener(OnDeath);
        }

        private void OnDeath()
        {
            FindObjectOfType<LevelEvents>().LevelFailed();
        }

        [Serializable]
        public enum CharacterType
        {
            Knight, Bowman, Spearman, Mage
        }
    }
}