using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using TreeLanguageEvolute;
using System.Linq;


public class ResultObject
{

    public List<SimPoint> positions { get; set; }
    public LevelReport levelReport { get; set; }
    public List<Mech> mechanics { get; set; }
    public SimPoint startLocation { get; set; }
    public List<string> stateReport { get; set; }
    public List<string> levelStates { get; set; }
    public Dictionary<Mechanic, int> frequencies { get; set; }
    public float avgWin { get; set; }
    public float avgHealth { get; set; }
    public float avgDist { get; set; }

    public ResultObject(MapReport report)
    {
        positions = report.Positions;
        mechanics = report.Mechanics;
        levelReport = report.LevelReport;
        startLocation = report.startLocation;
        stateReport = report.StateReport;
        levelStates = report.LevelStates;
        frequencies = report.Frequencies;
    }
}


public class SimSentientSketchbookInterface
{
    public bool _runningController;
    private bool _generateHeatmaps = false;

    public MapReport EvaluateMap(string levelString, string levelLabel, string controllerType, string utilityType, string msPerMove, string maxTurns, string numberOfRuns, List<SimPoint> positions, List<string> ascii_maps)
    {

        string result = "";
        int totalNumberOfRuns = int.Parse(numberOfRuns);
        List<MapReport> mapReports = RunController(levelString, levelLabel, ControllerTypeFromString(controllerType), UtilityFunctionFromString(utilityType), int.Parse(msPerMove), int.Parse(maxTurns), totalNumberOfRuns, positions, ascii_maps);
        if (totalNumberOfRuns > 1)
        {
            for (int j = 0; j < mapReports.Count; j++)
            {
                MapReport report = mapReports[j];
                result += report.LevelReport;
                result += "\n";
                for (int i = 0; i < report.Positions.Count; i++)
                {
                    SimPoint point = report.Positions[i];
                    result += "(";
                    result += point.X;
                    result += ",";
                    result += point.Y;
                    result += ")";
                    if (i < report.Positions.Count - 1)
                    {
                        result += ",";
                    }
                    else
                    {
                        result += "\n";
                    }
                }

            }

            return null;
        }
        else
        {
            return mapReports[0];
        }
    }
    public string EvaluateMap(string levelString, string levelLabel, string controllerType, string utilityType, string msPerMove, string maxTurns, string numberOfRuns, BaseProgram function)
    {
        List<ResultObject> preresult = new List<ResultObject>();
        int totalNumberOfRuns = int.Parse(numberOfRuns);
        int totalMovesAcrossRuns = 0;
        float wins = 0.0f;
        List<MapReport> mapReports = RunController(levelString, levelLabel, ControllerTypeFromString(controllerType), UtilityFunctionFromString(utilityType), int.Parse(msPerMove), int.Parse(maxTurns), totalNumberOfRuns, function);
        for (int j = 0; j < mapReports.Count; j++)
        {
            MapReport report = mapReports[j];
            ResultObject ro = new ResultObject(report);
            preresult.Add(ro);
        }

        string result = JsonConvert.SerializeObject(preresult);

        return result;
    }

    public ResultObject EvaluateMap(string levelString, string levelLabel, string controllerType, string utilityType, string msPerMove, string maxTurns, BaseProgram function)
    {
        string result = "";
        int totalMovesAcrossRuns = 0;
        float wins = 0.0f;
        List<MapReport> mapReports = RunController(levelString, levelLabel, ControllerTypeFromString(controllerType), UtilityFunctionFromString(utilityType), int.Parse(msPerMove), int.Parse(maxTurns), 1, function);
        MapReport report = mapReports[0];
        ResultObject ro = new ResultObject(report);

        return ro;
    }

    public static string GenerateHeatmap(string levelString, List<SimPoint> positions)
    {
        return GenerateHeatmap(levelString, positions, null);
    }

