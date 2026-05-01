using TMPro;
using UnityEngine;

public class KharonAcceptButton : MonoBehaviour
{
    public GameObject cardField;
    public TMP_Text buff;
    public TMP_Text debuff;
    void Awake()
    {
        PlayerMovement pm = FindFirstObjectByType<PlayerMovement>();
        for(int i = 0; i < 3; i++)
        {
            transform.GetChild(i).GetComponent<DraggableItem>();
        }
    }
}
