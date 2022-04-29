using System;
using System.Collections;
using System.Collections.Generic;

public class BFSNode
{
	public int X { get; set; }
	public int Y { get; set; }

	public BFSNode Parent { get; set; }

	public List<BFSNode> Children { get; set; }
	public List<BFSNode> Parents { get; set; }

	public int Depth { get; set; }

	public int Type { get; set; }

	public BFSNode(int x, int y)
	{
		X = x;
		Y = y;
		Type = 0;
		Parents = new List<BFSNode>();
		Children = new List<BFSNode>();
	}

	public void GenerateChildren(string[][] map)
	{
		// create four potential children (up, down, left, right)
		// 
		if (map[Y][X].Equals("e"))
		{
			Type = 1;
		}
		// generate the "up" child, if its not already in the parent list
		else if (!map[Y + 1][X].Equals("X") && Depth < (map.Length + map[0].Length))
		{
			BFSNode up = new BFSNode(X, Y + 1);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}
		// check bottom
		if (!map[Y - 1][X].Equals("X"))
		{
			BFSNode up = new BFSNode(X, Y - 1);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}
		// check left
		if (!map[Y][X - 1].Equals("X"))
		{
			BFSNode up = new BFSNode(X - 1, Y);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}

		// check right
		if (!map[Y][X + 1].Equals("X"))
		{
			BFSNode up = new BFSNode(X + 1, Y);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}
	}


	public void GenerateChildren(string[][] map, string c)
	{
		// create four potential children (up, down, left, right)
		// 
		if (map[Y][X].Equals(c))
		{
			Type = 1;
		}
		// generate the "up" child, if its not already in the parent list
		else if (!map[Y + 1][X].Equals("X") && Depth< (map.Length + map[0].Length))
		{
			BFSNode up = new BFSNode(X, Y + 1);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}
		// check bottom
		if (!map[Y - 1][X].Equals("X"))
		{
			BFSNode up = new BFSNode(X, Y - 1);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}
		// check left
		if (!map[Y][X - 1].Equals("X"))
		{
			BFSNode up = new BFSNode(X - 1, Y);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}

		// check right
		if (!map[Y][X + 1].Equals("X"))
		{
			BFSNode up = new BFSNode(X + 1, Y);
			bool check = false;
			foreach (BFSNode parent in Parents)
			{
				if (parent.X == up.X && parent.Y == up.Y)
				{
					check = true;
					break;
				}
			}
			if (!check)
			{
				// add child to children
				Children.Add(up);
				// add all previous parents to this child
				foreach (BFSNode parent in Parents)
				{
					up.Parents.Add(parent);
				}
				// add this parent to the child
				up.Parents.Add(this);
				up.Parent = this;
				up.Depth = Parent.Depth + 1;
			}
		}
	}
}

