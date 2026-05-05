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
        if(pm.playerUI.screenTransition.TryGetComponent(out ScreenTransition st))
        {
            pm.playerUI.screenTransition.SetActive(true);
            StartCoroutine(st.FadeToBlack());
        }
        if(gameStats.levelsAvailable.Count() == 0)
        {
            StartCoroutine(NextSceneTimer("BossArena1"));
        }
        if (gameStats.level >= 5)
        {
            if(rand.Next(0,101) >= 50)
                StartCoroutine(NextSceneTimer("Kharon Room"));
            return;
        }
        gameStats.level++;
        int levelIndex = rand.Next(0,gameStats.levelsAvailable.Count());
        string scenePickedName = gameStats.levelsAvailable[levelIndex];
        gameStats.levelsAvailable.Remove(scenePickedName);
        StartCoroutine(NextSceneTimer(scenePickedName));
    }
    private IEnumerator NextSceneTimer(string name)
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(name);
    }
}
