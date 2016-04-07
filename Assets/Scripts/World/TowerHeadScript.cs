using UnityEngine;
using System.Collections;

public class TowerHeadScript : Photon.MonoBehaviour
{


	public enum State
	{
		idle,

		searching,
		
		attackingRangue, 
		
		destroyed
		
		
	}

	private string bombCanon = "TowerBasic_mn";
	public GameObject trigger;
	public ParticleSystem attackParticle;
	public ParticleSystem launchParticle;
	public GameObject canvas;

	[HideInInspector] public State towerStatus = State.idle;

	private string myTeam = "blue";
	public Transform launchTransform;

	//ATTACK SPEED AND COUNTER
	public float timeAttack = 10;
	public float attackSpeed = 5;

	//TARGET ENEMY PLAYER
	[HideInInspector] public Vector3 target;

	[HideInInspector] public Vector3 objectPos;
	[HideInInspector] public Vector3 initialPos;
	[HideInInspector] public float distanceFromPlayer;
	public float maxDistance = 6;

	[HideInInspector] public GameObject targetObj;

	private Transform canonSpawn;
	private float range = 20;
	private RaycastHit floorHit;
	float angleToTarget;

	public float damageScaleTime;
	public float damageScaleValue;
	private LineRenderer lineRend;

	private PlayerStats playerStatsScript;

	public GameObject explotionObj;

	//REFERENCE GAMEOBJ
	private GameObject gameManager;
	private PlayerDatabase dataScript;

	void Awake()
	{
		playerStatsScript  = GetComponent<PlayerStats>();

		lineRend = GetComponent<LineRenderer>();


		myTeam = playerStatsScript.playerTeam;

	}
	
	void Start()
	{          

		gameManager = GameObject.Find("GameManager_mn");
		
		dataScript = gameManager.GetComponent<PlayerDatabase>();

		canonSpawn = transform.Find("Blaunch");

		InvokeRepeating("ScaleTowerDamage", 2, damageScaleTime);

	}

	void OnTriggerStay(Collider other)
	{

		if (PhotonNetwork.isMasterClient && playerStatsScript.myHealth > 0)
		{
			if (myTeam == "blue")
			{

				if(other.tag == "RedTeamTag" && targetObj == null || other.tag == "RedPlayerTag" && targetObj == null)
				{		
					PlayerStats enemyPlayerStats = other.gameObject.GetComponent<PlayerStats>();

					if(enemyPlayerStats.myHealth > 0)
					{
					targetObj = other.gameObject;
					towerStatus = State.searching;
					photonView.RPC("UpdateTargetToClients", PhotonTargets.Others, other.name);
					}
				
				}
			}
			else
			{
				if(other.tag == "BlueTeamTag" && targetObj == null || other.tag == "BluePlayerTag" && targetObj == null)
				{			
					PlayerStats enemyPlayerStats = other.gameObject.GetComponent<PlayerStats>();
					
					if(enemyPlayerStats.myHealth > 0)
					{
						targetObj = other.gameObject;
						towerStatus = State.searching;
						photonView.RPC("UpdateTargetToClients", PhotonTargets.Others, other.name);
					}
					
				}
			}
		}
		
	}
	void OnTriggerExit(Collider other)
	{
		if (PhotonNetwork.isMasterClient)
		{
	
			if (other.gameObject == targetObj)
			{
				targetObj = null;
				photonView.RPC("UpdateTargetToClients", PhotonTargets.Others, "");
			}
				

		}
		
	}

	void Update()
	{
		
		timeAttack += Time.deltaTime;

		if(targetObj)
			
		{
			lineRend.enabled = true;
			lineRend.SetPosition(0, launchTransform.position);
			Vector3 laserEdnPoint = new Vector3(targetObj.transform.position.x, targetObj.transform.position.y + 0.7f, targetObj.transform.position.z);
			lineRend.SetPosition(1, laserEdnPoint);
			attackParticle.emissionRate = 300;
			objectPos = targetObj.transform.position;
			if(Physics.Raycast( objectPos, new Vector3(0,-1,0), out floorHit, range))
			{
				if (floorHit.transform.tag == "Floor")
				{
				objectPos.y = floorHit.point.y;
				}

			}
			target = objectPos;
			target.y = transform.position.y;
		}
		else
		{
			if (attackParticle.emissionRate > 1)
			{
				attackParticle.emissionRate -= 40;
			}
			lineRend.enabled = false;


		}

		switch (towerStatus)
		{
		case State.idle:
			
			targetObj = null;
			break;

		case State.attackingRangue:



		
			break;
		case State.searching:

			if (targetObj)
			{
				if (targetObj.GetComponent<PlayerStats>().myHealth <=0)
				{
					targetObj = null;
					towerStatus = State.idle;
					return;
				}

				if (timeAttack > attackSpeed && PhotonNetwork.isMasterClient)
				{
					timeAttack = 0;
					towerStatus = State.searching;
					StartCoroutine(Shoot());


				}
			}
			else
			{
				
				towerStatus = State.idle;

			}

			break;
		}
		
		
		
	}

	void ScaleTowerDamage()
	{
		playerStatsScript._bAdValue += damageScaleValue;
		playerStatsScript._bApValue += damageScaleValue;

	}

	public IEnumerator Shoot() 
	{
		photonView.RPC("LaunchParticleOnClients", PhotonTargets.Others);
		launchParticle.Play();
		yield return new WaitForSeconds(0.5f);
		if (playerStatsScript.playerTeam == "blue" && targetObj)
		{
			object[] data = new object[6] {  "blue", gameObject.name, objectPos, targetObj.name, playerStatsScript._bAdValue, playerStatsScript._bApValue };
			PhotonNetwork.Instantiate(bombCanon, canonSpawn.position, Quaternion.identity, 0, data); 

		}
		if (playerStatsScript.playerTeam == "red" && targetObj)
		{
			object[] data = new object[6] {  "red", gameObject.name, objectPos, targetObj.name, playerStatsScript._bAdValue, playerStatsScript._bApValue };
			PhotonNetwork.Instantiate(bombCanon, canonSpawn.position, Quaternion.identity, 0, data); 
		}
		
		
		

		
	}

	[PunRPC]
	public IEnumerator LaunchParticleOnClients ()
	{
		
		launchParticle.Play();
		yield return new WaitForSeconds(0.5f);

	}
	

	
	public float DistanceSquaredTo(GameObject source, GameObject target) 
	{
		return Vector3.SqrMagnitude(source.transform.position - target.transform.position);
	}

	[PunRPC]
	public IEnumerator ImDead() 
	{
		towerStatus = State.destroyed;
		trigger.GetComponent<BoxCollider>().enabled = false;
		Instantiate(explotionObj, transform.position, Quaternion.identity);
		Destroy(canvas);
		targetObj = null;
		lineRend.enabled = true;
		attackParticle.emissionRate = 0;
		yield return new WaitForSeconds(playerStatsScript.respawnTime);
		trigger.GetComponent<BoxCollider>().enabled = false;
		towerStatus = State.idle;


	}

	[PunRPC]
	void UpdateTargetToClients (string targetName)
	{


		if (targetName != "")
		{
			attackParticle.emissionRate = 300;
			targetObj = GameObject.Find(targetName);
			if (targetObj.name == dataScript.playerName)
			{
				GetComponent<AudioSource>().Play();
			}
			towerStatus = State.searching;

		}
		else
		{
			attackParticle.emissionRate = 0;
			targetObj = null;
			towerStatus = State.idle;
		}
		
	}



}
