using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    class KeepScaleX : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(transform.parent.localScale.x))
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
