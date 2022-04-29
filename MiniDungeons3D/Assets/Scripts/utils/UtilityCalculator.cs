public struct NormalizedUtilityValues{

	public bool _didBacktrack;

	public double _distanceToExit;
	public double _distanceToTreasure;
	public double _healthLeft;
	
	public double _monsterKills;
	public double _minitaurKnockouts;
	public double _treasuresOpened;
	public double _stepsTaken;
	public double _javelinThrows;
	public double _potionsDrunk;
	public double _teleportsUsed;
	public double _trapsSprung;
	public double _exitReached;
	public double _interactablesRatio;
}

public static class UtilityCalculator {

	/*public static NormalizedUtilityValues NormalizeUtilities(MDLevel level, Hero hero){
		var result = new NormalizedUtilityValues();

		int totalMonsters = hero._meleeMonstersKilled + hero._rangedMonstersKilled + hero._ogresKilled + hero._blobsKilled;

		result._monsterKills = (float)totalMonsters/(float)level._monsters.Count;
		result._minitaurKnockouts = (float)hero._minitaurKnockouts/20.0f;
		result._treasuresOpened = (float)hero._treasuresOpened/(float)level._treasures.Count;
		result._stepsTaken = (float)hero._stepsTaken/100.0f;
		result._javelinThrows = (float)hero._javelinThrows/20.0f;
		result._potionsDrunk = (float)hero._potionsDrunk/(float)level._potions.Count;
		result._teleportsUsed = (float)hero._teleportsUsed/20.0f;
		result._trapsSprung = (float)hero._trapsSprung/(float)level._traps.Count;
		result._exitReached = (float)hero._exitsReached;
		return result;
	}

	public static float MonsterKillerUtilities(MDLevel level, Hero hero){
		//UtilityCalculator.PrintRawScores(hero);
		NormalizedUtilityValues norm = UtilityCalculator.NormalizeUtilities(level, hero);
		return (norm._monsterKills + norm._exitReached * 2.0f);
	}

	public static float ExitUtilities(MDLevel level, Hero hero){
		NormalizedUtilityValues norm = UtilityCalculator.NormalizeUtilities(level, hero);
		return norm._exitReached - norm._stepsTaken;
	}

	public static void PrintRawScores(Hero hero){
		string outString = 	"Melee:" + hero._meleeMonstersKilled + "\n" +
							"Ranged: " + hero._rangedMonstersKilled + "\n" +
							"Ogres: " + hero._ogresKilled + "\n" +
							"Blobs: " + hero._blobsKilled + "\n" +
							"Treasures:" + hero._treasuresOpened + "\n" +
							"Minitaurs: " + hero._minitaurKnockouts + "\n" +
							"Steps: " + hero._stepsTaken + "\n" +
							"Javelins:" + hero._javelinThrows + "\n" +
							"Potions:" + hero._potionsDrunk + "\n" +
							"Teleports: " + hero._teleportsUsed + "\n" +
							"Traps: " + hero._trapsSprung + "\n" +
							"Exits: " + hero._exitsReached;

		UnityEngine.Debug.Log(outString);
	}*/
}
