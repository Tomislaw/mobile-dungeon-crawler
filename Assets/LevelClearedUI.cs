using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders {

    [RequireComponent(typeof(AudioSource))]
    public class LevelClearedUI : MonoBehaviour
    {
        [SerializeField]
        private List<Animator> chestsAnimators = new List<Animator>();

        [SerializeField]
        private AudioClip equisiteChestOpenedSong;

        [SerializeField]
        private AudioClip chestOpenedSong;

        [SerializeField]
        private AudioClip noChestOpenedSong;


        public void LevelCleared()
        {
            var chests = LevelEvents.Instance.GetCollectedChests();

            AdventureData.ChestData.Type? songType = null;

            for(int i = 0; i < chests.Count && i < chestsAnimators.Count; i++)
            {
                chestsAnimators[i].SetBool("IsCollected", chests[i].Item2);
                chestsAnimators[i].SetBool("IsOpen", chests[i].Item1.chestData.acquired);
                chestsAnimators[i].SetLayerWeight((int) chests[i].Item1.chestData.type, 1);

                if (chests[i].Item1.chestData.type == AdventureData.ChestData.Type.Equisite && chests[i].Item2)
                    songType = AdventureData.ChestData.Type.Equisite;
                else if (songType == null && chests[i].Item2)
                    songType = AdventureData.ChestData.Type.Normal;

            }

            var audioSource = GetComponent<AudioSource>();

            switch (songType)
            {
                case AdventureData.ChestData.Type.Equisite:
                    audioSource.PlayOneShot(equisiteChestOpenedSong);
                    break;
                case AdventureData.ChestData.Type.Normal:
                    audioSource.PlayOneShot(chestOpenedSong);
                    break;
                default:
                    audioSource.PlayOneShot(noChestOpenedSong);
                    break;
            }

            LevelEvents.Instance.GiveLevelRewards();
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
