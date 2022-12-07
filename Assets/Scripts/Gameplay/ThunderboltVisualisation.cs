using UnityEngine;

namespace RuinsRaiders
{
    public class ThunderboltVisualisation : MonoBehaviour
    {
        private static readonly string ResolutionShaderProperty = "_Resolution";
        private static readonly string StartShaderProperty = "_Start";
        private static readonly string EndShaderProperty = "_End";
        private static readonly string VisibilityShaderProperty = "_DotLength";
        private static readonly string SeedShaderProperty = "_Seed";

        public GameObject start;
        private Vector3 _startOffset;

        public GameObject end;
        private Vector3 _endOffset;


        public bool positionBetweenEndpoints = true;

        [Range(0f, 1f)]
        public float visibility = 1;

        private Renderer _renderer;

        void Start()
        {
            _renderer = GetComponent<Renderer>();

            if(start != null)
            {
                var collider = start.GetComponent<Collider2D>();
                if (collider != null)
                    _startOffset = collider.bounds.center - collider.transform.position;
            }
            if (end != null)
            {
                var collider = end.GetComponent<Collider2D>();
                if (collider != null)
                    _endOffset = collider.bounds.center - collider.transform.position;
            }

            if (_renderer != null)
                _renderer.material.SetFloat(SeedShaderProperty, Random.value);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_renderer == null)
                return;

            Vector2 resolution = _renderer.material.GetVector(ResolutionShaderProperty);
            if (resolution == new Vector2())
                return;

            if (positionBetweenEndpoints)
                transform.position = Vector3.Lerp(start.transform.position, end.transform.position, 0.5f);

            var pos1 = start != null ? start.transform.position + _startOffset : transform.position;
            var pos2 = end != null ? end.transform.position + _endOffset : transform.position;

            pos1 = transform.worldToLocalMatrix.MultiplyPoint(pos1) * resolution;
            pos2 = transform.worldToLocalMatrix.MultiplyPoint(pos2) * resolution;


            _renderer.material.SetVector(StartShaderProperty, pos1);
            _renderer.material.SetVector(EndShaderProperty, pos2);

            if (visibility <= 0.1)
            {
                _renderer.material.SetFloat(VisibilityShaderProperty, -20);
            }
            else if (visibility < 0.2)
            {
                _renderer.material.SetFloat(VisibilityShaderProperty, -8);
            }
            if (visibility < 0.3)
            {
                _renderer.material.SetFloat(VisibilityShaderProperty, 1f);
            }
            else if (visibility < 0.4)
            {
                _renderer.material.SetFloat(VisibilityShaderProperty, 2);
            }
            else if (visibility < 0.5)
            {
                _renderer.material.SetFloat(VisibilityShaderProperty, 4f);
            }
            else
            {
                _renderer.material.SetFloat(VisibilityShaderProperty, resolution.x * resolution.y);
            }
        }
    }
}