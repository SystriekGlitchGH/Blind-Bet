using TMPro;
using UnityEngine;
using static Card;

public class KharonAcceptButton : MonoBehaviour
{
    public GameObject cardField;
    public TMP_Text buff;
    public TMP_Text debuff;
    public void AcceptHand()
    {
        Debug.Log("Button pressed");
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        for(int i = 0; i < 3; i++)
        {
            cardField.transform.GetChild(i).GetComponent<DraggableItem>().enabled = true;
            pm.playerStats.bench.Add(cardField.transform.GetChild(i).GetComponent<DraggableItem>().card);
            pm.playerStats.buffDebuffs.Add(buff.text);
            pm.playerStats.buffDebuffs.Add(debuff.text);
            Instantiate(cardField.transform.GetChild(i).gameObject,pm.playerUI.bench);
        }
    }
}