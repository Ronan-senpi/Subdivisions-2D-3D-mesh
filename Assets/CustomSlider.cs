using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private string title;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private bool asInt = true;

    private void FixedUpdate()
    {

        label.text = title;
        label.text += asInt ? (int)slider.value : slider.value;
    }
}
