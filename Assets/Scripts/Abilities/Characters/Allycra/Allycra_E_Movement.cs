using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Allycra_E_Movement : MobaStormAbility {


	protected override void OnStart ()
	{

		if (photonView.isMine) {
			originatorPlayerScript = originatorGameObj.GetComponent<PlayerStats>();
			originatorPlayerScript.agent.SetDestination(floorPos);
			originatorPlayerScript.agent.Resume();
			//SPEED UP BY 400%
			originatorPlayerScript.speedAdd += 400;

		}
		
	}




	void Update () 
	{

		if (!spellActive)
			return;

		if (photonView.isMine) {
			PlayerControllerRTS originatorControllerScript = originatorGameObj.GetComponent<PlayerControllerRTS>();
			originatorControllerScript.MoveOrChase();
		}


		Vector3 pos = originatorGameObj.transform.position;
		transform.position = pos;


		float distanceFromDestination = Vector3.Distance(floorPos,transform.position);
		
		if (distanceFromDestination < 0.3f)
		{

			PhotonNetwork.Destroy(gameObject);
		}
	}

	public void OnDestroy () {
		originatorPlayerScript.agent.Stop();
		//SLOWER DOWN BY 400%
		originatorPlayerScript.speedAdd -= 400;
		PlayerControllerRTS originatorControllerScript = originatorGameObj.GetComponent<PlayerControllerRTS>();
		originatorControllerScript.playerStatus = PlayerControllerRTS.PlayerState.idle;
		originatorPlayerScript.abilityLocked = false;
		originatorPlayerScript.charLocked = false;
	}



	protected override IEnumerator DestroyMyselfAfterSomeTime() 
	{
		yield return new WaitForSeconds(expireTime);	

		if (photonView.isMine) {

			PhotonNetwork.Destroy(gameObject);
		}

	}


}
