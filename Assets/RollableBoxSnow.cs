using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RuinsRaiders
{
    [RequireComponent(typeof(RollableBox))]
    public class RollableBoxSnow : MonoBehaviour
    {
        [SerializeField]
        private Sprite spriteTop;

        [SerializeField]
        private Sprite spriteTopLeft;

        [SerializeField]
        private Sprite spriteTopRight;

        [SerializeField]
        private Sprite spriteTopBottom;

        [SerializeField]
        private Sprite spriteTopLeftRight;

        [SerializeField]
        private Sprite spriteTopLeftBottom;

        [SerializeField]
        private Sprite spriteTopRightBottom;

        [SerializeField]
        private Sprite spriteAll;


        [SerializeField]
        private bool hasTop;

        private bool _hasRight;
        private bool _hasLeft;
        private bool _hasBottom;

        private RollableBox _box;
        private AStar _astar;
        private SpriteRenderer _spriteRenderer;

        private 

        void Start()
        {
            _astar = FindObjectOfType<AStar>();
            _box = GetComponent<RollableBox>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _box.OnRoll.AddListener(OnRoll);
            _box.OnFall.AddListener(OnRoll);
        }

        private void OnRoll()
        {
            var tileId = _astar.GetTileId(transform.position);
            var tile1 = _astar.GetTile(tileId);

            if (tile1 != null && tile1.name.Contains("Snow")) {
                ApplySnow();
                return;
            }

            var tile2 = _astar.GetTile(tileId + new Vector2Int(0, -1));
            if (tile2 != null && tile2.name.Contains("Snow")) { 
                ApplySnow();
                return;
            }
        }

        private void ApplySnow()
        {
            if (_spriteRenderer == null)
                return;

            if(hasTop == false)
                _box.SetRotation(180);
           
            int rotation = (int) Mathf.Ceil((_box.Rotation - 15f) / 90f);
            switch (rotation)
            {
                case 3:
                    if (_hasRight)
                        return;
                    _hasRight = true;
                    break;
                case 2:
                    if (hasTop)
                        return;
                    hasTop = true;
                break;
                case 1:
                    if (_hasLeft)
                        return;
                    _hasLeft = true;
                break;
                case 0:
                    if (_hasBottom)
                        return;
                    _hasBottom = true;
                    break;
            }


            if(hasTop && _hasBottom && _hasLeft && _hasRight)
                _spriteRenderer.sprite = spriteAll;
            else if (hasTop && _hasBottom && _hasLeft)
                _spriteRenderer.sprite = spriteTopLeftBottom;
            else if (hasTop && _hasBottom && _hasRight)
                _spriteRenderer.sprite = spriteTopRightBottom;
            else if (hasTop && _hasLeft && _hasRight)
                _spriteRenderer.sprite = spriteTopLeftRight;
            else if (hasTop && _hasBottom )
                _spriteRenderer.sprite = spriteTopBottom;
            else if (hasTop && _hasRight)
                _spriteRenderer.sprite = spriteTopRight;
            else if (hasTop && _hasLeft)
                _spriteRenderer.sprite = spriteTopLeft;
            else if (hasTop)
                _spriteRenderer.sprite = spriteTop;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}