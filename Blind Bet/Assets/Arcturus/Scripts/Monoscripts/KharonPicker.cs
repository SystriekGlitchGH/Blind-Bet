using UnityEngine;

public class KharonPicker : MonoBehaviour
{
    private bool interactedWith;
    public void SummonKharonMenu()
    {
        if (!interactedWith)
        {
            PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
            pm.playerUI.kharonMenu.SetActive(true);
            Time.timeScale = 0;
            interactedWith = true;
        }
    }
}
