using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImapX;

public class FarmManagementScript : MonoBehaviour { 
    //GameObject highlighterObject;
    List<Transform> farmlandTransforms;
    List<FarmScript> farmlandScripts;

    GameObject UIBackground;

    ImapClient client;
    bool credentials = false, guiEnabled = false;
    public string emailAddress, password;
    Folder currentFolder;

    inboxScript inboxButton;
    trashScript trashButton;
    sentScript sentButton;

    int totalMessagesInFolder;

    enum EmailState { E_CREDENTIALS, E_LOGIN, E_DOWNLOAD, E_BROWSE, E_VIEW };
    public enum FolderState { F_INBOX, F_SENT, F_TRASH, F_SPAM, F_NONE };
    EmailState emailState;
    public FolderState folderState;
    FolderState prevFolderState;

	void Start ()
    {
        emailState = EmailState.E_CREDENTIALS;
        folderState = FolderState.F_INBOX;
        prevFolderState = folderState;
        SetupFarms();
        SetupEmails();
    }

    void SetupFarms()
    {
        //highlighterObject = GameObject.Find("highlighter");
        Transform[] tempTransforms = gameObject.GetComponentsInChildren<Transform>();
        farmlandTransforms = tempTransforms.ToList();
        farmlandTransforms.RemoveAt(0);

        FarmScript[] tempScripts = gameObject.GetComponentsInChildren<FarmScript>();
        farmlandScripts = tempScripts.ToList();

        //highlighterObject.GetComponent<SpriteRenderer>().enabled = false;

        trashButton = GameObject.Find("trash").GetComponent<trashScript>();
        inboxButton = GameObject.Find("inbox").GetComponent<inboxScript>();
        sentButton = GameObject.Find("sent").GetComponent<sentScript>();

        //UIBackground = GameObject.Find("background"); //FINDGAMEOBJECTWITHTAG and tag ui stuff as UI
        //UIBackground.GetComponent<Renderer>().enabled = false;
    }

    void SetupEmails()
    {
        emailAddress = "Email Address";
        password = "password";
    }
	
	void Update ()
    {
        if (trashButton.clickedOn)
        {
            currentFolder = client.Folders.Trash;
            trashButton.clickedOn = false;
            folderState = FolderState.F_TRASH;
        }

        if (inboxButton.clickedOn)
        {
            currentFolder = client.Folders.Inbox;
            inboxButton.clickedOn = false;
            folderState = FolderState.F_INBOX;
        }

        if (sentButton.clickedOn)
        {
            currentFolder = client.Folders.Sent;
            sentButton.clickedOn = false;
            folderState = FolderState.F_SENT;
        }

        switch(emailState)
        {
            case EmailState.E_CREDENTIALS:
                break;
            case EmailState.E_LOGIN:
                if (Login())
                {
                    emailState = EmailState.E_DOWNLOAD;
                }
                break;
            case EmailState.E_DOWNLOAD:
                DownloadMessages();
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case EmailState.E_BROWSE:
                UpdateFarms();
                guiEnabled = true;

                if (folderState != prevFolderState)
                {
                    Debug.Log("state changed");
                    ChangeFolder();
                    Debug.Log("Change finished");
                }
                break;
            case EmailState.E_VIEW:
                //do nothing for now
                break;
            default:
                Debug.Log("Error! " + emailState.ToString() + " has no valid behaviour!");
                break;
        }       
	}

