using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour {
    [SerializeField]
    [TextArea]
    private string text;
    [SerializeField]
    private InputField inputField;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(ButtonClick);
	}

    void ButtonClick()
    {
        inputField.text = text;
    }
}
