using System;
using System.Collections.Generic;

public class AStarNode{

	//Total cost to reach this node
	public double GScore{get;set;}
	//Heuristic estimate of cost to get to goal state
	public double HScore{get;set;}
	//Sum of g and h = cost of this node
	public double FScore{get;set;}

	public AStarNode Parent{get;set;}
	public List<AStarNode> Children {get;set;}

	public SimLevel State{get;set;}
	public SimHeroAction Action{get;set;}
	public List<SimHeroAction> PossibleActions{get;set;}
	
	readonly Func<SimLevel,double> _utilityFunction;
	readonly Func<SimLevel,double> _aStarHeuristic;
	readonly Func<SimLevel,double> _aStarCost;
	public double Utility{get;set;}

	public AStarNode(AStarNode parent, SimHeroAction action, Func<SimLevel, double> utilityFunction, Func<SimLevel, double> aStarHeuristic, Func<SimLevel,double> aStarCost){
		Children = new List<AStarNode>();
		Parent = parent;
		Action = action;
		State = parent.State.Clone();
		State.RunTurn(Action);
		_utilityFunction = utilityFunction;
		_aStarHeuristic = aStarHeuristic;
		_aStarCost = aStarCost;

		Utility = utilityFunction(State);
		HScore = _aStarHeuristic(State);
		GScore = _aStarCost(State);
		FScore = HScore + GScore;

		PossibleActions = State.GetPossibleHeroActions();
	}

	public AStarNode(SimLevel rootState, Func<SimLevel, double> utilityFunction, Func<SimLevel, double> aStarHeuristic, Func<SimLevel,double> aStarCost){
		Children = new List<AStarNode>();
		Parent = null;
		State = rootState.Clone();
		_utilityFunction = utilityFunction;
		_aStarHeuristic = aStarHeuristic;
		_aStarCost = aStarCost;

		PossibleActions = State.GetPossibleHeroActions();

		Utility = utilityFunction(State);
		HScore = _aStarHeuristic(State);
		GScore = _aStarCost(State);
		FScore = HScore + GScore;
	}

	public void AddChild(AStarNode node){
		Children.Add(node);
	}

	public void RemoveChild(AStarNode node){
		Children.Remove(node);
	}

	public void Expand(){
		for(int action = 0; action < PossibleActions.Count; action++){
			var child = new AStarNode(this, PossibleActions[action], _utilityFunction, _aStarHeuristic, _aStarCost);
			AddChild(child);
		}
	}

	public bool IsTerminal(){
		return State.SimLevelState != SimLevelState.Playing;
	}

	public int GetDepth(){
		AStarNode node = this;
		int depth = 0;
		while(node.Parent != null){
			depth++;
			node = node.Parent;
		}
		return depth;
	}
}