using System.Collections;
using UnityEngine;

public class FadeFromStart : MonoBehaviour
{
    public CanvasGroup canvasG;
    void Start()
    {
        StartCoroutine(RemoveTimer());
    }
    private IEnumerator RemoveTimer()
    {
        yield return new WaitForSeconds(5);
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            canvasG.alpha -= 0.01f;
        }
        canvasG.alpha = 0;
    }
}
