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
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        pm.playerStats.Heal(15);
        if(pm.playerUI.screenTransition.TryGetComponent(out ScreenTransition st))
        {
            pm.playerUI.screenTransition.SetActive(true);
            StartCoroutine(st.FadeToBlack());
        }
        if(level == "Map1 Presentation" && !hasPicked)
        {
            pm.playerUI.cardPicker.SetActive(true);
            hasPicked = true;
            Time.timeScale = 0;
        }
        else
        {
            gameStats.level++;
            pm.playerStats.Heal(15);
            StartCoroutine(NextSceneTimer(level));
        }
        
    }
    private IEnumerator NextSceneTimer(string name)
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(name);
    }
}
