using UnityEngine;

namespace RuinsRaiders.UI
{
    // responsible for showing amount of currency player currently have
    public class GemsUI : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text tmpTextRedGems;
        [SerializeField]
        private TMPro.TMP_Text tmpTextBlueGems;
        [SerializeField]
        private TMPro.TMP_Text tmpTextGreenGems;
        [SerializeField]
        private TMPro.TMP_Text tmpTextSilverGems;

        [SerializeField]
        private PlayerData _data;

        private int _redGemsCount;
        private int _blueGemsCount;
        private int _greenGemsCount;
        private int _silverGemsCount;

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
            if (_redGemsCount != _data.gems.redGems)
                SetText(tmpTextRedGems, _redGemsCount = _data.gems.redGems);
            if (_blueGemsCount != _data.gems.blueGems)
                SetText(tmpTextBlueGems, _blueGemsCount = _data.gems.blueGems);
            if (_greenGemsCount != _data.gems.greenGems)
                SetText(tmpTextGreenGems, _greenGemsCount = _data.gems.greenGems);
            if (_silverGemsCount != _data.gems.silverGems)
                SetText(tmpTextSilverGems, _silverGemsCount = _data.gems.silverGems);
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