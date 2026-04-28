using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerMovement pm;
    public GameObject cardManager;
    [Header("Health")]
    public Slider healthbarSlider;
    public TMP_Text healthbarText;

    [Header("Chips")]
    public Slider chipbarSlider;
    public TMP_Text chipbarText;
    public Image[] chips = new Image[6];
    public Image currentChip;

    [Header("Hands")]
    public Transform bench;
    public bool cardManagerOpen;
    public AbilityShowcase abilityShowcase1;
    public AbilityShowcase abilityShowcase2;
    public AbilityShowcase abilityShowcase3;


    private void Start()
    {
        cardManager.SetActive(false);
    }
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
            chipbarSlider.value = Mathf.MoveTowards(chipbarSlider.value, pm.playerStats.currentChips/pm.playerStats.maxChips, 1f * Time.deltaTime);
        if(chipbarText.text != ""+pm.playerStats.currentChips)
            chipbarText.SetText("{0}",pm.playerStats.currentChips);
        if(chipbarSlider.value == 1 && chips[5].sprite != currentChip.sprite)
            SwitchChipPicture(5);
        else if(chipbarSlider.value >= 0.8f && chipbarSlider.value < 1 && chips[4].sprite != currentChip.sprite)
            SwitchChipPicture(4);
        else if(chipbarSlider.value >= 0.6f && chipbarSlider.value < 0.8f && chips[3].sprite != currentChip.sprite)
            SwitchChipPicture(3);
        else if(chipbarSlider.value >= 0.4f && chipbarSlider.value < 0.6f && chips[2].sprite != currentChip.sprite)
            SwitchChipPicture(2);
        else if(chipbarSlider.value > 0.1f && chipbarSlider.value < 0.4f && chips[1].sprite != currentChip.sprite)
            SwitchChipPicture(1);
        else if(chipbarSlider.value >= 0 && chipbarSlider.value <= 0.1f && chips[0].sprite != currentChip.sprite)
            SwitchChipPicture(0);
    }
    public void OpenCardMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            abilityShowcase1.UpdateShowcase(pm.playerStats.activeAbility.code, pm.playerStats.activeAbility.name);
            abilityShowcase2.UpdateShowcase(pm.playerStats.passiveAbility1.code, pm.playerStats.passiveAbility1.name);
            abilityShowcase3.UpdateShowcase(pm.playerStats.passiveAbility2.code, pm.playerStats.passiveAbility2.name);
            if (!cardManager.activeInHierarchy)
            {
                cardManager.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                cardManager.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    private void SwitchChipPicture(int chipIndex)
    {
        currentChip.sprite = chips[chipIndex].sprite;
    }

    public void AddCardToHand(MenuField.Field field)
    {
        pm.playerStats.AddCard(field.card, field.handNum);
        abilityShowcase1.UpdateShowcase(pm.playerStats.activeAbility.code, pm.playerStats.activeAbility.name);
        abilityShowcase2.UpdateShowcase(pm.playerStats.passiveAbility1.code, pm.playerStats.passiveAbility1.name);
        abilityShowcase3.UpdateShowcase(pm.playerStats.passiveAbility2.code, pm.playerStats.passiveAbility2.name);
    }

    public void RemoveCardFromHand(MenuField.Field field)
    {
        pm.playerStats.RemoveCard(field.card, field.handNum);
        abilityShowcase1.UpdateShowcase(pm.playerStats.activeAbility.code, pm.playerStats.activeAbility.name);
        abilityShowcase2.UpdateShowcase(pm.playerStats.passiveAbility1.code, pm.playerStats.passiveAbility1.name);
        abilityShowcase3.UpdateShowcase(pm.playerStats.passiveAbility2.code, pm.playerStats.passiveAbility2.name);
    }
}
