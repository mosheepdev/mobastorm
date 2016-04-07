using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(PlayerStats))]
public class MyScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var pScript = target as PlayerStats;
		pScript.isPlayer = GUILayout.Toggle(pScript.isPlayer, "Is a player?");
		pScript.isCreator = GUILayout.Toggle(pScript.isCreator, "Is creator or server?");		
		if (pScript.isCreator)
		{
			pScript.myHealth = EditorGUILayout.FloatField("MyHealth", pScript.myHealth);
			pScript.maxHealth = EditorGUILayout.FloatField("MaxHealth", pScript.maxHealth);
			pScript.gold = EditorGUILayout.FloatField("Gold", pScript.gold);
			pScript.healthRegenerate = EditorGUILayout.FloatField("Health Reg. Rate", pScript.healthRegenerate);
			pScript.baseMana = EditorGUILayout.FloatField("BaseMana", pScript.baseMana);
			pScript.rechargeRateMana = EditorGUILayout.FloatField("Mana Red. Rate", pScript.rechargeRateMana);
			pScript.expToGive = EditorGUILayout.IntField("Exp to give", pScript.expToGive);
			pScript.goldToGive = EditorGUILayout.IntField("Gold to give", pScript.goldToGive);
			pScript.adRes = EditorGUILayout.FloatField("Armor Res", pScript.adRes);
			pScript.apRes = EditorGUILayout.FloatField("Spell Res", pScript.apRes);
			pScript.speed = EditorGUILayout.FloatField("Move Speed", pScript.speed);
			pScript.speedAdd = EditorGUILayout.FloatField("Move Speed ADd", pScript.speedAdd);

			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("attackersList"), true);
			serializedObject.ApplyModifiedProperties();

			pScript.respawns = GUILayout.Toggle(pScript.respawns, "This respawns?");

			if(pScript.respawns)
			{
				pScript.respawnTime = EditorGUILayout.IntField("Respawn Time", pScript.respawnTime);

			}

			if(pScript.isPlayer)
			{
				pScript.abilityLocked = GUILayout.Toggle(pScript.abilityLocked, "Ability Locked?");
				pScript.charLocked = GUILayout.Toggle(pScript.charLocked, "charLocked Locked?");
				serializedObject.Update();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("stats"), true);
				serializedObject.ApplyModifiedProperties();
			}
			else
			{
				pScript._bAdValue = EditorGUILayout.FloatField("Attack Damage", pScript._bAdValue);
				pScript._bApValue = EditorGUILayout.FloatField("Spell Damage", pScript._bApValue);

			}

		}
		else
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("stats.charPortrait"), true);
			serializedObject.ApplyModifiedProperties();
		}

	}
}