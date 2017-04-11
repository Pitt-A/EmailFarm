using UnityEngine;
using System.Collections;

public class exitScript : MonoBehaviour {
    bool mouseOver = false;
    const string buttonName = "Close Program";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (mouseOver)
        {
            name = GUI.TextField(new Rect(Input.mousePosition.x + 10, -Input.mousePosition.y + 550, 150, 30), name);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("WARNING: This does not work without building the applicaiton first!");
        Application.Quit();
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
