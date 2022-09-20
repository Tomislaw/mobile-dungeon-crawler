using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinsRaiders
{
    [Serializable]
    public struct WalkData
    {
        [HideInInspector]
        public bool flying;
        [HideInInspector]
        public bool canSwim;
        [HideInInspector]
        public bool canUseLadder;
        [HideInInspector]
        public bool canUsePlatform;

        public float maxTimeOnNode;
        public int height;
        public int jumpHeight;
        public int jumpDistance;
    }
}
