using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ServerListObj : MonoBehaviour {
	public RoomInfo game; 
	//public string Ip;
	public int players;
	public Text textBoxName;
	public Text playersBLueText;
	public Text playersRedText;
	public GameObject clientObj;
	// Use this for initialization
	void Start () {
		UpdateData();
		clientObj = GameObject.Find("ClientGameObj");
	}

 	public void ConnectToIp () {
		PhotonNetwork.JoinRoom(game.name);
		//clientObj.GetComponent<Client>().ConnectToIp(server.host, server.port);
	}
	// Update is called once per frame
	void UpdateData () {

		//uLink.BitStream dataCopy = server.data;
		

		//playersBLueText.text = playersBlue;
		//playersRedText.text = playersRed;
		//players = numPlayers;
		//Ip = server.endpoint.ToString();

		textBoxName.text = game.name + "\r\n" + game.playerCount + " / " + game.maxPlayers;

		Invoke("UpdateData", 2);
	}

}
