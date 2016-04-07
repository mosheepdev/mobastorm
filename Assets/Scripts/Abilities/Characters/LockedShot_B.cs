using UnityEngine;
using System.Collections;

//Custom character spell
public class LockedShot_B : MobaStormAbility {
	
	public float projectileSpeed = 20;
	public GameObject particleObj;



	protected override void OnStart ()
	{

		originatorPlayerScript = originatorGameObj.GetComponent<PlayerStats>();
		originatorPlayerScript.abilityLocked = false;
		
	}
	public void OnDestroy () {

	}

	public void CreateExplotion () {
		Instantiate(explotionObj, transform.position, Quaternion.identity);
	}
	void Update () 
	{


		if (!spellActive)
			return;


		if (destinationGameObj && !expended)
		{
			transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);

			Vector3 destinationFinal = new Vector3(destinationGameObj.transform.position.x,transform.position.y,destinationGameObj.transform.position.z);
			transform.LookAt(destinationFinal);

			float distanceFromPlayer = Vector3.Distance(destinationFinal,transform.position);
			//If the projectile reach the enemy position
			if (distanceFromPlayer < 0.5f)
			{
				if (photonView.isMine)
				{
					Debug.Log("MOBA DEAL DMAGE");
					//Used to deal dmg on the target
					MobaDealDamage(destinationGameObj);

				}
				else
				{
					expended = true;

				}
				CreateExplotion();
				Destroy(particleObj);

			}
		}


	}

	protected override IEnumerator DestroyMyselfAfterSomeTime()
	{
		yield return new WaitForSeconds (expireTime);
		if (photonView.isMine)
		PhotonNetwork.Destroy(this.gameObject);

	}

}
