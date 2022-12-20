using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPicker : MonoBehaviour
{
    public GameObject GUI;
    public Color Color;
    public Image colorImage;
    public Slider Red;
    public Slider Green;
    public Slider Blue;
    public bool isActive = false;
    public InputMenu Menu;

    private int currId = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnValidate()
    {
        colorImage.color = Color;
    }

    public void updateColor(float value)
    {
        Color = new Color(Red.value, Green.value, Blue.value);
        colorImage.color = Color;
    }

    public void ShowGUI(int ID) {
        currId = ID;
        GUI.SetActive(true);
        isActive = true;
    }

    public void Press() {
        GUI.SetActive(false);
        Menu.updateColor(Color, currId);
        currId = 0;
        isActive = false;
    }
}
