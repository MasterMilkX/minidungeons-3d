using System;
using System.Collections;
using System.Collections.Generic;

class CellPair
{
	/// <summary>
	/// Gets or sets the map.
	/// </summary>
	/// <value>The map.</value>
	public string[][] Map { get; set; }

	/// <summary>
	/// Gets or sets the x.
	/// </summary>
	/// <value>The x.</value>
	public int X { get; set; }

	/// <summary>
	/// Gets or sets the y.
	/// </summary>
	/// <value>The y.</value>
	public int Y { get; set; }

	/// <summary>
	/// Gets or sets the children.
	/// </summary>
	/// <value>The children.</value>
	public List<CellPair> Children { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:CellPair"/> class.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public CellPair(int x, int y, string[][] map)
	{
		X = x;
		Y = y;
		Map = map;
		Children = new List<CellPair>();
	}

	/// <summary>
	/// Adds the children.
	/// </summary>
	/// <param name="c">C.</param>
	public void AddChildren(string c)
	{
		try
		{
			if (Map[Y - 1][X].Equals(c))
			{
				Children.Add(new CellPair(X, Y - 1, Map));
				Map[Y - 1][X] = "R";
			}
		}
		catch (IndexOutOfRangeException)
		{
			//lol we crashed
		}
		try
		{
			if (Map[Y + 1][X].Equals(c))
			{
				Children.Add(new CellPair(X, Y + 1, Map));
				Map[Y + 1][X] = "R";
			}
		}
		catch (IndexOutOfRangeException)
		{
			//lol we crashed
		}
		try
		{
			if (Map[Y][X - 1].Equals(c))
			{
				Children.Add(new CellPair(X - 1, Y, Map));
				Map[Y][X - 1] = "R";
			}
		}
		catch (IndexOutOfRangeException)
		{
			//lol we crashed
		}
		try
		{
			if (Map[Y][X + 1].Equals(c))
			{
				Children.Add(new CellPair(X + 1, Y, Map));
				Map[Y][X + 1] = "R";
			}
		}
		catch (IndexOutOfRangeException)
		{
			//lol we crashed
		}
	}

}
public class CreatorEvaluator
{
	private string[][] Map;
	public CreatorEvaluator()
	{

	}
	/// <summary>
	/// Evaluates the creator.
	/// </summary>
	/// <param name="c">C.</param>
	public string EvaluateCreator(Creator c)
	{
		c.GenerateMap();
		string[][] generatedMap = c.GetMap();
		Map = generatedMap;
		// calculate the empty/full ratio
		float emptyToFullRatio = EmptyToFullRatio(generatedMap);
		Console.WriteLine(emptyToFullRatio);
		string eToF = "" + emptyToFullRatio;

		string[][] cloneMap = CloneMap(c.GetMap());

		// evaluate the wall connected components
		int wallComponents = CountConnectedComponents(cloneMap, "X");
		Console.WriteLine("Free components: " + wallComponents);
		string wComp = "" + wallComponents;

		string pathBranching = PathBranching(generatedMap);

		Console.WriteLine("Openess: " + pathBranching);
		string row = string.Format("{0},{1},{2}", eToF, wComp, pathBranching);
		Console.WriteLine("*****");
		return row;
	}

	public string EvalulateMap(string[][] map)
	{
		string[][] generatedMap = map;
		Map = generatedMap;
		// calculate the empty/full ratio
		float emptyToFullRatio = EmptyToFullRatio(generatedMap);
		Console.WriteLine(emptyToFullRatio);
		string eToF = "" + emptyToFullRatio;

		string[][] cloneMap = CloneMap(generatedMap);

		// evaluate the wall connected components
		int wallComponents = CountConnectedComponents(cloneMap, "X");
		Console.WriteLine("Free components: " + wallComponents);
		string wComp = "" + wallComponents;

		string pathBranching = PathBranching(generatedMap);

		Console.WriteLine("Openess: " + pathBranching);
		string row = string.Format("{0},{1},{2}", eToF, wComp, pathBranching);
		Console.WriteLine("*****");
		return row;
	}

