using System;

class SimAStar<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>{
	
	protected override double Heuristic(PathNode inStart, PathNode inEnd)
    {
        return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
    }

    protected override double NeighborDistance(PathNode inStart, PathNode inEnd)
    {
        return Heuristic(inStart, inEnd);
    }

    public SimAStar(TPathNode[,] inGrid) : base(inGrid){}
}