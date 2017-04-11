using UnityEngine;
using System.Collections;

public class trashScript : MonoBehaviour {
    bool mouseOver = false;
    const string buttonName = "Junk Folder";
    FarmManagementScript manageScript;
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
            name = GUI.TextField(new Rect(Input.mousePosition.x + 10, -Input.mousePosition.y + 550, 120, 30), name);
        }
    }

    void OnMouseDown()
    {
        clickedOn = true;
        Debug.Log("Trash clicked!");
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
