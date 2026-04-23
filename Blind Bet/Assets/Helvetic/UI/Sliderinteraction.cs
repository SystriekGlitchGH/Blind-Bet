using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Slider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textField;
    [SerializeField] private bool ShowDecimalPoints;


    private void Reset()
    {
        slider = GetComponent<Slider>();
        textField = GetComponentInChildren<TMP_Text>();
    }
    
    void HandleSliderValueChanged(float value)
    {
        if (ShowDecimalPoints)
            textField.SetText(value.ToString(format:"F2"));
        else
            textField.SetText(value.ToString(format:"F0"));
    }
}
