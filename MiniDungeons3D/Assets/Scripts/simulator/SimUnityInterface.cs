using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

/*public enum ControllerTypes{
	Random,
	Greedy,
	BreadthFirst,
	AStarStateSpace,
	MCTS
}*/

public class SimUnityInterface : MonoBehaviour {

	private SimControllerHero _controllerHero;
	private bool _simulation;
	public bool _aarMode;
	public string _aarFile;
	public string _levelString;
	public bool _runningController;
	public ControllerTypes _controllerType;

	public UtilityFunctions _utilityFunction;

	public string startString = "";
	public string endString = "1";
	public string moveTimeLimitString = "1000";
	public string maxTurnsString = "200";
	public string numberOfRunsString = "1";

	bool _quitOnEnd;
	public bool _writeResultsToDisk;

	void Start(){
		string[] cmdLineArgs = System.Environment.GetCommandLineArgs ();
		//string[] cmdLineArgs = "./md_aar -batchmode -logFile -simulation -levelRange 1 5 -controller MCTS -utility E -writeResults -quit -turns 100 -runs 1 -time 100 -aar /Users/holmgard/Desktop/2015-07-10_AIIDE_PM/actionsForAAR/SimControllerHeroMCTS_Exit_map1_to_1_map1_actions_run1.txt".Split(' ');
		//string[] cmdLineArgs = "./md_aar -batchmode -logFile -simulation -levelRange 1 10 -controller MCTS -utility TC -aar /Users/holmgard/Desktop/2015-07-10_AIIDE_PM/experimentResults_60s/2015-07-12_01-36-58_SimControllerHeroMCTS_TreasureCollector_map5_to_5_map5_actions_run1.txt -writeResults -quit -turns 100 -runs 20 -time 100".Split(' ');
		if(cmdLineArgs.Length > 0){
			ParseCommandLine(cmdLineArgs);
		}
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

	public string GenerateAARFromActionTrace(string firstActionsFileName, int numberOfRuns, int firstLevel, int lastLevel, ControllerTypes controllerType, int msPerMove){
		string result = "";
		result += "originalController,originalUtilityfunction,comparingUtilityfunction,map,originalRun,innerRun,msPerMove,AAR";

		//for(int inputRun = inputRunStart; inputRun < inputRunEnd+1; inputRun++){
			
			string fileNameChunk = firstActionsFileName.Substring(firstActionsFileName.LastIndexOf("/")+1);
			string pathPart = firstActionsFileName.Replace(fileNameChunk, "");

			string[] actionsFileNameParts = fileNameChunk.Split('_');

			string controllerPart = actionsFileNameParts[0];
			string utilityPart = actionsFileNameParts[1];
			string firstMapPart = actionsFileNameParts[2];
			int firstMap = int.Parse(firstMapPart.Replace("map", ""));
			string stupidToPart = actionsFileNameParts[3];
			string lastMapPart = actionsFileNameParts[4];
			int lastMap = int.Parse(lastMapPart.Replace("map", ""));
			string actualMapPart = actionsFileNameParts[5];
			string fileTypePart = actionsFileNameParts[6];
			string runNumberPart = actionsFileNameParts[7];
			Debug.Log("Run number part: " + runNumberPart);
			string runNumberFixed = runNumberPart.Replace("run", "").Replace(".txt", "");
			Debug.Log("Run number fixed: " + runNumberFixed);
			int firstRun = int.Parse(runNumberFixed);
			int lastRun = firstRun + numberOfRuns-1;

			for(int run = firstRun; run < lastRun+1; run++){
				for(int map = firstLevel; map < lastLevel+1; map++)
				{
					string runFileName = controllerPart;
					runFileName += "_";
					runFileName += utilityPart;
					runFileName += "_map";
					runFileName += map;
					runFileName += "_";
					runFileName += stupidToPart;
					runFileName += "_";
					runFileName += map;
					runFileName += "_map";
					runFileName += map;
					runFileName += "_";
					runFileName += fileTypePart;
					runFileName += "_";
					runFileName += "run";
					runFileName += run;
					runFileName += ".txt";

					string actionString = File.ReadAllText(pathPart + runFileName);
					List<SimHeroAction> actionTrace = ActionTraceUtils.ReadActionString(actionString);

					Debug.Log("Actiontrace:" + actionTrace);
					string mapPath = "AsciiMaps/Map" + map;
					string mapString = Resources.Load<TextAsset>(mapPath).text;
					
					for(int innerRun = 0; innerRun < 20; innerRun++){
						var simLevel = new SimLevel(mapString);
						Debug.Log("SimLevel:" + simLevel);

						var controllerDecisions = new List<SimHeroAction>();
						SimControllerHero controllerHero = GetController(controllerType);
						SimControllerHeroMCTS mctsController = (SimControllerHeroMCTS) controllerHero;
						SimUtilityCalculator suc = new SimUtilityCalculator ();
						mctsController.UtilityFunction = suc.GetUtilityFunction(_utilityFunction);

						Debug.Log("Controllerhero: " + controllerHero);
						foreach(SimHeroAction action in actionTrace){
							var controllerAction = controllerHero.NextAction(simLevel.Clone(), msPerMove);
							controllerDecisions.Add(controllerAction);
							simLevel.RunTurn(action);
						}

						int hits = 0;
						for(int i = 0; i < actionTrace.Count; i++){
							if(actionTrace[i].Equals(controllerDecisions[i])){hits++;}
						}

						float aar = (float)hits/(float)actionTrace.Count;
						
						result += "\n";
						result += controllerPart;
						result += ",";
						result += utilityPart;
						result += ",";
						result += _utilityFunction;
						result += ",";
						result += map;
						result += ",";
						result += run;
						result += ",";
						result += innerRun;
						result += ",";
						result += msPerMove;
						result += ",";
						result += aar;
					}
				}		
			}
		//}
		return result;
	}

	void GenerateAARFromOtherController(){

	}

	void RunController(int startLevel, int endLevel, UtilityFunctions utilityFunction, int msPerMove, int maxTurns, int numberOfRuns){
		_utilityFunction = utilityFunction;

		var playReports = new List<MapReport>();
		var mapNumbers = new List<int>();

		for(int run = 0; run < numberOfRuns; run++){
			if(!_runningController){
				_runningController = true;
				
				
				Debug.Log("Running controller: " + _controllerType + " on map" + startLevel + " to " + endLevel);
				Debug.Log(MapReport.MapReportHeader);
				for(int i = startLevel; i < endLevel+1; i++){
					string mapPath = "AsciiMaps/Map" + i;
					_levelString = Resources.Load<TextAsset>(mapPath).text;

					_controllerHero = GetController(_controllerType);

					MapReport playReport = null;
					playReport = _controllerHero.PlayLevel(_levelString, _utilityFunction, msPerMove, maxTurns);
					playReport.Map = i;
					playReport.Run = run+1;

					if(playReport != null){
						Debug.Log(playReport.LevelReport);
						playReports.Add(playReport);
						mapNumbers.Add(i);
					}
				}
				_runningController = false;
			}
		}

		if(_writeResultsToDisk){
			DateTime now = DateTime.Now;
			string dateTimeFormat = "yyyy-MM-dd_HH-mm-ss";
			string fileNamePrefix = Application.dataPath + "/experimentResults/" + now.ToString(dateTimeFormat) + "_" + _controllerHero.GetType().Name + "_" + _utilityFunction + "_map" + startLevel + "_to_" + endLevel + "_";

			string levelReportOut = MapReport.MapReportHeader + ",Map,Max_ms_per_Move,Max_Turns_per_Move,Run_Number" + "\n";
			int mapCount = 0;
			foreach(MapReport report in playReports){
				
				levelReportOut += report.LevelReport +  ",map" + report.Map + "," + msPerMove + "," + maxTurns + "," + report.Run + "\n";
				
				string actions = "Move_Type,X,Y\n";
				foreach(SimHeroAction action in report.ActionReport){
					actions += action.ToString();
					actions += "\n";
				}
				File.WriteAllText(fileNamePrefix + "map" + report.Map + "_actions_run" + report.Run + ".txt", actions);
				
				string states = "";
				foreach(string state in report.StateReport){
					states += state;
					states += "\n";
				}
				
				File.WriteAllText(fileNamePrefix + "map" + report.Map + "_states_run" + report.Run + ".txt", states);
				mapCount++;
			}
			File.WriteAllText(fileNamePrefix + "_levelReports.txt", levelReportOut);
		}

		if(_quitOnEnd){
			Application.Quit();
		}
	}

	void OnGUI(){
		startString = GUI.TextField(new Rect(10f,50f,100f,40f), startString);
		GUI.Label(new Rect(110f,50f,100f,40f), "Start level");
		endString = GUI.TextField(new Rect(10f,90f,100f,40f), endString);
		GUI.Label(new Rect(110f,90f,100f,40f), "End level");
		moveTimeLimitString = GUI.TextField(new Rect(10f,130f,100f,40f), moveTimeLimitString);
		GUI.Label(new Rect(110f,130f,100f,40f), "Time limit per move in ms");
		maxTurnsString = GUI.TextField(new Rect(10f,170f,100f,40f), maxTurnsString);
		GUI.Label(new Rect(110f,170f,100f,40f), "Max moves allowed");
		numberOfRunsString = GUI.TextField(new Rect(10f,210f,100f,40f), numberOfRunsString);
		GUI.Label(new Rect(110f,210f,100f,40f), "Number of times to run this configuration");


		if(GUI.Button(new Rect(10f,10f,500f,40f), "Run " + _controllerType + " with " + _utilityFunction + " utility function.")){
			int startLevel = int.Parse(startString);
			int endLevel = int.Parse(endString);
			int moveTimeLimit = int.Parse(moveTimeLimitString);
			int maxTurnsPerLevel = int.Parse(maxTurnsString);
			int numberOfRuns = int.Parse(numberOfRunsString);
			RunController(startLevel, endLevel, _utilityFunction, moveTimeLimit, maxTurnsPerLevel, numberOfRuns);
		}
	}

	void ParseCommandLine(string[] cmdLineArgs){

		int firstLevel = 1;
		int lastLevel = 1;
		int moveTimeLimit = 1000;
		int maxTurns = 200;
		int numberOfRuns = 1;

		for(int i = 0; i < cmdLineArgs.Length; i++){
			if(cmdLineArgs[i].Equals("-simulation")){
				_simulation = true;
				Debug.Log("Setting MiniDungeons to Simulation mode...");
			}
			if(cmdLineArgs[i].Equals("-levelRange")){
				firstLevel = int.Parse(cmdLineArgs[i+1]);
				lastLevel = int.Parse(cmdLineArgs[i+2]);
			}
			if(cmdLineArgs[i].Equals("-writeResults")){
				_writeResultsToDisk = true;
			}
			if(cmdLineArgs[i].Equals("-quit")){
				_quitOnEnd = true;
			}
			if(cmdLineArgs[i].Equals("-time")){
				moveTimeLimit = int.Parse(cmdLineArgs[i+1]);
			}
			if(cmdLineArgs[i].Equals("-turns")){
				maxTurns = int.Parse(cmdLineArgs[i+1]);
			}
			if(cmdLineArgs[i].Equals("-moves")){
				maxTurns = int.Parse(cmdLineArgs[i+1]);
			}
			if(cmdLineArgs[i].Equals("-runs")){
				numberOfRuns = int.Parse(cmdLineArgs[i+1]);
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
				if (utilityType.Equals ("C")) {
					_utilityFunction = UtilityFunctions.Completionist;
				}
				if(utilityType.Equals("L")){
					_utilityFunction = UtilityFunctions.Loiterer;
				}
				if(utilityType.Equals("MK")){
					_utilityFunction = UtilityFunctions.MonsterKiller;
				}
				if(utilityType.Equals("TC")){
					_utilityFunction = UtilityFunctions.TreasureCollector;
				}
			}

			if(cmdLineArgs[i].Equals("-aar")){
				_aarMode = true;
				_aarFile = cmdLineArgs[i+1];
			}
		}
		if(_simulation && _aarMode){
			string aarResult = GenerateAARFromActionTrace(_aarFile, numberOfRuns, firstLevel, lastLevel, _controllerType, moveTimeLimit);
			DateTime now = DateTime.Now;
			string dateTimeFormat = "yyyy-MM-dd_HH-mm-ss";
			string fileName = Application.dataPath + "/experimentResults/" + "AAR_" + now.ToString(dateTimeFormat) + "_" + _controllerType + "_" + moveTimeLimit + "ms.txt";
			File.WriteAllText(fileName, aarResult);
		} else if(_simulation){
			RunController(firstLevel, lastLevel, _utilityFunction, moveTimeLimit, maxTurns, numberOfRuns);
		}
	}
}
