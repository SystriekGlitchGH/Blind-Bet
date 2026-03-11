using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Sensor : MonoBehaviour
{
    [SerializeField] EnemyMovement enemyMovement;
	private void Update()
	{
		transform.position = enemyMovement.transform.position;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			enemyMovement.target = collision.GetComponent<PlayerMovement>();
		}
	}
}
