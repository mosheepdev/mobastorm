using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



public class ItemManagement : Photon.MonoBehaviour {
	public List<Items> myItems = new List<Items>();

	public List<GameObject> mySlotItems = new List<GameObject>();
	
	private GameObject gameManager;
	public GameObject itemPanelObj;
	private ItemDataBase itemDataScript;
	private GameEvents gameEventScript;
	private RectTransform itemPanelRect;
	public GameObject itemSlotPrefab;
	private Transform parentRect;
	private float itemSlotSize = 30;
	private int itemSlots;
	private int itemColumns = 7;
	private int itemRows;
	private int itemSlotCount = 1;
	private float itemPanelWidth, itemPanelHeight;
	private float itemPaddingLeft = 15;
	private float itemPaddingTop = 20;

	private PlayerStats playerStScript;

	public AudioClip buyClip;
	public AudioClip sellClip;
	// Use this for initialization
	void Start () {

		gameManager = GameObject.Find("GameManager_mn");
		
		itemDataScript= gameManager.GetComponent<ItemDataBase>();

		gameEventScript = gameManager.GetComponent<GameEvents>();
		playerStScript = GetComponent<PlayerStats>();



		if (photonView.isMine)
		{
		CreateItemStore();
		//ADD ALL MYITEMSLOTS GAMEOBJECTS TO A LIST
		foreach(GameObject slotObj in GameObject.FindGameObjectsWithTag("MyItemSlot")) {
			mySlotItems.Add(slotObj);
			//SEND A REFERENCE OF THIS SCRIPT TO EACH SLOT
			slotObj.GetComponent<ItemMySlot>().itemManagementScript = GetComponent<ItemManagement>();

		}

		}
		//FILL MY CURRENT ITEMS LIST WITH EMPTY DATA
		for (int i= 1; i <=6; i++)
		{
			Items tmpItem = new Items();
			tmpItem.itemType = ItemDataClass.ItemType.Empty;
			myItems.Add(tmpItem);
		}





	}
	
	// Update is called once per frame
	void Update () {


	
	}

	public void BuyItem(Items currentItem) {
		//photonView.RPC("BuyItemServer", PhotonTargets.AllBuffered,  currentItem.itemType);
		BuyItemOwner(currentItem.itemType.ToString());
	}

	public void UseItem(Items currentItem, int slotNumber) {
		//photonView.RPC("UseItemServer", uLink.RPCMode.Server,  currentItem.itemType, slotNumber);
		UseItemOwner(currentItem.itemType.ToString(), slotNumber);
	}

	
	
