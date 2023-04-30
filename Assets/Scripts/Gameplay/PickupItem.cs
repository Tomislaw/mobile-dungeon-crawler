using UnityEngine;

namespace RuinsRaiders
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PickupItem : MonoBehaviour
    {
        public Sprite redGem;
        public Sprite blueGem;
        public Sprite silverGem;
        public Sprite greenGem;

        public PlayerData.GemsType type;

        public Color flashColor;
        public AnimationCurve flashCurve;

        public float Lifetime { 
            get => _lifetime; 
            set {
                _startinglifetime = value;
                _lifetime = value;
            }
        }

        private float _startinglifetime;
        [SerializeField]
        private float _lifetime;

        private SpriteRenderer _spriteRenderer;

        public Rigidbody2D Rigidbody2D { get => _body2D; }
        private Rigidbody2D _body2D;

        void Awake()
        {
            _startinglifetime = _lifetime;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _body2D = GetComponent<Rigidbody2D>();
            _spriteRenderer.sprite = type switch
            {
                PlayerData.GemsType.Red => redGem,
                PlayerData.GemsType.Blue => blueGem,
                PlayerData.GemsType.Green => greenGem,
                PlayerData.GemsType.Silver => silverGem,
                _ => null,
            };

            _body2D.velocity += new Vector2(Random.Range(0,10f), Random.Range(0, 10f));

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            var flash = flashCurve.Evaluate(1f - _lifetime / _startinglifetime) > 0.5f;
            _spriteRenderer.material.SetFloat("Flash", flash ? 1: 0);

            _lifetime -= Time.fixedDeltaTime;
            if (_lifetime < 0)
                Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var items = collision.gameObject.GetComponent<ItemsController>();
            if(items != null)
            {
                items.AddGems(gameObject, PlayerData.Gems.One(type));
                Lifetime = items.gemLifetime;
                _body2D.simulated = false;
                _body2D.isKinematic = true;
            }
        }
    }
}
