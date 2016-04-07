using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogInManager : Photon.MonoBehaviour {
	private string user;
	private string pass;

	public string regUser;
	public string regPass;
	public string regPassConfirm;

	public string email;
	private int win;
	private int lose;

	//public Button connectButton;
	private bool connected = false;

	public GameObject clientObj;
	//public GameObject mainMenu;
	// Use this for initialization
	void Awake () {

		if (!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)
		
		//Load name from PlayerPrefs
		PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));
	}

	public void OnConnectedToMaster()
	{
		Debug.Log("Connected to master");
		// this method gets called by PUN, if "Auto Join Lobby" is off.
		// this demo needs to join the lobby, to show available rooms!


	}

	public void OnJoinedLobby() 
	{
		Debug.Log( "OnJoinedLobby() : Hey, You're in a Lobby ! " + PhotonNetwork.PhotonServerSettings.ServerAddress );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void LogUserTxt (string userInput)
	{
		PhotonNetwork.playerName = userInput;

		//sliderText.text = sliderTextString;
	}
	public void LogPassrTxt (string passInput)
	{
		pass = passInput;
		//sliderText.text = sliderTextString;
	}
	public void regUserTxt (string regUserInput)
	{
		regUser = regUserInput;
		//sliderText.text = sliderTextString;
	}
	public void RegPassrTxt (string regPassInput)
	{
		regPass = regPassInput;
		//sliderText.text = sliderTextString;
	}
	public void RegConfirmPassrTxt (string regPassInput)
	{
		regPassConfirm = regPassInput;
		//sliderText.text = sliderTextString;
	}
	public void RegEmail (string regEmailInput)
	{
		email = regEmailInput;
		//sliderText.text = sliderTextString;
	}
	public void DoLogin()
	{
		//StartCoroutine(TimeOut());
		Debug.Log("dologin");
		PhotonNetwork.JoinLobby();  // this joins the "default" lobby
		////clientObj.GetComponent<Client>().ConnectToLobby();
		//Lobby.RPC("AccountInfoToServer", LobbyPeer.lobby, user, pass, Lobby.peer);
		//this.gameObject.GetComponent<MenuManager>().LoadingScreens(true);
	}

	public void DoRegister()
	{
		
		//clientObj.GetComponent<Client>().ConnectToLobby();
		//Lobby.RPC("RegInfoToServer", LobbyPeer.lobby, regUser, regPass, regPassConfirm, email, Lobby.peer);
		this.gameObject.GetComponent<MenuManager>().LoadingScreens(true);
	}

	IEnumerator TimeOut()
	{
		yield return new WaitForSeconds(5f);
		if (!connected)
		{
			this.gameObject.GetComponent<MenuManager>().LoadingScreens(false);

			//clientObj.GetComponent<Client>().windowLogin.GetComponent<Menu>().errorDialogText.color = Color.red;
			//clientObj.GetComponent<Client>().windowLogin.GetComponent<Menu>().errorDialogText.text = "Cant connect to the lobby. Try again";
		}

	} 

	private void uLobby_OnConnected()
	{
	
		connected = true;
	}


}
