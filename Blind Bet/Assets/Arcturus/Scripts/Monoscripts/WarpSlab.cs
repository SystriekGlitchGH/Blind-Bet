using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WarpSlab : MonoBehaviour
{
    [SerializeField] GameStats gameStats;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerMovement pm))
        {
            gameStats.level++;
            // change to be random later, testing saving rn
            SceneManager.LoadScene("Map000");
        }
    }
}
