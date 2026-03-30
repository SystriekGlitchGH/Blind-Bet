using UnityEngine;

public class ContBlade : Bullet
{
    protected void Start()
    {
        transform.localScale = pm.playerStats.weapon.baseAttackSize*pm.playerStats.GetAttackSizeMod();
    }
    protected override void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= 0.5)
        {
            Destroy(gameObject);
        }
    }
}
