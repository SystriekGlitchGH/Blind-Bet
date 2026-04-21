using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public EnemyMovement em;
    public Slider healthbarSlider;
    void Update()
    {
        if(healthbarSlider.value != em.enemyStats.currentHealth / em.enemyStats.maxHealth)
        {
            healthbarSlider.value = Mathf.MoveTowards(healthbarSlider.value, em.enemyStats.currentHealth / em.enemyStats.maxHealth, 2f * Time.deltaTime);
        }
    }
}
