using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticLevelComplexityCheck : MonoBehaviour {
	bool _didRun;

	// Use this for initialization
	void Start () {
		_didRun = false;
		Debug.Log("Press R to run");
	}

	
	void Update(){
		if(!_didRun && Input.GetKeyUp(KeyCode.R)){
			_didRun = true;
			CheckStaticComplexity();
		}
	}

	void CheckStaticComplexity(){
		string mapPath = "AsciiMaps/Map666";
		string levelString = Resources.Load<TextAsset>(mapPath).text;
		var level = new SimLevel(levelString);

		List <SimPoint> allPoints = new List<SimPoint>();
		List<SimHeroAction> allActionsAllFields = new List<SimHeroAction>();
		List<int> validActionCounts = new List<int>();
		List<int> validMoveCounts = new List<int>();
		List<int> validJavelinCounts = new List<int>();

		for(int x = 0; x < level.BaseMap.GetLength(0); x++){
			for(int y = 0; y < level.BaseMap.GetLength(1); y++){
				if(level.BaseMap[x,y].TileType == TileTypes.empty){
					allPoints.Add(new SimPoint(x,y));
					var clone = level.Clone();
					clone.SimHero.Point = new SimPoint(x,y);
					var possibleActions = clone.GetPossibleHeroActions();
					foreach(SimHeroAction action in possibleActions){
						validActionCounts.Add(possibleActions.Count);
						int moveCount = 0;
						int javelinCount = 0;
						if(action.ActionType == HeroActionTypes.Move){
							moveCount++;
						}
						if(action.ActionType == HeroActionTypes.JavelinThrow){
							javelinCount++;
						}
						validMoveCounts.Add(moveCount);
						validJavelinCounts.Add(javelinCount);
					}
					allActionsAllFields.AddRange(possibleActions);
				}
			}
		}

		int javelinThrows = 0;
		int moves = 0;
		foreach(SimHeroAction action in allActionsAllFields){
			if(action.ActionType == HeroActionTypes.Move){
				moves++;
			}
			if(action.ActionType == HeroActionTypes.JavelinThrow){
				javelinThrows++;
			}
		}

		Debug.Log("Number of valid points: " + allPoints.Count);
		Debug.Log("Number of actions: " + allActionsAllFields.Count);
		Debug.Log("Number of moves: " + moves);
		Debug.Log("Number of throws: " + javelinThrows);

		int validActions = 0;
		int validMoves = 0;
		int validJavelins = 0;

		for(int i = 0; i < validActionCounts.Count; i++){
			validActions += validActionCounts[i];
		}

		for(int i = 0; i < validMoveCounts.Count; i++){
			validMoves += validMoveCounts[i];
		}
		for(int i = 0; i < validJavelinCounts.Count; i++){
			validJavelins += validJavelinCounts[i];
		}

		//Debug.Log("Average number of actions per field" + ((float)validActions/(float)validActionCounts.Count));
		//Debug.Log("Average number of moves per field" + ((float)validMoves/(float)validMoveCounts.Count));
		//Debug.Log("Average number of throws per field" + ((float)validJavelins/(float)validJavelinCounts.Count));
	}
}
