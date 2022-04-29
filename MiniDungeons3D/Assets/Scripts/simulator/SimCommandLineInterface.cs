using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class SimCommandLineInterface {

	private SimControllerHero _controllerHero;
	
	public string _inputFile;
	public string _outputFile;

	public int _mapHeight = 20;
	public int _mapWidth = 10;

	public string[] _mapStrings;

	private bool _simulation;

	public bool _runningController;
	public ControllerTypes _controllerType;

	public UtilityFunctions _utilityFunction;

	public int _msPerMove;
	public int _maxTurns;
	public int _numberOfRuns;

	public bool _quitOnEnd;
	public bool _writeResultsToDisk;

	void Start(){
		Debug.Log("MiniDungeons 2 map tester here, hello.");
		string[] cmdLineArgs = Environment.GetCommandLineArgs ();
		//string[] cmdLineArgs = "./md_aar -batchmode -logFile -simulation -levelRange 1 5 -controller MCTS -utility E -writeResults -quit -turns 100 -runs 1 -time 100 -aar /Users/holmgard/Desktop/2015-07-10_AIIDE_PM/actionsForAAR/SimControllerHeroMCTS_Exit_map1_to_1_map1_actions_run1.txt".Split(' ');
		//string[] cmdLineArgs = "./md_aar -batchmode -logFile -simulation -levelRange 1 10 -controller MCTS -utility TC -aar /Users/holmgard/Desktop/2015-07-10_AIIDE_PM/experimentResults_60s/2015-07-12_01-36-58_SimControllerHeroMCTS_TreasureCollector_map5_to_5_map5_actions_run1.txt -writeResults -quit -turns 100 -runs 20 -time 100".Split(' ');
		//string inputFile = "/Users/holmgard/Desktop/mdsstest/schemas.txt";
		//string outputFile = "/Users/holmgard/Desktop/mdsstest/output.txt";
		//string myArgs = "-batchmode -logFile -simulation -input " + inputFile + " -output " + outputFile + " -controller MCTS -utility E  -turns 100 -runs 1 -time 10";
		//string[] cmdLineArgs = myArgs.Split(' ');
		if(cmdLineArgs.Length > 0){
			ParseCommandLine(cmdLineArgs);
		}
		if(_simulation){
			Debug.Log("Running simulation...");
			_mapStrings = LoadMapsFromFile(_inputFile);
			EvaluateMaps();	
		}
		Debug.Log("OK, all done");
		if(_quitOnEnd){
			Application.Quit();
		}
	}

	/*public string EvaluateMap(string levelString, string levelLabel, string utilityType, string msPerMove, string maxTurns, string numberOfRuns){
		List<MapReport> mapReports = RunController(levelString, levelLabel, UtilityFunctionFromString(utilityType), int.Parse(msPerMove), int.Parse(maxTurns), int.Parse(numberOfRuns));
		return "done";
	}*/

	void EvaluateMaps(){
		var results = new List<MapReport>();
		/*for(int map = 0; map < _mapStrings.Length; map++){
			Debug.Log(_mapStrings[map]);
		}*/

		for(int map = 0; map < _mapStrings.Length; map++){
			var reports = RunController(_mapStrings[map], map.ToString(), _utilityFunction, _msPerMove, _maxTurns, _numberOfRuns);
			results.AddRange(reports);
		}
		WriteResultsToDisk(results, _outputFile);
	}

	string[] LoadMapsFromFile(string inputPath){
		string[] mapsLines = File.ReadAllLines(inputPath);
		if(mapsLines.Length%_mapHeight != 0){Debug.Log("Warning: Map file did not split to exact number of maps.");}
		int numMaps = mapsLines.Length/_mapHeight;
		string[] result = new String[numMaps];
		for(int mapNum = 0; mapNum < numMaps; mapNum++){
			string map = "";
			for(int mapLine = mapNum * _mapHeight; mapLine < mapNum * _mapHeight + _mapHeight; mapLine++){
				map += mapsLines[mapLine];
				if(mapLine < ((mapNum * _mapHeight + _mapHeight)-1) ){map += "\n";}
			}
			result[mapNum] = map;
		}
		return result;
	}

	SimControllerHero GetController(ControllerTypes type){
		SimControllerHero result = null;
		switch(type){
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
		}
		return result;
	}

	List<MapReport> RunController(string levelString, string levelLabel, UtilityFunctions utilityFunction, int msPerMove, int maxTurns, int numberOfRuns){
		var playReports = new List<MapReport>();

		_runningController = true;
		for(int run = 0; run < numberOfRuns; run++){	
				_controllerHero = GetController(_controllerType);
				//Debug.Log(_controllerHero);
				MapReport playReport = _controllerHero.PlayLevel(levelString, utilityFunction, msPerMove, maxTurns);
				playReport.Run = run;
				playReport.Label = levelLabel;
				if(playReport != null){
					//Debug.Log(playReport.LevelReport);
					playReports.Add(playReport);
				}
		}
		_runningController = false;
		return playReports;
	}

	void WriteResultsToDisk(List<MapReport> results, string outputPath){
		string levelReportOut = "";
		foreach(MapReport report in results){
			levelReportOut += "Label," + MapReport.MapReportHeader + ",Run_Number" + Environment.NewLine;
			levelReportOut += report.Label;
			levelReportOut += ",";
			levelReportOut += report.LevelReport;
			levelReportOut += ",";
			levelReportOut += report.Run;
			levelReportOut += ",";
			levelReportOut += Environment.NewLine;
			
			var visitMap = new string[_mapHeight,_mapWidth];
			var javelinMap = new string[_mapHeight,_mapWidth];

			for(int row = 0; row < visitMap.GetLength(0); row++){
				for(int col = 0; col < visitMap.GetLength(1); col++){
					visitMap[row,col] = ".";
					javelinMap[row,col] = ".";
				}
			}



			SimPoint heroLocation = report.startLocation;
			//Debug.Log(heroLocation);
			//Debug.Log(visitMap.GetLength(0));
			//Debug.Log(visitMap.GetLength(1));
			//Debug.Log("-----");
			visitMap[heroLocation.Y,heroLocation.X] = "X";
			foreach(SimHeroAction action in report.ActionReport){
				if(action.ActionType == HeroActionTypes.Move){
					heroLocation += action.DirectionOrTarget;
					//Debug.Log(action.DirectionOrTarget + " " + heroLocation);
					visitMap[heroLocation.Y,heroLocation.X] = "X";
				}
				if(action.ActionType == HeroActionTypes.JavelinThrow){
					javelinMap[action.DirectionOrTarget.Y,action.DirectionOrTarget.X] = "J";
				}
				//Debug.Log(heroLocation);
			}

			string visitString = "";
			for(int row = 0; row < visitMap.GetLength(0); row++){
				for(int col = 0; col < visitMap.GetLength(1); col++){
					visitString += visitMap[row,col];
				}
				visitString += Environment.NewLine;
			}
			string javelinString = "";
			for(int row = 0; row < javelinMap.GetLength(0); row++){
				for(int col = 0; col < javelinMap.GetLength(1); col++){
					javelinString += javelinMap[row,col];
				}
				javelinString += Environment.NewLine;
			}
			levelReportOut += "VisitMap:" + Environment.NewLine;
			levelReportOut += visitString;
			levelReportOut += "JavelinMap:" + Environment.NewLine;
			levelReportOut += javelinString;

			
		}
		File.WriteAllText(outputPath, levelReportOut);
	}

	/*void OnGUI(){
		GUI.Label(new Rect(110f,90f,100f,40f), "End level");
		_moveTimeLimitString = GUI.TextField(new Rect(10f,130f,100f,40f), _moveTimeLimitString);
		GUI.Label(new Rect(110f,130f,100f,40f), "Time limit per move in ms");
		_maxTurnsString = GUI.TextField(new Rect(10f,170f,100f,40f), _maxTurnsString);
		GUI.Label(new Rect(110f,170f,100f,40f), "Max moves allowed");
		_numberOfRunsString = GUI.TextField(new Rect(10f,210f,100f,40f), _numberOfRunsString);
		GUI.Label(new Rect(110f,210f,100f,40f), "Number of times to run this configuration");


		if(GUI.Button(new Rect(10f,10f,500f,40f), "Run " + _controllerType + " with " + _utilityFunction + " utility function.")){
			int moveTimeLimit = int.Parse(_moveTimeLimitString);
			int maxTurnsPerLevel = int.Parse(_maxTurnsString);
			int numberOfRuns = int.Parse(_numberOfRunsString);
			//RunController(startLevel, endLevel, _utilityFunction, moveTimeLimit, maxTurnsPerLevel, numberOfRuns);
		}
	}*/

	UtilityFunctions UtilityFunctionFromString(string utilityType){
		if(utilityType.Equals("E")) return UtilityFunctions.Exit;
		if(utilityType.Equals("R")) return UtilityFunctions.Runner;
		if(utilityType.Equals("L")) return UtilityFunctions.Loiterer;
		if(utilityType.Equals("C")) return UtilityFunctions.Completionist;
		if(utilityType.Equals("MK")) return UtilityFunctions.MonsterKiller;
		if(utilityType.Equals("TC")) return UtilityFunctions.TreasureCollector;

		return UtilityFunctions.Exit;
	}

	void ParseCommandLine(string[] cmdLineArgs){
		for(int i = 0; i < cmdLineArgs.Length; i++){
			if(cmdLineArgs[i].Equals("-simulation")){
				_simulation = true;
			}
			if(cmdLineArgs[i].Equals("-writeResults")){
				_writeResultsToDisk = true;
			}
			if(cmdLineArgs[i].Equals("-quit")){
				_quitOnEnd = true;
			}
			if(cmdLineArgs[i].Equals("-input")){
				_inputFile = cmdLineArgs[i+1];
			}
			if(cmdLineArgs[i].Equals("-output")){
				_outputFile = cmdLineArgs[i+1];	
			}
			if(cmdLineArgs[i].Equals("-time")){
				_msPerMove = int.Parse(cmdLineArgs[i+1]);
			}
			if(cmdLineArgs[i].Equals("-turns")){
				_maxTurns = int.Parse(cmdLineArgs[i+1]);
			}
			if(cmdLineArgs[i].Equals("-moves")){
				_maxTurns = int.Parse(cmdLineArgs[i+1]);
			}
			if(cmdLineArgs[i].Equals("-runs")){
				_numberOfRuns = int.Parse(cmdLineArgs[i+1]);
			}
			
			if(cmdLineArgs[i].Equals("-controller")){
				string controllerType = cmdLineArgs[i+1];
				if(controllerType.Equals("Random")){
					_controllerType = ControllerTypes.Random;
				}	
				if(controllerType.Equals("Greedy")){
					_controllerType = ControllerTypes.Greedy;
				}
				if(controllerType.Equals("BFS")){
					_controllerType = ControllerTypes.BreadthFirst;
				}
				if(controllerType.Equals("AStar")){
					_controllerType = ControllerTypes.AStarStateSpace;
				}
				if(controllerType.Equals("MCTS")){
					_controllerType = ControllerTypes.MCTS;
				}
			}

			if(cmdLineArgs[i].Equals("-utility")){
				string utilityType = cmdLineArgs[i+1];
				if(utilityType.Equals("E")){
					_utilityFunction = UtilityFunctions.Exit;
				}	
				if(utilityType.Equals("R")){
					_utilityFunction = UtilityFunctions.Runner;
				}
				if(utilityType.Equals("L")){
					_utilityFunction = UtilityFunctions.Loiterer;
				}
				if(utilityType.Equals("C")) {
					_utilityFunction = UtilityFunctions.Completionist;
				}
				if(utilityType.Equals("MK")){
					_utilityFunction = UtilityFunctions.MonsterKiller;
				}
				if(utilityType.Equals("TC")){
					_utilityFunction = UtilityFunctions.TreasureCollector;
				}
			}
		}
	}
}
