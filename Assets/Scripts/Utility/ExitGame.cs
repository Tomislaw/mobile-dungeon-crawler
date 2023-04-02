using UnityEngine;

namespace RuinsRaiders
{
    public class ExitGame : MonoBehaviour
    {
        public void Exit()
        {
            SaveableData.SaveAll();
            Application.Quit();
        }
    }
}