using System;
using UnityEngine;

[Serializable]
public class Effect
{
    public string name;
    public float duration;
    public float elapsedTime;
    public Effect(string n, float d)
    {
        name = n;
        duration = d;
        elapsedTime = 0;
    }
}
