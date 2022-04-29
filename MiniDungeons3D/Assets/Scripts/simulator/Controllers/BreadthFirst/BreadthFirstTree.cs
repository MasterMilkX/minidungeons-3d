using System;
using System.Collections.Generic;
using System.Linq;

public class BreadthFirstTree {

	public int Depth{get;set;}

	public SimLevel RootState{get;set;}
	public BreadthFirstNode RootNode{get;set;}
	readonly Func<SimLevel,double> _utilityFunction;

	List<List<BreadthFirstNode>> layers;

	public BreadthFirstTree(int depth, SimLevel rootState, Func<SimLevel,double> utilityFunction){
		Depth = depth;
		RootState = rootState;
		_utilityFunction = utilityFunction;
		layers = new List<List<BreadthFirstNode>>();
	}

	public void BuildTree(int msPerMove){
		RootNode = new BreadthFirstNode(RootState, _utilityFunction);
		
		layers.Add(new List<BreadthFirstNode>());
		layers[0].Add(RootNode);

		var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        float msElapsed = 0;

        int layer = 0;
        while(msElapsed < (float)msPerMove){
        	layers.Add(new List<BreadthFirstNode>());
        	for(int i = 0; i < layers[layer].Count; i++){
        		if(!layers[layer][i].IsTerminal()){
        			layers[layer][i].Expand();
        			layers[layer+1].AddRange(layers[layer][i].Children);
        		}
        	}
        	layer++;
        	msElapsed = stopWatch.ElapsedMilliseconds;
        }

		/*for(int i = 0; i < Depth-1; i++){
			layers[i+1] = new List<BreadthFirstNode>();
			foreach(BreadthFirstNode node in layers[i]){
				if(!node.IsTerminal()){
					node.Expand();
					layers[i+1].AddRange(node.Children);
				}
			}
		}*/
	}

	public BreadthFirstNode GetBestTerminalNode(){
		var terminalNodes = new List<BreadthFirstNode>();
		foreach(BreadthFirstNode node in layers[layers.Count-1]){
			terminalNodes.Add(node);
		}
		for(int i = 0; i < layers.Count-1; i++){
			var layer = layers[i];
			foreach(BreadthFirstNode node in layer){
				if(node.IsTerminal()){
					terminalNodes.Add(node);
				}
			}
		}
		var orderedNodes = terminalNodes.OrderByDescending(node => node.Utility).ToList(); 
		return orderedNodes[0];
	}

	public BreadthFirstNode GetBestNode(){
		var best = GetBestTerminalNode();
		while(best.Parent != RootNode){
			best = best.Parent;
		}
		return best;
	}
}