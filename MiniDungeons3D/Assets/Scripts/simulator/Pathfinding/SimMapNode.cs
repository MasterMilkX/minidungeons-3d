public class SimMapNode : IPathNode<object>{
	
	public SimPoint Point{get;set;}
	public TileTypes TileType{get;set;}

	public SimMapNode(SimPoint point, TileTypes tileType){
		Point = point;
		TileType = tileType;
	}

	public SimMapNode(SimMapNode other){
		Point = other.Point;
		TileType = other.TileType;
	}

	public bool IsWalkable(object unused){
		return TileType == TileTypes.empty;
	}

	public SimMapNode Clone(){
		return new SimMapNode(this);
	}
}