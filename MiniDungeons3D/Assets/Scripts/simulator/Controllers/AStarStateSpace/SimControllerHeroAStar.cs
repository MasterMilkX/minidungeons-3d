using System;
using System.Collections.Generic;
using System.Linq;

public class SimControllerHeroAStar : SimControllerHero
{
    public static bool Flawed = false;

    public double flawedChance = 0.25;
    public double treeSize = 500;
    public int _depth = 4;
    public bool _hasInput;

    public bool _foundOptimalPath;

	Func<SimLevel,double> _utilityFunction;
	Func<SimLevel,double> _aStarHeuristic;
	Func<SimLevel,double> _aStarCost;

	Func<SimLevel,double> GetHeuristicFunction(UtilityFunctions utilityFunction){
		Func<SimLevel,double> result = null;
        switch(utilityFunction){
            case UtilityFunctions.Exit:
            result = AStarHeuristics.ExitHeuristic;
            break;
            case UtilityFunctions.Runner:
            result = AStarHeuristics.RunnerHeuristic;
            break;
			case UtilityFunctions.Completionist:
            result = AStarHeuristics.CompletionistHeuristic;
            break;
			case UtilityFunctions.Loiterer:
			result = AStarHeuristics.SurvivalistHeuristic;
			break;
            case UtilityFunctions.MonsterKiller:
            result = AStarHeuristics.MonsterKillerHeuristic;
            break;
            case UtilityFunctions.TreasureCollector:
            result = AStarHeuristics.TreasureHeuristic;
            break;
        }
        return result;
    }

	Func<SimLevel,double> GetCostFunction(UtilityFunctions utilityFunction){
		Func<SimLevel,double> result = null;
        switch(utilityFunction){
            case UtilityFunctions.Exit:
            result = AStarCostFunctions.ExitCost;
            break;
            case UtilityFunctions.Runner:
            result = AStarCostFunctions.RunnerCost;
            break;
			case UtilityFunctions.Loiterer:
            result = AStarCostFunctions.SurvivalistCost;
            break;
			case UtilityFunctions.Completionist:
			result = AStarCostFunctions.CompletionistCost;
			break;
            case UtilityFunctions.MonsterKiller:
            result = AStarCostFunctions.MonsterCost;
            break;
            case UtilityFunctions.TreasureCollector:
            result = AStarCostFunctions.TreasureCost;
            break;
        }
        return result;
    }
    public SimHeroAction flawedCheck(SimHeroAction nextAction, SimLevel level, Random random)
    {
        if (SimControllerHeroAStar.Flawed)
        {
            double chance = random.NextDouble();
            if (chance < flawedChance)
            {
                List<SimHeroAction> possibleActions = level.GetPossibleHeroActions();
                if (possibleActions.Count > 1)
                    possibleActions.Remove(nextAction);
                int index = random.Next(possibleActions.Count);
                return possibleActions[index];
            }
        }

        return nextAction;
    }

    public override MapReport PlayLevel(string levelString, UtilityFunctions utilityFunction, int msPerMove, int maxTurns){
		SimUtilityCalculator suc = new SimUtilityCalculator ();
		_utilityFunction = suc.GetUtilityFunction(utilityFunction);
        _aStarHeuristic = GetHeuristicFunction(utilityFunction);
        _aStarCost = GetCostFunction(utilityFunction);

        var level = new SimLevel(levelString);

        var states = new List<string>();
        var actions = new List<SimHeroAction>();
		var positions = new List<SimPoint> ();
        Random random = new Random();

        states.Add(level.ToAsciiMap());

        int counter = 0;

        var stopwatch = new System.Diagnostics.Stopwatch();
        var playReport = new MapReport();
        playReport.startLocation = level.SimHero.Point;
        stopwatch.Start();

        while(level.SimLevelState == SimLevelState.Playing && counter < maxTurns){
			positions.Add (level.SimHero.Point);
			SimHeroAction nextAction = NextAction(level, msPerMove);
            nextAction = flawedCheck(nextAction, level, random);
            actions.Add(nextAction);
            level.RunTurn(nextAction);
            states.Add(nextAction + "\n" +  level.ToAsciiMap());
            //Console.WriteLine(SimSentientSketchbookInterface.GenerateHeatmap(levelString, positions));

            counter++;
        }

        stopwatch.Stop();
        float timeTaken = (float)stopwatch.ElapsedMilliseconds;
        //float turns = (float)counter;
        //float timePerTurn = timeTaken / turns;

        //Console.WriteLine(SimSentientSketchbookInterface.GenerateHeatmap(levelString, positions));
        playReport.LevelReport = LevelReport(GetType().Name, _utilityFunction, _utilityFunction(level), timeTaken, counter, level);
        playReport.ActionReport = actions;
        playReport.StateReport = states;
        playReport.Positions = positions;
        playReport.Mechanics = level.Logger.Mechanics;
        playReport.Frequencies = level.Logger.GetFrequencies();
        return playReport;
    }

    

