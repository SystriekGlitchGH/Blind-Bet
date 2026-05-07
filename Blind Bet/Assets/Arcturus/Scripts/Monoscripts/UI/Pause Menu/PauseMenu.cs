using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public PlayerUI playerUI;
    public GameObject settingsSubMenu;
    public void ResumeFromPause()
    {
        if(!playerUI.cardManager.activeSelf && !playerUI.manual.activeSelf)
            Time.timeScale = 1;
        playerUI.pauseMenu.SetActive(false);
    }
    public void OpenSettings()
    {
        if(!settingsSubMenu.activeSelf)
            settingsSubMenu.SetActive(true);
        else
            settingsSubMenu.SetActive(false);
    }
    public void ToMainMenu()
    {
        Time.timeScale = 1;
        string json = JsonUtility.ToJson(playerUI.pm.gameStatsReset);
        JsonUtility.FromJsonOverwrite(json, playerUI.pm.gamestats);
        json = JsonUtility.ToJson(playerUI.pm.playerReset);
        JsonUtility.FromJsonOverwrite(json, playerUI.pm.playerStats);
        SceneManager.LoadScene("MainMenu");
    }
    // private IEnumerator ToMainMenuTimer()
    // {
    //     if(playerUI.screenTransition.TryGetComponent(out ScreenTransition st))
    //     {
    //         StartCoroutine(st.FadeToBlack());
    //         yield return new WaitForSeconds(2.5f);
    //         string json = JsonUtility.ToJson(playerUI.pm.gameStatsReset);
    //         JsonUtility.FromJsonOverwrite(json, playerUI.pm.gamestats);
    //         json = JsonUtility.ToJson(playerUI.pm.playerReset);
    //         JsonUtility.FromJsonOverwrite(json, playerUI.pm.playerStats);
    //         SceneManager.LoadScene("MainMenu");
    //     }
        
    // }
    public void QuitGame()
    {
        Application.Quit();
    }
}
