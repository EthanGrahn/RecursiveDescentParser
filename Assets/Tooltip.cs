using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    [SerializeField]
    private GameObject tooltipObject;

    private void Start()
    {
        tooltipObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        tooltipObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        tooltipObject.SetActive(false);
    }
}
