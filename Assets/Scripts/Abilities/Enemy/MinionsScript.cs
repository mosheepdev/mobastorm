/// <summary>
/// AI character minion controller.
/// Just A basic AI Character controller
/// will looking for a Target and moving to and Attacking
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;

public class MinionsScript : Photon.MonoBehaviour { 

	public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding

	public float updateTargetRate = 10F;
	public GameObject targetObj;
	public GameObject trigger;
	public GameObject healthCanvas;
	public Transform target;
	public enum State
	{
		idle,
		
		chasing,
		
		attackingMelee,
		
		attackingRangue, 

		retreat,

		dead
		
		
	}

	public float attackSpeed = 5;
	private float timeAttack = 10;

	public State creepStatus = State.idle;
	private Animator animator;


	private float distanceFromPlayer;
	public float maxDistance = 6;

	//CHILD SCRIPT CREEP HEALTH
	private PlayerStats playerStatScript;

	//CALCULATE POSITION AND ROTATIONS
	private float distanceFromOrigin;
	private Vector3 objectPos;
	private Vector3 initialPos;

	//AUDIO CLIPS
	public AudioClip attackingSound;


	//agent variables
	private Vector3 move;
	private float m_MovingTurnSpeed = 360;
	private float m_StationaryTurnSpeed = 1000;
	float m_TurnAmount;
	float m_ForwardAmount;
	private Transform targetPath; 


	void Start () {

		animator = GetComponent<Animator>();

		playerStatScript = GetComponent<PlayerStats>();

		//waypointsScript = GetComponent<WaypointProgressTracker>();

		initialPos = transform.position;

		if (PhotonNetwork.isMasterClient)
		{
			agent = GetComponentInChildren<NavMeshAgent>();
			agent.updateRotation = false;
			agent.updatePosition = true;
		}

	}

	void OnTriggerStay(Collider other)
	{

		if (playerStatScript.playerTeam == "boss" && !targetObj)
		{
			//Debug.Log("EnemyDetected");
			if(other.tag == "RedPlayerTag" || other.tag == "BluePlayerTag")
			{			
				if (other.GetComponent<PlayerStats>().myHealth >0)
				{
					targetObj = other.gameObject;
				}
			}
		}

		if (playerStatScript.playerTeam == "blue" && !targetObj)
		{
			//Debug.Log("EnemyDetected");
			if(other.tag == "RedPlayerTag" || other.tag == "RedTeamTag")
			{			
				if (other.GetComponent<PlayerStats>().myHealth >0)
				{
					targetObj = other.gameObject;
				}
			}

		}

		if (playerStatScript.playerTeam == "red" && !targetObj)
		{
			//Debug.Log("EnemyDetected");
			if(other.tag == "BluePlayerTag" || other.tag == "BlueTeamTag")
			{			
				if (other.GetComponent<PlayerStats>().myHealth >0)
				{
					targetObj = other.gameObject;
				}
			}

		}
	}

	
	void Update () 
	{
		if (playerStatScript.abilityLocked && playerStatScript.myHealth > 0)
		{
			if (PhotonNetwork.isMasterClient)
			{
				agent.Stop();
				creepStatus = State.idle;
			}
			animator.enabled = false;
			return;
		}
		else
		{
			animator.enabled = true;
		}

		if (PhotonNetwork.isMasterClient  && !playerStatScript.charLocked)
		{
			//targetPath = waypointsScript.target;
			timeAttack += Time.deltaTime;
			var myPos = this.gameObject.transform.position;
			distanceFromOrigin = Vector3.Distance(initialPos,myPos);
			
			if(targetObj)			
			{
				objectPos = targetObj.transform.position;
				objectPos.y = myPos.y;
				distanceFromPlayer = Vector3.Distance(objectPos,myPos);	

				if (targetObj.GetComponent<PlayerStats>().myHealth <=0)
				{
					targetObj = null;
					creepStatus = State.idle;
					return;
				}

			}
			else
			{
				creepStatus = State.idle;
			}

			switch (creepStatus)
			{
			case State.idle:

				if (playerStatScript.myHealth <= 0)
				{
					creepStatus = State.dead;
					return;
				}

				animator.SetInteger("AnimType", (int)State.chasing);
				MoveOrChase();

				if(distanceFromPlayer<=maxDistance - 2 && targetObj) {
					initialPos = transform.position;
					creepStatus = State.attackingMelee;
					Debug.Log("Attacking MElee");
				}

				break;
			case State.dead:
				

				animator.SetInteger("AnimType", (int)State.dead);

				break;
			case State.attackingMelee:
				if (playerStatScript.myHealth <= 0)
				{
					creepStatus = State.dead;
					return;
				}

				if(distanceFromPlayer<=4) {
					if (timeAttack > attackSpeed)
					{
						transform.LookAt(targetObj.transform.position);
						agent.Stop();
						animator.SetInteger("AnimType", (int)State.attackingMelee);
						playerStatScript.charLocked = true;
						timeAttack = 0;
					}
					else
					{
						transform.LookAt(targetObj.transform.position);
						agent.Stop();
						animator.SetInteger("AnimType", (int)State.idle);
					}
				}
				else
				{
					animator.SetInteger("AnimType", (int)State.chasing);
					
					MoveOrChase();
				}
			
				

				if(distanceFromPlayer>=maxDistance || distanceFromOrigin > maxDistance)
				{	
					targetObj = null;
					creepStatus = State.idle;
					return;
				}

				break;
			
			case State.retreat:


					targetObj = null;

					creepStatus = State.idle;


				break;
			}
		}
	}

