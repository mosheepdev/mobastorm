using UnityEngine;
using System.Collections;

public class MouseOver : MonoBehaviour {
	public GameObject mesh;
	private Renderer rend;
	private GameObject gameManager;
	private PlayerDatabase dataScript;
	// Use this for initialization
	void Start () {
		rend = mesh.GetComponent<Renderer>();
		gameManager = GameObject.Find("GameManager_mn");
		dataScript = gameManager.GetComponent<PlayerDatabase>();
	}
	
	// Update is called once per frame
	void Update () {
		

	}

	void OnMouseOver () {
		if (dataScript.playerTeam == "blue")
		{
			if (gameObject.tag == "BlueTeamTriggerTag")
				rend.material.SetColor("_OutColor", Color.cyan);
			if (gameObject.tag == "RedTeamTriggerTag")
				rend.material.SetColor("_OutColor", Color.red);
			if (gameObject.tag == "RedPlayerTriggerTag")
				rend.material.SetColor("_OutColor", Color.red);

		}

		if (dataScript.playerTeam == "red")
		{
			if (gameObject.tag == "RedTeamTriggerTag")
				rend.material.SetColor("_OutColor", Color.cyan);

			if (gameObject.tag == "BlueTeamTriggerTag")
				rend.material.SetColor("_OutColor", Color.red);
			if (gameObject.tag == "BluePlayerTriggerTag")
				rend.material.SetColor("_OutColor", Color.red);

		}

		if (gameObject.tag == "CreepTriggerTag")
			rend.material.SetColor("_OutColor", Color.red);


	}

	void OnMouseExit () {
		
		rend.material.SetColor("_OutColor", Color.black);
	}
}
