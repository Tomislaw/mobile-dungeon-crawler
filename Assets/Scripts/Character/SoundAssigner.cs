
using UnityEngine;

namespace RuinsRaiders
{
    public class SoundAssigner : MonoBehaviour
    {
        [SerializeField]
        private AudioClip Death;
        [SerializeField]
        private AudioClip Hit;
        [SerializeField]
        private AudioClip Walk;
        [SerializeField]
        private AudioClip Jump;
        [SerializeField]
        private AudioClip Swim;
        [SerializeField]
        private AudioClip Attack;
        [SerializeField]
        private AudioClip ChargedAttack;

        private MovementController movementController;
        private AttackController attackController;
        private HealthController healthController;

        private AudioSource source;

        void Start()
        {
            movementController = GetComponent<MovementController>();
            attackController = GetComponent<AttackController>();
            healthController = GetComponent<HealthController>();
            source = GetComponent<AudioSource>();

            if (movementController != null)
            {
                movementController.OnWalk.AddListener(PlayWalk);
                movementController.OnJump.AddListener(PlayJump);
                movementController.OnSwim.AddListener(PlaySwim);
            }

            if (attackController != null)
            {
                attackController.OnAttack.AddListener(PlayAttack);
                attackController.OnChargedAttack.AddListener(PlayChargedAttack);
            }

            if (healthController != null)
            {
                healthController.OnDamage.AddListener(PlayHitInternal);
                healthController.OnDeath.AddListener(PlayDeath);
            }
        }

        public void PlayWalk()
        {
            PlayClip(Walk);
        }

        public void PlayJump()
        {
            PlayClip(Jump);
        }

        public void PlaySwim()
        {
            PlayClip(Swim);
        }

        public void PlayDeath()
        {
            PlayClip(Death);
        }

        private void PlayHitInternal(GameObject go)
        {
            PlayHit();
        }
        public void PlayHit()
        {
            PlayClip(Hit);
        }

        public void PlayAttack()
        {
            PlayClip(Attack);
        }

        public void PlayChargedAttack()
        {
            PlayClip(ChargedAttack);
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip != null && source != null)
                source.PlayOneShot(clip);
        }

    }
}