using UnityEngine;
using System.Collections;

//Custom character spell
public class FrontSkillShot : MobaStormAbility {

	public float spellSpeed = 10;
	private RaycastHit hit;
	public float range = 2f;

	//Called when script has recieved all network data
	protected override void OnStart ()
	{
		originatorPlayerScript = originatorGameObj.GetComponent<PlayerStats>();
		originatorPlayerScript.abilityLocked = false;
	}
	
	public void OnDestroy () {
		Instantiate(explotionObj, transform.position, Quaternion.identity);
	}


	void Update () 
	{
		

	}

	void FixedUpdate()
	{
		if (expended)
		{
			//networkView.RPC("LaunchExplotion", uLink.RPCMode.Others);
			PhotonNetwork.Destroy(this.gameObject);

		}
		
		if (!spellActive)
			return;
		
		transform.Translate(Vector3.forward * spellSpeed * Time.deltaTime);
		//Raycast to the vector.forward to detect collisions with enemies using layermask mask
		if (Physics.Raycast(transform.position, transform.forward, out hit, range, mask) && expended == false && photonView.isMine && !expended)
		{		
			//Used to deal dmg on the target
			MobaDealDamage(hit.transform.gameObject);
		}
	}

	protected override IEnumerator DestroyMyselfAfterSomeTime() 
	{
		yield return new WaitForSeconds(expireTime);	
		if (photonView.isMine) {	
			PhotonNetwork.Destroy(gameObject);
		}
	}
		


}
