using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChatBoxGame : Photon.MonoBehaviour {

	public string chatText;
	public Text chatBoxText;
	public InputField ChatInputField;
	private string[] colorNames = new string[] {"cyan", "green", "lightblue","olive", "orange", "purple","red", "silver", "teal","yellow"};
	private int index = 0;
	private string userNameColor;

	//ACCES GAME MANAGER OBJECT AND PLAYERDATABASE SCRIPT

	private PlayerDatabase databaseScript;
	//private Color 
	// Use this for initialization
	void Start () {
		databaseScript = GameObject.Find("GameManager_mn").GetComponent<PlayerDatabase>();
		int random = Random.Range(0, colorNames.Length);
		userNameColor = colorNames[random];
	}
	public void addText(string text)
	{

		chatText = text;

	}


	// Update is called once per frame
	void Update () {
		if (ChatInputField.GetComponent<InputField>().isFocused == true) {
			if (Input.GetKeyDown(KeyCode.Return))
			{

				ChatInputField.text = ChatInputField.text.Remove(ChatInputField.text.Length - 1);
				photonView.RPC("SendGameChatInput", PhotonTargets.All, "<color="+ userNameColor+ ">" + databaseScript.playerName +" -</color> "  + ChatInputField.text);
				ChatInputField.text = "";
			}
		}
	}

	//void uLink_OnPlayerConnected(uLink.NetworkPlayer networkPlayer) 
	//{
	//	string colorName = colorNames[index];
	//	index++;

	//	networkView.RPC("SetPlayerColor", networkPlayer, colorName);
	//}

	[PunRPC]
	public void SendGameChatInput(string text)
	{
		//if (uLink.Network.isServer)
		//{
		//	networkView.RPC("SendGameChatInput", uLink.RPCMode.Others, text);
		//}
		//else
		//{
			chatBoxText.text += text + "\r\n";
		//}
	} 

	[PunRPC]
	public void SetPlayerColor(string text)
	{
		userNameColor = text;
	}



}
