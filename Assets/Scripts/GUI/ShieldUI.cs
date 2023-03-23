using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showinh hearts above player and enemies
    public class ShieldUI : HealthUI
    {

        protected override int GetHealth()
        {
            return healthController.shield;
        }

        protected override int GetMaxHealth()
        {
            return healthController.maxShield;
        }

    }
}