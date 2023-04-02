using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.UI
{
    public class JumpButton : MonoBehaviour
    {
        [SerializeField]
        private ControlData control;

        private bool _isJumping;

        // Update is called once per frame

        private void Start()
        {
            control.move.y = 0;
        }
        private void LateUpdate()
        {
            if (_isJumping)
                control.move.y = 1;
        }
        private void Update()
        {
            if (_isJumping)
                control.move.y = 1;
        }

        private void FixedUpdate()
        {
            if (_isJumping)
                control.move.y = 1;
        }


        public void Jump(bool jump)
        {
            _isJumping = jump;
            if(jump == false)
                control.move.y = 0;
        }
    }
}