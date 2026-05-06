using UnityEngine;
using UnityEngine.SceneManagement;
public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip fullHouse;
    [SerializeField] AudioClip area1;
    [SerializeField] AudioClip kharon;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
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