    public static string GenerateHeatmap(string levelString, List<SimPoint> positions, List<SimPoint> monsterPoints)
    {
        char[] splitsies = { '\n' };
        string[] rows = levelString.Split(splitsies, System.StringSplitOptions.RemoveEmptyEntries);
        int numRows = rows.Length;
        int numCols = rows[0].Length;

        //int tileWidth = 80;
        //int tileHeight = 100;

        //int mapHeight = numRows * tileHeight;
        //int mapWidth = numCols * tileWidth;

        /*Dictionary<string,Bitmap> tileSet = new Dictionary<string,Bitmap> ();
		//Terrain
		tileSet.Add ("X", new Bitmap("tileset/bg_wall.png"));
		tileSet.Add (" ", new Bitmap ("tileSet/bg_empty.png"));
		tileSet.Add ("H", new Bitmap ("tileSet/bg_entrance.png"));
		tileSet.Add ("e", new Bitmap ("tileSet/bg_exit.png"));
		tileSet.Add ("E", new Bitmap ("tileSet/bg_entrance.png"));
		//Items
		tileSet.Add ("T", new Bitmap ("tileSet/bg_chest.png"));
		tileSet.Add ("P", new Bitmap ("tileSet/bg_potion.png"));
		tileSet.Add ("p", new Bitmap ("tileSet/bg_portal.png"));
		tileSet.Add ("t", new Bitmap ("tileSet/bg_trap.png"));
		//Monsters
		tileSet.Add ("m", new Bitmap ("tileSet/bg_minitaur.png"));
		tileSet.Add ("M", new Bitmap ("tileSet/bg_goblin_armored.png"));
		tileSet.Add ("R", new Bitmap ("tileSet/bg_goblin_mage.png"));
		tileSet.Add ("b", new Bitmap ("tileSet/bg_blob.png"));
		tileSet.Add ("o", new Bitmap ("tileSet/bg_ogre.png"));
		tileSet.Add (" ", new Bitmap ("tileSet/bg_empty.png"));

		Bitmap heatmap = new Bitmap (mapWidth, mapHeight);*/

        char[,] heatmap = new char[numRows, numCols];

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].Length; col++)
            {
                heatmap[row, col] = rows[row][col];

                /*int offsetX = row * tileHeight;
				int offsetY = col * tileWidth;

				string levelSymbol = rows [row].Substring (col, 1);
				Bitmap levelTile;
				tileSet.TryGetValue (levelSymbol, out levelTile);
				if (levelTile != null) {
					for (int tileX = 0; tileX < levelTile.Width; tileX++) {
						for (int tileY = 0; tileY < levelTile.Height; tileY++) {
							Color c = levelTile.GetPixel (tileX, tileY);
							heatmap.SetPixel ((tileX + offsetX), (tileY + offsetY), c);
						}
					}
				}*/
            }
        }
        //heatmap.Save ("heatmap.png", ImageFormat.Png);

        foreach (SimPoint position in positions)
        {
            string current = heatmap[position.Y, position.X].ToString();
            int curInt;

            if (Int32.TryParse(current, out curInt))
            {
                curInt += 1;
            }
            else
            {
                curInt = 1;
            }
            if (curInt > 9 || current.Equals("@"))
            {
                heatmap[position.Y, position.X] = '@';
            }
            else
            {
                heatmap[position.Y, position.X] = curInt.ToString()[0];
            }
        }

        string heatmapString = "";

        for (int k = 0; k < heatmap.GetLength(0); k++)
        {
            for (int l = 0; l < heatmap.GetLength(1); l++)
            {
                heatmapString += heatmap[k, l];
            }
            heatmapString += "\n";
        }

        if (monsterPoints != null)
        {

            foreach (SimPoint point in monsterPoints)
            {
                int oneDindex = point.X + point.Y * 11;
                char[] chars = heatmapString.ToCharArray();
                chars[oneDindex] = '☹';
                heatmapString = new string(chars);
            }
        }

        return heatmapString;
    }

    private SimControllerHero GetController(ControllerTypes type, List<SimPoint> positions, List<string> ascii_maps)
    {
        SimControllerHero result = null;
        switch (type)
        {
            case ControllerTypes.Random:
                result = new SimControllerHeroRandom();
                break;
            case ControllerTypes.Greedy:
                result = new SimControllerHeroGreedy();
                break;
            case ControllerTypes.BreadthFirst:
                result = new SimControllerHeroBreadthFirst();
                break;
            case ControllerTypes.AStarStateSpace:
                result = new SimControllerHeroAStar();
                break;
            case ControllerTypes.MCTS:
                result = new SimControllerHeroMCTS();
                break;
            case ControllerTypes.ActionAgreement:
                result = new SimControllerHeroActionAgreement(positions, ascii_maps);
                break;
        }
        return result;
    }
    private SimControllerHero GetController(ControllerTypes type, BaseProgram function)
    {
        SimControllerHero result = null;
        switch (type)
        {
            case ControllerTypes.Random:
                result = new SimControllerHeroRandom();
                break;
            case ControllerTypes.Greedy:
                result = new SimControllerHeroGreedy();
                break;
            case ControllerTypes.BreadthFirst:
                result = new SimControllerHeroBreadthFirst();
                break;
            case ControllerTypes.AStarStateSpace:
                result = new SimControllerHeroAStar();
                break;
            case ControllerTypes.MCTS:
                result = new SimControllerHeroMCTS(function);
                break;
            case ControllerTypes.MCTSOnline:
                result = new SimControllerHeroOnlineMCTS();
                break;
        }
        return result;
    }
    public List<MapReport> RunController(string levelString, string levelLabel, ControllerTypes controllerType, UtilityFunctions utilityFunction, int msPerMove, int maxTurns, int numberOfRuns, List<SimPoint> positions, List<string> ascii_maps)
    {
        var playReports = new List<MapReport>();
        _runningController = true;
        int wins = 0;
        for (int run = 0; run < numberOfRuns; run++)
        {
            SimControllerHero controllerHero = GetController(controllerType, positions, ascii_maps);
            MapReport playReport = controllerHero.PlayLevel(levelString, utilityFunction, msPerMove, maxTurns);
            playReport.Run = run;
            playReport.Label = levelLabel;
            if(playReport != null){
                playReports.Add(playReport);
            }

        }
        _runningController = false;

        return playReports;
    }
    public List<MapReport> RunController(string levelString, string levelLabel, ControllerTypes controllerType, UtilityFunctions utilityFunction, int msPerMove, int maxTurns, int numberOfRuns, BaseProgram function)
    {
        var playReports = new List<MapReport>();
        _runningController = true;

        //using (var progress = new ProgressBar())
        //{
        for (int run = 0; run < numberOfRuns; run++)
        {
            SimControllerHero controllerHero = GetController(controllerType, function);
            MapReport playReport = controllerHero.PlayLevel(levelString, utilityFunction, msPerMove, maxTurns);
            SimLevel tempLevel = new SimLevel(levelString);
            playReport.LevelReport.startDistanceToExit = PathDatabase.Lookup(tempLevel.SimHero.Point, tempLevel.SimExit.Point, tempLevel).Length;
            playReport.Run = run;
            playReport.Label = levelLabel;
            if (playReport != null)
            {
                playReports.Add(playReport);
            }
            //double progval = (run + 1) / (1.0 * numberOfRuns);
            //progress.Report(progval);

        }
        //}
        //Console.WriteLine("\nComplete!");
        _runningController = false;
        return playReports;
    }

    /// <summary>
    /// Runs the controller for testing purposes in the level editor.
    /// </summary>
    /// <returns>The controller.</returns>
    /// <param name="levelString">Level string.</param>
    /// <param name="controllerType">Controller type.</param>
    /// <param name="utilityFunction">Utility function.</param>
    /// <param name="function">Function.</param>
    public MapReport RunController(string levelString, string controllerType, string utilityFunction, BaseProgram function)
    {
        // make the utility function a thing
        _runningController = true;
        SimControllerHero controllerHero = GetController(ControllerTypeFromString(controllerType), function);
        MapReport playReport = controllerHero.PlayLevel(levelString, UtilityFunctionFromString(utilityFunction), 40, 500);
        playReport.Run = 1;
        playReport.Label = "null";

        _runningController = false;
        return playReport;
    }


    public static ControllerTypes ControllerTypeFromString(string controllerTypeString)
    {
        ControllerTypes controllerType = ControllerTypes.Random;
        if (controllerTypeString.Equals("Random")) controllerType = ControllerTypes.Random;
        if (controllerTypeString.Equals("Greedy")) controllerType = ControllerTypes.Greedy;
        if (controllerTypeString.Equals("BFS")) controllerType = ControllerTypes.BreadthFirst;
        if (controllerTypeString.Equals("AStar")) controllerType = ControllerTypes.AStarStateSpace;
        if (controllerTypeString.Equals("MCTS")) controllerType = ControllerTypes.MCTS;
        if (controllerTypeString.Equals("MCTSGP")) controllerType = ControllerTypes.MCTSGP;
        if (controllerTypeString.Equals("MCTSO")) controllerType = ControllerTypes.MCTSOnline;
        if (controllerTypeString.Equals("ActionAgreement")) controllerType = ControllerTypes.ActionAgreement;
        return controllerType;
    }

    public static UtilityFunctions UtilityFunctionFromString(string utilityType)
    {
        UtilityFunctions utilityFunction = UtilityFunctions.Exit;
        if (utilityType.Equals("E")) utilityFunction = UtilityFunctions.Exit;
        if (utilityType.Equals("R")) utilityFunction = UtilityFunctions.Runner;
        if (utilityType.Equals("L")) utilityFunction = UtilityFunctions.Loiterer;
        if (utilityType.Equals("C")) utilityFunction = UtilityFunctions.Completionist;
        if (utilityType.Equals("MK")) utilityFunction = UtilityFunctions.MonsterKiller;
        if (utilityType.Equals("TC")) utilityFunction = UtilityFunctions.TreasureCollector;
        return utilityFunction;
    }

    /// <summary>
    /// Builds the function.
    /// </summary>
    /// <returns>The function.</returns>
    /// <param name="filepath">Filepath.</param>
    public TreeProgram BuildFunction(string filepath)
    {
        // build function for runner
        GeneticProgrammingEngine genEngine = new GeneticProgrammingEngine();
        // number of steps taken
        genEngine.DeclareVariable("stepsTaken");
        // number of monsters slain
        genEngine.DeclareVariable("monstersSlain");
        // number of potions taken
        genEngine.DeclareVariable("potionsTaken");
        // number of treasure chests opened
        genEngine.DeclareVariable("treasuresOpened");
        // number of minitaur knockouts
        genEngine.DeclareVariable("minotaurKnockouts");
        // number of javelins thrown
        genEngine.DeclareVariable("javelinThrows");
        // number of teleports used
        genEngine.DeclareVariable("teleportsUsed");
        // number of traps sprung
        genEngine.DeclareVariable("trapsSprung");
        // distance from exit
        genEngine.DeclareVariable("distanceFromExit");
        // health left
        genEngine.DeclareVariable("healthLeft");
        // reward in the node
        genEngine.DeclareVariable("rReward");
        // interactable ratio
        genEngine.DeclareVariable("interactableRatio");
        genEngine.SetFunctions = new FunctionType[]
        { new Add(), new Substract(), new Multiply(), new Divide() };
        genEngine.SetValues = new TreeLanguageEvolute.ValueType[] { new RandomMacro() };
        //.............// TreeProgram function = genEngine.LoadProgram(@filepath);
        TreeProgram function = genEngine.LoadProgram(filepath);
        return function;
    }
}
