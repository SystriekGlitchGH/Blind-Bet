using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "Scriptable Objects/GameStats")]
public class GameStats : ScriptableObject
{
    public List<string> levelsAvailable;
    public int level;
    public float kills;
    public float enemies;
    public bool finishedTutorial;
}
