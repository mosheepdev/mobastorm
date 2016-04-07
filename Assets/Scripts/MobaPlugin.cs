using UnityEngine;
using System.Collections;

namespace MobaPlugin
{
	public class MobaPlugin : Photon.MonoBehaviour {

		
		/// <summary>
		/// Creates a room with given name but fails if this room(name) is existing already. Creates random name for roomName null.
		/// </summary>
		/*
		public static void SetPlayerTrigger(GameObject obj ,string t, PhotonView pView)
		{
			if (pView.isMine)
			{
				pView.RPC("SetPlayerTeam", PhotonTargets.OthersBuffered, t);
			}
			if (t == "blue")
			{
				obj.layer = 9;
				obj.tag = "BluePlayerTag";
				GameObject trigger = obj.transform.FindChild("Trigger").gameObject;
				trigger.tag = "BluePlayerTriggerTag";
				trigger.layer = 16;
					//return obj;
			}
			else
			{
				obj.layer = 10;
				obj.tag = "RedPlayerTag";
				GameObject trigger = obj.transform.FindChild("Trigger").gameObject;
				trigger.tag = "RedPlayerTriggerTag";
				trigger.layer = 17;
					//return obj;
				}



		}
*/
			//public void SetPlayerTrigger(GameObject obj, string team)
			//{

			//}
	}
}
