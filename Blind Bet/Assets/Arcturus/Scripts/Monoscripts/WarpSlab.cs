using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
public class WarpSlab : MonoBehaviour
{
    [SerializeField] GameStats gameStats;
    Random rand = new Random();
    public void ChangeScene()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        pm.playerStats.Heal(15);
        if (gameStats.level >= 5)
        {
            if(rand.Next(0,101) >= 50)
                SceneManager.LoadScene("Kharon Room");
            return;
        }
        gameStats.level++;
        int levelIndex = rand.Next(0,gameStats.levelsAvailable.Count());
        string scenePickedName = gameStats.levelsAvailable[levelIndex];
        gameStats.levelsAvailable.Remove(scenePickedName);
        SceneManager.LoadScene(scenePickedName);
    }
}
