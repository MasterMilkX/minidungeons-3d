using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState  {

	GameCell[][] gameBoard;
	EntranceEntity entrance;
	ExitEntity exit;
	List<GoblinEntity> goblins = new List<GoblinEntity>();
	List<RangedGoblinEntity> rangedGoblins = new List<RangedGoblinEntity>();
	List<Entity> potions = new List<Entity>();
	List<Entity> blobs = new List<Entity>();
	List<TreasureEntity> treasures = new List<TreasureEntity>();
	List<OgreEntity> ogres = new List<OgreEntity>();



	public  GameState(GameCell[][] GameBoard, EntranceEntity Entrance, ExitEntity Exit, List<GoblinEntity> Goblins, List<RangedGoblinEntity> RangedGoblins,
		List<Entity> Potions, List<Entity> Blobs, List<TreasureEntity> Treasures, List<OgreEntity> Ogres){

		gameBoard = GameBoard;
		entrance = Entrance;
		exit = Exit;
		goblins = Goblins;
		rangedGoblins = RangedGoblins;
		potions = Potions;
		blobs = Blobs;
		treasures = Treasures;
		ogres = Ogres;
	}

	public GameState(){

	}

	public  void DeepCopy(GameState gs){
		GameState clone = new GameState();
		clone.gameBoard = gs.gameBoard;
		clone.entrance = gs.entrance;
		clone.exit = gs.exit;
		clone.goblins = gs.goblins;
		clone.rangedGoblins = gs.rangedGoblins;
		clone.potions = gs.potions;
		clone.blobs = gs.blobs;
		clone.treasures = gs.treasures;
		clone.ogres = gs.ogres;

		clone.exit = null;
		clone.blobs = null;

		Debug.Log ("..............clone value..............");
		Debug.Log (clone.gameBoard);
		Debug.Log (clone.entrance);
		Debug.Log (clone.exit);
		Debug.Log (clone.goblins);
		Debug.Log (clone.rangedGoblins);
		Debug.Log (clone.potions);
		Debug.Log (clone.blobs);
		Debug.Log (clone.treasures);
		Debug.Log (clone.ogres);

	}
}