	void MoveOrChase()
	{


			updateTargetRate++;
			if (updateTargetRate > 20)
			{
			updateTargetRate = 0;
				if (targetObj != null)
				{
					agent.SetDestination(targetObj.transform.position);
				}
				else
				{
				
				agent.SetDestination(target.position);
				}
			}

		agent.Resume();
		
		
		move = agent.desiredVelocity;
		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);
		move = Vector3.ProjectOnPlane(move, Vector3.zero);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;
		

		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		//transform.LookAt(agent.nextPosition);

		
	}
	//METHOD CALLED FROM THE ANIMATION EVENTS TO UNLOCK THE CHARACTER
	void ExitAnim() 
	{
		playerStatScript.charLocked = false;
		
	}

	void Attacking(State type) {
		//if (PhotonNetwork.isMasterClient)
		//{
		GetComponent<AudioSource>().clip = attackingSound;
		GetComponent<AudioSource>().Play();
		//}

		if (!PhotonNetwork.isMasterClient)
			return;
	

		switch (type)
		{
		case State.attackingMelee:
			

			creepStatus = State.attackingMelee;

			break;
		}

		if (targetObj)
		{

				PlayerStats enemyPlayerStatsScript = targetObj.GetComponent<PlayerStats>();
			enemyPlayerStatsScript.photonView.RPC("DrainHealth", photonView.owner,  playerStatScript._bAdValue, playerStatScript._bApValue, transform.name, gameObject.tag, CharacterClass.charName.Empty.ToString());
			//enemyPlayerStatsScript.dra(playerStatScript._bAdValue, playerStatScript._bApValue, transform.name, gameObject.tag, CharacterClass.charName.Empty.ToString());
			if (enemyPlayerStatsScript.myHealth <= 0)
				{
					targetObj = null;
					creepStatus = State.idle;
				}

		}
		else
		{
			creepStatus = State.idle;
		}



	}

	[PunRPC]
	public void Respawn ()
	{

		if (PhotonNetwork.isMasterClient)
		{
			//Destroy(targetPath.gameObject);
			PhotonNetwork.Destroy(this.gameObject);

		}
		
	}


	[PunRPC]
	public IEnumerator ImDead() 
	{

		creepStatus = State.dead;
		//CreepLabel labelScript = GetComponent<CreepLabel>();
		//Destroy(labelScript.canvasObj);
		//labelScript.enabled = false;

		trigger.GetComponent<BoxCollider>().enabled = false;
		animator.SetInteger("AnimType", (int)State.dead);

		if (PhotonNetwork.isMasterClient)
		{
		agent.enabled = false;
		}
		
		Destroy(healthCanvas);
		
		yield return new WaitForSeconds(playerStatScript.respawnTime);

		targetObj = null;
		Respawn();
		
		
		
	}


}
