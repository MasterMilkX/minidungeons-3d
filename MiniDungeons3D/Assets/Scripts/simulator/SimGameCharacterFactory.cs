using UnityEngine;

public class SimGameCharacterFactory : MonoBehaviour {

	private static SimGameCharacterFactory _instance;

	public GameObject prefabEntrance;
	public GameObject prefabExit;
	public GameObject prefabMeleeMonster;
	public GameObject prefabPotion;
	public GameObject prefabRangedMonster;
	public GameObject prefabRangedMonsterAttack;
	public GameObject prefabOgreMonster;
	public GameObject prefabBlobMonster;
	public GameObject prefabMinitaur;
	public GameObject prefabPortal;
	public GameObject prefabTrap;
	public GameObject prefabTreasure;
	public GameObject prefabHero;
	public GameObject prefabJavelin;
	public GameObject prefabHealthBadge;
	public GameObject prefabBloodDecoration;

	public static SimGameCharacterFactory instance{
		get{
			if(_instance == null){
				_instance = GameObject.FindObjectOfType<SimGameCharacterFactory>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
	
	void Awake(){
		if(_instance == null){
			_instance = this;
			DontDestroyOnLoad(this);
		} else {
			if(this != _instance){Destroy(this.gameObject);}
		}
	}

	public GameObject SpawnGameCharacter(GameCharacterTypes gameCharacterType, Vector3 spawnPosition){

		GameObject spawn = null;

		switch (gameCharacterType) {
		case GameCharacterTypes.Entrance:
			spawn = (GameObject)Instantiate(prefabEntrance,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.Exit:
			spawn = (GameObject)Instantiate(prefabExit,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.MeleeMonster:
			spawn = (GameObject)Instantiate(prefabMeleeMonster,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.Potion:
			spawn = (GameObject)Instantiate(prefabPotion,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.RangedMonster:
			spawn = (GameObject)Instantiate(prefabRangedMonster,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.RangedMonsterAttack:
			spawn = (GameObject)Instantiate(prefabRangedMonsterAttack,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.OgreMonster:
			spawn = (GameObject)Instantiate(prefabOgreMonster, spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.BlobMonster:
			spawn = (GameObject)Instantiate(prefabBlobMonster, spawnPosition, Quaternion.identity);
			break;
		case GameCharacterTypes.MinitaurMonster:
			spawn = (GameObject)Instantiate(prefabMinitaur, spawnPosition, Quaternion.identity);
			break;
		case GameCharacterTypes.Portal:
			spawn = (GameObject)Instantiate(prefabPortal,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.Trap:
			spawn = (GameObject)Instantiate(prefabTrap,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.Treasure:
			spawn = (GameObject)Instantiate(prefabTreasure,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.Hero:
			spawn = (GameObject)Instantiate(prefabHero,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.Javelin:
			spawn = (GameObject)Instantiate(prefabJavelin,spawnPosition,Quaternion.identity);
			break;
		case GameCharacterTypes.BloodDecoration:
			spawn = (GameObject)Instantiate(prefabBloodDecoration, spawnPosition,Quaternion.identity);
			break;
		}
		
		return spawn;
	}
}
