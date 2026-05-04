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
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        if(pm.playerUI.screenTransition.TryGetComponent(out ScreenTransition st))
        {
            pm.playerUI.screenTransition.SetActive(true);
            StartCoroutine(st.FadeToBlack());
        }
        gameStats.level++;
        int levelIndex = rand.Next(0,gameStats.levelsAvailable.Count());
        string scenePickedName = gameStats.levelsAvailable[levelIndex];
        gameStats.levelsAvailable.Remove(scenePickedName);
        StartCoroutine(NextSceneTimer(scenePickedName));
    }
    public void SummonCardPicker()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        pm.playerUI.cardPicker.SetActive(true);
        Time.timeScale = 0;
    }
    private IEnumerator NextSceneTimer(string name)
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(name);
    }
}
