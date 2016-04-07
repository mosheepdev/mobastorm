using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[System.Serializable]
public class PlayerStats : BaseStats {

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	//void uLink_OnSerializeNetworkView (uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{

		// Always send transform (depending on reliability of the network view)
		if (stream.isWriting) {
			stream.SendNext(networkOwner);
			stream.SendNext(gameObject.name);
			stream.SendNext(myHealth);
			stream.SendNext (maxHealth);
			stream.SendNext (healthRegenerate);
			stream.SendNext(mana);
			stream.SendNext(baseMana);
			stream.SendNext(rechargeRateMana);
			stream.SendNext(playerTeam);
			stream.SendNext(playerScore);
			stream.SendNext(playerLvl);
			stream.SendNext(playerExp);
			stream.SendNext(maxPlayerExp);
			stream.SendNext(kills);
			stream.SendNext(deaths);
			stream.SendNext(assist);
			stream.SendNext(gold);
			stream.SendNext(rechargeRateGold);

			if (isPlayer)
			{
				stream.SendNext(destroyed);
				stream.SendNext(playerLvl);
				stream.SendNext(stats.basic.ad);
				stream.SendNext(stats.basic.ad);
				stream.SendNext(adRes);
				stream.SendNext(apRes);
				stream.SendNext(_adValueAdd);
				stream.SendNext(_apValueAdd);
				stream.SendNext(adResAdd);
				stream.SendNext(apResAdd);
				stream.SendNext(speed);
				stream.SendNext(speedAdd);
				stream.SendNext(attackRedAdd);
			}

		} else {
			
			networkOwner = (PhotonPlayer) stream.ReceiveNext();
			gameObject.name = (string) stream.ReceiveNext();
			myHealth = (float) stream.ReceiveNext();
			maxHealth = (float) stream.ReceiveNext();
			healthRegenerate = (float) stream.ReceiveNext();
			mana = (float) stream.ReceiveNext();
			baseMana = (float) stream.ReceiveNext();
			rechargeRateMana = (float) stream.ReceiveNext();
			playerTeam = (string) stream.ReceiveNext();
			playerScore = (int) stream.ReceiveNext();
			playerLvl = (int) stream.ReceiveNext();
			playerExp = (int) stream.ReceiveNext();
			maxPlayerExp = (int) stream.ReceiveNext();
			kills = (int) stream.ReceiveNext();
			deaths = (int) stream.ReceiveNext();
			assist = (int) stream.ReceiveNext();
			gold = (float) stream.ReceiveNext();
			rechargeRateGold = (float) stream.ReceiveNext();

			if (isPlayer)
			{
				destroyed = (bool) stream.ReceiveNext();
				playerLvl = (int) stream.ReceiveNext();
				stats.basic.ad = (float) stream.ReceiveNext();
				stats.basic.ap = (float) stream.ReceiveNext();
				adRes = (float) stream.ReceiveNext();
				apRes = (float) stream.ReceiveNext();
				_adValueAdd = (float) stream.ReceiveNext();
				_apValueAdd = (float) stream.ReceiveNext();
				adResAdd = (float) stream.ReceiveNext();
				apResAdd = (float) stream.ReceiveNext();
				speed = (float) stream.ReceiveNext();
				speedAdd = (float) stream.ReceiveNext();
				attackRedAdd = (float) stream.ReceiveNext();
			}

		}

	}

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find("GameManager_mn");
		
		dataScript = gameManager.GetComponent<PlayerDatabase>();

		playerCScript = GetComponent<PlayerControllerRTS>();

		agent = GetComponentInChildren<NavMeshAgent>();


		gameEventScript = gameManager.GetComponent<GameEvents>();


		mana = baseMana;
		myHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Regenerates the player health, mana and gold
		if (!destroyed)
			UpdateRegeneration();
		//MobaStorm.UpdateRegeneration(myHealth, maxHealth, healthRegenerate, mana, baseMana,rechargeRateMana, gold, rechargeRateGold, out myHealth, out mana, out gold);

		if (photonView.isMine && isPlayer)
		{
			CheckPlayerLevel();
		}

	}

	//SEND THE INITIAL PLAYER DATA TO ALL CLIENTS
	public void SetPlayerData (string pName,string pTeam)
	{
		photonView.RPC("SetPlayerDataPhoton", PhotonTargets.AllBuffered, pName, pTeam);
	}

	[PunRPC]
	void SetPlayerDataPhoton (string pName, string pTeam, PhotonMessageInfo info)
	{
		gameObject.name = pName;
		playerTeam = pTeam;
		//dataScript.AddPlayerDataToListPhoton(pName, pTeam, info.sender);

		if (pTeam == "blue")
		{
			this.gameObject.layer = 9;
			this.gameObject.tag = "BluePlayerTag";
			GameObject trigger = this.gameObject.transform.FindChild("Trigger").gameObject;
			trigger.tag = "BluePlayerTriggerTag";
			trigger.layer = 16;
		}
		else
		{
			this.gameObject.layer = 10;
			this.gameObject.tag = "RedPlayerTag";
			GameObject trigger = this.gameObject.transform.FindChild("Trigger").gameObject;
			trigger.tag = "RedPlayerTriggerTag";
			trigger.layer = 17;
		}

	}

	void CheckPlayerLevel()
	{
		if (playerExp >= maxPlayerExp)
		{
			ShowAbilityUpUi();
			playerLvl++;
			stats.basic.ad += 15;
			stats.basic.ap += 15;
			maxHealth += 25;
			playerExp = 0;
			//originatorPlayerScript.UpdateStatsOnClients();

		}
	}

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("Player Connected" + newPlayer);
	}



	/*
	[PunRPC]
	void UpdateDataToClients (bool sIsPlayer ,string sTeam, string sName, float sMyhealth, float sMaxHealth, float sHealthRegenRate, float sMana, float sBaseMana, bool sDestroyed, int slvl, float sAd, float sAp, float sAdRes,float sApRes,
		float sAdAdd, float sApAdd, float sAdResAdd,float sApResAdd, float sPlayerExp,float sMaxPlayerExp,int sGold, float sSpeed, float sSpeedAdd, float sAttackRedAdd)
	{
		Debug.Log("Data Updated");
		isPlayer = sIsPlayer;
		playerTeam = sTeam;
		gameObject.name = sName;
		myHealth = sMyhealth;
		maxHealth = sMaxHealth;
		mana = sMana;
		baseMana = sBaseMana;
		healthRegenerate = sHealthRegenRate;
		destroyed = sDestroyed;
		playerLvl = slvl;
		_bAdValue = sAd;
		_bApValue = sAp;
		adRes = sAdRes;
		apRes = sApRes;
		_adValueAdd = sAdAdd;
		_apValueAdd = sApAdd;
		adResAdd = sAdResAdd;
		apResAdd = sApResAdd;
		gold = sGold;
		playerExp = sPlayerExp;
		maxPlayerExp = sMaxPlayerExp;
		speed = sSpeed;
		speedAdd = sSpeedAdd;
		attackRedAdd = sAttackRedAdd;

		gameObject.name = sName;

		if (isPlayer)
		{
			if (sTeam == "blue")
			{
				gameObject.layer = 9;
				gameObject.tag = "BluePlayerTag";
				trigger = transform.FindChild("Trigger").gameObject;
				trigger.tag = "BluePlayerTriggerTag";
				trigger.layer = 16;
			}
			else
			{
				gameObject.layer = 10;
				gameObject.tag = "RedPlayerTag";
				trigger = transform.FindChild("Trigger").gameObject;
				trigger.tag = "RedPlayerTriggerTag";
				trigger.layer = 17;
			}
		}
	}
	*/


	//RUNNING ON THE CLIENT SIDE OWNER ONLY
	[PunRPC]
	private void UpdateInitDataPlayers (string sPrefabPath, DmgDataclass.type bType, DmgDataclass.type qType, DmgDataclass.type wType, DmgDataclass.type eType, DmgDataclass.type rType)
	{
		stats.prefabPath = sPrefabPath;
		stats.basic.weaponType = bType;
		stats.q.weaponType = qType;
		stats.w.weaponType = wType;
		stats.e.weaponType = eType;
		stats.r.weaponType = rType;


	}
	//USED TO SEND A RPC TO THE CLIENTS WITH THE HEALTH
	public void UpdateAdds ()
	{
		//if (uLink.Network.isServer)
		//{
			//networkView.RPC("UpdateAddsClient", uLink.RPCMode.Others, maxHealth, baseMana, _adValueAdd, _apValueAdd, adResAdd, apResAdd, speedAdd, attackRedAdd, cdRedAdd);
		//}
		
	}
	[PunRPC]
	private void UpdateAddsClient (float SmaxHealth, float SbaseMana, float S_adValueAdd,float S_apValueAdd,float SadResAdd,float SapResAdd,float SspeedAdd,float SattackRedAdd,float ScdRedAdd)
	{
		maxHealth = SmaxHealth;
		baseMana = SbaseMana;
		_adValueAdd = S_adValueAdd;
		_apValueAdd = S_apValueAdd;
		adResAdd = SadResAdd;
		apResAdd = SapResAdd;
		speedAdd = SspeedAdd;
		attackRedAdd = SattackRedAdd;
		cdRedAdd = ScdRedAdd;
	}

	//RUNING ON THE CLIENT ONLY
	[PunRPC]
	void UpdateClientHealth (float sHealth, float sMaxHealth)
	{
		myHealth =  sHealth;
		maxHealth = sMaxHealth;
	}


	//USED TO SEND A RPC TO THE CLIENTS WITH THE MANA
	public void UpdateManaToClients ()
	{
		//networkView.RPC("UpdateClientMana", uLink.RPCMode.Others, mana, baseMana, rechargeRateMana);
		
	}

	//USED TO SEND A RPC TO THE CLIENTS WITH THE NEW GOLD ADDED
	public void UpdateGoldToClients (int goldToAdd)
	{
		gold += goldToAdd;
		//networkView.RPC("UpdateClientGold", uLink.RPCMode.Others, gold);
		
	}
	[PunRPC]
	private void UpdateClientGold (int value) {
		gold = value;
	}
	
	//USED TO SEND A RPC TO THE CLIENTS WITH THE NEW EXP ADDED
	public void UpdateExpToClients (int expToAdd)
	{
		playerExp += expToAdd;
		//networkView.RPC("UpdateClientExp", uLink.RPCMode.Others, playerExp);
		
	}
	[PunRPC]
	void UpdateClientExp (int value) {
		playerExp = value;
	}

	//USED TO SEND A RPC TO THE CLIENTS WITH THE ABILITYLOCKED VALUE
	public void UpdateAbilityLockedClients (bool value) 
	{
		//if (uLink.Network.isServer) {
		//	abilityLocked = value;
			//networkView.RPC("UpdateClientAbilityLocked", uLink.RPCMode.Others, value);
		//}
	}
	[PunRPC]
	void UpdateClientAbilityLocked (bool sAbilityLocked) 
	{
		abilityLocked =  sAbilityLocked;	
	}
	//AddPlayerLevelOwner
	//CALCULATE THE TOTAL DMG
	private float GetFinalDmg (float adDmg, float apDmg, float adRes, float apRes)
	{
		float dmgTotal = 0f;
		float dmgTotalAd = 0f;
		float dmgTotalAp = 0f;

		dmgTotalAd = adDmg - adRes;
		dmgTotalAp= apDmg - apRes;
		if (dmgTotalAd<0)
			dmgTotalAd = 0;
		if (dmgTotalAp<0)
			dmgTotalAp = 0;

		dmgTotal = dmgTotalAd + dmgTotalAp;
		return dmgTotal;
	}
	//USED TO DRAIN HEALTH ON THE PHOTONVIEW OWNER ONLY
	[PunRPC]
	public void DrainHealthPhotonOLD (float _ad, float _ap, string attacker, string originatorTag, string originatorCharName)
	{
		
		photonView.RPC("DrainHealth", photonView.owner,  _ad,  _ap,  attacker,  originatorTag, originatorCharName);
	}
	//USED TO DRAIN HEALTH ON THE PHOTONVIEW OWNER ONLY
	[PunRPC]
	public void DrainHealth (float _ad, float _ap, string attacker, string originatorTag, string originatorCharName)
	{

		Debug.Log("Draining Health on this view");
			GameObject originatorObj = GameObject.Find(attacker);

			if (!destroyed)
			{
				myHealth = myHealth - GetFinalDmg(_ad, _ap, adRes + adResAdd, apRes + apResAdd);
				if (originatorObj.tag == "RedPlayerTag" || originatorObj.tag == "BluePlayerTag")
				{
					AddCaptureAttackers(attacker,originatorObj, Time.time);
				}
			}

			if (myHealth <= 0 && destroyed == false)
			{
				photonView.RPC("ImDead", PhotonTargets.All);
				myHealth = 0;
				destroyed = true;

				for(int i = 0; i < dataScript.PlayerList.Count; i++)
				{

					if(dataScript.PlayerList[i].playerName == attacker)
					{
						//show dmg
						photonView.RPC("ShowGoldUi", dataScript.PlayerList[i].networkPlayer, this.transform.name, attacker, goldToGive, true);
					}

				}

				for (int i = 0; i < attackersList.Count; i++) // Loop with for.
				{
					attackersList[i]._Obj.GetComponent<PlayerStats>().UpdateExpToClients(expToGive / attackersList.Count);
					attackersList[i]._Obj.GetComponent<PlayerStats>().UpdateGoldToClients(goldToGive / attackersList.Count);
				}

				if (gameObject.tag == "RedPlayerTag" || gameObject.tag == "BluePlayerTag")
				{
					//call the method SendSlainAlert(); on GAME MANAGER
					CharacterClass.charName thisCharName = GetComponent<PlayerStats>().stats.characterName;
					gameManager.GetComponent<GameEvents>().SendSlainAlert(originatorCharName, attacker, thisCharName.ToString(), this.gameObject.name);

				}

				if (originatorTag == "RedPlayerTag" || originatorTag == "BluePlayerTag")
				{

					//If the reciever  is a player 
					if (gameObject.tag == "RedPlayerTag" || gameObject.tag == "BluePlayerTag")
					{
						dataScript.EditPlayerListWithScore(attacker);
						PlayerStats originatorPlayerScript = originatorObj.GetComponent<PlayerStats>();
						originatorPlayerScript.AddPlayerKills();
					}
					else
					{
						//ADD HERE THE CODE IF YOU KILL A MONSTER /TOWER / MINION
					}

				}

			}

		
	}
	//USED TO DRAIN HEALTH ON ANY MINION / TOWER / MONSTER
	[PunRPC]
	public void DrainHealthMasterOLD (float _ad, float _ap, string attacker, string originatorTag, string originatorCharName)
	{
		
		if (PhotonNetwork.isMasterClient)
		{
			GameObject originatorObj = GameObject.Find(attacker);
			if (!destroyed)
			{
				myHealth = myHealth - GetFinalDmg(_ad, _ap, adRes + adResAdd, apRes + apResAdd);
				if (originatorObj.tag == "RedPlayerTag" || originatorObj.tag == "BluePlayerTag")
				{
					AddCaptureAttackers(attacker,originatorObj, Time.time);
				}
			}

			if (myHealth <= 0 && destroyed == false)
			{
				myHealth = 0;
				destroyed = true;
				photonView.RPC("ImDead", PhotonTargets.All);
				for(int i = 0; i < dataScript.PlayerList.Count; i++)
				{
					if(dataScript.PlayerList[i].playerName == attacker)
					{
						photonView.RPC("ShowGoldUi", dataScript.PlayerList[i].networkPlayer, this.transform.name, attacker, goldToGive, true);
					}
				}

				for (int i = 0; i < attackersList.Count; i++) // Loop with for.
				{
					attackersList[i]._Obj.GetComponent<PlayerStats>().UpdateExpToClients(expToGive / attackersList.Count);
					attackersList[i]._Obj.GetComponent<PlayerStats>().UpdateGoldToClients(goldToGive / attackersList.Count);
				}

				if (gameObject.tag == "RedPlayerTag" || gameObject.tag == "BluePlayerTag")
				{
					//call the method SendSlainAlert(); on GAME MANAGER
					CharacterClass.charName thisCharName = GetComponent<PlayerStats>().stats.characterName;
					gameManager.GetComponent<GameEvents>().SendSlainAlert(originatorCharName, attacker, thisCharName.ToString(), this.gameObject.name);
				}

				if (originatorTag == "RedPlayerTag" || originatorTag == "BluePlayerTag")
				{
					//If the reciever  is a player 
					if (gameObject.tag == "RedPlayerTag" || gameObject.tag == "BluePlayerTag")
					{
						dataScript.EditPlayerListWithScore(attacker);
						PlayerStats originatorPlayerScript = originatorObj.GetComponent<PlayerStats>();
						originatorPlayerScript.AddPlayerKills();
					}
					else
						//If the  reciever is not a player
					{
						//ADD HERE THE CODE IF YOU KILL A MONSTER /TOWER / MINION
					}

				}

			}
		}
		else
		{
			photonView.RPC("DrainHealthPhoton", PhotonTargets.MasterClient,  _ad,  _ap,  attacker,  originatorTag, originatorCharName);
		}
	}
	//ADD THE CURRENT ATTACKER NAME TO THE LIST attackersList
	void AddCaptureAttackers(string attackerTag, GameObject originatorObj, float time)
	{
		for (int i = 0; i < attackersList.Count; i++) // Loop with for.
		{
			if (attackersList[i]._Name == attackerTag)
			{
				RemoveCaptureAttackers(i);
			}

		}
		AttackersDataClass capture = new AttackersDataClass ();
		capture._Name = attackerTag;
		capture._Obj = originatorObj;
		capture._time = time;
		attackersList.Add(capture);

	}
	//REMOVES THE CURRENT ATTACKER INDEX ON THE LIST attackersList
	void RemoveCaptureAttackers(int index)
	{
		attackersList.RemoveAt(index);
	}

	public void AddPlayerKills()
	{
		//photonView.RPC("AddPlayerLevelOwner", photonView.owner);
	}

	[PunRPC]
	public void AddPlayerKillsOwner()
	{
		kills++;
	}
	//SEND A RPC WHEN THE PLAYER LEVEL UP TO SHOW THE SPELL LEVEL UP BUTTONS ON THE OWNER PLAYER
	[PunRPC]
	public void ShowAbilityUpUi()
	{
		gameEventScript.ShowSpellUpButtons();
		//networkView.RPC("ShowAbilityUpUi", networkOwner);
		photonView.RPC("ShowLvlUpParticle", PhotonTargets.All);
	
	}

	//SEND A RPC TO LEVEL UP ANY ABILITY
	public void UpgradeAbility(PlayerControllerRTS.PlayerState type, bool levelUpAbility)
	{
		Debug.Log("UPGRADING ABILITY");
		//networkView.RPC("UpgradeAbilityServer", uLink.RPCMode.Server, type, levelUpAbility);
		switch (type)
		{
		case PlayerControllerRTS.PlayerState.attackingBasic:
			if (playerCScript.playerStScript.stats.basic.weaponLvl < 1 && levelUpAbility)
			{
				playerCScript.playerStScript.stats.basic.weaponLvl ++;
				playerCScript.playerStScript.stats.basic.cdrTStamp = Time.time;
			}

			//networkView.RPC("UpgradeAbilityClient", uLink.RPCMode.Owner, PlayerControllerRTS.PlayerState.attackingBasic, playerCScript.playerStScript.stats.basic.cdr, 
			// playerCScript.playerStScript.stats.basic.ad, playerCScript.playerStScript.stats.basic.ap, playerCScript.playerStScript.stats.basic.weaponLvl);


			break;
		case PlayerControllerRTS.PlayerState.attackingQ:
			if (playerCScript.playerStScript.stats.q.weaponLvl<1 && levelUpAbility)
			{
				playerCScript.playerStScript.stats.q.weaponLvl ++;
				playerCScript.playerStScript.stats.q.cdrTStamp = Time.time;
				playerCScript.cdrUiQScript.active = true;
			}

			//networkView.RPC("UpgradeAbilityClient", uLink.RPCMode.Owner, PlayerControllerRTS.PlayerState.attackingQ, playerCScript.playerStScript.stats.q.cdr, 
			// playerCScript.playerStScript.stats.q.ad, playerCScript.playerStScript.stats.q.ap, playerCScript.playerStScript.stats.q.weaponLvl);
			break;
		case PlayerControllerRTS.PlayerState.attackingW:
			if (playerCScript.playerStScript.stats.w.weaponLvl<1 && levelUpAbility)
			{
				playerCScript.playerStScript.stats.w.weaponLvl ++;
				playerCScript.playerStScript.stats.w.cdrTStamp = Time.time;
				playerCScript.cdrUiWScript.active = true;
			}

			//networkView.RPC("UpgradeAbilityClient", uLink.RPCMode.Owner, PlayerControllerRTS.PlayerState.attackingW, playerCScript.playerStScript.stats.w.cdr, 
			//playerCScript.playerStScript.stats.w.ad, playerCScript.playerStScript.stats.w.ap, playerCScript.playerStScript.stats.w.weaponLvl);
			break;
		case PlayerControllerRTS.PlayerState.attackingE:
			if (playerCScript.playerStScript.stats.e.weaponLvl<1 && levelUpAbility)
			{
				playerCScript.playerStScript.stats.e.weaponLvl ++;
				playerCScript.playerStScript.stats.e.cdrTStamp = Time.time;
				playerCScript.cdrUiEScript.active = true;
			}

			//networkView.RPC("UpgradeAbilityClient", uLink.RPCMode.Owner, PlayerControllerRTS.PlayerState.attackingE, playerCScript.playerStScript.stats.e.cdr, 
			//playerCScript.playerStScript.stats.e.ad, playerCScript.playerStScript.stats.e.ap, playerCScript.playerStScript.stats.e.weaponLvl);
			break;
		case PlayerControllerRTS.PlayerState.attackingR:
			if (playerCScript.playerStScript.stats.r.weaponLvl <1 && levelUpAbility)
			{
				playerCScript.playerStScript.stats.r.weaponLvl ++;
				playerCScript.playerStScript.stats.r.cdrTStamp = Time.time;
				playerCScript.cdrUiRScript.active = true;
			}

			//networkView.RPC("UpgradeAbilityClient", uLink.RPCMode.Owner, PlayerControllerRTS.PlayerState.attackingR, playerCScript.playerStScript.stats.r.cdr, 
			//playerCScript.playerStScript.stats.r.ad, playerCScript.playerStScript.stats.r.ap, playerCScript.playerStScript.stats.r.weaponLvl);
			break;
		}

		
	}



	//SPAWN THE uiDamageDealt OBJ TO SHOW THE DAMAGE POP UP ON THE CLIENT
	[PunRPC]
	void ShowDmgUi (string reciever, string attacker, float totalDmg, bool dealing)
	{

		GameObject uiDamage = Instantiate(uiDamageDealt, new Vector3 (0,0,0), Quaternion.identity) as GameObject; 
		DamageDealtUI label = uiDamage.GetComponent<DamageDealtUI>();
		label.totalDmg = totalDmg;
		label.reciever = reciever;
		label.dealingDamage = dealing;

	}

	[PunRPC]
	void ShowGoldUi (string reciever, string attacker, int totalGold, bool dealing)
	{
		if (dataScript.playerName == attacker)
		{
			GameObject uiDamage = Instantiate(uiDamageDealt, new Vector3 (0,0,0), Quaternion.identity) as GameObject; 
			DamageDealtUI label = uiDamage.GetComponent<DamageDealtUI>();
			label.totalDmg = totalGold;
			label.reciever = reciever;
			label.dealingDamage = dealing;
		}
		
	}


}
