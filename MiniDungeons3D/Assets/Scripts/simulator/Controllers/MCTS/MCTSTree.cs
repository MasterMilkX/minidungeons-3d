//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TreeLanguageEvolute;
//
//public class MCTSTree {
//
//	public Dictionary<SimPoint, double> exitPathDatabase;
//
//	#region Search
//
//	Random rnd;
//
//	public double C { get; set;}
//
//	public Func<SimLevel,double> UtilityFunction { get; set;}
//
//	public SimLevel rootGame;
//	public SimLevel gameClone;
//
//	public MCTSNode RootNode { get; set;}
//	public MCTSNode CurrentNode { get; set;}
//
//	public BaseProgram EvaluationFunction { get; set; }
//
//	int rollOutLimit = 10;
//	int treeSize = 500;
//	bool guidedRollout = true;
//
//	public List<SimHeroAction> actions;
//
//
//	public int LARGEST_DEPTH = 0;
//
//
//	public MCTSTree(SimLevel game, double C, Func<SimLevel,double> utilityFunction, BaseProgram evaluationFunction){
//		rnd = new Random ();
//		rootGame = game;
//		this.C = C;
//		UtilityFunction = utilityFunction;
//
//		exitPathDatabase = SimUtilityCalculator.exitPathDatabase;
//		EvaluationFunction = evaluationFunction;
//	}
//
//	public SimHeroAction Search(float timeLimitMs){
//		List<SimHeroAction> actions = new List<SimHeroAction> ();
//		this.actions = actions;
//		var stopwatch = new System.Diagnostics.Stopwatch();
//		stopwatch.Start();
//
//		//UCT Search start
//		RootNode = new MCTSNode(rootGame, EvaluationFunction);
//
//		CurrentNode = RootNode;
//		double delta = 0f;
//
//		int iterations = 0;
//		//while (stopwatch.ElapsedMilliseconds < timeLimitMs) {
//		while (iterations < treeSize
//			&& stopwatch.ElapsedMilliseconds < timeLimitMs) {
//
//			CurrentNode = TreePolicy (RootNode);
//
//
//			actions.Add (CurrentNode.Action);
//			delta = DefaultPolicy ();
//			Backup (CurrentNode, delta);
//			iterations++;
//		}
//
//		stopwatch.Stop();
//
//		MCTSNode bestChild = BestChild (RootNode, C);
//		if (bestChild.Action.ActionType == HeroActionTypes.Move) {
//			SimPoint newlocation = rootGame.SimHero.Point + bestChild.Action.DirectionOrTarget;
//			if (newlocation == rootGame.SimHero.PriorPoint) {
//				//Console.WriteLine ("argh blargh");
//			}
//		}
//		return bestChild.Action;
//	}
//
//	public void BuildTree(float timeLimitMs) {
//		List<SimHeroAction> actions = new List<SimHeroAction> ();
//		this.actions = actions;
//		var stopwatch = new System.Diagnostics.Stopwatch();
//		stopwatch.Start();
//
//		//UCT Search start
//		RootNode = new MCTSNode(rootGame, EvaluationFunction);
//
//		CurrentNode = RootNode;
//		double delta = 0f;
//
//		int iterations = 0;
//		//while (stopwatch.ElapsedMilliseconds < timeLimitMs) {
//		while (stopwatch.ElapsedMilliseconds < timeLimitMs) {
//
//			CurrentNode = TreePolicy (RootNode);
//			actions.Add (CurrentNode.Action);
//			delta = DefaultPolicy ();
//			Backup (CurrentNode, delta);
//			iterations++;
//		}
//		stopwatch.Stop();
//	}
//
//	// TODO new method searchTree, returns list of SimHeroAction
//	public List<SimHeroAction> SearchTree(float timeLimitMs) {
//		List<SimHeroAction> actions = new List<SimHeroAction> ();
//		this.actions = actions;
//		var stopwatch = new System.Diagnostics.Stopwatch();
//		stopwatch.Start();
//
//		//UCT Search start
//		RootNode = new MCTSNode(rootGame, EvaluationFunction);
//		CurrentNode = RootNode;
//		double delta = 0f;
//
//		int iterations = 0;
//		while (stopwatch.ElapsedMilliseconds < timeLimitMs) {
////		while(RootNode.Game.SimLevelState.Equals(SimLevelState.Playing)) {
//			CurrentNode = TreePolicy (RootNode);
//
//			// check depth for testing
//			if (CurrentNode.Depth > LARGEST_DEPTH) {
//				LARGEST_DEPTH = CurrentNode.Depth;
//			}
//
//			actions.Add (CurrentNode.Action);
//			delta = DefaultPolicy ();
//			Backup (CurrentNode, delta);
//			iterations++;
//		}
//		Console.WriteLine ("Iterations: " + iterations);
//
//		stopwatch.Stop();
//		// list of best actions to take
//		List<SimHeroAction> bestActions = new List<SimHeroAction> ();
//		MCTSNode bestChild = BestChild (RootNode, 0d);
//		bestActions.Add (bestChild.Action);
//		// iteratively go through every best child
//		while (bestChild.Children.Count != 0) {
//			bestActions.Add (bestChild.Action);
//			bestChild = BestChild (bestChild, 0d);
//		}
//		return bestActions;
//	}
//
//	public List<SimHeroAction> Test (float timeLimitMs) {
//		int counter = 0;
//		var stopwatch = new System.Diagnostics.Stopwatch();
//		stopwatch.Start();
//		while (stopwatch.ElapsedMilliseconds < timeLimitMs) {
//			CurrentNode = RootNode;
//			gameClone = rootGame.Clone ();
//
//			gameClone.RunTurn (new SimHeroAction(HeroActionTypes.None, SimPoint.Zero));
//			counter++;
//		}
//		Console.WriteLine (counter);
//		return null;
//	}
//
//	public MCTSNode TreePolicy(MCTSNode node){
//		MCTSNode v = node;
//		gameClone = rootGame.Clone ();
//
//		while (v.IsNonTerminalNode(gameClone)) {
//			if (v.NotFullyExpanded ()) {
//				return Expand (v);
//			} else {
//				v = BestChild (v, C);
//			}
//		}
//		return v;
//	}
//
//	public MCTSNode Expand(MCTSNode node){
//		MCTSNode v = node;
//		MCTSNode vPrime;
//
//		List<SimHeroAction> untriedActions = v.GetUntriedActions ();
//		List<SimHeroAction> untriedActionsShuffled = untriedActions.OrderBy (item => rnd.Next ()).ToList();
//		SimHeroAction a = untriedActionsShuffled [0];
//
//		FastforwardGameClone (CurrentNode, a);
//
//		vPrime = new MCTSNode (v, gameClone, a, EvaluationFunction);
//		v.AddChild (vPrime);
//		return vPrime;
//	}
//
//	public void FastforwardGameClone(MCTSNode v, SimHeroAction a){
//		//Compile a list of all the actions from this node and up.
//		//NOTE: this list is backwards, but we don't care, we just iterate downwards later.
//		List<SimHeroAction> seachedActions = new List<SimHeroAction> ();
//		MCTSNode backwardsNode = v;
//		while (backwardsNode.Parent != null) {
//			seachedActions.Add (backwardsNode.Action);
//			backwardsNode = backwardsNode.Parent;
//		}
//
//		//Grab a game clone and apply all the actions.
//		gameClone = rootGame.Clone ();
//		for (int i = seachedActions.Count - 1; i >= 0; i--) {
//			gameClone.RunTurn (seachedActions [i]);
//		}
//		gameClone.RunTurn (a);
//		/*if (gameClone.SimLevelState == SimLevelState.Lost) {
//			Console.WriteLine ("Level should not be losable in test mode");
//		}*/
//		//The gameClone is now is the state of this node plus the next action
//	}
//
//	public double DefaultPolicy(){
//		int rollOutCounter = 0;
//		while (gameClone.SimLevelState == SimLevelState.Playing && rollOutCounter < rollOutLimit) {
//			List<SimHeroAction> possibleActions = gameClone.GetPossibleHeroActions ();
//			if (guidedRollout) {
//				if (possibleActions.Count > 1) {
//					SimPoint forbiddenPointDelta = gameClone.SimHero.PriorPoint - gameClone.SimHero.Point;
//					List<SimHeroAction> prunedActions = new List<SimHeroAction>();
//					for (int i = 0; i < possibleActions.Count; i++) {
//						if (possibleActions[i].DirectionOrTarget != forbiddenPointDelta & possibleActions[i].DirectionOrTarget != (forbiddenPointDelta+forbiddenPointDelta))
//						{
//							prunedActions.Add(possibleActions[i]);
//						}
//						/*else
//						{
//							Console.WriteLine("Removed an action");
//						}*/
//					}
//					possibleActions = prunedActions;
//				}
//			}
//			gameClone.RunTurn(possibleActions[rnd.Next(0,possibleActions.Count)]);
//			rollOutCounter++;
//		}
//		double reward = UtilityFunction (gameClone);
//		//gameClone = null;
//		return reward;
//	}
//
//	public void Backup(MCTSNode vl, double delta){
//		MCTSNode backwardsNode = vl;
//		while (backwardsNode != null) {
//			backwardsNode.Visits = backwardsNode.Visits + 1;
//			backwardsNode.reward += delta;
//			backwardsNode = backwardsNode.Parent;
//		}
//	}
//
//	public MCTSNode BestChild(MCTSNode node, double C){
//		double bestUCT = double.NegativeInfinity;
//		int bestUCTIndex = -1;
//		for(int i = 0; i < node.Children.Count; i++){
//			double childUCTScore = node.Children [i].GetUCTScore (C);
//			if(childUCTScore > bestUCT){
//				bestUCT = node.Children[i].GetUCTScore(C);
//				bestUCTIndex = i;
//			}
//		}
//		if (bestUCTIndex < 0) {
//			Console.WriteLine ("Index error in FindBestChild");
//		}
//		return node.Children[bestUCTIndex];
//	}
//	#endregion
//
//	#region Diagnostics
//
//	#endregion
//}
