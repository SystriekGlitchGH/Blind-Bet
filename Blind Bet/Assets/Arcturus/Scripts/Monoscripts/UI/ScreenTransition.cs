using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ScreenTransition : MonoBehaviour
{
    public CanvasGroup canvasG;
    void Start()
    {
        StartCoroutine(FadeFromBlack());
    }

    public IEnumerator FadeFromBlack()
    {
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            canvasG.alpha -= 0.01f;
        }
        canvasG.alpha = 0;
        gameObject.SetActive(false);
    }
    public IEnumerator FadeToBlack()
    {
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            canvasG.alpha += 0.01f;
        }
        canvasG.alpha = 1;
    }
}
