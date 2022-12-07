using System.Collections.Generic;
using UnityEngine;

namespace RuinsRaiders.GUI
{
    // Responsible for showinh hearts above player and enemies
    public class ShieldUI : HealthUI
    {

        protected override int GetHealth()
        {
            if (healthController is ShieldedHealthController)
                return (healthController as ShieldedHealthController).shield;
            else return 0;
        }

        protected override int GetMaxHealth()
        {
            if (healthController is ShieldedHealthController)
                return (healthController as ShieldedHealthController).maxShield;
            else return 0;
        }

    }
}