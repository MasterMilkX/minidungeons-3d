using System;
using System.Collections;
using System.IO;
//using System.Drawing;
using System.Collections.Generic;
using TreeLanguageEvolute;
using System.Timers;
public class HeuristicGeneticProgramming {
	
	// The genetic programming engine
	private static GeneticProgrammingEngine genEngine;
	// Population size
	private static int POPULATION = 100;
	// Number of generations
	private static int GENERATION_COUNT = 100;
	// Minimum initial depth of a tree
	private static int MIN_INIT_TREE_DEPTH = 2;
	// Maximum initial depth of a tree
	private static int MAX_INIT_TREE_DEPTH = 5;
	// Maximum overall depth of a tree
	private static int MAX_OVERALL_TREE_DEPTH = 8;
	// Fitness goal of the game engine (MIN_FITNESS or MAX_FITNESS)
	private static BaseEngine.FitnessGoalEnum FITNESS_GOAL = BaseEngine.FitnessGoalEnum.MAX_FITNESS;
	// Overselection size for tournament selection
	private static int OVERSELECTION = 2;
	// Chance that crossover may occur
	private static float CHANCE_FOR_CROSSOVER = 1.0f;
	// Chance for mutation to occur
	private static float MUTATION_CHANCE = 0.1f;
	// Elite
	private static float SAVE_TOP_INDIVIDUALS = 0.15f;
	// Number of threads used in evaluating 
	private static int NUMBER_OF_THREADS = 100;

	private static int NUMBER_OF_ISLANDS = 5;

	private static int EVALUATED_OVER_GENERATIONS = 500;

	private static int MIGRATIONS_PER_MIGRATION_GEN = 5;

	private static int MIGRATION_GENERATION = 1;

	private static string strLabelString;

	private static bool _generateHeatmaps = true;

	private static int chromeCounter = 1;

	private static string[] mapset;

