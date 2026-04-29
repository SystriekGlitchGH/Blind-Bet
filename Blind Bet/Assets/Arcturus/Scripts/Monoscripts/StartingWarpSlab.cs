using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Random = System.Random;
public class StartingWarpSlab : MonoBehaviour
{
    [SerializeField] GameStats gameStats;
    Random rand = new Random();
    public void ChangeScene()
    {
        gameStats.level++;
        int levelIndex = rand.Next(0,gameStats.levelsAvailable.Count());
        string scenePickedName = gameStats.levelsAvailable[levelIndex];
        gameStats.levelsAvailable.Remove(scenePickedName);
        SceneManager.LoadScene(scenePickedName);
    }
    public void SummonCardPicker()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        pm.playerUI.cardPicker.SetActive(true);
        Time.timeScale = 0;
    }
}
