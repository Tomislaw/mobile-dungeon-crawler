using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders
{
    public class DoorTile : MonoBehaviour
    {
        private const string OpenAnimation = "Open";

        [SerializeField]
        private AudioClip lockedSound;

        [SerializeField]
        private AudioClip openSound;

        [SerializeField]
        private GameObject lockGameObject;


        private bool _isOpen = false;
        private readonly List<ItemsController> _colliders = new();

        private Animator _animator;
        private Collider2D _collider;

        private AudioSource _audio;
        void Start()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isOpen)
                return;

            var itemController = collision.gameObject.GetComponent<ItemsController>();
            if (itemController == null)
                return;

            if (itemController.HaveKeys)
            {
                itemController.RemoveKey();
                Open();
            }
            else
            {
                if (_audio != null)
                    _audio.PlayOneShot(lockedSound);

                _colliders.Add(itemController);
                if (lockGameObject)
                    lockGameObject.SetActive(true);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            var itemController = collision.gameObject.GetComponent<ItemsController>();
            if (itemController == null)
                return;

            _colliders.Remove(itemController);
            if (_colliders.Count == 0 && lockGameObject != null)
                lockGameObject.SetActive(false);

        }

        public void Open()
        {
            if (_audio != null)
                _audio.PlayOneShot(openSound);

            _isOpen = true;
            if (_animator)
                _animator.Play(OpenAnimation);
            if (_collider)
                _collider.enabled = false;
        }
    }
}