	public void Main(string[] args) {


	}
	public static void Init() {
		genEngine = new GeneticProgrammingEngine();
		// set some vars for genEngine
		genEngine.NumberOfPrograms 		= POPULATION;
		genEngine.MinInitialTreeDepth 	= MIN_INIT_TREE_DEPTH;
		genEngine.MaxInitialTreeDepth 	= MAX_INIT_TREE_DEPTH;
		genEngine.MaxOverallTreeDepth 	= MAX_OVERALL_TREE_DEPTH;
		genEngine.FitnessGoal 			= FITNESS_GOAL;
		genEngine.Overselection 		= OVERSELECTION;
		genEngine.ChanceForCrossover 	= CHANCE_FOR_CROSSOVER;
		genEngine.SaveTopIndividuals 	= SAVE_TOP_INDIVIDUALS;
		genEngine.NumberOfThreads 		= NUMBER_OF_THREADS;
		genEngine.MutationChance 		= MUTATION_CHANCE;
		genEngine.NumberOfIslands 		= NUMBER_OF_ISLANDS;
		genEngine.MigrationsPerMigrationGeneration = MIGRATIONS_PER_MIGRATION_GEN;
		genEngine.OnceInHowManyGenerationsPerformMigrationGeneration = MIGRATION_GENERATION;
		genEngine.OnceInHowManyGenerationsTopIndividualsShouldBeEvaluated = EVALUATED_OVER_GENERATIONS;
		// add basic mathematic functionality to the tree formation process
		genEngine.SetFunctions = new FunctionType[]
		{ new Add (), new Substract (), new Multiply (), new Divide () };
		DeclareVariables ();
		genEngine.SetValues = new TreeLanguageEvolute.ValueType[] { new RandomMacro () };

		genEngine.EvalFitnessForProgramEvent += new GeneticProgrammingEngine.EvalFitnessHandler (genEngine_EvalFitnessForProgramEvent);
		genEngine.GenerationIsCompleteEvent += new BaseEngine.GenerationIsCompleteHandler (genEngine_GenerationIsCompleteEvent);

		using (System.IO.StreamWriter file = 
			new System.IO.StreamWriter(@"GenDetails.txt", true))
		{
			file.WriteLine("Generation Details: \n");
		}
//		PathDatabase.BigDatabase = new List<HashSet<Pairing>>();
		PathDatabase.BigDatabase = new List<Dictionary<string, SimPoint[]>>();
		PathDatabase.levels = new ArrayList();

		//mapset = new string[4];

		//mapset [0] = "XXXXXXXXXX|XH       X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X       eX|XXXXXXXXXX";
		//mapset [1] = "XXXXXXXXXX|XH       X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X   m    X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X        X|X       eX|XXXXXXXXXX";
		//mapset [2] = "XXXXXXXXXXXX|XmH        X|XXXXXXXXXX X|X          X|X XXXTXXXXTX|X          X|XXXXXXXXXX X|X          X|X XXXXXXXXXX|X          X|XXXXXXXXXX X|Xe         X|XXXXXXXXXXXX";
		//mapset [3] = "XXXXXXXXXX|Xm  H  MTX|XXXXX  XXX|XR      PX|XM    XXTX|XXXXX XXXX|XP  M M  X|XXXX XXXXX|X  t    pX|X XX XXX X|X Xb b   X|X X XXXXXX|X        X|XXXXXXXX X|XM       X|X XX  XX X|X XX  XX X|X  M  M  X|Xp      eX|XXXXXXXXXX";

		mapset = new string[6];
		mapset [0] = "XXXXXXXXXX|Xm  H  MTX|XXXXX  XXX|XR      PX|XM    XXTX|XXXXX XXXX|XP  M M  X|XXXX XXXXX|X  t    pX|X XX XXX X|X Xb b   X|X X XXXXXX|X        X|XXXXXXXX X|XM       X|X XX  XX X|X XX  XX X|X  M  M  X|Xp      eX|XXXXXXXXXX";
		mapset [1] = "XXXXXXXXXX|Xm RX  beX|X X o X bX|X   X  X X|X XP MXP X|X  XT  X X|X X X  X X|X P XpX PX|X X  X  XX|X  XXX   X|X XTT X  X|X X X X  X|X oX MpX X|XT   XPR X|X X X  X X|X tMXX   X|X X XT X X|X    X  mX|XH X   X X|XXXXXXXXXX";
		mapset [2] = "XXXXXXXXXX|XmX HXMTRX|X X  XMTRX|X X  X X X|X XT   X X|X XXX    X|X XT     X|X        X|XXXXPPXXXX|XM      MX|X  XXXX  X|Xo XbTX  X|X        X|X   XX   X|XT  TP   X|X   TP   X|X        X|X        X|X  b  b eX|XXXXXXXXXX";
		mapset [3] = "XXXXXXXXXX|XR      oX|X XTPX X X|X XXXX X X|X  XRM X X|XX X   X X|X PX X MRX|X TX  XXPX|X XXX XTPX|XeXm HXTTX|X XXX XXTX|X X    XPX|X X X  XPX|X X XbTXRX|X X XXTXRX|X X XPPX X|X X      X|X XXXXXX X|XR      oX|XXXXXXXXXX";
		mapset [4] = "XXXXXXXXXX|XXX  R   X|XXXRXP XpX|XXX PX XXX|XXX    XTX|XXXMXX t X|XXXTXP   X|XXX XX t X|XXm X  XTX|X   pXoXXX|X  TXX XXX|XRTXXX  TX|XeXXXX  XX|XXXXXX   X|XXXXXX XTX|XXXXXX XXX|XXXPRX XXX|XXXX X XXX|XXXb  H bX|XXXXXXXXXX";
		mapset [5] = "XXXXXXXXXX|XXXXXXXXXX|XRTT  mXXX|Xttt   XXX|X       XX|X XX  X XX|X XP TXpXX|X eX XX XX|XXXXR   XX|XXXXTXX XX|XX pXXX XX|XM XXXP XX|XX  XXXRXX|XX    X XX|XT  R b oX|X XXTX X X|X XXPX XTX|X XXXX X X|Xb    P HX|XXXXXXXXXX";
		foreach (string map in mapset) {

			SimLevel lev = new SimLevel (SplitLevelString(map));
			PathDatabase.BuildDB (lev);
		}
		string info = String.Format("{0,-15} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15} {7,-15}", "Chrome #","Score", "1", "2", "3", "4", "7", "10");
		Console.WriteLine (info);
//		using (System.IO.StreamWriter file = 
//			new System.IO.StreamWriter(@"debugLog.txt", true))
//		{
//			file.WriteLine(info);
//		}
		genEngine.RunEvoluteOnThread(GENERATION_COUNT);

	}

