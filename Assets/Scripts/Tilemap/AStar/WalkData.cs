using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuinsRaiders
{
    [Serializable]
    public struct WalkData
    {
        public float maxTimeOnNode;
        public bool flying;
        public bool canSwim;
        public bool canUseLadder;
        public int height;
        public int jumpHeight;
        public int jumpDistance;
    }
}