    void UpdateFarms()
    {
        for (int i = 0; i < farmlandScripts.Count; i++)
        {
            if (farmlandScripts[i].clickedOn)
            {
                //highlighterObject.transform.position = farmlandTransforms[i].transform.position;
                //highlighterObject.GetComponent<SpriteRenderer>().enabled = true;
                farmlandScripts[i].clickedOn = false;

                if (i >= totalMessagesInFolder/*client.Folders.Inbox.Messages[i].Equals(null)*/)
                {
                    Debug.Log("No message!");
                    Debug.Log("No message!");
                }
                else
                {
                    Debug.Log(currentFolder.Messages[i].Subject);
                    Debug.Log(currentFolder.Messages[i].Body.Text);
                }

                client.Folders.Inbox.Messages[i].Seen = true;
                if (farmlandScripts[i].currentPlantStage == 1)
                {
                    farmlandScripts[i].ChangePlantGrowth(2);
                }
            }

            if (farmlandScripts[i].rightClickedOn)
            {
                farmlandScripts[i].rightClickedOn = false;
                if (folderState == FolderState.F_INBOX && client.Folders.Inbox.Messages[i] != null)
                {
                    client.Folders.Inbox.Messages[i].MoveTo(client.Folders.Trash);
                }
                else if (folderState == FolderState.F_TRASH && client.Folders.Trash.Messages[i] != null)
                {
                    client.Folders.Trash.Messages[i].Remove();
                }
                else if (folderState == FolderState.F_SENT && client.Folders.Sent.Messages[i] != null)
                {
                    client.Folders.Sent.Messages[i].MoveTo(client.Folders.Trash);
                }
                ReloadLand();
            }

            farmlandScripts[i].guiEnabled = guiEnabled;
        }
    }

