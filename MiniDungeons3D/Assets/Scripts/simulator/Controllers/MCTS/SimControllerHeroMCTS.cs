using System;
using System.Collections.Generic;
using System.Linq;
using TreeLanguageEvolute;
#if UNITY_EDITOR
using UnityEngine;
#endif
public class SimControllerHeroMCTS : SimControllerHero {

	public static double C = 1.0d;
    public static int rolloutCount = 20;
	// one minute tree search time
	public static int TREE_SEARCH_TIME = 120000;

//	public static int TREE_SEARCH_TIME = 100;
	public static Dictionary<SimPoint,double> exitPathDatabase;

	public Func<SimLevel,double> UtilityFunction { get; set; }

	public BaseProgram EvaluationFunction { get; set; }

	public SimControllerHeroMCTS() {

	}
	public SimControllerHeroMCTS (BaseProgram evaluationFunction) {
		EvaluationFunction = evaluationFunction;
	}

    private void InitExitPathDatabase(SimLevel level){
        //Receive level
        //Initialize dictionary
		exitPathDatabase = new Dictionary<SimPoint,double>();
        int maxLength = int.MinValue;
        int minLength = int.MaxValue;
        //For each point in the 2D array, calculate path length to exit
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        foreach(SimMapNode tile in level.BaseMap){
			if(tile.TileType == TileTypes.empty){
                int length = level.AStar(tile.Point,level.SimExit.Point).Length;
                if(length > maxLength) maxLength = length;
                if(length < minLength) minLength = length;
				if(exitPathDatabase.ContainsKey(tile.Point)) {
					exitPathDatabase.Add(tile.Point,(double)length);
				}
            }
        }
		Dictionary<SimPoint,double> tempDataBase = new Dictionary<SimPoint,double>();
		foreach(KeyValuePair<SimPoint,double> entry in exitPathDatabase){
			double length = entry.Value;
			double normalizedLength = 1d-((double)length - (double)minLength) / ((double)maxLength - (double)minLength);
            tempDataBase.Add(entry.Key, normalizedLength);
        }
        exitPathDatabase = tempDataBase;
        stopwatch.Stop();
        float timeTaken = (float)stopwatch.ElapsedMilliseconds;
    }

	// override function of PlayLevel, this time using an AI
    public override MapReport PlayLevel(string levelString, UtilityFunctions utilityFunction, int msPerMove, int maxTurns){
		SimUtilityCalculator suc = new SimUtilityCalculator ();
		UtilityFunction = suc.GetUtilityFunction(utilityFunction);

        var level = new SimLevel(levelString);
        var states = new List<string>();
        var mapStates = new List<string>();
        var actions = new List<SimHeroAction>();
		var positions = new List<SimPoint> ();

        states.Add(level.ToAsciiMap());

        int counter = 0;

        var stopwatch = new System.Diagnostics.Stopwatch();
        var playReport = new MapReport();
        playReport.startLocation = level.SimHero.Point;

		/** START MOVE DECISION MAKING PROCESS **/
         stopwatch.Start();

		// Init MCTS
		Tree tree = new Tree(level, C, UtilityFunction, EvaluationFunction, SimControllerHeroMCTS.rolloutCount);
		tree.MaxHeight = 500;
//		tree.MaxHeight = 60;
        Node final = tree.TreePolicy(TREE_SEARCH_TIME);
        SimHeroAction[] bestActions = tree.GetPath(final);

        int actionIndex = 0;
		while((level.SimLevelState == SimLevelState.Playing) && (bestActions.Length > 0) && (bestActions.Length > actionIndex) && (actionIndex < maxTurns)) {
            positions.Add(level.SimHero.Point);
            // take next actions
            SimHeroAction nextAction = bestActions[actionIndex];
            if (nextAction.ActionType == HeroActionTypes.Move) {
                level.LogMechanic(nextAction.GetMechanicVersion());
            }
            actionIndex += 1;
            actions.Add(nextAction);
            level.RunTurn(nextAction);
            states.Add(nextAction + "\n" + level.ToAsciiMap());
            mapStates.Add(level.ToAsciiMap());
		}
        while((actionIndex < maxTurns) && (level.SimLevelState == SimLevelState.Playing)) {
            SimHeroAction nextAction = new SimHeroAction(HeroActionTypes.None, new SimPoint(0, 0));
            level.LogMechanic(Mechanic.None);
            actions.Add(nextAction);
            level.RunTurn(nextAction);
            states.Add(nextAction + "\n" + level.ToAsciiMap());
            mapStates.Add(level.ToAsciiMap());
            actionIndex += 1;
        }
        if (level.SimLevelState == SimLevelState.Playing)
        {
            level.SimLevelState = SimLevelState.Timeout;
        }
        stopwatch.Stop();
		/** END MOVE DECISION MAKING PROCESS **/
    	// Console.WriteLine (SimSentientSketchbookInterface.GenerateHeatmap (levelString, positions));
        float timeTaken = (float)stopwatch.ElapsedMilliseconds;
        playReport.LevelReport = LevelReport(GetType().Name, UtilityFunction, UtilityFunction(level), timeTaken, counter, level);
        playReport.ActionReport = actions;
        playReport.StateReport = states;
		playReport.Positions = positions;
        playReport.Mechanics = level.Logger.Mechanics;
        playReport.Frequencies = level.Logger.GetFrequencies();
        return playReport;
    }


	public override SimHeroAction NextAction(SimLevel level, int msPerMove){
		return new SimHeroAction(HeroActionTypes.None, SimPoint.Zero);
	}

}
