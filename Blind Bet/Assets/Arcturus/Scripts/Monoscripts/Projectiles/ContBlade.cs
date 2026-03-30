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
        if(elapsedTime > 1)
        {
            Destroy(gameObject);
        }
    }
}
