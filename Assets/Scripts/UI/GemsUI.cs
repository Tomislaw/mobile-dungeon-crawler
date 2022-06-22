using UnityEngine;

namespace RuinsRaiders.UI
{
    // responsible for showing amount of currency player currently have
    public class GemsUI : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text redGems;
        [SerializeField]
        private TMPro.TMP_Text blueGems;
        [SerializeField]
        private TMPro.TMP_Text greenGems;
        [SerializeField]
        private TMPro.TMP_Text silverGems;

        [SerializeField]
        private PlayerData data;

        private int red;
        private int blue;
        private int green;
        private int silver;

        private void Start()
        {
            Invalidate();
        }

        private void FixedUpdate()
        {
            Invalidate();
        }

        public void Invalidate()
        {
            if (red != data.gems.redGems)
                SetText(redGems, red = data.gems.redGems);
            if (blue != data.gems.blueGems)
                SetText(blueGems, blue = data.gems.blueGems);
            if (green != data.gems.greenGems)
                SetText(greenGems, green = data.gems.greenGems);
            if (silver != data.gems.silverGems)
                SetText(silverGems, silver = data.gems.silverGems);
        }

        private void SetText(TMPro.TMP_Text text, int count)
        {
            if (count >= 1000)
                text.text = "+999";
            else if (count > 99)
                text.text = count.ToString();
            else if (count >= 10)
                text.text = "0" + count.ToString();
            else if (count >= 0)
                text.text = "00" + count.ToString();
        }
    }
}