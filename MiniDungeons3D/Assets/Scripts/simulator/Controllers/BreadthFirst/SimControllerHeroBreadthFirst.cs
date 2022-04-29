using System;
using System.Collections.Generic;
using System.Linq;

public class SimControllerHeroBreadthFirst : SimControllerHero
{
    public int _depth = 4;
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
            SimHeroAction nextAction = NextAction(level, msPerMove);
            actions.Add(nextAction);
            level.RunTurn(nextAction);
            states.Add(level.ToAsciiMap());
            counter++;
        }

        stopwatch.Stop();
        float timeTaken = (float)stopwatch.ElapsedMilliseconds;
        //float turns = (float)counter;
        //float timePerTurn = timeTaken / turns;

        playReport.LevelReport = LevelReport(this.GetType().Name, _utilityFunction, _utilityFunction(level), timeTaken, counter, level);
        playReport.ActionReport = actions;
        playReport.StateReport = states;
		playReport.Positions = positions;
        playReport.Mechanics = level.Logger.Mechanics;
        return playReport;
    }

    public override SimHeroAction NextAction(SimLevel level, int msPerMove){
        SimHeroAction result = new SimHeroAction(HeroActionTypes.Move, SimPoint.Zero);

        SimLevel rootState = level.Clone();

        BreadthFirstTree tree = new BreadthFirstTree(_depth, rootState, _utilityFunction);
        tree.BuildTree(msPerMove);
        BreadthFirstNode bestNode = tree.GetBestNode();
        result = bestNode.Action;
        return result;
    }
}