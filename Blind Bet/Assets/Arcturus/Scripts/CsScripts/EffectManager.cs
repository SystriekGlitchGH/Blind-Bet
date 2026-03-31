using System.Collections.Generic;
using UnityEngine;

public class EffectManager
{
    public struct Effect
    {
        public string name;
        public float duration;
        public bool isApplied;
    }

    public List<Effect> effects;
}
