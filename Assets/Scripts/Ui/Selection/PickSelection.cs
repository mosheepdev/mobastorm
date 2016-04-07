using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class PickSelection : Photon.MonoBehaviour {

	//Public list to store all character class portraits
	public List<Char_Pick_DataClass> charPickList = new List<Char_Pick_DataClass>();

	private GameObject gameManager;

	private PlayerDatabase dataScript;
	
	//public string PlayerName;

	[HideInInspector] private int currentSlot =1;

	private int currentSelection;

	private float inventoryWidth, inventoryHight;

	public int totalSlots;
	
	private float slotSize = 60;
	

	private GameObject newSlot;

	[HideInInspector] public List<GameObject> allslots;

	private int emptySlot;
	

	//char selection variables
	private List<GameObject> allCharslots;
	
	private GameObject charRectObj;

	private RectTransform charselectionRect;

	public GameObject charselectionObj;

	public float charSlotPaddingLeft, charSlotPaddingTop;

	private float charSelWidth, charSelHight;

	private int charRows = 2;

	private int charColumns = 6;

	private int charSlotCount = 0;

	private int charSlots = 0;

	private string prefab;
	//Directory of the character portraits prefabs
	private string portraitDir = "UiResources/Characters/";
	//Directory of the characters prefabs
	private string prefabDir = "Characters/";

	public string charPickedDir;
	public CharacterClass.charName charPickedName;
	public string charPickedTeam;
	
	public Client clientScript;


	void Start () {

		//YOU NEED TO ADD THE NEW CHARACTERS HERE IN ORDER TO SHOW IN THE PICK SELECTION
		AddCharToList(CharacterClass.charName.Galilei, true, portraitDir + "Galilei", prefabDir + "Galilei@", "Galilei");
		AddCharToList(CharacterClass.charName.Allycra, true, portraitDir + "Allycra", prefabDir +  "Allycra@", "Allycra");
	
		gameManager = GameObject.Find("GameManager_mn");
		
		dataScript = gameManager.GetComponent<PlayerDatabase>();
		//DontDestroyOnLoad(this.gameObject);

	}

	void AddCharToList(CharacterClass.charName charName, bool _available, string charPortraitDir, string charPrefabDir, string toolTip)
	{
		Char_Pick_DataClass capture = new Char_Pick_DataClass ();
		capture.characterName = charName;
		capture._available = _available;
		capture.charPortraitDir = charPortraitDir;
		capture.toolTip = toolTip;
		capture.charPrefabDir = charPrefabDir;
		charPickList.Add(capture);
	}
	// Update is called once per frame
	void Update () {
	
	}



	public void CreatePickLayout()
	{


		for(int i = 0; i < charPickList.Count; i++)
		{
			charSlots++;
		}

		allCharslots = new List<GameObject>();


		if (charSlots> charColumns)
		{
		charRows = charSlots / charColumns;
		}
		else
		{
			charRows = 1;
		}

		charSelWidth = (charColumns * (slotSize + charSlotPaddingLeft)) + charSlotPaddingLeft;
		
		charSelHight = charRows * (slotSize + charSlotPaddingTop) + charSlotPaddingTop;
		

		charRectObj = Instantiate(charselectionObj) as GameObject;
		charselectionRect = charRectObj.GetComponent<RectTransform>();
		charselectionRect.transform.SetParent(this.transform);
		charselectionRect.localPosition = new Vector3(640 - charSelWidth/2, -160, 0);
		charselectionRect.localScale = Vector3.one;
		charselectionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, charSelWidth);
		
		charselectionRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, charSelHight);

		for (int y = 0; y < charRows; y++)
		{
			for (int x = 0; x < charColumns; x++)
			{

			//Create portraits from a list of available characters
					if (charPickList[charSlotCount]._available == true)
					{
						prefab = charPickList[charSlotCount].charPortraitDir;
						
						GameObject newSlot = Instantiate(Resources.Load(prefab, typeof(GameObject))) as GameObject;
						RectTransform slotRect = newSlot.GetComponent<RectTransform>();
						

						
						charSlotCount++;
						

						
						newSlot.transform.SetParent(this.transform);
					slotRect.localPosition = charselectionRect.localPosition + new Vector3(charSlotPaddingLeft + ((slotSize +charSlotPaddingLeft)  * x), -charSlotPaddingTop * (y+1) - (slotSize * y));
					slotRect.localScale = Vector3.one;
						
						slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize );
						
						slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize );
						
						allCharslots.Add(newSlot);

						
					}
					if (charSlotCount == charSlots)
					return;

				

			}
			
			
			
		}


	}

	public void CreateLayout()
	{
		allslots = new List<GameObject>();

		foreach(GameObject slotObj in GameObject.FindGameObjectsWithTag("Slot")) {
			allslots.Add(slotObj);
		}
		foreach (GameObject slot in allslots) 
		{
			Char_Slot slotScript = slot.GetComponent<Char_Slot>();


			if(slotScript.slotNumber > totalSlots)
			{
			Destroy(slot.transform.parent.gameObject);
			}

		}
	
	}
	public void uLink_OnDisconnectedFromServer()
	{
		foreach (GameObject slot in allslots) 
		{
			Char_Slot tmp = slot.GetComponent<Char_Slot>();
			tmp.charName = CharacterClass.charName.Empty;
			tmp.isEmpty = true;
			tmp.isReady = false;
			tmp.NameBox.text = ""; 
			Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();
			tmp.ChangePortrait("", PhotonNetwork.player , tmpSlot.slotEmpty, true);

	
		}
	}
	


	public void ChangeSlot(int slotNumber)
	{
		//networkView.RPC("UpdateChangeSlot", uLink.RPCMode.Server, PhotonNetwork.playerName, currentSelection,currentSlot, slotNumber, uLink.Network.player);
		UpdateChangeSlot(PhotonNetwork.playerName, currentSelection,currentSlot, slotNumber, PhotonNetwork.player);
	}

	//WHEN A PLAYER CONNECTS TO THE SERVER, IT WILL SEND AN RPC TO RETRIEVE ALL THE DATA ABOUT THE CURRENT PICK SELECTION
	void OnJoinedRoom()
	{
		GetComponent<AudioSource>().Play();
		if (PhotonNetwork.isMasterClient)
		{
			UpdatePortraits(PhotonNetwork.playerName.ToString(), PhotonNetwork.player);
			//photonView.RPC ("UpdatePortraits", PhotonTargets.MasterClient, PhotonNetwork.playerName.ToString(), PhotonNetwork.player);
		}
		else
		{
			photonView.RPC ("UpdatePortraits", PhotonTargets.MasterClient, PhotonNetwork.playerName.ToString(), PhotonNetwork.player);

		}
	}

	//THIS METHOD WILL UPDATE THE PLAYER WITH EACH SLOT INFO
	[PunRPC]
	void UpdatePortraits(string Pname, PhotonPlayer Owner)
	{
		
		
		if (PhotonNetwork.isMasterClient)
		{
			bool slotGiven = false;
			//THIS LINE ADD THE NEW CONECTED PLAYER TO THE PLAYER DATABASE ON GAME MANAGER OBJ
			dataScript.AddPlayerNameToList(Pname, Owner);
			
			for(int i = 0; i < totalSlots + 1; i++)
			{

				foreach (GameObject slot in allslots) 
				{
					Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();
					if (tmpSlot.isEmpty && slotGiven == false && tmpSlot.slotNumber == i) {
						tmpSlot.ChangePortrait(Pname, Owner, tmpSlot.slotEmpty, false);

						photonView.RPC ("SendSlotNumber", PhotonTargets.Others, Pname, tmpSlot.slotNumber, Owner);
						photonView.RPC ("UpdateCharPick", PhotonTargets.Others, tmpSlot.NameBox.text, tmpSlot.slotNumber, tmpSlot.charName, Owner, tmpSlot.isReady);
						//networkView.RPC("SendSlotNumber", uLink.RPCMode.Others, Pname, tmpSlot.slotNumber, Owner);
						//networkView.RPC("UpdateCharPick", Owner, tmpSlot.NameBox.text, tmpSlot.slotNumber, tmpSlot.charName, Owner, tmpSlot.isReady);
						
						slotGiven = true;
						
					}
					
					if (tmpSlot.isEmpty == false) {
						
						
						photonView.RPC ("UpdateCharPick", PhotonTargets.Others, tmpSlot.NameBox.text, tmpSlot.slotNumber, tmpSlot.charName, tmpSlot.networkOwner, tmpSlot.isReady);
						//networkView.RPC("UpdateCharPick", Owner, tmpSlot.NameBox.text, tmpSlot.slotNumber, tmpSlot.charName, tmpSlot.networkOwner, tmpSlot.isReady);
						
					}
					
				}
				
			}
		}
		
	}

	public void Ready()
	{
		//if (PhotonNetwork.isMasterClient)
		//{
			ReadyRPC(currentSlot, PhotonNetwork.player);
			GetComponent<AudioSource>().Stop();
		//}
		//else
		//{
		//	photonView.RPC ("ReadyRPC", PhotonTargets.MasterClient, currentSlot, PhotonNetwork.player);
		//	GetComponent<AudioSource>().Stop();
		//}

	}
	[PunRPC]
	void ReadyRPC(int currentSlotRPC, PhotonPlayer Owner)
	{
		//if (PhotonNetwork.isMasterClient)
		//{
		GameObject particleCamObj = GameObject.Find("CameraParticle");
		particleCamObj.SetActive(false);
		clientScript.ShowGameMenu();
		//Destroy(charRectObj);

		charSlotCount = 0;
		foreach (GameObject slot in allslots) 
		{
			Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();

			if (tmpSlot.slotNumber == currentSlotRPC) {
				tmpSlot.isReady = true;
			}

		}
		foreach (GameObject charSlot in allCharslots) 
		{
			Char_Pick tmp = charSlot.GetComponent<Char_Pick>();
			tmp.charButton.interactable = false;
			//Destroy(charSlot);


		}

		foreach (GameObject slot in allslots) 
		{
			Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();

			if (tmpSlot.slotNumber == currentSlotRPC && tmpSlot.networkOwner == Owner) {
				tmpSlot.isReady = true;
				//networkView.RPC("ReadyRPC", Owner, currentSlotRPC, uLink.Network.player);

				//CHARACTER SPAWN
				if (currentSlotRPC%2==1)
				{
				//transform.parent = null;
					//Application.LoadLevel("MainScene");
					//Debug.Log("test");

					GameObject spawnManager = GameObject.Find("SpawnManager_mn");
					SpawnScript SpawnScr	= spawnManager.GetComponent<SpawnScript>();
					
					for(int i = 0; i < charPickList.Count; i++)
					{
						

						if (charPickList[i].characterName == tmpSlot.charName)
						{
							
							//Debug.Log(charPickList[i].charPrefabDir);
							//Debug.Log(tmpSlot.charName);
							//charPickedDir = charPickList[i].charPrefabDir;
							//charPickedName = tmpSlot.charName;
							//charPickedTeam = "red";
							SpawnScr.SpawnBlueTeamPlayer(Owner, charPickList[i].charPrefabDir, tmpSlot.charName);
							return;
						}
					}
				
				}
				else
				{
					GameObject spawnManager = GameObject.Find("SpawnManager_mn");
					SpawnScript SpawnScr	= spawnManager.GetComponent<SpawnScript>();
					for(int i = 0; i < charPickList.Count; i++)
					{
						if (charPickList[i].characterName == tmpSlot.charName)
						{
							//charPickedDir = charPickList[i].charPrefabDir;
							//harPickedName = tmpSlot.charName;
							//charPickedTeam = "blue";
							SpawnScr.SpawnRedTeamPlayer(Owner, charPickList[i].charPrefabDir, tmpSlot.charName);
							return;
						}
					}
				}


			}
				
			//}
		}
		//if (uLink.Network.isClient)
		//{

		//}
	}


	[PunRPC]
	void SendSlotNumber(string Pname, int currentSlotRPC, PhotonPlayer Owner)
	{
		if (PhotonNetwork.player == Owner)
		currentSlot = currentSlotRPC;
		foreach (GameObject slot in allslots) 
		{
			Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();
			if (tmpSlot.slotNumber == currentSlotRPC) {
				tmpSlot.ChangePortrait(Pname, Owner, tmpSlot.slotEmpty, false);
			
			}
			
		}
	}



	//SEND AN RPC TO THE SERVER WITH THE NEW CHARACTER SELECTION
	public void AddChar(int charNumber)
	{
		if (PhotonNetwork.isMasterClient)
		{
			//networkView.RPC("UpdateCharPick", uLink.RPCMode.Server, PhotonNetwork.playerName, currentSlot, charNumber, uLink.Network.player, false);
			UpdateCharPick(PhotonNetwork.playerName.ToString(), currentSlot, charNumber, PhotonNetwork.player, false);
			//photonView.RPC ("UpdateCharPick", PhotonTargets.MasterClient, PhotonNetwork.playerName.ToString(), currentSlot, charNumber, PhotonNetwork.player, false);
		}
		else
		{
			photonView.RPC ("UpdateCharPick", PhotonTargets.MasterClient, PhotonNetwork.playerName.ToString(), currentSlot, charNumber, PhotonNetwork.player, false);
		}
	}

	[PunRPC]
	void UpdateCharPick(string Pname,int currentSlotRPC, int charNumber, PhotonPlayer Owner, bool sReady)
	{
		if (PhotonNetwork.isMasterClient)
		{
			//networkView.RPC("UpdateCharPick", uLink.RPCMode.Others, Pname,currentSlotRPC, charNumber, Owner, sReady);
			photonView.RPC ("UpdateCharPick", PhotonTargets.Others, Pname, currentSlotRPC, charNumber, Owner, sReady);
		}
		//give the charnumber to the current selection of the owner player
		if (PhotonNetwork.player == Owner)
		currentSelection = charNumber;

		//if the player already pick a character, look for the portrait and send it to the slot
		if (charNumber > 0)
		{
			foreach (GameObject charSlot in allCharslots) 
			{
				Char_Pick tmp = charSlot.GetComponent<Char_Pick>();
				if ((int)tmp.charName == charNumber) {


					foreach (GameObject slot in allslots) 
					{
						Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();
						if (tmpSlot.slotNumber == currentSlotRPC) {

							tmpSlot.charName = tmp.charName;
							tmpSlot.isReady = sReady;
							tmpSlot.ChangePortrait(Pname, Owner, tmp.GetComponent<Image>().sprite, false);
							emptySlot--;
						}						
					}
				}			
			}
		}
		else
		{
			//if the player hasnt pick the character, only set the name
			foreach (GameObject slot in allslots) 
			{
				Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();
				if (tmpSlot.slotNumber == currentSlotRPC) {
					

					tmpSlot.ChangePortrait(Pname, Owner, tmpSlot.slotEmpty, false);
				
				}
				
			}
		}
	}
	//THIS RPC IS RECIEVED WHEN A PLAYER CHANGES THE CURRENT SLOT POSITION
	[PunRPC]
	void UpdateChangeSlot(string Pname,int currentSelectionRPC, int currentSlotRPC, int slotNumber, PhotonPlayer Owner)
	{
		//THIS LOOP SEARCH FOR THE CURRENT SELECTED CHARACTER AND MOVES TO THE NEXT SLOT
		foreach (GameObject charSlot in allCharslots) 
		{
			Char_Pick tmp = charSlot.GetComponent<Char_Pick>();
			// IF THE CHARACTER MATCH THE CURRENT SELECTION
			if ((int)tmp.charName == currentSelectionRPC) {

				foreach (GameObject slot in allslots) 
				{
					Char_Slot tmpSlot = slot.GetComponent<Char_Slot>();

					if (tmpSlot.slotNumber == slotNumber) {
						
						if (PhotonNetwork.isMasterClient)
						{
							if (tmpSlot.isEmpty)
							{
								Debug.Log("RPC SENT TO CLIENTTS");
								photonView.RPC("UpdateChangeSlot", PhotonTargets.Others, Pname, currentSelectionRPC, currentSlotRPC, slotNumber, Owner);
							}
							else
							{
							return;
							}

						}
						tmpSlot.charName = tmp.charName;
						tmpSlot.ChangePortrait(Pname, Owner, tmp.GetComponent<Image>().sprite, false);
						emptySlot--;
					}

					if (tmpSlot.slotNumber == currentSlotRPC) {
						tmpSlot.ChangePortrait(Pname, Owner,tmp.GetComponent<Image>().sprite, true);
						emptySlot--;
					}

					
				}
				if (PhotonNetwork.player == Owner)
				{
				currentSlot = slotNumber;
				}

			}
			
		}
	}

}
