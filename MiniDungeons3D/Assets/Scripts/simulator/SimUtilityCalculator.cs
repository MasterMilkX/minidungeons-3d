using System;
using System.Collections;
using System.Collections.Generic;
using TreeLanguageEvolute;

public enum UtilityFunctions{
	Exit,
	Runner,
	Loiterer,
	Completionist,
	MonsterKiller,
	TreasureCollector
}

public class SimUtilityCalculator{

//	public static Dictionary<SimPoint,double> exitPathDatabase2;
	public Dictionary<SimPoint, double> exitPathDatabase;

//	private static BaseProgram function1;
	private BaseProgram function;

	private Dictionary<SimPoint, double> InitExitPathDatabase(SimLevel level) {
		//Receive level
		//Initialize dictionary
		Dictionary<SimPoint,double> exitPathDB = new Dictionary<SimPoint,double>();
		int maxLength = int.MinValue;
		int minLength = int.MaxValue;
		//For each point in the 2D array, calculate path length to exit
		var stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start();

		foreach(SimMapNode tile in level.BaseMap){
			if(tile.TileType == TileTypes.empty){
                try
                {
					int length = level.AStar(tile.Point, level.SimExit.Point).Length;
					if (length > maxLength) maxLength = length;
					if (length < minLength) minLength = length;
					exitPathDB.Add(tile.Point, (double)length);
				}
                catch (Exception ex)
                {
                }
				
			}
		}

		Dictionary<SimPoint,double> tempDataBase = new Dictionary<SimPoint,double>();
		foreach(KeyValuePair<SimPoint,double> entry in exitPathDB){
			double length = entry.Value;
			double normalizedLength = 1d-((double)length - (double)minLength) / ((double)maxLength - (double)minLength);
			tempDataBase.Add(entry.Key, normalizedLength);
			//Console.WriteLine(length + " " + normalizedLength);
		}

		exitPathDB = tempDataBase;
		stopwatch.Stop();
		float timeTaken = (float)stopwatch.ElapsedMilliseconds;
		//Console.WriteLine("SimUtilityCalculator built exit path database in " + timeTaken + " ms.");
		return exitPathDB;
	}


	public Func<SimLevel, double> GetUtilityFunction(UtilityFunctions utilityFunction) {

		Func<SimLevel,double> result = null;
		switch(utilityFunction){
		case UtilityFunctions.Exit:
			result = ExitUtility;
			break;
		case UtilityFunctions.Runner:
			result = RunnerUtility;
			break;
		case UtilityFunctions.Loiterer:
			result = LoitererUtility;
			break;
		case UtilityFunctions.Completionist:
			result = CompletionistUtility;
			break;
		case UtilityFunctions.MonsterKiller:
			result = MonsterKillerUtility;
			break;
		case UtilityFunctions.TreasureCollector:
			result = TreasureCollectorUtility;
			break;
		}
		return result;
	}