    void DownloadMessages()
    {
        
        Debug.Log("Downloading messages...");
        totalMessagesInFolder = 0;
        currentFolder = client.Folders.Inbox;
        currentFolder.Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Basic, -1); //CHANGE THIS TO 40 FOR SPEED OR -1 FOR ALL
        currentFolder = client.Folders.Sent;
        currentFolder.Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Basic, -1);
        currentFolder = client.Folders.Trash;
        currentFolder.Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Basic, -1);
        currentFolder = client.Folders.Junk;
        currentFolder.Messages.Download("ALL", ImapX.Enums.MessageFetchMode.Basic, -1);
        Debug.Log("Messages downloaded");
        currentFolder = client.Folders.Inbox;

        ReloadLand();

        emailState = EmailState.E_BROWSE;
        Debug.Log("READY TO BROWSE");
    }

    void ChangeFolder()
    {
        switch (folderState)
        {
            case FolderState.F_INBOX:
                currentFolder = client.Folders.Inbox;
                break;
            case FolderState.F_SENT:
                currentFolder = client.Folders.Sent;
                break;
            case FolderState.F_TRASH:
                currentFolder = client.Folders.Trash;
                break;
            case FolderState.F_SPAM:
                currentFolder = client.Folders.Junk;
                break;
        }
        prevFolderState = folderState;
        ReloadLand();
    }

    void ReloadLand()
    {
        totalMessagesInFolder = 0;
        if (folderState == FolderState.F_TRASH)
        {
            client.Folders.Trash.Messages.Download();
        }
        else if (folderState == FolderState.F_SENT)
        {
            client.Folders.Sent.Messages.Download();
        }
        foreach (Message mes in currentFolder.Messages)
        {
            totalMessagesInFolder += 1;
        }
        Debug.Log(totalMessagesInFolder);

        for (int i = 0; i < farmlandScripts.Count; i++)
        {
            farmlandScripts[i].ChangePlantGrowth(0);
        }

        if (totalMessagesInFolder > farmlandScripts.Count)
        {
            for (int i = 0; i < farmlandScripts.Count; i++)
            {
                if (currentFolder.Messages[i].Equals(null))
                {
                    farmlandScripts[i].ChangePlantGrowth(0);
                }
                else if (currentFolder.Messages[i].Seen)
                {
                    if (folderState == FolderState.F_INBOX)
                    {
                        farmlandScripts[i].ChangePlantGrowth(2);
                    }
                    else if (folderState == FolderState.F_TRASH)
                    {
                        farmlandScripts[i].ChangePlantGrowth(3);
                    } 
                    else if (folderState == FolderState.F_SENT)
                    {
                        farmlandScripts[i].ChangePlantGrowth(4);
                    }                   
                }
                else if (!currentFolder.Messages[i].Seen)
                {
                    if (folderState == FolderState.F_INBOX)
                    {
                        farmlandScripts[i].ChangePlantGrowth(1);
                    }
                    else if (folderState == FolderState.F_TRASH)
                    {
                        farmlandScripts[i].ChangePlantGrowth(3);
                    }
                    else if (folderState == FolderState.F_SENT)
                    {
                        farmlandScripts[i].ChangePlantGrowth(4);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < totalMessagesInFolder; i++)
            {
                Debug.Log(i);
                if (currentFolder.Messages[i].Equals(null))
                {
                    farmlandScripts[i].ChangePlantGrowth(0);
                }
                else if (currentFolder.Messages[i].Seen)
                {
                    if (folderState == FolderState.F_INBOX)
                    {
                        farmlandScripts[i].ChangePlantGrowth(2);
                    }
                    else if (folderState == FolderState.F_TRASH)
                    {
                        farmlandScripts[i].ChangePlantGrowth(3);
                    }
                    else if (folderState == FolderState.F_SENT)
                    {
                        farmlandScripts[i].ChangePlantGrowth(4);
                    }
                }
                else if (!currentFolder.Messages[i].Seen)
                {
                    if (folderState == FolderState.F_INBOX)
                    {
                        farmlandScripts[i].ChangePlantGrowth(1);
                    }
                    else if (folderState == FolderState.F_TRASH)
                    {
                        farmlandScripts[i].ChangePlantGrowth(3);
                    }
                    else if (folderState == FolderState.F_SENT)
                    {
                        farmlandScripts[i].ChangePlantGrowth(4);
                    }
                }
            }
        }

        for (int i = 0; i < farmlandScripts.Count(); i++)
        { 
            farmlandScripts[i].SetStrings(" ", " ");
        }

        if (currentFolder.Messages.Count() > 40)
        {
            for (int i = 0; i < 39; i++)
            {
                farmlandScripts[i].SetStrings(currentFolder.Messages[i].Subject.ToString(), currentFolder.Messages[i].Body.Text);
            }
        }
        else
        {
            for (int i = 0; i < currentFolder.Messages.Count(); i++)
            {
                farmlandScripts[i].SetStrings(currentFolder.Messages[i].Subject.ToString(), currentFolder.Messages[i].Body.Text);
            }
        }
        
    }

    bool Login()
    {
        if (client.Login(emailAddress, password))
        {
            Debug.Log("Signed in!");
            credentials = true;
            return true;
        }
        else
        {
            Debug.Log("Error! Failed to sign in!");
            credentials = false;
            emailState = EmailState.E_CREDENTIALS;
            client.Disconnect();
            Debug.Log("Client disconnected!");
            return false;
        }
    }

    void OnGUI()
    {
        if (!credentials)
        {
            emailAddress = GUI.TextField(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 15 - 40, 200, 30), emailAddress);
            password = GUI.PasswordField(new Rect((Screen.width / 2) - 100, (Screen.height / 2) + 15 - 40, 200, 30), password, '*');
            if (GUI.Button(new Rect((Screen.width / 2) - 100, (Screen.height / 2) + 50 - 40, 200, 30), "Login with Gmail") || Input.GetKeyDown("enter") || Input.GetKeyDown("return")) //KEYBOARD INPUT NOT PICKED UPWHEN TABBED INTO TEXTAREA
            {
                client = new ImapClient("imap.gmail.com", 993, true, false);
                if (client.Connect())
                {
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    emailState = EmailState.E_LOGIN;
                    Debug.Log("Client connected!");
                    credentials = true;
                }
                else
                {
                    Debug.Log("Error! Client not connnected!");
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (emailState != EmailState.E_CREDENTIALS)
        {
            client.Disconnect();
        }
        Debug.Log("Client disconnected!");
    }
}
