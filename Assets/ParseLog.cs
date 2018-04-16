using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseLog : MonoBehaviour {

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private float padding = 4;
    [SerializeField]
    private Transform content;
    private Vector3 topPosition = new Vector3(0,-32,0);
    private float height;

    private List<GameObject> items = new List<GameObject>();

	// Use this for initialization
	void Start () {
        height = itemPrefab.GetComponent<RectTransform>().rect.height;
	}

    public void AddItem(string text, Color color)
    {
        Vector3 newPosition = new Vector3(0, topPosition.y - ((height + padding) * items.Count), 0);
        GameObject tmp = Instantiate(itemPrefab, content);
        tmp.GetComponent<RectTransform>().anchoredPosition = newPosition;
        tmp.GetComponent<LogItem>().Set(text, color);
        items.Add(tmp);
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (50 + padding) * items.Count);
    }

    public void Clear()
    {
        foreach(GameObject obj in items)
            Destroy(obj);

        items = new List<GameObject>();
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 50);
    }
}
