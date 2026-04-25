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
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerMovement pm))
        {
            gameStats.level++;
            int levelIndex = rand.Next(0,gameStats.levelsAvailable.Count()-1);
            string scenePickedName = gameStats.levelsAvailable[levelIndex];
            gameStats.levelsAvailable.Remove(scenePickedName);
            SceneManager.LoadScene(scenePickedName);
        }
    }
}
