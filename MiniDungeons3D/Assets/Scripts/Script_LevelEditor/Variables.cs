using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//this class stores variables which are exchanged between scenes. 
public class Variables : MonoBehaviour{

	public static bool isPlayTest = false;
	public static bool playTested = false;
	public static string levelName = null;
	public static Variables instance;
	public bool isPersistant;

	public static Color oldColor;
	public static SpriteRenderer highlighted;


	public static string _healthLeft, _monstersKilled, _minitaursKnockedOut, _treasuresCollected, _potionsDrunk, _stepsTaken,runner,monsterKiller,treasureCollertor,complitionist;

	public virtual void Awake() {
		if(isPersistant) {
			if(!instance) {
				instance = this;
			}
			else {
				DestroyObject(gameObject);
			}
			DontDestroyOnLoad(gameObject);
		}
		else {
			instance = this;
		}
	}

	public static void ResetVariables(){
		_healthLeft = "";
		_monstersKilled = "";
		_minitaursKnockedOut = "";
		_treasuresCollected = "";
		_potionsDrunk = "";
		_stepsTaken = "";
		runner  = null;
		monsterKiller = null;
		treasureCollertor = null;
		complitionist = null;

	}
}