using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class BaseStats : Photon.MonoBehaviour {

	public CharacterClass stats = new CharacterClass ();
	//HEALTH VARIABLES
	public float myHealth 			= 100;
	public float maxHealth			= 100;
	public float healthRegenerate 	= 1.3f;

	//MANA VARIABLES
	public float mana;
	public float baseMana 			= 100;
	public float rechargeRateMana 	= 5f;


	//PLAYER VARIABLES
	public CharacterClass.charName charName;
	public PhotonPlayer networkOwner;
	public bool isPlayer = false;
	public bool isCreator = false;	
	public PlayerControllerRTS playerCScript;
	public string playerTeam;
	public int playerScore;
	public int playerLvl 			= 1;
	public int playerExp;
	public int maxPlayerExp 		= 250;
	public int kills;
	public int deaths;
	public int assist;
	public float gold;
	public float rechargeRateGold 	= 1;
	public float adResAdd;
	public float apResAdd;
	public float speedAdd;
	public float attackRedAdd;
	public float cdRedAdd;
	public float _adValueAdd;
	public float _apValueAdd;
	public GameObject hotObj;
	public GameObject dotObj;
	public GameObject stunObj;
	public GameObject slowObj;
	public GameObject recallObj;
	public bool recalling;
	//how much time players will stay on the attackers list of the damaged enemy
	public List<AttackersDataClass> attackersList = new List<AttackersDataClass>();
	
	//GENERAL VARIABLES
	public float _bAdValue;
	public float _bApValue;
	public bool destroyed;
	public int expToGive 		= 40;
	public int goldToGive 	= 150;
	public bool respawns 		= false;
	public int respawnTime 	= 10;
	public float adRes;
	public float apRes;
	public float speed 			= 1.4f;
	public NavMeshAgent agent;

	//USED WHEN AN ABILITY OR SOMETHING OVERRIDE THE MOVEMENTS OR ANIMATION OF THE PLAYER
	public bool charLocked = false;
	public bool abilityLocked = false;

	//REFERENCE GAMEOBJ
	public GameObject gameManager;
	public PlayerDatabase dataScript;
	//public GameObject trigger;

	//THIS SCRIPT IS USED UPDATE EVENTS FOR THE PLAYER LEVELING
	public GameEvents gameEventScript;

	//GAMEOBJ THAT SHOWS THE DAMAGE POP UP
	public	GameObject uiDamageDealt;




	public void UpdateRegeneration()
	{
		if (!destroyed)
		{

			//REGENERATING MANA
			if(mana < baseMana){
				mana = mana + rechargeRateMana * Time.deltaTime;	
			}
			if(mana > baseMana) {
				mana = baseMana;	
			}
			if(mana < 0) {
				mana = 0;	
			}
			
			//REGENERATING HEALTH
			if (maxHealth > myHealth && myHealth > 0) {
				myHealth = myHealth + healthRegenerate * Time.deltaTime;
			}
			//IF THE PLAYER HEALTH EXCEEDS THE MAXHEALTH THEN SET IT BACK TO THE MAX HEALTH
			if (myHealth > maxHealth) {
				myHealth = maxHealth;
			}
			//INCREASE THE GOLD OVERTIME
			gold = gold + rechargeRateGold  * Time.deltaTime;

		}
	}

}

	