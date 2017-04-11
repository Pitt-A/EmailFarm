using UnityEngine;
using System.Collections;

public class inboxScript : MonoBehaviour {
    bool mouseOver = false;
    const string buttonName = "Inbox";
    public bool clickedOn = false;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (mouseOver)
        {
            name = GUI.TextField(new Rect(Input.mousePosition.x + 10, -Input.mousePosition.y + 550, 60, 30), name);
        }
    }

    void OnMouseDown()
    {
        clickedOn = true;
        Debug.Log("Inbox clicked!");
    }

    void OnMouseOver()
    {
        if (!mouseOver)
        {
            mouseOver = true;
        }
    }

    void OnMouseExit()
    {
        if (mouseOver)
        {
            mouseOver = false;
        }
    }
}
