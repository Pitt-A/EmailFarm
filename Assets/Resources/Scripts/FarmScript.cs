using UnityEngine;
using System.Collections;

public class FarmScript : MonoBehaviour {
    Sprite[] farmSprites;
    public int currentPlantStage = 0;
    public bool clickedOn = false, guiEnabled = false, rightClickedOn = false;
    bool activateGUI = false;
    string header;
    string body;
    string shortenedMessage;

    // Use this for initialization
    void Start ()
    {
        farmSprites = new Sprite[5];
        farmSprites[0] = Resources.Load<Sprite>("Sprites/farmland");
        farmSprites[1] = Resources.Load<Sprite>("Sprites/plant");

        int tempInt = Random.Range(0, 3);
        switch (tempInt)
        {
            case 0:
                farmSprites[2] = Resources.Load<Sprite>("Sprites/carrot");
                break;
            case 1:
                farmSprites[2] = Resources.Load<Sprite>("Sprites/onion");
                break;
            case 2:
                farmSprites[2] = Resources.Load<Sprite>("Sprites/tomato");
                break;
            default:
                Debug.Log("Error: farmSprite[2] is not assigned!");
                break;
        }
        farmSprites[3] = Resources.Load<Sprite>("Sprites/weeds");
        farmSprites[4] = Resources.Load<Sprite>("Sprites/holes");

        shortenedMessage = " ";
	}

    public void SetStrings(string _header, string _body)
    {
        header = _header;
        body = _body;

        if (_header.Length > 50)
        {
            shortenedMessage = header.Remove(50) + "\n" + body;
        }
        else
        {
            shortenedMessage = header + "\n\n" + body;
        }        
    }
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<SpriteRenderer>().sprite = farmSprites[currentPlantStage];
	}

    void OnMouseDown()
    {
        clickedOn = true;
    }

    void OnMouseOver()
    {
        if (!activateGUI)
        {
            activateGUI = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            rightClickedOn = true;
        }
    }

    void OnMouseExit()
    {
        if (activateGUI)
        {
            activateGUI = false;
        }
    }

    void OnGUI()
    {
        GUI.skin.font = (Font)Resources.Load("Fonts/Pixeled");

        if (activateGUI && guiEnabled)
        {
            shortenedMessage = GUI.TextArea(new Rect(Input.mousePosition.x + 10, -Input.mousePosition.y + 550, 400, 200), shortenedMessage);
        }
    }

    public void ChangePlantGrowth(int tempInt)
    {
        if (tempInt >= 0 || tempInt <= farmSprites.Length)
        {
            currentPlantStage = tempInt;
        }
    }
}
