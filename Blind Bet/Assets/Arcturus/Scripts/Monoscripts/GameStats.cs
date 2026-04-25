using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "Scriptable Objects/GameStats")]
public class GameStats : ScriptableObject
{
    public List<string> levelsAvailable;
    public int level;
    public int kills;
    public int enemies;
}
