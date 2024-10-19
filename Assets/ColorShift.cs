using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorShift : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.color = Color.HSVToRGB(Time.time/3 - (int)(Time.time/3), 1, 1);
    }
}
