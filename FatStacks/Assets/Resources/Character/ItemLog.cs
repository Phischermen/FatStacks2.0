using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemLog : MonoBehaviour
{
    public static ItemLog singleton;

    public float textHeight;
    public GameObject logEntry;
    public LinkedList<GameObject> log = new LinkedList<GameObject>();

    private void Start()
    {
        if (!singleton)
        {
            singleton = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    public static void AddItem(string text)
    {
        GameObject entry = Instantiate(singleton.logEntry, singleton.transform);
        entry.GetComponent<Text>().text = text;
        singleton.log.AddFirst(entry);
        singleton.ArrangeEntries();
    }

    void ArrangeEntries()
    {
        int i = 0;
        foreach (GameObject obj in log)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, textHeight * i);
            ++i;
        }
    }
}