	/// <summary>
	/// Gets the string from map.
	/// </summary>
	/// <returns>The string from map.</returns>
	/// <param name="map">Map.</param>
	public string GetStringFromMap(string[][] map)
	{
		string mapString = "";
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				mapString += map[i][j];
			}
			if (i != 19)
			{
				mapString += "\n";
			}
		}
		return mapString;
	}

	public string GetMap()
	{
		return GetStringFromMap(Map);
	}
	/// <summary>
	/// Calculates the empty space to full space ratio.
	/// </summary>
	/// <returns>The to full ratio.</returns>
	/// <param name="map">Map.</param>
	public float EmptyToFullRatio(string[][] map)
	{
		int empty = 0;
		int full = 0;
		for (int i = 1; i < map.Length-1; i++)
		{
			for (int j = 1; j < map[i].Length-1; j++)
			{
				if (map[i][j].Equals("X"))
				{
					full++;
				}
				else
				{
					empty++;
				}
			}
		}
		float fullRatio = full;
		fullRatio = empty / (empty + fullRatio);
		return fullRatio;
	}

	/// <summary>
	/// Counts the connected components.
	/// </summary>
	/// <returns>The connected components.</returns>
	public int CountConnectedComponents(string[][] map, string c)
	{
		int number = 0;

		while (CheckForString(c, map))
		{
			FloodMap(map, c);
			number++;
		}


		return number;
	}

	private bool CheckForString(string c, string[][] map)
	{
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				if (map[i][j].Equals(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	private string[][] CloneMap(string[][] o)
	{
		string[][] n = new string[20][];
		for (int i = 0; i < n.Length; i++)
		{
			n[i] = new string[10];
		}

		for (int i = 0; i < o.Length; i++)
		{
			for (int j = 0; j < o[i].Length; j++)
			{
				n[i][j] = o[i][j];
			}
		}
		return n;
	}

	public void FloodMap(string[][] map, string c)
	{
		int startY = 0;
		int startX = 0;
		bool found = false;
		for (int i = 0; i < map.Length && !found; i++)
		{
			for (int j = 0; j < map[i].Length && !found; j++)
			{
				if (map[i][j].Equals(c))
				{
					startY = i;
					startX = j;
					found = true;
				}
			}
		}

		// flood the map from this point
		map[startY][startX] = "F";
		CellPair Start = new CellPair(startX, startY, map);
		Start.AddChildren(c);
		List<CellPair> Pairs = new List<CellPair>();
		foreach (CellPair child in Start.Children)
		{
			Pairs.Add(child);
		}
		while (Pairs.Count > 0)
		{
			// take the next guy in the list
			CellPair Next = Pairs[0];
			Pairs.RemoveAt(0);

			// flood the place : "F"
			map[Next.Y][Next.X] = "F";

			// add children
			Next.AddChildren(c);
			foreach (CellPair child in Next.Children)
			{
				Pairs.Add(child);
			}
		}
	}

	public string PathBranching(string[][] map)
	{
		float branches = 0.0f;
		List<int> branchInts = new List<int>();
		int count = 0;

		for (int i = 1; i < map.Length-1; i++)
		{
			for (int j = 1; j < map[i].Length-1; j++)
			{
				int local = 0;
				// make sure current map node is passable
				if (!map[i][j].Equals("X"))
				{
					// check each of the four surrounding nodes for passibility
					if(!map[i+1][j].Equals("X"))
					{
						branches++;
						local++;
					}
					if(!map[i-1][j].Equals("X"))
					{
						branches++;
						local++;
					}
					if(!map[i][j+1].Equals("X"))
					{
						branches++;
						local++;
					}
					if(@map[i][j-1].Equals("X"))
					{
						branches++;
						local++;
					}
					branchInts.Add(local);
					count++;
				}
			}
		}
		float mean = branches / count;
		double variance = 0.0;
		for(int i = 0; i < branchInts.Count; i++)
		{
			variance += Math.Pow((branchInts[i] - mean), 2);

		}
		variance = variance / branchInts.Count;
		return "" + mean + "," + variance;
	}

}
