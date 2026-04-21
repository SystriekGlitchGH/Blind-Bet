using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerMovement pm;
    [Header("Health")]
    public Slider healthbarSlider;
    public TMP_Text healthbarText;

    [Header("Chips")]
    public Slider chipbarSlider;
    public TMP_Text chipbarText;


    private void Update()
    {
        // health
        if(healthbarSlider.value != pm.playerStats.currentHealth / pm.playerStats.maxHealth)
        {
            healthbarSlider.value = Mathf.MoveTowards(healthbarSlider.value, pm.playerStats.currentHealth / pm.playerStats.maxHealth, 2f * Time.deltaTime);
        }
            
        if(healthbarText.text != pm.playerStats.currentHealth+"/"+pm.playerStats.maxHealth)
            healthbarText.SetText("{0}"+"/"+"{1}",pm.playerStats.currentHealth,pm.playerStats.maxHealth);
        // chips
        if(chipbarSlider.value != pm.playerStats.currentChips/pm.playerStats.maxChips)
            chipbarSlider.value = Mathf.MoveTowards(chipbarSlider.value, pm.playerStats.currentChips/pm.playerStats.maxChips, 1f * Time.deltaTime);;
        if(chipbarText.text != ""+pm.playerStats.currentChips)
            chipbarText.SetText("{0}",pm.playerStats.currentChips);
    }
}
