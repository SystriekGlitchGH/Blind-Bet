using UnityEngine;

[CreateAssetMenu(fileName = "GameStats", menuName = "Scriptable Objects/GameStats")]
public class GameStats : ScriptableObject
{
    public Player savedPlayerStats;
    public int level;
    public int kills;
    public int enemies;
}
