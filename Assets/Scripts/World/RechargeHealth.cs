using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the RechargeHealthPickup. When a player
/// walks into this object they are given some health and the pickup 
/// dissappears for some duration.
/// 
/// This script accesses the player's HealthAndDamage script to 
/// increase their health.
/// </summary>

public class RechargeHealth : Photon.MonoBehaviour {
	
	//Variables Start_________________________________________________________	
	
	private bool taken = false;
	
	public float HealthGain = 50;
	
	public float reSpawnTime = 20;
	
	private float rotateSpeed = 1;

	public bool thisRespawn = false;
	//Variables End___________________________________________________________
	
		
	
	// Update is called once per frame
	void Update () 
	{
		//Make the RechargeHealthPickup constantly rotate.
		
		transform.Rotate(Vector3.up * (rotateSpeed + Time.deltaTime));
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "BlueTeamTrigger" && taken == false || other.tag == "RedTeamTrigger" && taken == false)
		{			
			PlayerStats playerStatsScript = other.GetComponent<PlayerStats>();
			
			playerStatsScript.myHealth += HealthGain;
			
			//If health jumps up above max health then set it
			//back to the max health level.
			
			if(playerStatsScript.myHealth > playerStatsScript.maxHealth)
			{
				playerStatsScript.myHealth = playerStatsScript.maxHealth;	
			}
			
			
			//Only the server deactivates and reactivates the pickup. It does this
			//across the network and uses buffered RPCs so players just joining can't
			//use the pickups.
			
			if(PhotonNetwork.isMasterClient)
			{	
				if (thisRespawn == false)
				{
					PhotonNetwork.Destroy(gameObject);
				}
				else
				{
					photonView.RPC("DeactivateHealthPickup", PhotonTargets.AllBuffered);
					
					StartCoroutine(ReSpawn());

					taken = true;
				}
			
			}

		}
		
	}
	
	IEnumerator ReSpawn() 
	{	
		//After a certain duration make the cube visible again
		//and turn its light back on.
		
        yield return new WaitForSeconds(reSpawnTime);
		
		photonView.RPC("ReactivateHealthPickup", PhotonTargets.AllBuffered);
    }

	
	[PunRPC]
	void DeactivateHealthPickup ()
	{			
		transform.GetComponent<Renderer>().enabled = false;
		
		transform.GetComponent<Light>().enabled = false;
		
		taken = true;
	}
	
	
	[PunRPC]
	void ReactivateHealthPickup ()
	{
		taken = false;
		
		transform.GetComponent<Renderer>().enabled = true;
		
		transform.GetComponent<Light>().enabled = true;
	}
}
