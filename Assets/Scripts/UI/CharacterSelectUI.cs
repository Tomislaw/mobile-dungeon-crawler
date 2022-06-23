using UnityEngine;
using UnityEngine.Events;

namespace RuinsRaiders.UI
{
    // responsible for character selection ui at the beginning of each level
    public class CharacterSelectUI : MonoBehaviour
    {
        public UnityEvent isLockedEvent;
        public UnityEvent isUnlockedEvent;

        [SerializeField]
        private PlayerData playerData;

        [SerializeField]
        private PlayerData.CharacterType characterType;

        void OnEnable()
        {
            if (IsCharacterUnlocked)
                isUnlockedEvent.Invoke();
            else
                isLockedEvent.Invoke();
        }

        public bool IsCharacterUnlocked
        {
            get
            {
                return characterType switch
                {
                    PlayerData.CharacterType.Knight => playerData.knight.unlocked,
                    PlayerData.CharacterType.Mage => playerData.mage.unlocked,
                    PlayerData.CharacterType.Archer => playerData.archer.unlocked,
                    PlayerData.CharacterType.Spearman => playerData.spearman.unlocked,
                    _ => false,
                };
            }
        }
    }
}