	/*
	 * Variables that we will provide values for will be declared in here
	 * ## VARIABLES TO INCLUDE ##
	 * 
	 */
	private static void DeclareVariables() {
		// number of steps taken
		genEngine.DeclareVariable ("stepsTaken");
		// number of monsters slain
		genEngine.DeclareVariable ("monstersSlain");
		// number of potions taken
		genEngine.DeclareVariable ("potionsTaken");
		// number of treasure chests opened
		genEngine.DeclareVariable ("treasuresOpened");
		// number of minitaur knockouts
		genEngine.DeclareVariable ("minotaurKnockouts");
		// number of javelins thrown
		genEngine.DeclareVariable ("javelinThrows");
		// number of teleports used
		genEngine.DeclareVariable ("teleportsUsed");
		// number of traps sprung
		genEngine.DeclareVariable ("trapsSprung");
		// distance from exit
		genEngine.DeclareVariable ("distanceFromExit");
		// health left
		genEngine.DeclareVariable ("healthLeft");
		// node reward
		genEngine.DeclareVariable ("rReward");
		// interactable ratio
		genEngine.DeclareVariable ("interactableRatio");
	}


	public static List<MapReport> RunController(string levelString, string levelLabel, UtilityFunctions utilityFunction, int msPerMove, int maxTurns, int numberOfRuns, BaseProgram function){
		var playReports = new List<MapReport>();
		for(int run = 0; run < numberOfRuns; run++){
			SimControllerHeroMCTS controllerHero = new SimControllerHeroMCTS (function);
			MapReport playReport = controllerHero.PlayLevel(levelString, utilityFunction, msPerMove, maxTurns);
			playReport.Run = run;
			playReport.Label = levelLabel;
			if(playReport != null){
				playReports.Add(playReport);
			}
		}
		return playReports;
	}

	public static string EvaluateMap(string levelString, string levelLabel, string controllerType, string utilityType, string msPerMove, string maxTurns, string numberOfRuns, BaseProgram function){
		string result = "";
		int totalNumberOfRuns = int.Parse (numberOfRuns);
		int totalMovesAcrossRuns = 0;
		List<MapReport> mapReports = RunController(levelString, levelLabel, SimSentientSketchbookInterface.UtilityFunctionFromString(utilityType), int.Parse(msPerMove), int.Parse(maxTurns), totalNumberOfRuns, function);
		for(int j = 0; j < mapReports.Count; j++){
			MapReport report = mapReports [j];
			//result += "\n--START---\n";
			//result += "\n-LEVELREPORT-\n";
			result += report.LevelReport;
			//result += "\n-ACTIONREPORT-\n";
			//foreach(SimHeroAction action in report.ActionReport){
			//	result += action.ToString ();
			result += "\n";
			for (int i = 0; i < report.Positions.Count; i++){
				SimPoint point = report.Positions[i];
				result += "(";
				result += point.X;
				result += ",";
				result += point.Y;
				result += ")";
				if (i < report.Positions.Count - 1) {
					result += ",";
				} else {
					result += "\n";
				}
			}

			//}
			//result += "\n--END---\n";
			if (_generateHeatmaps) {
				//result += "\n";
				result += GenerateHeatmap (levelString, report.Positions);
			}

			/*Console.WriteLine ("Full state list:");
			foreach (string state in report.StateReport) {
				Console.WriteLine ("---------");
				Console.WriteLine (state);
				Console.WriteLine ("---------");
			}*/
			totalMovesAcrossRuns += report.Positions.Count;
		}
		result += "\nNumber of runs:" + numberOfRuns;
		result += "\nAverage moves per run:" + (totalMovesAcrossRuns/totalNumberOfRuns);

		return result;
	}

	public static void genEngine_EvalFitnessForProgramEvent(BaseProgram progProgram, BaseEngine sender)
	{

		string controllerType = "MCTSGP";
		string utilityFunction = "C";
		string msPerMove = "40";
		string maxMovesPerTurn = "200";
		string numRuns = "1";

		float[] fitnesses = new float[mapset.Length];

		for (int i = 0; i < mapset.Length; i++) {
			string result = EvaluateMap (SplitLevelString(mapset[i]), "label", controllerType, utilityFunction, msPerMove, maxMovesPerTurn, numRuns, progProgram);
			//Console.Write (result);
			int first = 0;
			int last = 0;
			first = result.IndexOf ("Utility: ") + "Utility: ".Length;
			last = result.IndexOf (", Time: ");
			string utilityGained = result.Substring(first, last - first);
			float fitness = (float) Double.Parse(utilityGained);
			fitnesses [i] = fitness;
		}
		float overallFitness = 0.0f;
		for (int i = 0; i < fitnesses.Length; i++) {
			overallFitness = overallFitness + (float) fitnesses [i];
		}
		overallFitness = overallFitness / fitnesses.Length;
		//string info = String.Format ("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", chromeCounter, overallFitness, fitnesses[0], fitnesses[1],
			//fitnesses[2], fitnesses[3]);
		string info = String.Format ("{0,-15} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15} {7,-15}", chromeCounter, overallFitness, fitnesses[0], fitnesses[1],
			fitnesses[2], fitnesses[3], fitnesses[4], fitnesses[5]);
		//string info = String.Format ("{0,-10} {1,-10}", chromeCounter, overallFitness);
		Console.WriteLine (info);
//		using (System.IO.StreamWriter file = 
//			new System.IO.StreamWriter(@"debugLog.txt", true))
//		{
//			file.WriteLine(info);
//		}
//		stepsTaken.Value = NormalizedUtilityValues;
		// we need to call a method to simulate games for this specific utility function
			// run the game here, return win-lose and what we care about
			//float fExpectedResult = (5 * varX.Value * varX.Value * varX.Value) + (3 * varX.Value * varX.Value) + varX.Value;
		// run games, get back score
		//progProgram.Fitness += Math.Abs (fExpectedResult - fActualResult);
		progProgram.Fitness = overallFitness;
		chromeCounter++;
	}

