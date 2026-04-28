using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderInteraction : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textField;
    [SerializeField] private bool showDecimalPoints;


    private void Reset()
    {
        slider = GetComponent<Slider>();
        textField = GetComponentInChildren<TMP_Text>();
    }

    public void HandleSliderValueChanged(float value)
    {
        if (showDecimalPoints)
            textField.SetText(value.ToString(format: "F2") + "%");
        else
            textField.SetText(value.ToString(format: "F0"));
    }
}