    public override SimHeroAction NextAction(SimLevel level, int msPerMove){
        if(_aStarResults == null){
            _aStarResults = RunAStarSearch(level, msPerMove);
        }
        if(_aStarResults.Count <= 0){
            //Console.WriteLine("Error: No actions available, maybe A* failed?");
            //Console.WriteLine("Re-running A* from this position...");
            _aStarResults = RunAStarSearch(level, msPerMove);
        }
        if (_aStarResults.Count == 0)
        {
            Random random = new Random();
            int index = random.Next(level.GetPossibleHeroActions().Count);
            return level.GetPossibleHeroActions()[index];
        }
        else { 
            return _aStarResults.Pop();
        }
    }

    public SimHeroAction NextAction(SimLevel level, int msPerMove, UtilityFunctions utilityFunction)
    {
        SimUtilityCalculator suc = new SimUtilityCalculator();
        _utilityFunction = suc.GetUtilityFunction(utilityFunction);
        _aStarHeuristic = GetHeuristicFunction(utilityFunction);
        _aStarCost = GetCostFunction(utilityFunction);
        if (_aStarResults == null)
        {
            _aStarResults = RunAStarSearch(level, msPerMove);
        }
        if (_aStarResults.Count <= 0)
        {
            //Console.WriteLine("Error: No actions available, maybe A* failed?");
            //Console.WriteLine("Re-running A* from this position...");
            _aStarResults = RunAStarSearch(level, msPerMove);
        }
        if (_aStarResults.Count == 0)
        {
            Random random = new Random();
            int index = random.Next(level.GetPossibleHeroActions().Count);
            return level.GetPossibleHeroActions()[index];
        }
        else
        {
            return _aStarResults.Pop();
        }
    }

    public Stack<SimHeroAction> _aStarResults;
    public Stack<SimHeroAction> RunAStarSearch(SimLevel level, int msPerMove){
        
	//	int largestDepth = 0;

        var result = new Stack<SimHeroAction>();

        SimLevel rootState = level.Clone();
        var rootNode = new AStarNode(rootState, _utilityFunction,_aStarHeuristic,_aStarCost);

        var openList = new List<AStarNode>();
        var closedList = new List<AStarNode>();

        openList.Add(rootNode);

        AStarNode foundNode = null;
        AStarNode currentNode = null;
        
        int searchCount = 0;

        var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        float msElapsed = 0;
        // while(openList.Count > 0 && msElapsed < (float)msPerMove){
        int treeSize = 0;
        while (openList.Count > 0 && treeSize < this.treeSize) {
            treeSize += 1;
            openList = openList.OrderBy(x => x.FScore).ToList();


            currentNode = openList[0];
            currentNode.Expand();    
            
            for(int i = 0; i < currentNode.Children.Count; i++){
                bool discardNode = false;
                for(int j = 0; j < openList.Count; j++){
                    if(currentNode.Children[i].State.Equals(openList[j].State)){
                        if(openList[j].FScore < currentNode.Children[i].FScore){
                            discardNode = true;
                        } else {
                            openList.Remove(openList[j]);
                        }
                    }
                }
                for(int k = 0; k < closedList.Count; k++){
                    if(currentNode.Children[i].State.Equals(closedList[k].State)){
                        if(closedList[k].FScore < currentNode.Children[i].FScore){
                            discardNode = true;
                        } else {
                            closedList.Remove(closedList[k]);
                        }
                    }
                }
                if(!discardNode){
                    openList.Add(currentNode.Children[i]);
                }
            }
            

            //openList.AddRange(currentNode.Children);
            
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            searchCount++;
            msElapsed = stopWatch.ElapsedMilliseconds;
        }
        //		Console.WriteLine ("Largest Depth: " + largestDepth);
        //if (searchCount >= searchBudget)
        //{
        //    Console.WriteLine("Search budget expended.");
        //}

        closedList = closedList.OrderBy(x => x.FScore).ToList();
        
        for(int i = 0; i < closedList.Count; i++){
            if(closedList[i].State.SimLevelState == SimLevelState.Won){
                foundNode = closedList[i];
                _foundOptimalPath = true;
                break;
            }
        }

        if(foundNode == null){
            //Console.WriteLine("Error: A* search failed");
            //Console.WriteLine("Falling back on best bet instead... hope for the best...");
            foundNode = closedList[0];
            //Console.WriteLine("Best open option: " + foundNode.GScore + ", " + foundNode.Utility);
            if (foundNode == null){
                openList = openList.OrderBy(x => x.FScore).ToList();
                foundNode = openList[0];
                
            }
        }

        
        if(_foundOptimalPath){
            //Console.WriteLine("A* search succeeded");
            while (foundNode.Parent != null){
                result.Push(foundNode.Action);
                foundNode = foundNode.Parent;            
            }        
        } else {
            while(foundNode.Parent != null){
                foundNode = foundNode.Parent;            
            }
			double bestFScore = double.MaxValue;
            int bestIndex = -1;
            for(int i = 0; i < foundNode.Children.Count; i++){
                if(foundNode.Children[i].FScore < bestFScore){
                    bestFScore = foundNode.Children[i].FScore;
                    bestIndex = i;
                }
            }
            if (bestIndex == -1)
            {
                bestIndex = 0;
            }
            result.Push(foundNode.Children[bestIndex].Action);
        }
        return result;
    }
}