	public static void genEngine_GenerationIsCompleteEvent(Statistics stsStatistics, BaseEngine sender)
	{
		chromeCounter = 1;
		strLabelString = "Generation number " + stsStatistics.GenerationNumber
		+ " : min fitness = " + stsStatistics.MinFitnessProgram.Fitness
		+ " : max fitness = " + stsStatistics.MaxFitnessProgram.Fitness
		+ " : average fitness = " + stsStatistics.AvgFitness
		+ ", Total nodes = " + stsStatistics.TotalNodes;

		if (stsStatistics.GenerationNumber % 1 == 0 || stsStatistics.GenerationNumber == GENERATION_COUNT - 1) {
			string filename = @"GenProgram" + stsStatistics.GenerationNumber + ".txt";
			genEngine.SaveProgram (filename, (TreeProgram)stsStatistics.MaxFitnessProgram);
		}
		// after init pop fitness, if we need to cut down on threads, do so now
		if (stsStatistics.GenerationNumber == 0 && SAVE_TOP_INDIVIDUALS > 0.0f) {
			genEngine.NumberOfThreads = POPULATION - (int) (POPULATION * SAVE_TOP_INDIVIDUALS);

		}
		Console.WriteLine (strLabelString);
		using (System.IO.StreamWriter file = 
			new System.IO.StreamWriter(@"GenDetails.txt", true)) {
			file.WriteLine(strLabelString);
		}
		string info = String.Format("{0,-15} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15} {7,-15}", "Chrome #","Score", "1", "2", "3", "4", "7", "10");
		Console.WriteLine(info);
//		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"debugLog.txt", true)) {
//			file.WriteLine (strLabelString);
//			file.WriteLine(info);
//		}
	}
	public static string SplitLevelString(string oneDLevel){
		return oneDLevel.Replace('|','\n');

	}
	public static string GenerateHeatmap(string levelString, List<SimPoint> positions){
		return GenerateHeatmap (levelString, positions, null);	
	}

	public static string GenerateHeatmap(string levelString, List<SimPoint> positions, List<SimPoint> monsterPoints){
		char[] splitsies = { '\n' };
		string[] rows = levelString.Split (splitsies, System.StringSplitOptions.RemoveEmptyEntries);
		int numRows = rows.Length;
		int numCols = rows[0].Length;



		char[,] heatmap = new char[numRows, numCols];

		for (int row = 0; row < rows.Length; row++) {
			for (int col = 0; col < rows [row].Length; col++) {
				heatmap [row, col] = rows [row] [col];
			}
		}
		foreach(SimPoint position in positions){
			string current = heatmap[position.Y,position.X].ToString();
			int curInt;

			if(Int32.TryParse(current, out curInt)){
				curInt += 1;
			} else {
				curInt = 1;
			}
			if (curInt > 9 || current.Equals("@")) {
				heatmap[position.Y,position.X] = '@';
			} else {
				heatmap[position.Y,position.X] = curInt.ToString()[0];
			}
		}

		string heatmapString = "";

		for (int k = 0; k < heatmap.GetLength (0); k++) {
			for (int l = 0; l < heatmap.GetLength (1); l++) {
				heatmapString += heatmap [k, l];
			}
			heatmapString += "\n";
		}

		if (monsterPoints != null) {

			foreach (SimPoint point in monsterPoints) {
				int oneDindex = point.X + point.Y * 11;
				char[] chars = heatmapString.ToCharArray ();
				chars[oneDindex] = '☹';
				heatmapString = new string (chars);
			}
		}

		return heatmapString;
	}

}