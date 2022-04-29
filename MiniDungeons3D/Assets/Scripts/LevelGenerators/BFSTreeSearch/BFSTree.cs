using System;
using System.Collections;
using System.Collections.Generic;
public class BFSTree
{
	public List<List<BFSNode>> Paths { get; set; }

	private string[][] map;

	public List<BFSNode> Exits { get; set; }
	int iterations = 0;
	public BFSTree(Pairing entrance, Pairing exit, string[][] map)
	{
		this.map = map;
		Exits = new List<BFSNode>();
		BFSNode root = BuildTree(entrance, exit);
		Paths = new List<List<BFSNode>>();

		// loop through all the exits and form paths
		foreach (BFSNode e in Exits)
		{
			List<BFSNode> path = BuildPath(e);
			Paths.Add(path);
		}

	}

	public BFSTree(string[][] map)
	{
        this.map = map;
		Exits = new List<BFSNode>();
	}

	public BFSNode BuildTree(Pairing entrance, Pairing exit)
	{
		BFSNode root = new BFSNode(entrance.X, entrance.Y);
		List<BFSNode> queue = new List<BFSNode>();
		queue.Add(root);
		BFSNode current = root;
		root.Parent = root;
		while (queue.Count > 0)
		{
			current = queue[0];
			queue.RemoveAt(0);
			current.GenerateChildren(this.map);
			foreach (BFSNode child in current.Children)
			{
				queue.Add(child);
			}

			// check if this node is an exit leaf. If it is, then add it to the list.
			if (current.Type == 1)
			{
				Exits.Add(current);
			}
			iterations++;
		}
		return root;
	}

	public BFSNode BuildTree(Pairing start, string c, int maxDepth)
	{
		BFSNode root = new BFSNode(start.X, start.Y);
		List<BFSNode> queue = new List<BFSNode>();
		queue.Add(root);
		BFSNode current = root;
		root.Parent = root;
		while (queue.Count > 0)
		{
			current = queue[0];
			queue.RemoveAt(0);
			if (current.Depth < maxDepth)
			{
				current.GenerateChildren(this.map, c);

				foreach (BFSNode child in current.Children)
				{
					queue.Add(child);
				}
			}
			// check if this node is an exit leaf. If it is, then add it to the list.
			if (current.Type == 1)
			{
				Exits.Add(current);
			}
			iterations++;
		}
		return root;
	}
	public List<BFSNode> BuildPath(BFSNode exit)
	{
		List<BFSNode> path = new List<BFSNode>();

		BFSNode current = exit;
		while (true)
		{
			if (!current.Parent.Equals(current))
			{
				path.Add(current);
				current = current.Parent;
			}
			else
			{
				break;
			}
		}
		path.Add(current);
		return path;
	}
}

