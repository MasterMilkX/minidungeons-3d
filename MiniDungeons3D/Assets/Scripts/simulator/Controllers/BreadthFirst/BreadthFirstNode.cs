using System;
using System.Collections.Generic;

public class BreadthFirstNode {

	public BreadthFirstNode Parent{get;set;}
	public List<BreadthFirstNode> Children {get;set;}

	public SimLevel State{get;set;}
	public SimHeroAction Action{get;set;}
	public List<SimHeroAction> PossibleActions{get;set;}
	
	readonly Func<SimLevel,double> _utilityFunction;
	public double Utility{get;set;}

	public BreadthFirstNode(BreadthFirstNode parent, SimHeroAction action, Func<SimLevel, double> utilityFunction){
		Children = new List<BreadthFirstNode>();
		Parent = parent;
		Action = action;
		State = parent.State.Clone();
		State.RunTurn(Action);
		_utilityFunction = utilityFunction;
		Utility = utilityFunction(State);
		PossibleActions = State.GetPossibleHeroActions();
	}

	public BreadthFirstNode(SimLevel rootState, Func<SimLevel, double> utilityFunction){
		Children = new List<BreadthFirstNode>();
		Parent = null;
		State = rootState.Clone();
		_utilityFunction = utilityFunction;
		PossibleActions = State.GetPossibleHeroActions();
	}

	public void AddChild(BreadthFirstNode node){
		Children.Add(node);
	}

	public void RemoveChild(BreadthFirstNode node){
		Children.Remove(node);
	}

	public void Expand(){
		for(int action = 0; action < PossibleActions.Count; action++){
			var child = new BreadthFirstNode(this, PossibleActions[action], _utilityFunction);
			AddChild(child);
		}
	}

	public bool IsTerminal(){
		return State.SimLevelState != SimLevelState.Playing;
	}

	public int GetDepth(){
		BreadthFirstNode node = this;
		int depth = 0;
		while(node.Parent != null){
			depth++;
			node = node.Parent;
		}
		return depth;
	}
}
