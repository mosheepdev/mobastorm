using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PlayerDatabase : Photon.MonoBehaviour
	{
	public List<PlayerDataClass> PlayerList = new List<PlayerDataClass>();
	public string playerName;
	public int playerScore;
	public string playerTeam;
	private ScoreTable scoreScript;

	// Use this for initialization

	void Start () {
		scoreScript = GetComponent<ScoreTable>();
	}
	
	// Update is called once per frame

	void Update () 
	{
	
	}
	//Accesed by PickSelection script to add the current joined player to the list playerlist
	public void AddPlayerNameToList (string pName, PhotonPlayer nPlayer)
	{
		
		photonView.RPC("AddPlayerNameToListPhoton", PhotonTargets.AllBufferedViaServer, pName);
			
	}

	//Add the current joined player to the list playerlist on clients
	[PunRPC]
	void AddPlayerNameToListPhoton (string pName, PhotonMessageInfo info)
	{
		if (PlayerList.Exists(x => x.playerName == pName))
		{
			Debug.Log("CANT ADD PLAYER NAME TO LIST. PLAYER EXISTS ON THE DATA!!");
		}
		else
		{
			PlayerDataClass capture = new PlayerDataClass ();
			capture.networkPlayer = info.sender;
			capture.playerName = pName;
			PlayerList.Add(capture);
		}
	}
	//Accesed by SpawnScript script to add the current team and Obj to the list playerlist
	public void AddPlayerDataToList (string pName, string team)
	{
		
	photonView.RPC("AddPlayerDataToListPhoton", PhotonTargets.AllBufferedViaServer, pName, team);
			
	}

	//Accesed by SpawnScript script to add the current team and Obj to the list playerlist
	[PunRPC]
	public void AddPlayerDataToListPhoton (string pName, string team, PhotonMessageInfo info)
	{
		for (int i = 0; i < PlayerList.Count; i++) 
		{
			if (PlayerList[i].playerName == pName)
			{
				PlayerList[i].playerTeam = team;
				PlayerList[i].networkPlayer = info.sender;
				//photonView.RPC("AddTeamOnClients", PhotonTargets.OthersBuffered, PlayerList[i].playerName, team);
			}


		}
	}
	//Returns the name of the player assigned to the current networkplayer nPlayer
	public string GetPlayerName (PhotonPlayer nPlayer)
	{
		string pName = "";
		for (int i = 0; i < PlayerList.Count; i++) 
		{

			if (PlayerList[i].networkPlayer == nPlayer)
			{
				pName = PlayerList[i].playerName;
			}
			

		}
		return pName;
	}




	//Add the current joined player team to the list playerlist on clients
	[PunRPC]
	void AddTeamOnClients (string sPname, string team)
	{
		for (int i = 0; i < PlayerList.Count; i++) 
		{
			if (PlayerList[i].playerName == sPname)
			{
				PlayerList[i].playerTeam = team;

			}
		}
	}



	[PunRPC]
	void RemovePlayerFromList (PhotonPlayer nPlayer)
	{

		for (int i = 0; i < PlayerList.Count; i++) 
		{
			if (PlayerList[i].networkPlayer == nPlayer)
			{
				PlayerList.RemoveAt(i);
			}

		}
	}


	/*
	[PunRPC]
	void EditPlayerListWithName (uLink.NetworkPlayer nPlayer, string pName)
	{
		if (uLink.Network.isServer)
		networkView.RPC("EditPlayerListWithName", uLink.RPCMode.OthersBuffered, nPlayer, pName);
		//FIND THE PLAYER IN THE PLAYER LIST

		for (int i = 0; i < PlayerList.Count; i++) 
		{
			if (PlayerList[i].networkPlayer == nPlayer)
			{
				PlayerList[i].playerName = pName;

			}

		}
	}
	*/


	//Accesed by PlayerStats script to increment the player score when a player kills an enemy
	//An RPC is sent out across the network so that everyone gets the latest team score.
	[PunRPC]
	public void EditPlayerListWithScore (string nPlayer)
	{
		photonView.RPC("EditPlayerListWithScorePhoton", PhotonTargets.AllBufferedViaServer, nPlayer);


	}

	[PunRPC]
	public void EditPlayerListWithScorePhoton (string nPlayer)
	{
		//photonView.RPC("EditPlayerListWithScore", PhotonTargets.Others, nPlayer);
		//if (uLink.Network.isServer)
		//{
		//photonView.RPC("EditPlayerListWithScore", PhotonTargets.Others, nPlayer);
		//}
		for (int i = 0; i < PlayerList.Count; i++) 
		{
			if (PlayerList[i].playerName == nPlayer)
			{
				PlayerList[i].playerScore ++;

				if (PlayerList[i].playerTeam == "blue")
				{
					scoreScript.UpdateBlueTeamScore();
				}
				else
				{
					scoreScript.UpdateRedTeamScore();
				}

			}

		}
	}

}
