using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogItem : MonoBehaviour {
    
    private string label;
    private Color bgColor;

    public void Set(string text, Color color)
    {
        transform.GetChild(0).GetComponent<Text>().text = text;
        GetComponent<Image>().color = color;
    }
}
