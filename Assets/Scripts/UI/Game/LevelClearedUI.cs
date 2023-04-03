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
        private float equisiteChestOpenedSongDelay = 1;
        [SerializeField]
        private AudioClip equisiteChestOpenedSong;

        [SerializeField]
        private float chestOpenedSongDelay = 1;
        [SerializeField]
        private AudioClip chestOpenedSong;

        [SerializeField]
        private float noChestOpenedSongDelay = 0;
        [SerializeField]
        private AudioClip noChestOpenedSong;


        [SerializeField]
        private LevelClearedParticles levelClearedParticles;

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

                if (chests[i].Item2 && levelClearedParticles != null)
                {
                    levelClearedParticles.StartParticleSystem(chests[i].Item1.chestData.type, i);
                }

            }

            StartCoroutine(PlaySong(songType));

            LevelEvents.Instance.GiveLevelRewards();
        }

        private IEnumerator PlaySong(AdventureData.ChestData.Type? songType)
        {
            var audioSource = GetComponent<AudioSource>();


            switch (songType)
            {
                case AdventureData.ChestData.Type.Equisite:
                    yield return new WaitForSeconds(equisiteChestOpenedSongDelay);
                    audioSource.PlayOneShot(equisiteChestOpenedSong);
                    break;
                case AdventureData.ChestData.Type.Normal:
                    yield return new WaitForSeconds(chestOpenedSongDelay);
                    audioSource.PlayOneShot(chestOpenedSong);
                    break;
                default:
                    yield return new WaitForSeconds(noChestOpenedSongDelay);
                    audioSource.PlayOneShot(noChestOpenedSong);
                    break;
            }
        }
    }
}
