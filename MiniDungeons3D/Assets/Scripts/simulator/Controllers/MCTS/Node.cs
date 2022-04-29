using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using TreeLanguageEvolute;

public class Node {

	public SimHeroAction Action{get;set;}
	public int Simulations {get;set;}
	public int Visits {get;set;}

	public int Depth {get; set;}
	public double Q{ get; set; }

	public double TotalValue {get;set;}
	public double MaxValue { get; set; }

	public SimUtilityCalculator Suc { get; set; }
	public Node Parent {get;set;}
	public Dictionary<SimHeroAction,Node> Children {get;set;}

	public SimLevel Game { get; set;}

	public BaseProgram EvaluationFunction {get;set;}

	public List<SimHeroAction> PossibleActions {get;set;}

	public Node(SimLevel game, BaseProgram evaluationFunction, SimUtilityCalculator suc) {
		Children = new Dictionary<SimHeroAction, Node> ();
		Action = new SimHeroAction (HeroActionTypes.None, new SimPoint(0,0));
		EvaluationFunction = evaluationFunction;
		PossibleActions = game.GetPossibleHeroActions ();
		Game = game;
		Suc = suc;
		Depth = 0;
		Q = 0.25;
		MaxValue = Int32.MinValue;
	}

	public Node(Node parent, SimLevel game, SimHeroAction action, BaseProgram evaluationFunction, SimUtilityCalculator suc) {
		Children = new Dictionary<SimHeroAction, Node> ();
		Parent = parent;
		Action = action;
		Game = game;
		EvaluationFunction = evaluationFunction;
		PossibleActions = game.GetPossibleHeroActions ();
		Suc = suc;
		Depth = Parent.Depth + 1;
		Q = 0.25;
		MaxValue = Int32.MinValue;
	}

	public void AddChild(SimHeroAction action, Node node) {
		Children.Add(action, node);
	}

	public void RemoveChild(SimHeroAction action) {
		Children.Remove(action);
	}

	public bool NotFullyExpanded() {
		foreach(SimHeroAction a in Game.GetPossibleHeroActions ()) {
			if(!Children.ContainsKey(a)) {
				return true;
			}
		}
		return false;
	}
	public SimHeroAction PickExpansion() {
		SimHeroAction b = new SimHeroAction ();
		foreach(SimHeroAction a in Game.GetPossibleHeroActions ()) {
			if(!Children.ContainsKey(a)) {
				return a;
			}
		}
		return b;
	}
	public bool IsNonTerminalNode() {
		if(Parent == null) {
			return true;
		}
		return (Game.SimLevelState == SimLevelState.Playing);
	}
	public bool IsWinningNode() {
		return (Game.SimLevelState == SimLevelState.Won);
	}

	public double GetUCTScore(double C) {
		if (Parent == null) {
			return Int32.MinValue;
		}
		double n = (double) Parent.Visits;
		double nj = (double) Visits;
		double reward = (1 - Q) * (TotalValue / nj) + Q * MaxValue;
		if(nj <= 0) {
			return double.MaxValue;
		}

		//double step1 = 2*System.Math.Log(n);
		//double step2 = step1 / nj;
		//double step3 = C * Math.Sqrt(step2);
		//double uctScore = reward + step3;

		//return uctScore;
		
		NormalizedUtilityValues utilityValues = Suc.NormalizeUtilities (Game);
		if (EvaluationFunction != null)
		{
			EvaluationFunction.Fitness = 0;
			Hashtable hsVariables = EvaluationFunction.GetVariables();
			Variable stepsTaken = (Variable)hsVariables["stepsTaken"];
			Variable monstersSlain = (Variable)hsVariables["monstersSlain"];
			Variable potionsTaken = (Variable)hsVariables["potionsTaken"];
			Variable treasuresOpened = (Variable)hsVariables["treasuresOpened"];
			Variable minotaurKnockouts = (Variable)hsVariables["minotaurKnockouts"];
			Variable javelinThrows = (Variable)hsVariables["javelinThrows"];
			Variable teleportsUsed = (Variable)hsVariables["teleportsUsed"];
			Variable trapsSprung = (Variable)hsVariables["trapsSprung"];
			Variable distanceFromExit = (Variable)hsVariables["distanceFromExit"];
			Variable healthLeft = (Variable)hsVariables["healthLeft"];
			Variable rReward = (Variable)hsVariables["rReward"];
			Variable interactableRatio = (Variable)hsVariables["interactableRatio"];

			stepsTaken.Value = (float)utilityValues._stepsTaken;
			monstersSlain.Value = (float)utilityValues._monsterKills;
			potionsTaken.Value = (float)utilityValues._potionsDrunk;
			treasuresOpened.Value = (float)utilityValues._treasuresOpened;
			minotaurKnockouts.Value = (float)utilityValues._minitaurKnockouts;
			javelinThrows.Value = (float)utilityValues._javelinThrows;
			teleportsUsed.Value = (float)utilityValues._teleportsUsed;
			trapsSprung.Value = (float)utilityValues._trapsSprung;
			distanceFromExit.Value = (float)utilityValues._distanceToExit;
			healthLeft.Value = (float)utilityValues._healthLeft;
			interactableRatio.Value = (float)utilityValues._interactablesRatio;
			rReward.Value = (float)reward;
			EvaluationFunction.Run();
			double weight = (double)EvaluationFunction.Result;

			// TODO add in reward
			//double step3 = weight * Math.Sqrt(step2);

			return weight;
		} else
        {
			return reward;
        }
	}
		

	public Node GetRootNode() {
		Node root = this;
		while(root.Parent != null) {
			root = root.Parent;
		}
		return root;
	}
}
