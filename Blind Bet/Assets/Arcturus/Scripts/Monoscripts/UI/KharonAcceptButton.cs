using TMPro;
using UnityEngine;

public class KharonAcceptButton : MonoBehaviour
{
    public GameObject cardField;
    public TMP_Text buff;
    public TMP_Text debuff;
    public void AcceptHand()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        for(int i = 0; i < 3; i++)
        {
            DraggableItem di = cardField.transform.GetChild(i).GetComponent<DraggableItem>();
            di.enabled = true;
            pm.playerStats.bench.Add(di.card);
            Instantiate(di,pm.playerUI.bench);
        }
        pm.playerStats.buffDebuffs.Add(buff.text);
        pm.playerStats.buffDebuffs.Add(debuff.text);
        Time.timeScale = 1;
        pm.playerUI.kharonMenu.SetActive(false);
    }
}
