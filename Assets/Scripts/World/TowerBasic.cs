using UnityEngine;
using System.Collections;

//THIS SCRIPT IS ACCESED BY THE FIREBLASTER SCRIPT

public class TowerBasic : MobaStormAbility {
	
	
	
	//TOTAL SPEED OF THE PROJECTILE
	public float projectileSepeed = 4;
	public GameObject particleObj;

	
	protected override void OnStart ()
	{
		

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
			transform.Translate(Vector3.forward * projectileSepeed * Time.deltaTime);
			
			Vector3 destinationFinal = new Vector3(destinationGameObj.transform.position.x,destinationGameObj.transform.position.y + 1,destinationGameObj.transform.position.z);
			transform.LookAt(destinationFinal);
			
			float distanceFromPlayer = Vector3.Distance(destinationFinal,transform.position);
			
			if (distanceFromPlayer < 0.2f)
			{
				if (photonView.isMine)
				{
					MobaDealDamage(destinationGameObj);
					expended = true;
					CreateExplotion();

				}
				else
				{
					expended = true;
					CreateExplotion();
					Destroy(particleObj);
				}
				//uLink.Network.Destroy(this.gameObject);
				
			}
		}
		
		
	}
	
	protected override IEnumerator DestroyMyselfAfterSomeTime()
	{
		yield return new WaitForSeconds (expireTime);
		if (PhotonNetwork.isMasterClient)
		PhotonNetwork.Destroy(this.gameObject);
		
	}
	
}
