using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Sensor : MonoBehaviour
{
    [SerializeReference] EnemyMovement enemyMovement;
	private void Update()
	{
		transform.position = enemyMovement.transform.position;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			enemyMovement.enemyTarget = collision.GetComponent<PlayerMovement>();
			enemyMovement.movementTarget = collision.transform;
		}
		if (collision.CompareTag("Enemy"))
		{
			EnemyMovement em = collision.GetComponent<EnemyMovement>();
			if(em.isHealer == true)
			{
				enemyMovement.healerTarget = em;
			}
		}
	}
}