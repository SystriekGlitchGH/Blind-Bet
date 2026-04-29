using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartingButton : MonoBehaviour
{
    public GameObject chosenCardsField;
    public TMP_Text buttonText;
    void Update()
    {
        if(buttonText.text != "Not Enough Cards" && chosenCardsField.transform.childCount < 2)
        {
            buttonText.text = "Not Enough Cards";
            buttonText.fontSize = 80;
        }
        else if(buttonText.text != "START" && chosenCardsField.transform.childCount == 2)
        {
            buttonText.text = "START";
            buttonText.fontSize = 200;
        }
    }
    public void StartRun()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        StartingWarpSlab sw = FindFirstObjectByType<StartingWarpSlab>();
        if(chosenCardsField.transform.childCount == 2)
        {
            for(int i = 0; i < 2; i++)
            {
                pm.playerStats.AddCard(chosenCardsField.transform.GetChild(i).GetComponent<DraggableItem>().card,1);
            }
            pm.playerStats.SortHandCards(pm.playerStats.activeHand, 1);
            Time.timeScale = 1;
            sw.ChangeScene();
            pm.playerUI.cardPicker.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("not enough cards");
            return;
        }
    }
}
