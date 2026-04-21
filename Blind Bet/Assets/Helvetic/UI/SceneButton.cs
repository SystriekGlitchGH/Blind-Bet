using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public void LoadLevel(int scene1) 
    {
        SceneManager.LoadScene(scene1);
    }
}
