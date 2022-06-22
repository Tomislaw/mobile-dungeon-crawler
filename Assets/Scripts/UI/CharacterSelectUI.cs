using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders.UI
{
    // responsible for character selection ui at the beginning of each level
    public class CharacterSelectUI : MonoBehaviour
    {

        public UnityEvent IsLocked;
        public UnityEvent IsUnlocked;

        [SerializeField]
        private PlayerData playerData;
        [SerializeField]
        private PlayerData.CharacterType characterType;

        void OnEnable()
        {
            if (IsCharacterUnlocked)
                IsUnlocked.Invoke();
            else
                IsLocked.Invoke();
        }

        public bool IsCharacterUnlocked
        {
            get
            {
                switch (characterType)
                {
                    case PlayerData.CharacterType.Knight:
                        return playerData.knight.unlocked;
                    case PlayerData.CharacterType.Mage:
                        return playerData.mage.unlocked;
                    case PlayerData.CharacterType.Archer:
                        return playerData.archer.unlocked;
                    case PlayerData.CharacterType.Spearman:
                        return playerData.spearman.unlocked;
                    default:
                        return false;
                };
            }
        }
    }
}