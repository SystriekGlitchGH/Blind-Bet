using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class WarpPresentation : MonoBehaviour
{
    [SerializeField] GameStats gameStats;
    public string level;
    private bool hasPicked;
    Random rand = new Random();
    public void ChangeScene()
    {
        if(level == "Map1 Presentation" && !hasPicked)
        {
            PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
            pm.playerUI.cardPicker.SetActive(true);
            hasPicked = true;
            Time.timeScale = 0;
        }
        else
        {
            gameStats.level++;
            PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
            pm.playerStats.Heal(15);
            SceneManager.LoadScene(level);
        }
        
    }
}
