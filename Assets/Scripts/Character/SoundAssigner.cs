using UnityEngine;

namespace RuinsRaiders
{
    public class SoundAssigner : MonoBehaviour
    {
        [SerializeField]
        private AudioClip death;
        [SerializeField]
        private AudioClip hit;
        [SerializeField]
        private AudioClip walk;
        [SerializeField]
        private AudioClip jump;
        [SerializeField]
        private AudioClip swim;
        [SerializeField]
        private AudioClip attack;
        [SerializeField]
        private AudioClip chargedAttack;

        private MovementController _movementController;
        private AttackController _attackController;
        private HealthController _healthController;

        private AudioSource _source;

        void Start()
        {
            _movementController = GetComponent<MovementController>();
            _attackController = GetComponent<AttackController>();
            _healthController = GetComponent<HealthController>();
            _source = GetComponent<AudioSource>();

            if (_movementController != null)
            {
                _movementController.onWalk.AddListener(PlayWalk);
                _movementController.onJump.AddListener(PlayJump);
                _movementController.onSwim.AddListener(PlaySwim);
            }

            if (_attackController != null)
            {
                _attackController.onAttack.AddListener(PlayAttack);
                _attackController.onChargedAttack.AddListener(PlayChargedAttack);
            }

            if (_healthController != null)
            {
                _healthController.onDamage.AddListener(PlayHitInternal);
                _healthController.onDeath.AddListener(PlayDeath);
            }
        }

        public void PlayWalk()
        {
            PlayClip(walk);
        }

        public void PlayJump()
        {
            PlayClip(jump);
        }

        public void PlaySwim()
        {
            PlayClip(swim);
        }

        public void PlayDeath()
        {
            PlayClip(death);
        }

        private void PlayHitInternal(GameObject go)
        {
            PlayHit();
        }
        public void PlayHit()
        {
            PlayClip(hit);
        }

        public void PlayAttack()
        {
            PlayClip(attack);
        }

        public void PlayChargedAttack()
        {
            PlayClip(chargedAttack);
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip != null && _source != null)
                _source.PlayOneShot(clip);
        }

    }
}