using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;

public class MinionSpawnManager : Photon.MonoBehaviour {

	public GameObject BlueMinionCreator;
	public Transform BlueTarget;
	public GameObject RedMinionCreator;
	public Transform RedTarget;
	public string team;
	public WaypointCircuit circuitScript;
	private PlayerStats playerStatScript;
	private MinionsScript minionScript;
	//private WaypointProgressTracker waypointScript;

	private int waveNumber;
	public float health;
	public float healthRegen;
	public int lvl;
	public int expToGive;
	public int goldToGive;
	public float ad;
	public float ap;
	public float adRes;
	public float apRes;

	public float adFinal;
	public float apFinal;
	GameObject gameManager;
	ScoreTable scoreScript;

	void Start () {
		StartCoroutine(MinionSpawner());
		gameManager = GameObject.Find("GameManager_mn");
		
		scoreScript = gameManager.GetComponent<ScoreTable>();
	}

	IEnumerator MinionSpawner () {
		if (PhotonNetwork.isMasterClient)
		{
		
			SpawnMinion();
		
			yield return new WaitForSeconds(2);

			SpawnMinion();

			yield return new WaitForSeconds(2);

			SpawnMinion();
		


			//ADD A LEVEL AFTER EACH WAVE SPAWN
			lvl ++;

		}
		yield return new WaitForSeconds(20);
		StartCoroutine(MinionSpawner());
	}
	// This method spawns each enemy wave for the teams
	// Sends data to each minion playerstats script
	void SpawnMinion () {



		if (team == "blue")
		{
			adFinal = scoreScript.blueTeamScore;
			adFinal *= 2;
			GameObject minionObj = PhotonNetwork.Instantiate("MinionsBlueCreator_mn", transform.position, transform.rotation, 0);
			//waypointScript = minionObj.GetComponent<WaypointProgressTracker>();
			playerStatScript = minionObj.GetComponent<PlayerStats>();
			minionScript = minionObj.GetComponent<MinionsScript>();
			minionScript.target = BlueTarget;
			//waypointScript.circuit = circuitScript;
			minionObj.name = "BlueWave" + waveNumber.ToString();


		}
		if (team == "red")
		{
			adFinal = scoreScript.redTeamScore;
			adFinal *= 2;
			GameObject minionObj = PhotonNetwork.Instantiate("MinionsRedCreator_mn", transform.position, transform.rotation, 0);
			//waypointScript = minionObj.GetComponent<WaypointProgressTracker>();
			playerStatScript = minionObj.GetComponent<PlayerStats>();
			minionScript = minionObj.GetComponent<MinionsScript>();
			minionScript.target = RedTarget;
			//waypointScript.circuit = circuitScript;
			minionObj.name = "RedWave" + waveNumber.ToString();
		}
		playerStatScript.playerTeam = team;
		playerStatScript.maxHealth = health + adFinal;
		playerStatScript.myHealth = health	+ adFinal;
		playerStatScript.healthRegenerate = healthRegen;
		playerStatScript.playerLvl = lvl;
		playerStatScript.expToGive = expToGive;
		playerStatScript.goldToGive = goldToGive;
		playerStatScript._bAdValue = ad + adFinal;
		playerStatScript._bApValue = ap;
		playerStatScript.adRes = adRes;
		playerStatScript.apRes = apRes;
		minionScript.attackSpeed = 3;

		waveNumber++;
	}

	void Update () {
		
	}
}
