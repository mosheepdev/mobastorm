using UnityEngine;
using System.Collections;

//THIS SCRIPT IS ATTACHED TO THE SPAWN MANAGER AND IT ALLOWS THE PLAYER TO SPAWN INTO MULTIPLAYER GAME
public class SpawnScript : Photon.MonoBehaviour {
	
	private GameObject[] redSpawnPoints;
	private GameObject[] blueSpawnPoints;
	private GameObject playerPickObj;
	private GameObject gameManager;
	private PlayerDatabase dataScript;
	private PlayerStats playerStatsScript;

	void Start () {

		gameManager = GameObject.Find("GameManager_mn");
		dataScript = gameManager.GetComponent<PlayerDatabase>();

	}


	public void SpawnRedTeamPlayer(PhotonPlayer Owner, string PrefabDir, CharacterClass.charName charName)
	{
		redSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnRed");
		GameObject randomRedSpawn = redSpawnPoints[Random.Range(0, redSpawnPoints.Length)];
		//GameObject player = (GameObject)uLink.Network.Instantiate(Owner, PrefabDir+ "Owner_mn",PrefabDir+ "Owner_mn",PrefabDir+ "Creator_mn", randomRedSpawn.transform.position, randomRedSpawn.transform.rotation, 0);
		GameObject player = (GameObject)PhotonNetwork.Instantiate(PrefabDir+ "Owner_mn", randomRedSpawn.transform.position, randomRedSpawn.transform.rotation, 0);

		//player.name = PhotonNetwork.playerName;
		//player.layer = 9;
		//player.tag = "RedPlayerTag";
		//GameObject Trigger = player.transform.FindChild("Trigger").gameObject;
		//Trigger.tag = "RedPlayerTriggerTag";
		//Trigger.layer = 17;
		playerStatsScript = player.GetComponent<PlayerStats>();
		playerStatsScript.name = player.name;
		//playerStatsScript.gameObject.name = player.name;
		playerStatsScript.SetPlayerData(PhotonNetwork.playerName,"red");
		playerStatsScript.networkOwner = Owner;
		//playerStatsScript.UpdateStatsOnClients();
		//ADD THE CURRENT PLAYER NETWORKPLAYER, THE TEAM, AND THE GAMEOBJECT TO THE PLAYERDATABASE SCRIPT ON THE GAME MANAGER GAMEOBJECT
		dataScript.AddPlayerDataToList(player.name, "red");

	


		}

	public void SpawnBlueTeamPlayer(PhotonPlayer Owner, string PrefabDir, CharacterClass.charName charName)
	{

		blueSpawnPoints = GameObject.FindGameObjectsWithTag("SpawnBlue");
		GameObject randomBlueSpawn = blueSpawnPoints[Random.Range(0, blueSpawnPoints.Length)];
		//GameObject player = (GameObject)uLink.Network.Instantiate(Owner, PrefabDir+ "Owner_mn",PrefabDir+ "Owner_mn",PrefabDir+ "Creator_mn", randomBlueSpawn.transform.position, randomBlueSpawn.transform.rotation, 0);
		GameObject player = (GameObject)PhotonNetwork.Instantiate(PrefabDir+ "Owner_mn", randomBlueSpawn.transform.position, randomBlueSpawn.transform.rotation, 0);


		player.name = PhotonNetwork.playerName;
		//player.layer = 9;
		//player.tag = "BluePlayerTag";
		//GameObject Trigger = player.transform.FindChild("Trigger").gameObject;
		//Trigger.tag = "BluePlayerTriggerTag";
		//Trigger.layer = 16;
		playerStatsScript = player.GetComponent<PlayerStats>();
		playerStatsScript.name = player.name;
		//playerStatsScript.gameObject.name = player.name;
		playerStatsScript.SetPlayerData(PhotonNetwork.playerName,"blue");
		playerStatsScript.networkOwner = Owner;
		//playerStatsScript.UpdateStatsOnClients();
		//ADD THE CURRENT PLAYER NETWORKPLAYER, THE TEAM, AND THE GAMEOBJECT TO THE PLAYERDATABASE SCRIPT ON THE GAME MANAGER GAMEOBJECT
		dataScript.AddPlayerDataToList(player.name, "blue");


	}



}
