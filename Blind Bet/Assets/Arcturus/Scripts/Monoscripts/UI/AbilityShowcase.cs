using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AbilityShowcase : MonoBehaviour
{
    public TMP_Text abilityTitle;
    public TMP_Text abilityDesc;
    public Image suit;
    public Image icon;
    public Sprite[] icons;

    public void UpdateShowcase(string abilityCode, string abilityName, Card.Suit abilitySuit)
    {
        if(abilityCode == "na")
        {
            abilityTitle.text = "no ability";
            abilityDesc.text = "";
        }
    }
}
