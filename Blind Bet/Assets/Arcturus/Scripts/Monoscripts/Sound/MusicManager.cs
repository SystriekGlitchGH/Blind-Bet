using UnityEngine;
using UnityEngine.SceneManagement;
public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip fullHouse;
    [SerializeField] AudioClip area1;
    [SerializeField] AudioClip kharon;
    private static MusicManager instance;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Full House")
        {
            audioSrc.clip = fullHouse;
            audioSrc.Play();
        }
        else if(scene.name == "Kharon Room" || scene.name == "Kharon Room Presentation")
        {
            audioSrc.clip = kharon;
            audioSrc.Play();
        }
        else if(scene.name == "MainMenu")
        {
            audioSrc.Stop();
        }
        else
        {
            AudioClip previousClip = audioSrc.clip;
            audioSrc.clip = area1;
            if(previousClip != audioSrc.clip)
            {
                audioSrc.Play();
            }
        }
    }
}
