using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//Custom character spell
public class AoeExplotion : MobaStormAbility {
	private RaycastHit rocketHit;
	private float blastRadius = 3;
	//Called when script has recieved all network data
	protected override void OnStart ()
	{
		if (photonView.isMine) {
			originatorPlayerScript = originatorGameObj.GetComponent<PlayerStats>();
			originatorPlayerScript.abilityLocked = false;
		}
		if (explotionObj) {
			Instantiate(explotionObj, transform.position, Quaternion.identity);
		}
	}
	
	void Update () 
	{
		if (!spellActive)
			return;
		
		//This spell follows the position of the player originator
		Vector3 pos = originatorGameObj.transform.position;
		
		transform.position = pos;

		if (photonView.isMine && !expended)
		{
			expended = true;
			
			List<Collider> struckObjects = new List<Collider>(Physics.OverlapSphere(rocketHit.point, blastRadius));
			
			foreach(Collider objectsHit in struckObjects)
			{
				//Used to deal dmg on the target
				MobaDealDamage(objectsHit.gameObject);

			}
		}
		
	}
	
	public void OnDestroy () {

	}
	
	protected override IEnumerator DestroyMyselfAfterSomeTime() 
	{
		yield return new WaitForSeconds(expireTime);	
		if (photonView.isMine) {
			
			PhotonNetwork.Destroy(gameObject);
		}
	}
}
