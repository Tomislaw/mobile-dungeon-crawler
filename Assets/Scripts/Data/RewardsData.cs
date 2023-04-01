using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RuinsRaiders.PlayerData;

namespace RuinsRaiders
{
    [CreateAssetMenu(fileName = "RewardsData", menuName = "RuinsRaiders/RewardsData", order = 1)]
    public class RewardsData : ScriptableObject
    {
        public Gems normalChestReward;
        public Gems exquisiteChestReward;
    }
}
