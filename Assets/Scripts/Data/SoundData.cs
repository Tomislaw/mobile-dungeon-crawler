using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SoundData", menuName = "RuinsRaiders/SoundData", order = 1)]
public class SoundData : ScriptableObject
{
    public List<SoundPair> sounds;

    public class SoundPair
    {
        public string name;
        public AudioClip clip;
    }
}