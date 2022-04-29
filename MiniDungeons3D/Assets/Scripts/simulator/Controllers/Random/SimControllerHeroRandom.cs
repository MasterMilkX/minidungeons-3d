using System;
using System.Collections.Generic;
using System.Linq;

public class SimControllerHeroRandom : SimControllerHero
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


    Random rnd = new Random();
    
    public override SimHeroAction NextAction(SimLevel level, int msPerMove){
        return NextAction(level);
    }

    public SimHeroAction NextAction(SimLevel level){
        var result = new SimHeroAction(HeroActionTypes.Move, SimPoint.Zero);
        
        SimLevel rootState = level.Clone();
        List<SimHeroAction> possibleActions = rootState.GetPossibleHeroActions();
        
        int choice = rnd.Next(0, possibleActions.Count);
        result = possibleActions[choice];
        SimPoint nextPoint = rootState.SimHero.Point + possibleActions[choice].DirectionOrTarget;

        while(nextPoint == rootState.SimHero.Point){
            choice = rnd.Next(0, possibleActions.Count);
            nextPoint = rootState.SimHero.Point + possibleActions[choice].DirectionOrTarget;
            result = possibleActions[choice];
        }

        return result;
    }
}