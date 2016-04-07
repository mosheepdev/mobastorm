﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//Custom character spell
public class FlyingBomb : MobaStormAbility {

	public float firingAngle 		= 45.0f;
	public float gravity 			= 9.8f;
	private Transform myTransform;
	private RaycastHit rocketHit;
	private float range 			= 1f;
	public float blastRadius 		= 1;


	void Start () {

	
	
	}

	protected override void DisableAbility () {
		Vector3 floorPosition = rocketHit.point;
		floorPosition.y = floorPosition.y - 0.2f;
		Instantiate(explotionObj, floorPosition, Quaternion.identity);
	}
	//Called when script has recieved all network data
	protected override void OnStart ()
	{
		originatorPlayerScript = originatorGameObj.GetComponent<PlayerStats>();
		originatorPlayerScript.abilityLocked = false;
		myTransform = transform;
		StartCoroutine(SimulateProjectile(myTransform));

	}


	void Update () 
	{

		if (!spellActive)
			return;
		
		if(Physics.Raycast( myTransform.position, new Vector3(0,-1f,0), out rocketHit, range) && expended == false && photonView.isMine)
		{
			
			//If the ray collider is the floor
			if (rocketHit.transform.tag == "Floor")
			{
				expended = true;

				List<Collider> struckObjects = new List<Collider>(Physics.OverlapSphere(rocketHit.point, blastRadius));
				
				foreach(Collider objectsHit in struckObjects)
				{
					//Used to deal dmg on the target
					MobaDealDamage(objectsHit.gameObject);

				}


				DisableAbility();
			

			}

		}
		if (transform.position.y < 1 && !expended)
		{
			expended = true;
			
			List<Collider> struckObjects = new List<Collider>(Physics.OverlapSphere(rocketHit.point, blastRadius));
			
			foreach(Collider objectsHit in struckObjects)
			{
				//Used to deal dmg on the target
				MobaDealDamage(objectsHit.gameObject);
				
			}
			
			DisableAbility();
		}
	}



	protected override IEnumerator DestroyMyselfAfterSomeTime()
	{
		yield return new WaitForSeconds (expireTime);
		PhotonNetwork.Destroy(this.gameObject);
		
	}

	IEnumerator SimulateProjectile(Transform Projectile)
	{
		Projectile = transform;
		myTransform = transform;
		// Move projectile to the position of throwing object + add some offset if needed.
		Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);
		
		// Calculate distance to target
		float target_Distance = Vector3.Distance(Projectile.position, floorPos);
		
		// Calculate the velocity needed to throw the object to the target at specified angle.
		float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
		
		// Extract the X  Y componenent of the velocity
		float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
		
		// Calculate flight time.
		float flightDuration = target_Distance / Vx;
		
		// Rotate projectile to face the target.
		Projectile.rotation = Quaternion.LookRotation(floorPos - Projectile.position);
		
		float elapse_time = 0;
		
		while (elapse_time < flightDuration && expended == false)
		{
			Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
			
			elapse_time += Time.deltaTime;
			
			yield return null;
		}


	}  
}
