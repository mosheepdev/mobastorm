using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
//using MobaStorm;

public class Client : MonoBehaviour
{
	public string lobbyIP = "127.0.0.1";
	public int lobbyPort = 45000;
	public bool clientActive= false;

	public string serverPublicIp = "127.0.0.1";
	public GameObject windowInit;
	public GameObject windowLobby;
	public GameObject windowLogin;
	public GameObject windowReg;
	public GameObject windowServerCreation;
	public GameObject windowPickSelection;
	public GameObject windowGame;
	public MenuManager mainMenuManager;
	public Menu lobbyMenu;

	public GameObject layoutServers;
	public GameObject ServerRect;
	RectTransform serverRectTransform;
	public GameObject ServerBoxPrefab;

	public Text chatBoxText;
	private int serverRow;
	public List<GameObject> allServerslots;

	private string serverName;
	public AudioSource introSource;

	private string roomName = "myRoom";
	private Vector2 scrollPos = Vector2.zero;
	public void Start()
	{
		allServerslots = new List<GameObject>();
		serverRectTransform = ServerRect.GetComponent<RectTransform>();
		DontDestroyOnLoad(this.gameObject);
	
	}


	public void UpdateServerName (string name)
	{
		serverName = name;
	}


	public void CreateRoom()
	{
		// using null as TypedLobby parameter will also use the default lobby
		PhotonNetwork.CreateRoom(serverName, new RoomOptions() { maxPlayers = 10 }, TypedLobby.Default);
	}



	void OnJoinedRoom()
	{
		mainMenuManager.ShowMenu(windowPickSelection.GetComponent<Menu>());
		mainMenuManager.loadingScreenObj.SetActive(false);

		// GIVE THE NAME OF THE PLAYER TO THE PICK SELECTION WINDOW
		GameObject PickSelectionObj = GameObject.Find("Window_PickSelection_mn");

		PickSelection pickSelScript = PickSelectionObj.GetComponent<PickSelection>();

		//pickSelScript.PlayerName = PlayerPrefs.GetString("playername");
		pickSelScript.CreateLayout();
		pickSelScript.CreatePickLayout();
	}

	public void uLink_OnConnectedToServer()
	{
		introSource.Stop();
		mainMenuManager.ShowMenu(windowPickSelection.GetComponent<Menu>());
		mainMenuManager.loadingScreenObj.SetActive(false);
		
		// GIVE THE NAME OF THE PLAYER TO THE PICK SELECTION WINDOW
		GameObject PickSelectionObj = GameObject.Find("Window_PickSelection_mn");
		
		PickSelection pickSelScript = PickSelectionObj.GetComponent<PickSelection>();
		
		//pickSelScript.PlayerName = PlayerPrefs.GetString("playername");
		pickSelScript.CreateLayout();
		pickSelScript.CreatePickLayout();
	}


	public void Update()
	{
		if (introSource.volume > 0.20f)
		{
			introSource.volume -= 0.01f;
		}






	}





	public void ShowGameMenu()
	{
		
		mainMenuManager.ShowMenu(windowGame.GetComponent<Menu>());
		
	}


	public void OnJoinedLobby() 
	{
		mainMenuManager.ShowMenu(windowLobby.GetComponent<Menu>());
		mainMenuManager.LoadingScreens(false);
		ShowServers();
	}

	void OnGUIo()
	{
		


		if (PhotonNetwork.room != null)
			return; //Only when we're not in a Room


		GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

		GUILayout.Label("Main Menu");

		//Player name
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player name:", GUILayout.Width(150));
		PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
		if (GUI.changed)//Save name
			PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
		GUILayout.EndHorizontal();

		GUILayout.Space(15);


		//Join room by title
		GUILayout.BeginHorizontal();
		GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
		roomName = GUILayout.TextField(roomName);
		if (GUILayout.Button("GO"))
		{
			PhotonNetwork.JoinRoom(roomName);
		}
		GUILayout.EndHorizontal();

		//Create a room (fails if exist!)
		GUILayout.BeginHorizontal();
		GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
		roomName = GUILayout.TextField(roomName);
		if (GUILayout.Button("GO"))
		{
			// using null as TypedLobby parameter will also use the default lobby
			PhotonNetwork.CreateRoom(roomName, new RoomOptions() { maxPlayers = 10 }, TypedLobby.Default);
		}
		GUILayout.EndHorizontal();

		//Join random room
		GUILayout.BeginHorizontal();
		GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			GUILayout.Label("..no games available...");
		}
		else
		{
			if (GUILayout.Button("GO"))
			{
				PhotonNetwork.JoinRandomRoom();
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(30);
		GUILayout.Label("ROOM LISTING:");
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			GUILayout.Label("..no games available..");
		}
		else
		{
			//Room listing: simply call GetRoomList: no need to fetch/poll whatever!
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			foreach (RoomInfo game in PhotonNetwork.GetRoomList())
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
				if (GUILayout.Button("JOIN"))
				{
					PhotonNetwork.JoinRoom(game.name);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}

		GUILayout.EndArea();
	}

	public void ShowServers()
	{

		for(int i = 0; i < allServerslots.Count; i++)
		{
			Destroy(allServerslots[i]);
			allServerslots.Remove(allServerslots[i]);
		}




		foreach (RoomInfo game in PhotonNetwork.GetRoomList())
		{
			Debug.Log("ONE ROOM FOUND");
			GameObject newServerBox = Instantiate(ServerBoxPrefab) as GameObject;
			RectTransform newServerBoxRect = newServerBox.GetComponent<RectTransform>();
			ServerListObj serverListScript = newServerBox.GetComponent<ServerListObj>();
			serverListScript.game = game;


			newServerBox.transform.SetParent(ServerRect.transform);

			newServerBoxRect.localPosition =  new Vector3(0, (-serverRow * 50), 0);
			newServerBoxRect.localScale = Vector3.one;

			allServerslots.Add(newServerBox);
			serverRow++;
			serverRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50 * serverRow);


		}
		serverRow = 0;
		Invoke("ShowServers", 1);

	}
	public void AccountInfoResponse(bool response, string playerName, string msj)
	{
		if (response == true) {
			PlayerPrefs.SetString("playername", playerName);
			Debug.Log(playerName);
			mainMenuManager.ShowMenu(windowLobby.GetComponent<Menu>());

			ShowServers();
			mainMenuManager.LoadingScreens(false);


		}
		else
		{


			mainMenuManager.LoadingScreens(false);
			windowLogin.GetComponent<Menu>().errorDialogText.color = Color.red;
			windowLogin.GetComponent<Menu>().errorDialogText.text = msj;

		}
	}
	[PunRPC]
	public void RegInfoResponse(bool response, string playerName, string msj)
	{
		Debug.Log(response);
		if (response == true) {
			PlayerPrefs.SetString("playername", playerName);
			mainMenuManager.ShowMenu(windowLogin.GetComponent<Menu>());
			windowLogin.GetComponent<Menu>().errorDialogText.text = msj;
			windowLogin.GetComponent<Menu>().errorDialogText.color = Color.green;

			mainMenuManager.LoadingScreens(false);
			
		}
		else
		{
		
			windowReg.GetComponent<Menu>().errorDialogText.text = msj;
			windowLogin.GetComponent<Menu>().errorDialogText.color = Color.red;
			mainMenuManager.LoadingScreens(false);

		}
	}

	[PunRPC]
	public void SendChatInputAll(string text)
	{
		Debug.Log(text);
		chatBoxText.text += text + "\r\n";
	}


}
