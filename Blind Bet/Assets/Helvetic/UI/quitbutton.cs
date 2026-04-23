using UnityEngine;

public class quitbutton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();

    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
