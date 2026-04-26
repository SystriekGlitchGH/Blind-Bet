using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameStats gamestats;
    Random rand = new Random();
    // void Awake()
    // {
    //     Instantiate(enemies[rand.Next(0,enemies.Count-1)],transform.position,Quaternion.Euler(Vector3.zero));
    // }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(enemies[rand.Next(0,enemies.Count-1)],transform.position,Quaternion.Euler(Vector3.zero));
            Destroy(gameObject);
        }
        
    }
}
