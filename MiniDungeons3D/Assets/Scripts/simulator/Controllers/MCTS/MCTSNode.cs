using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using TreeLanguageEvolute;

public class MCTSNode {

	public SimHeroAction Action{get;set;}
	public List<SimHeroAction> PossibleActions{get;set;}

	public int Simulations {get; set;}
	public int Visits {get;set;}

	public int Depth { get; set; }

	public double reward = 0f;
	public double Xj = 0;

	public double UCTScore { get; set; }

	public double preSeed;

	public MCTSNode Parent {get;set;}
	public List<MCTSNode> Children {get;set;}

	public SimLevel Game {get; set;}

	public BaseProgram EvaluationFunction { get; set; }

	public MCTSNode(SimLevel game, BaseProgram evaluationFunction){
		Children = new List<MCTSNode> ();
		PossibleActions = game.GetPossibleHeroActions ();
		Action = new SimHeroAction (HeroActionTypes.None, new SimPoint (0, 0));
		EvaluationFunction = evaluationFunction;

		Game = game;
		Depth = 0;
	}

	public MCTSNode(MCTSNode parent, SimLevel game, SimHeroAction action, BaseProgram evaluationFunction){
		Children = new List<MCTSNode> ();
		Parent = parent;
		PossibleActions = game.GetPossibleHeroActions ();
		if (PossibleActions.Count < 0) {
			Console.WriteLine ("Possible actions was incorrect, should never be < 1");
		}
		Action = action;

		Game = game;

		PreSeed (SimControllerHeroMCTS.exitPathDatabase, game);
		EvaluationFunction = evaluationFunction;

		Depth = Parent.Depth + 1;
	}

	public void PreSeed(Dictionary<SimPoint,double> seedSource, SimLevel game){
		double seedReward;
		seedSource.TryGetValue(game.SimHero.Point, out seedReward);
		preSeed = seedReward;

		reward += seedReward * (0.5f);
	}

	public void AddChild(MCTSNode node){
		Children.Add(node);
	}

	public void RemoveChild(MCTSNode node){
		Children.Remove(node);
	}

	public bool NotFullyExpanded(){
		bool result = (Children.Count < PossibleActions.Count);
		return result;
	}

	public bool IsNonTerminalNode(SimLevel game){
		if (Parent == null) { //If we're the root node we're not terminal
			return true;
		}
		return (game.SimLevelState == SimLevelState.Playing);
	}

	public double GetUCTScore(double C){
		Xj = this.reward/(double)Visits;
		double nParent = (double)Parent.Visits;
		double njChild = (double)Visits;

		if (njChild <= 0) {
			return double.MaxValue;
		}

		double logN = System.Math.Log (nParent);
		double twoLogN = 2d * logN;
		double twoLogNOverNj = twoLogN / njChild;
		double squareRootOfAbove = Math.Sqrt (twoLogNOverNj);
		double CpTimesTheAbove = C * squareRootOfAbove;
		double uctScore = Xj + CpTimesTheAbove;
		UCTScore = uctScore;
//		UCTScore = 1;
//		uctScore = 1;
//		/*if(CpTimesTheAbove != 0)
//			Console.WriteLine (CpTimesTheAbove);*/

		// add function here
//		NormalizedUtilityValues utilityValues = SimUtilityCalculator.NormalizeUtilities (Game);
//		EvaluationFunction.Fitness = 0;
//		Hashtable hsVariables 		= EvaluationFunction.GetVariables ();
//		Variable stepsTaken 		= (Variable)hsVariables ["stepsTaken"];
//		Variable monstersSlain 		= (Variable)hsVariables ["monstersSlain"];
//		Variable potionsTaken 		= (Variable)hsVariables ["potionsTaken"];
//		Variable treasuresOpened 	= (Variable)hsVariables ["treasuresOpened"];
//		Variable minotaurKnockouts 	= (Variable)hsVariables ["minotaurKnockouts"];
//		Variable javelinThrows 		= (Variable)hsVariables ["javelinThrows"];
//		Variable teleportsUsed 		= (Variable)hsVariables ["teleportsUsed"];
//		Variable trapsSprung 		= (Variable)hsVariables ["trapsSprung"];
//		Variable distanceFromExit 	= (Variable)hsVariables ["distanceFromExit"];
//		Variable healthLeft 		= (Variable)hsVariables ["healthLeft"];
//
//		stepsTaken.Value 		= (float) utilityValues._stepsTaken;
//		monstersSlain.Value 	= (float) utilityValues._monsterKills;
//		potionsTaken.Value 		= (float) utilityValues._potionsDrunk;
//		treasuresOpened.Value 	= (float) utilityValues._treasuresOpened;
//		minotaurKnockouts.Value = (float) utilityValues._minitaurKnockouts;
//		javelinThrows.Value 	= (float) utilityValues._javelinThrows;
//		teleportsUsed.Value 	= (float) utilityValues._teleportsUsed;
//		trapsSprung.Value 		= (float) utilityValues._trapsSprung;
//		distanceFromExit.Value 	= (float) utilityValues._distanceToExit;
//		healthLeft.Value 		= (float) utilityValues._healthLeft;
//
//		EvaluationFunction.Run ();
//		double uctScore = (double) EvaluationFunction.Result;
//		UCTScore = uctScore;
		return uctScore;
	}

	public List<SimHeroAction> GetUntriedActions(){
		List<SimHeroAction> triedActions = new List<SimHeroAction>();
		for (int i = 0; i < Children.Count; i++) {
			triedActions.Add(Children[i].Action);
		}
		List<SimHeroAction> untriedActions = PossibleActions.Except(triedActions).ToList();
		return untriedActions;
	}

	public MCTSNode GetRootNode(){
		MCTSNode rootNode = this.Parent;
		while (rootNode.Parent != null) {
			rootNode = rootNode.Parent;
		}
		return rootNode;
	}

}
