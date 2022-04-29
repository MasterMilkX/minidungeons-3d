using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeLanguageEvolute;

public class Tree {

    public double C{get;set;}
	public int MaxHeight{ get; set; }
    public Func<SimLevel,double> UtilityFunction {get;set;}
    public SimLevel rootGame;
    public SimLevel gameClone;

    public Node RootNode {get;set;}
    public Node CurrentNode {get;set;}
	public SimUtilityCalculator suc;
    public BaseProgram EvaluationFunction {get;set;}

    public int rolloutCount { get; set; }
	public int MAX_TREE_SIZE = 10000;
	public int MAX_DEPTH = 200;
    Random rnd;

    public Tree(SimLevel game, double C, Func<SimLevel, double> utilityFunction, BaseProgram evaluationFunction, int rolloutCount) {
        rnd = new Random();

        rootGame = game;
        this.C = C;
        this.rolloutCount = rolloutCount;
        UtilityFunction = utilityFunction;
		suc = new SimUtilityCalculator ();
        EvaluationFunction = evaluationFunction;
    }

    public Node TreePolicy(float timeLimitMs) {
        List<SimHeroAction> actions = new List<SimHeroAction> ();
        RootNode = new Node(rootGame, EvaluationFunction, suc);
        int iterations = 0;
        double delta = 0f;

        var stopwatch = new System.Diagnostics.Stopwatch ();
        stopwatch.Start ();
        CurrentNode = RootNode;
		Node bestNode = CurrentNode;
		while(!CurrentNode.IsWinningNode() && (MAX_TREE_SIZE > iterations)) {
            SimHeroAction a;
			iterations += 1;

			if (CurrentNode.Depth >= MaxHeight) {
				double r = Rollout(rolloutCount, CurrentNode);
				Backprop(CurrentNode, r);
				CurrentNode = RootNode;
			}
            else if( (CurrentNode.NotFullyExpanded()) != false) {
                a = CurrentNode.PickExpansion();
                CurrentNode = Expand(CurrentNode, a);
                double r = Rollout(rolloutCount, CurrentNode);
                Backprop(CurrentNode, r);
                CurrentNode = RootNode;
            } else {
                CurrentNode = Select(CurrentNode);
            }
			if (CurrentNode.GetUCTScore (C) > bestNode.GetUCTScore (C)) {
				bestNode = CurrentNode;
			}
        }

//		Console.WriteLine ("Tree Size: " + iterations);
		stopwatch.Stop ();
//		Console.WriteLine ("Tree Build Time: " + stopwatch.ElapsedMilliseconds + " ms");
		if (CurrentNode.IsNonTerminalNode ()) {
			return bestNode;
		}
		return CurrentNode;
    }
    public Node Select(Node node) {
        // go through all children, call UCT
        double maxUCT = Int32.MinValue;
        Node maxNode = null;
		double tmp;
        foreach(KeyValuePair<SimHeroAction,Node> entry in node.Children) {
            tmp = entry.Value.GetUCTScore(C);
            if(tmp > maxUCT) {
                maxUCT = tmp;
                maxNode = entry.Value;
            }
		}
		if (maxNode == null) {
			maxNode = node.Children.ElementAt(rnd.Next (node.Children.Count)).Value;
		}
        return maxNode;
        // pick child with highest UCT and return
    }

    public Node Expand(Node node, SimHeroAction a) {
        SimLevel gameClone = node.Game.Clone();
        gameClone.RunTurn(a);
        Node child = new Node(node, gameClone, a, EvaluationFunction, suc);

        node.AddChild(a, child);
        return child;
    }

    public double Rollout(int count, Node node) {
        SimLevel gameClone = node.Game.Clone();
        for(int i = 0; i < count; i++) {
			if (gameClone.SimLevelState != SimLevelState.Playing) {
				break;
			}
            List<SimHeroAction> possibleActions = gameClone.GetPossibleHeroActions();
            int chosenIndex = rnd.Next(possibleActions.Count - 1);
            gameClone.RunTurn(possibleActions[chosenIndex]);
        }
        return UtilityFunction(gameClone);
    }

    public void Backprop(Node node, double reward) {
        if(node == null) {
            return;
        }
        node.TotalValue += reward;
		if (node.MaxValue < reward) {
			node.MaxValue = reward;
		}
        node.Visits += 1;
        Backprop(node.Parent, reward);
    }

    public SimHeroAction[] GetPath(Node node) {
        SimHeroAction[] actions = new SimHeroAction[node.Depth];
        while(node.Parent != null) {
            actions[node.Depth - 1] = node.Action;
            node = node.Parent;
        }
		return actions;
    }

	public bool IsFullExplored() {
		Node curr = RootNode;
		List<Node> queue = new List<Node> ();
		queue.Add (curr);
		while (queue.Count > 0) {
			curr = queue.First();
			if (curr.Children.Count == 0) {
				if (curr.Depth < MAX_DEPTH) {
					return false;
				}
			} else {
				foreach (KeyValuePair<SimHeroAction,Node> entry in curr.Children) {
					queue.Add (entry.Value);
				}
			}
			queue.Remove (curr);
		}
		return false;
	}
}
