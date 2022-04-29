using System;
using System.Collections.Generic;
using System.Linq;

public class SimControllerHeroGreedy : SimControllerHero
{
    public bool _hasInput;

	Func<SimLevel,double> _utilityFunction;

    public override MapReport PlayLevel(string levelString, UtilityFunctions utilityFunction, int msPerMove, int maxTurns){
		SimUtilityCalculator suc = new SimUtilityCalculator ();
		_utilityFunction = suc.GetUtilityFunction(utilityFunction);
        var level = new SimLevel(levelString);
        
        var states = new List<string>();
        var actions = new List<SimHeroAction>();
		var positions = new List<SimPoint> ();

        states.Add(level.ToAsciiMap());

        int counter = 0;

        var stopwatch = new System.Diagnostics.Stopwatch();
        var playReport = new MapReport();
        playReport.startLocation = level.SimHero.Point;
        stopwatch.Start();

        while(level.SimLevelState == SimLevelState.Playing){
			positions.Add (level.SimHero.Point);
            SimHeroAction nextAction = NextAction(level);
            actions.Add(nextAction);
            level.RunTurn(nextAction);
            states.Add(nextAction + "\n" + level.ToAsciiMap());
			Console.WriteLine (SimSentientSketchbookInterface.GenerateHeatmap (levelString, positions));

            counter++;
        }

        stopwatch.Stop();
        float timeTaken = (float)stopwatch.ElapsedMilliseconds;
        //float turns = (float)counter;
        //float timePerTurn = timeTaken / turns;

        playReport.LevelReport = LevelReport(GetType().Name, _utilityFunction, _utilityFunction(level), timeTaken, counter, level);
        playReport.ActionReport = actions;
        playReport.StateReport = states;
		playReport.Positions = positions;
        playReport.Mechanics = level.Logger.Mechanics;
        return playReport;
    }

    public override SimHeroAction NextAction(SimLevel level, int msPerMove){
        return NextAction(level);
    }

    public SimHeroAction NextAction(SimLevel level){
        var result = new SimHeroAction(HeroActionTypes.Move, SimPoint.Zero);

        SimLevel rootState = level.Clone();

        List<SimHeroAction> possibleActions = rootState.GetPossibleHeroActions();
        var oneStepFutures = new SimLevel[possibleActions.Count];

		var futureUtilities = new double[possibleActions.Count];
		double maxFutureUtility = double.MinValue;
        int bestUtilityIndex = -1;

        for(int future = 0; future < oneStepFutures.Length; future++){
            oneStepFutures[future] = rootState.Clone();
            oneStepFutures[future].RunTurn(possibleActions[future]);
            futureUtilities[future] = _utilityFunction(oneStepFutures[future]);
            if(futureUtilities[future] > maxFutureUtility){
                maxFutureUtility = futureUtilities[future];
                bestUtilityIndex = future;
            }
        }        
        result = possibleActions[bestUtilityIndex];

		return result;
    }
}