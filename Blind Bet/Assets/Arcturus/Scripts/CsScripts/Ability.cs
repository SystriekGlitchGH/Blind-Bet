using UnityEngine;
using System;

[Serializable]
public class Ability
{
    public string name;
    public string code;
    public Ability(string name, string code)
    {
        this.name = name;
        this.code = code;
    }
}
