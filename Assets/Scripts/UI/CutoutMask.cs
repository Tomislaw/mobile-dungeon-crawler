using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace RuinsRaiders.UI
{
    // Helper function for creating transparent holes in images
    public class CutoutMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                var mmaterial = new Material(base.materialForRendering);
                mmaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return mmaterial;
            }

        }
    }
}