	[PunRPC]
	public void UseItemOwner(string currentType, int slotNumber) {
		//Debug.Log("item Used");
		for(int i = 0; i < itemDataScript.items.Count; i++)
		{
			if (itemDataScript.items[i].itemType.ToString() == currentType.ToString())
			{

				if (myItems[slotNumber].stacks >1)
				{
					AddRemoveItemStats(myItems[slotNumber].itemType, false);
					myItems[slotNumber].stacks --;
					//playerStScript.UpdateGoldToClients(0);
					photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, currentType, true,  slotNumber, myItems[slotNumber].stacks, "Item Purchased No stackable");

				}
				else
				{
					AddRemoveItemStats(myItems[slotNumber].itemType, false);
					playerStScript.gold += itemDataScript.items[i].cost / 2;
					myItems[slotNumber].name = "";
					myItems[slotNumber].desc = "";
					myItems[slotNumber].stacks = 0;
					myItems[slotNumber].itemType = ItemDataClass.ItemType.Empty;
					//playerStScript.UpdateGoldToClients(0);
					photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, Items.ItemType.Empty.ToString(), true,  slotNumber, myItems[slotNumber].stacks, "Item Purchased No stackable");

				}

			}
		}
				

		}
						


				
			
			
		
		
		

	[PunRPC]
	public void BuyItemOwner(string currentType) {

		for(int i = 0; i < itemDataScript.items.Count; i++)
		{
			if (itemDataScript.items[i].itemType.ToString() == currentType.ToString())
			{
				//If the player has the money available
				if (itemDataScript.items[i].cost <= playerStScript.gold)
				{
					//If the item is not stackable
					if (itemDataScript.items[i].stacks == 0)
					{
						//Search the list to check if there any empty slotItem to place the new item
						for(int x = 0; x < myItems.Count; x++)
						{
							if (myItems[x].itemType == ItemDataClass.ItemType.Empty)
							{
								myItems[x].stacks ++;
								myItems[x].itemType = itemDataScript.items[i].itemType;
								playerStScript.gold -= itemDataScript.items[i].cost;
								photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, currentType, true,  x, myItems[x].stacks, "Item Purchased No stackable");
								AddRemoveItemStats(myItems[x].itemType, true);

								return;
							}
						}
											

					}
					else
						//If the item is stackable, add to a existing item if found
					{
						for(int x = 0; x < myItems.Count; x++)
						{
							//If the slot has not reached the maximun stacks availables
							if (myItems[x].itemType == itemDataScript.items[i].itemType && myItems[x].stacks < itemDataScript.items[i].stacks)
							{
								myItems[x].stacks ++;
								myItems[x].itemType = itemDataScript.items[i].itemType;
								playerStScript.gold -= itemDataScript.items[i].cost;
								photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, currentType, true,  x, myItems[x].stacks, "Item Purchased Stackable");
								AddRemoveItemStats(myItems[x].itemType, true);

								return;
							}
						}
						//If there is no stacks available, add the item to a new stack
						for(int x = 0; x < myItems.Count; x++)
						{
							if (myItems[x].itemType == ItemDataClass.ItemType.Empty)
							{
								myItems[x].stacks ++;
								myItems[x].itemType = itemDataScript.items[i].itemType;
								playerStScript.gold -= itemDataScript.items[i].cost;
								photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, currentType, true,  x, myItems[x].stacks, "Item Purchased Stackable New Stack");
								AddRemoveItemStats(myItems[x].itemType, true);
								Debug.Log("Item Added to a new stack");
								return;
							}
						}
						photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, currentType,false,0,  0,  "No slots available");

					}
				}
				else
				{
					photonView.RPC("BuyItemResponse", PhotonTargets.AllBuffered, currentType, false, 0,  0, "Dont have enought money");
				}
			}
			
		}


	}

	private void AddRemoveItemStats(Items.ItemType item, bool adding)
	{
		Debug.Log(item);
		Debug.Log(adding);
		float addSusValue = 1;
		if (!adding)
		{
			addSusValue = -1;
		}

		switch (item) {
		case ItemDataClass.ItemType.Empty:

			break;
		case ItemDataClass.ItemType.HealthPotMini:
			if (!adding)
			{
				playerStScript.myHealth += 80;
				//playerStScript.UpdateHealthClients();
			}

			break;
		case ItemDataClass.ItemType.HealthPotBig:
			if (!adding)
			{
			playerStScript.myHealth += 150;
			//playerStScript.UpdateHealthClients();
			}
			break;
		case ItemDataClass.ItemType.ManaPotMini:
			if (!adding)
			{
				playerStScript.mana += 100;
				//playerStScript.UpdateManaToClients();
			}
			
			break;
		case ItemDataClass.ItemType.ManaPotBig:
			if (!adding)
			{
				playerStScript.mana += 200;
				//playerStScript.UpdateManaToClients();
			}
			break;
		case ItemDataClass.ItemType.SwordMini:
			playerStScript._adValueAdd += 20 * addSusValue;
			break;
		case ItemDataClass.ItemType.SwordBig:
			playerStScript._adValueAdd += 80 * addSusValue;
			playerStScript.attackRedAdd += 20 * addSusValue;

			break;
		case ItemDataClass.ItemType.ShieldMini:
			playerStScript.adResAdd += 15 * addSusValue;

			break;
		case ItemDataClass.ItemType.ShieldBig:
			playerStScript.adResAdd += 35 * addSusValue;
			playerStScript.maxHealth += 150 * addSusValue;
			//playerStScript.UpdateHealthClients();
			break;
		case ItemDataClass.ItemType.MagicMini:
			playerStScript._apValueAdd += 40 * addSusValue;

			break;
		case ItemDataClass.ItemType.MagicBig:
			playerStScript._apValueAdd += 95 * addSusValue;
			playerStScript.baseMana += 250 * addSusValue;
			//playerStScript.UpdateManaToClients();
			break;
		case ItemDataClass.ItemType.CloakMini:
			playerStScript.apResAdd += 10 * addSusValue;
			break;
		case ItemDataClass.ItemType.CloakBig:
			playerStScript.apResAdd += 20 * addSusValue;
			playerStScript.baseMana += 380 * addSusValue;
			//playerStScript.UpdateManaToClients();
			break;
		case ItemDataClass.ItemType.BootsMedium:
			playerStScript.speedAdd += 10 * addSusValue;

			break;
		case ItemDataClass.ItemType.BootsFast:
			playerStScript.speedAdd += 25 * addSusValue;
			break;

		}

		//playerStScript.UpdateAdds();
	

	}

	[PunRPC]
	public void BuyItemResponse(string currentType, bool value, int slotNumber, int stacks, string msj) {
		//IF THE PURCHASE WAS APROVED BY THE SERVER
		//ADD THE ITEM TO myItems LIST
		Debug.Log(currentType + msj);
		if (value)
		{
			//GetComponent<AudioSource>().PlayOneShot(buyClip);
		


			for(int i = 0; i < itemDataScript.items.Count; i++)
			{
				if (itemDataScript.items[i].itemType.ToString() == currentType)
				{
					myItems[slotNumber].name = itemDataScript.items[i].name;
					myItems[slotNumber].itemType = itemDataScript.items[i].itemType;
					myItems[slotNumber].desc = itemDataScript.items[i].desc;
					myItems[slotNumber].stacks = stacks;

					if (photonView.isMine)
					{
						//PLAY BUY SOUND
						GetComponent<AudioSource>().PlayOneShot(buyClip);
						//UPDATES THE UI WITH THE NEW ITEM PURCHASE ONLY IN THE OWNER NETWORKVIEW
						foreach (GameObject slotItem in mySlotItems) 
						{
							ItemMySlot itemMyslotScript = slotItem.GetComponent<ItemMySlot>();
							if (itemMyslotScript.slotNumber == slotNumber)
							{
								itemMyslotScript.item.name = itemDataScript.items[i].name;
								itemMyslotScript.item.desc = itemDataScript.items[i].desc;
								itemMyslotScript.item.itemType = itemDataScript.items[i].itemType;
								itemMyslotScript.item.itemSprite = itemDataScript.items[i].itemSprite;
								itemMyslotScript.item.stacks = stacks;

								return;
							}
						}
					}
				}

			}
		}

	}

	public void SellItem(int currentSlot) {
		
	}

	//THIS METHOD WILL CREATE ALL THE ITEMS AVAILABLE TO SHOW ON THE INGAME STORE
	public void CreateItemStore()
	{

		//CHECK HOW MANY ITEMS WE HAVE AVAILABLE ON THE ITEMDATABASE SCRIPT
		for(int i = 0; i < itemDataScript.items.Count; i++)
		{
			itemSlots++;
		
		}

		
		if (itemSlots> itemColumns)
		{

			itemRows = itemSlots / itemColumns;
		
			if (itemRows * itemColumns  < itemSlots)
			{
				itemRows ++;
			}

		}
		else
		{
			itemRows = 1;
		}
		
		itemPanelWidth = (itemColumns * (itemSlotSize + itemPaddingLeft)) + itemPaddingLeft;
		
		itemPanelHeight = itemRows * (itemSlotSize + itemPaddingTop) + itemPaddingTop;
		
		
		GameObject itemRectObj = Instantiate(itemPanelObj) as GameObject;
		itemPanelRect = itemRectObj.GetComponent<RectTransform>();

		parentRect = GameObject.Find("ItemShopMenu").transform;

		itemPanelRect.transform.SetParent(parentRect);
		gameEventScript.itemCanvas  = parentRect.GetComponent<CanvasGroup>();
		gameEventScript.tooltipObj.transform.SetParent(parentRect);
		itemPanelRect.localPosition = new Vector3(640 - itemPanelWidth/2, -160, 0);
		itemPanelRect.localPosition = new Vector3(0, 0, 0);
		itemPanelRect.localScale = Vector3.one;
		itemPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemPanelWidth);
		
		itemPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemPanelHeight);
		
		for (int y = 0; y < itemRows; y++)
		{
			for (int x = 0; x < itemColumns; x++)
			{
				

				GameObject newItem = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				RectTransform slotRect = newItem.GetComponent<RectTransform>();
					
				newItem.transform.SetParent(parentRect);	

				slotRect.localPosition = itemPanelRect.localPosition + new Vector3(itemPaddingLeft + ((itemSlotSize +itemPaddingLeft)  * x), -itemPaddingTop * (y+1) - (itemSlotSize * y));
				slotRect.localScale = Vector3.one;

				newItem.transform.SetParent(itemRectObj.transform);

				if (itemSlotCount < itemDataScript.items.Count)
				{
					ItemStoreSlot itemSlotScript = newItem.GetComponent<ItemStoreSlot>();
					itemSlotScript.item = itemDataScript.items[itemSlotCount];
					//Send this script reference  to each item on the store
					itemSlotScript.itemManagementScript = GetComponent<ItemManagement>();
					newItem.GetComponent<Image>().sprite = itemDataScript.items[itemSlotCount].itemSprite;
		

				}


				itemSlotCount++;
				if (itemSlotCount == itemSlots)
					return;
				
				
				
			}
			
			
			
		}
		
		
	}
}
