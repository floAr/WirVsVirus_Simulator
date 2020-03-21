using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Directive : MonoBehaviour
{
    public string Title;
    public bool Active;
    public bool RequiresValue;
    public float SliderMin, SliderMax;
    public float CurrentValue;

    public Toggle ActiveToggle;
    public Slider ValueSlider;
    public TextMeshProUGUI TitleField;

    private void Start()
    {
        ValueSlider.minValue = SliderMin;
        ValueSlider.maxValue = SliderMax;
        ValueSlider.value = CurrentValue;
        TitleField.SetText(Title);
        if (!RequiresValue)
            ValueSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        Active = ActiveToggle.isOn;
        CurrentValue = ValueSlider.value;

    }
}