	public NormalizedUtilityValues NormalizeUtilities(SimLevel level) {
		if (exitPathDatabase == null) {
			exitPathDatabase = InitExitPathDatabase (level);
		}

		var result = new NormalizedUtilityValues();

		//Trying out penalties for backtracking
		if(level.SimHero.PriorPoint.Equals(level.SimHero.Point)){result._didBacktrack = true;}

		exitPathDatabase.TryGetValue (level.SimHero.Point, out result._distanceToExit);

		SimTreasure nearestTreasure = null;
		double nearestTreasureDistance = double.MaxValue;
		for (int t = 0; t < level.Treasures.Count; t++) {
			if (level.Treasures [t].Alive) {
				double dist = level.Treasures [t].Point.EuclidianDistance (level.SimHero.Point);
				if (dist < nearestTreasureDistance) {
					nearestTreasureDistance = dist;
					nearestTreasure = level.Treasures [t];
				}
			}
		}
		if (nearestTreasure != null) {
			result._distanceToTreasure = (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length;
			if (result._distanceToTreasure == 0) {
				Console.Write ("Wait a minute");
			}
		}

		result._healthLeft = (double)level.SimHero.Health/10d;
		result._monsterKills = 0d;
		for(int i = 0; i < level.Characters.Length; i++){
			if(level.Characters[i].CharacterType == GameCharacterTypes.MeleeMonster && !level.Characters[i].Alive){
				result._monsterKills++;
				continue;
			}
			if(level.Characters[i].CharacterType == GameCharacterTypes.RangedMonster && !level.Characters[i].Alive){
				result._monsterKills++;
				continue;
			}
			if(level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster && !level.Characters[i].Alive){
				result._monsterKills++;
				continue;
			}
			if(level.Characters[i].CharacterType == GameCharacterTypes.OgreMonster && !level.Characters[i].Alive){
				result._monsterKills++;
				continue;
			}
		}
		// normalize monsters
		// make interactables normalized
		int minitaurCount = 0;
		foreach(SimMonsterCharacter monst in level.Monsters) {
			if(monst is SimMinitaurMonster) {
				minitaurCount++;
			}
		}
		result._treasuresOpened = (double)level.SimHero.TreasureCollected;
		result._potionsDrunk = (double)level.SimHero.PotionsCollected;


		result._interactablesRatio = (result._monsterKills + result._potionsDrunk + result._treasuresOpened) / (((double)level.Monsters.Count - minitaurCount) + (double)level.Potions.Count + (double)level.Treasures.Count);
		if (level.Monsters.Count > 0 || minitaurCount > 0) { 
			result._monsterKills = result._monsterKills / (level.Monsters.Count - minitaurCount);
		}
		if ((double)level.Treasures.Count > 0)
        {
			result._treasuresOpened = result._treasuresOpened / (double)level.Treasures.Count;
		}
		if ((double)level.Potions.Count > 0)
		{ 
			result._potionsDrunk = result._potionsDrunk / (double)level.Potions.Count;
		}
		result._stepsTaken = (double)level.SimHero.StepsTaken;
		result._javelinThrows = (double)level.SimHero.JavelinsThrown;
		result._exitReached = level.SimLevelState == SimLevelState.Won ? 1d : 0d;
		return result;
	}


	public double ExitUtility(SimLevel level){
		if (level.SimLevelState == SimLevelState.Won)
		{
			return 1d;
		}
		else {
			return 0d;
		}
	}

	public double RunnerUtility(SimLevel level){
		NormalizedUtilityValues utilityValues = NormalizeUtilities(level);
		double exitUtil = 0.0;
		exitUtil = utilityValues._distanceToExit - utilityValues._stepsTaken / 100;
		// included in R1
		if (utilityValues._healthLeft <= 0) {
			exitUtil -= 5;
		}
		return exitUtil;
	}
	// TODO removing survivalist
	public double LoitererUtility(SimLevel level){
		NormalizedUtilityValues utilityValues = NormalizeUtilities(level);
		double sScore = utilityValues._stepsTaken;
		// included in L1
		if (utilityValues._healthLeft <= 0) {
			sScore -= 5;
		}
		return sScore;
	}
	public double CompletionistUtility(SimLevel level) {
		NormalizedUtilityValues utilityValues = NormalizeUtilities(level);
		//double sScore = 0.3 * utilityValues._distanceToExit + 0.7 * utilityValues._interactablesRatio;
		// included in C1
		double sScore = 0.3 * utilityValues._distanceToExit + 0.7 * (utilityValues._monsterKills + utilityValues._treasuresOpened + utilityValues._potionsDrunk) / 3;
		if (utilityValues._healthLeft <= 0) {
			sScore -= 5;
		}
		return sScore;
	}
	public double MonsterKillerUtility(SimLevel level){
		NormalizedUtilityValues utilityValues = NormalizeUtilities(level);
		double mkScore = 0.0;
        mkScore = utilityValues._monsterKills * 0.7 + utilityValues._distanceToExit * 0.3;
        //mkScore = utilityValues._distanceToExit - utilityValues._stepsTaken / 100;
		// included in MK1
		if (utilityValues._healthLeft <= 0) {
			mkScore -= 5;
		}		
		return mkScore;
	}
	public double TreasureCollectorUtility(SimLevel level){
		NormalizedUtilityValues utilityValues = NormalizeUtilities(level);
		double tScore = 0.0;

		tScore = utilityValues._treasuresOpened * 0.7 + utilityValues._distanceToExit * 0.3;
		// included in TC1
		if (utilityValues._healthLeft <= 0) {
			tScore -= 5;
		}
		return tScore;
	}
//	public static Func<SimLevel,double> GetUtilityFunction1(UtilityFunctions utilityFunction){
//
//		Func<SimLevel,double> result = null;
//		switch(utilityFunction){
//		case UtilityFunctions.Exit:
//			result = SimUtilityCalculator.ExitUtility;
//			break;
//		case UtilityFunctions.Runner:
//			result = SimUtilityCalculator.RunnerUtility;
//			break;
//		case UtilityFunctions.Survivalist:
//			result = SimUtilityCalculator.SurvivalistUtility;
//			break;
//		case UtilityFunctions.MonsterKiller:
//			result = SimUtilityCalculator.MonsterKillerUtility;
//			break;
//		case UtilityFunctions.TreasureCollector:
//			result = SimUtilityCalculator.TreasureCollectorUtility;
//			break;
//		}
//		return result;
//	}

//	public static NormalizedUtilityValues NormalizeUtilities1(SimLevel level){
//		if (exitPathDatabase == null) {
//			exitPathDatabase = InitExitPathDatabase (level);
//		}
//		//		exitPathDatabase = InitExitPathDatabase (level);
//		var result = new NormalizedUtilityValues();
//
//		//Trying out penalties for backtracking
//		if(level.SimHero.PriorPoint.Equals(level.SimHero.Point)){result._didBacktrack = true;}
//
//		/*int distX = level.SimExit.Point.X - level.SimHero.Point.X;
//		int distY = level.SimExit.Point.Y - level.SimHero.Point.Y;
//		double hypotenuse = (double)Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY,2));
//		result._distanceToExit = 1 - (hypotenuse / 25f);*/
//		//result._distanceToExit = (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length;
//		exitPathDatabase.TryGetValue (level.SimHero.Point, out result._distanceToExit);
//
//		SimTreasure nearestTreasure = null;
//		double nearestTreasureDistance = double.MaxValue;
//		for (int t = 0; t < level.Treasures.Count; t++) {
//			if (level.Treasures [t].Alive) {
//				double dist = level.Treasures [t].Point.EuclidianDistance (level.SimHero.Point);
//				if (dist < nearestTreasureDistance) {
//					nearestTreasureDistance = dist;
//					nearestTreasure = level.Treasures [t];
//				}
//			}
//		}
//		if (nearestTreasure != null) {
//			result._distanceToTreasure = (double)level.AStar(level.SimHero.Point, level.SimExit.Point).Length;
//			if (result._distanceToTreasure == 0) {
//				Console.Write ("Wait a minute");
//			}
//		}
//
//		result._healthLeft = (double)level.SimHero.Health/10d;
//		//if(result._healthLeft>0)Console.WriteLine (result._healthLeft);
//		result._monsterKills = 0d;
//		for(int i = 0; i < level.Characters.Length; i++){
//			if(level.Characters[i].CharacterType == GameCharacterTypes.MeleeMonster && !level.Characters[i].Alive){
//				result._monsterKills++;
//				continue;
//			}
//			if(level.Characters[i].CharacterType == GameCharacterTypes.RangedMonster && !level.Characters[i].Alive){
//				result._monsterKills++;
//				continue;
//			}
//			if(level.Characters[i].CharacterType == GameCharacterTypes.BlobMonster && !level.Characters[i].Alive){
//				result._monsterKills++;
//				continue;
//			}
//			if(level.Characters[i].CharacterType == GameCharacterTypes.OgreMonster && !level.Characters[i].Alive){
//				result._monsterKills++;
//				continue;
//			}
//		}
//		result._treasuresOpened = (double)level.SimHero.TreasureCollected/(double)level.Treasures.Count;
//		result._stepsTaken = (double)level.SimHero.StepsTaken;
//		result._javelinThrows = (double)level.SimHero.JavelinsThrown;
//		result._potionsDrunk = (double)level.SimHero.PotionsCollected/(double)level.Potions.Count;
//		result._exitReached = level.SimLevelState == SimLevelState.Won ? 1d : 0d;
//		return result;
//	}
//	private static Dictionary<SimPoint,double> InitExitPathDatabase1(SimLevel level){
//		//Receive level
//		//Initialize dictionary
//		Dictionary<SimPoint,double> exitPathDB = new Dictionary<SimPoint,double>();
//		int maxLength = int.MinValue;
//		int minLength = int.MaxValue;
//		//For each point in the 2D array, calculate path length to exit
//		var stopwatch = new System.Diagnostics.Stopwatch();
//		stopwatch.Start();
//
//		foreach(SimMapNode tile in level.BaseMap){
//			if(tile.TileType == TileTypes.empty){
//				int length = level.AStar(tile.Point,level.SimExit.Point).Length;
//				if(length > maxLength) maxLength = length;
//				if(length < minLength) minLength = length;
//				exitPathDB.Add(tile.Point,(double)length);
//			}
//		}
//		//		for (int i = 0; i < level.BaseMap.Length; i++) {
//		//			for(int j = 0; j < level.BaseMap[i].
//		//			SimMapNode tile = level.BaseMap [i];
//		//			if(tile.TileType == TileTypes.empty){
//		//				int length = level.AStar(tile.Point,level.SimExit.Point).Length;
//		//				if(length > maxLength) maxLength = length;
//		//				if(length < minLength) minLength = length;
//		//				exitPathDatabase.Add(tile.Point,(float)length);
//		//			}
//		//		}
//		//Normalize to range 0-1
//		//Console.WriteLine(maxLength + " " + minLength);
//		Dictionary<SimPoint,double> tempDataBase = new Dictionary<SimPoint,double>();
//		foreach(KeyValuePair<SimPoint,double> entry in exitPathDB){
//			double length = entry.Value;
//			double normalizedLength = 1d-((double)length - (double)minLength) / ((double)maxLength - (double)minLength);
//			tempDataBase.Add(entry.Key, normalizedLength);
//			//Console.WriteLine(length + " " + normalizedLength);
//		}
//		//		for (int i = 0; i < exitPathDatabase.Count; i++) {
//		//			KeyValuePair<SimPoint,float> entry = exitPathDatabase [i];
//		//			float length = entry.Value;
//		//			float normalizedLength = 1f-((float)length - (float)minLength) / ((float)maxLength - (float)minLength);
//		//			tempDataBase.Add(entry.Key, normalizedLength);
//		//		}
//		exitPathDB = tempDataBase;
//		stopwatch.Stop();
//		float timeTaken = (float)stopwatch.ElapsedMilliseconds;
//		//Console.WriteLine("SimUtilityCalculator built exit path database in " + timeTaken + " ms.");
//		return exitPathDB;
//	}
}