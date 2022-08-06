using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTesting : MonoBehaviour
{

    public Color color;
    public float grey = 0;
    public Color gamma;
    public Color linear;
    // Start is called before the first frame update
    private void OnValidate()
    {
        grey = color.grayscale;
        gamma = color.gamma;
        linear = color.linear;
    }
}
