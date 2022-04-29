using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TreeLanguageEvolute;

public class FurnisherEvaluator
{
	private Furnisher F;
	public FurnisherEvaluator()
	{
		
	}

	public string SuperEvaluation(string[][] Map, Furnisher f)
	{
		F = f;
		//c.PrintMap();
		string[][] map = CloneMap(Map);
		f.GenerateMap(map);
		//f.PrintMap();
		map = f.GetMap();

		PathDatabase.GameType = 1;
		PathDatabase.BigDatabase = new List<Dictionary<string, SimPoint[]>>();
		PathDatabase.levels = new ArrayList();
		PathDatabase.Farthest = new Dictionary<SimLevel, Pair>();

		SimLevel level = new SimLevel(GetMapString(map));
		PathDatabase.BuildDB(level);
		// Find the length of the shortest path from entry to exit
		int pathLength = LengthOfShortestPath(map);
		Console.WriteLine("Shortest Path: " + pathLength);
		string shortPath = "" + pathLength;
		// interactables
		int potionCount = PotionCount(map);
		Console.WriteLine("Potions: " + potionCount);
		string potions = "" + potionCount;

		int treasureCount = TreasureCount(map);
		Console.WriteLine("Treasures: " + treasureCount);
		string treasures = "" + treasureCount;

		int trapCount = TrapCount(map);
		Console.WriteLine("Traps: " + trapCount);
		string traps = "" + trapCount;

		int portalCount = PortalCount(map);
		Console.WriteLine("Portals: " + portalCount);
		string portals = "" + portalCount;
		// monsters
		int meleeCount = MeleeGoblinCount(map);
		Console.WriteLine("Melee Goblins: " + meleeCount);
		string mGoblins = "" + meleeCount;

		int rangedCount = RangedGoblinCount(map);
		Console.WriteLine("Ranged Goblins: " + rangedCount);
		string rGoblins = "" + rangedCount;

		int blobCount = BlobCount(map);
		Console.WriteLine("Blobs: " + blobCount);
		string blobs = "" + blobCount;

		int ogreCount = OgreCount(map);
		Console.WriteLine("Ogres: " + ogreCount);
		string ogres = "" + ogreCount;

		int minitarCount = MinitaurCount(map);
		Console.WriteLine("Minitaurs: " + minitarCount);
		string minitaurs = "" + minitarCount;

		int guardedPotions = GuardedPotions(map);
		Console.WriteLine("Guarded potions: " + guardedPotions);
		string gPotions = "" + guardedPotions;

		int guardedTreasures = GuardedTreasures(map);
		Console.WriteLine("Guarded treasures: " + guardedTreasures);
		string gTreasures = "" + guardedTreasures;

		int mOnPath = CountMonstersOnShortestPath(map);
		Console.WriteLine("Monsters on Path: " + mOnPath);
		string mPath = "" + mOnPath;

		// have agents play through it
		f.PrintMap();
		string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
								   shortPath, potions, treasures, traps, portals, mGoblins,
								   rGoblins, blobs, ogres, minitaurs, gPotions, gTreasures,
								   mPath);
		return row;	
	}

	public string SuperEvaluation(Creator c, Furnisher f)
	{
		F = f;
		//c.PrintMap();
		string[][] map = CloneMap(c.GetMap());
		f.GenerateMap(map);
		//f.PrintMap();
		map = f.GetMap();

		PathDatabase.GameType = 1;
		PathDatabase.BigDatabase = new List<Dictionary<string, SimPoint[]>>();
		PathDatabase.levels = new ArrayList();
		PathDatabase.Farthest = new Dictionary<SimLevel, Pair>();

		SimLevel level = new SimLevel(GetMapString(map));
		PathDatabase.BuildDB(level);
		// Find the length of the shortest path from entry to exit
		int pathLength = LengthOfShortestPath(map);
		Console.WriteLine("Shortest Path: " + pathLength);
		string shortPath = "" + pathLength;
		// interactables
		int potionCount = PotionCount(map);
		Console.WriteLine("Potions: " + potionCount);
		string potions = "" + potionCount;

		int treasureCount = TreasureCount(map);
		Console.WriteLine("Treasures: " + treasureCount);
		string treasures = "" + treasureCount;

		int trapCount = TrapCount(map);
		Console.WriteLine("Traps: " + trapCount);
		string traps = "" + trapCount;

		int portalCount = PortalCount(map);
		Console.WriteLine("Portals: " + portalCount);
		string portals = "" + portalCount;
		// monsters
		int meleeCount = MeleeGoblinCount(map);
		Console.WriteLine("Melee Goblins: " + meleeCount);
		string mGoblins = "" + meleeCount;

		int rangedCount = RangedGoblinCount(map);
		Console.WriteLine("Ranged Goblins: " + rangedCount);
		string rGoblins = "" + rangedCount;

		int blobCount = BlobCount(map);
		Console.WriteLine("Blobs: " + blobCount);
		string blobs = "" + blobCount;

		int ogreCount = OgreCount(map);
		Console.WriteLine("Ogres: " + ogreCount);
		string ogres = "" + ogreCount;

		int minitarCount = MinitaurCount(map);
		Console.WriteLine("Minitaurs: " + minitarCount);
		string minitaurs = "" + minitarCount;

		int guardedPotions = GuardedPotions(map);
		Console.WriteLine("Guarded potions: " + guardedPotions);
		string gPotions = "" + guardedPotions;

		int guardedTreasures = GuardedTreasures(map);
		Console.WriteLine("Guarded treasures: " + guardedTreasures);
		string gTreasures = "" + guardedTreasures;

		int mOnPath = CountMonstersOnShortestPath(map);
		Console.WriteLine("Monsters on Path: " + mOnPath);
		string mPath = "" + mOnPath;

		// have agents play through it
		f.PrintMap();
		string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
								   shortPath, potions, treasures, traps, portals, mGoblins,
								   rGoblins, blobs, ogres, minitaurs, gPotions, gTreasures,
								   mPath);
		return row;
	}

	public string AgentTesting(string map)
	{
		SimLevel level = new SimLevel(map);
		string result = "";
		result += AgentPlaythrough(level, "R");
		result += ",";
		result += AgentPlaythrough(level, "MK");
		result += ",";
		result += AgentPlaythrough(level, "TC");
		result += ",";
		result += AgentPlaythrough(level, "C");
		return result;
	}

	/// <summary>
	/// Runs the runner agent on this map.
	/// </summary>
	public LevelReport AgentPlaythrough(SimLevel level, string type)
	{
		string controllerType = "MCTS";
		string utilityFunction = type;

		PathDatabase.levels = new System.Collections.ArrayList();
		PathDatabase.BigDatabase = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, SimPoint[]>>();
		PathDatabase.Farthest = new System.Collections.Generic.Dictionary<SimLevel, Pair>();
		PathDatabase.GameType = 1;
		PathDatabase.BuildDB(level);

		SimSentientSketchbookInterface minidungeons2 = new SimSentientSketchbookInterface();
		TreeProgram function = minidungeons2.BuildFunction("GenProgram99" + type + ".txt");
		MapReport report = minidungeons2.RunController(level.LevelString, controllerType, utilityFunction, function);

		return report.LevelReport;
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


	public string EvaluateFurnisher(Creator c, Furnisher f) 
	{
		F = f;
		c.GenerateMap();
		c.PrintMap();
		f.GenerateMap(c.GetMap());
		f.PrintMap();
		//f.PrintMap();
		string[][] map = f.GetMap();

		PathDatabase.GameType = 1;
		PathDatabase.BigDatabase = new List<Dictionary<string, SimPoint[]>>();
		PathDatabase.levels = new ArrayList();
		PathDatabase.Farthest = new Dictionary<SimLevel, Pair>();

		SimLevel level = new SimLevel(GetMapString(map));
		PathDatabase.BuildDB(level);
		// Find the length of the shortest path from entry to exit
		int pathLength = LengthOfShortestPath(map);
		Console.WriteLine("Shortest Path: " + pathLength);
		string shortPath = "" + pathLength;
		// interactables
		int potionCount = PotionCount(map);
		Console.WriteLine("Potions: " + potionCount);
		string potions = "" + potionCount;

		int treasureCount = TreasureCount(map);
		Console.WriteLine("Treasures: " + treasureCount);
		string treasures = "" + treasureCount;

		int trapCount = TrapCount(map);
		Console.WriteLine("Traps: " + trapCount);
		string traps = "" + trapCount;

		int portalCount = PortalCount(map);
		Console.WriteLine("Portals: " + portalCount);
		string portals = "" + portalCount;
		// monsters
		int meleeCount = MeleeGoblinCount(map);
		Console.WriteLine("Melee Goblins: " + meleeCount);
		string mGoblins = "" + meleeCount;

		int rangedCount = RangedGoblinCount(map);
		Console.WriteLine("Ranged Goblins: " + rangedCount);
		string rGoblins = "" + rangedCount;

		int blobCount = BlobCount(map);
		Console.WriteLine("Blobs: " + blobCount);
		string blobs = "" + blobCount;

		int ogreCount = OgreCount(map);
		Console.WriteLine("Ogres: " + ogreCount);
		string ogres = "" + ogreCount;

		int minitarCount = MinitaurCount(map);
		Console.WriteLine("Minitaurs: " + minitarCount);
		string minitaurs = "" + minitarCount;

		int guardedPotions = GuardedPotions(map);
		Console.WriteLine("Guarded potions: " + guardedPotions);
		string gPotions = "" + guardedPotions;

		int guardedTreasures = GuardedTreasures(map);
		Console.WriteLine("Guarded treasures: " + guardedTreasures);
		string gTreasures = "" + guardedTreasures;

		int mOnPath = CountMonstersOnShortestPath(map);
		Console.WriteLine("Monsters on Path: " + mOnPath);
		string mPath = "" + mOnPath;

		// have agents play through it

		string row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", 
		                           shortPath, potions, treasures, traps, portals, mGoblins,
		                           rGoblins, blobs, ogres, minitaurs, gPotions, gTreasures,
		                           mPath);


		//map = new string[][] { 
		//	new string [] { "X" , "X", "X", "X", "X", "X", "X", "X", "X", "X" }, 
		//	new string [] { "X" , "X", " ", " ", "H", " ", " ", " ", " ", "X" },
		//	new string [] { "X" , "X", " ", " ", " ", " ", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", " ", " ", " ", " ", " ", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", "X", " ", "X", " ", "X", " ", "X" },
		//	new string [] { "X" , "X", " ", " ", "e", " ", " ", " ", " ", "X" },
		//	new string [] { "X" , "X", "X", "X", "X", "X", "X", "X", "X", "X" }
		//};
		//Cluster(map);
		//AgentPlaythrough();
		return row;
	}


	public string GetMap()
	{
		string[][] map = F.GetMap();
		string m = "";
		for (int k = 0; k<map.Length; k++)
		{
			for (int j = 0; j<map[k].Length; j++)
			{
				m += map[k][j];
			}
			if (k != 19)
			{
				m += "\n";
			}
		}
		return m;
	}
	/// <summary>
	/// Gets the length of shortest path.
	/// </summary>
	/// <returns>The of shortest path.</returns>
	/// <param name="map">Map.</param>
	public int LengthOfShortestPath(string[][] map)
	{
		SimPoint entrance = FindEntrance(map);
		SimPoint exit = FindExit(map);

		SimPoint[] shortestPath = FindPath(entrance, exit, map);

		return shortestPath.Length;
	}
	/// <summary>
	/// Finds the path.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	/// <param name="map">Map.</param>
	public SimPoint[] FindPath(SimPoint start, SimPoint end, string[][] map)
	{

		SimPoint[] path = PathDatabase.Lookup(start, end, GetStringFromMap(map));
		return path;
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
		}
		return mapString;	
	}

	public string GetMapString(string[][] map)
	{
		string mapString = "";
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				mapString += map[i][j];
			}
			if (i != map.Length - 1)
			{
				mapString += "\n";
			}
		}
		return mapString;	
	}

	/// <summary>
	/// Count the potions.
	/// </summary>
	/// <returns>The potion count.</returns>
	/// <param name="map">Map.</param>
	public int PotionCount(string[][] map)
	{
		int count = 0;
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				if (map[i][j].Equals("P"))
				{
					count++;
				}
			}
		}

		return count;
	}

	/// <summary>
	/// Count the melee goblins.
	/// </summary>
	/// <returns>The goblin melee count.</returns>
	/// <param name="map">Map.</param> 
	public int MeleeGoblinCount(string[][] map)
	{
		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("M"))
				{
					count++;
				}
			}
		}
		return count;
	}
	/// <summary>
	/// Counts the ranged goblins.
	/// </summary>
	/// <returns>The ranged goblin count.</returns>
	/// <param name="map">Map.</param>
	public int RangedGoblinCount(string[][] map)
	{
	
	int count = 0;
	for (int i = 0; i<map.Length; i++)
	{
		for (int j = 0; j<map[i].Length; j++)
		{
			if (map[i][j].Equals("R"))
			{
				count++;
			}
		}
	}
	return count;	
	}

	/// <summary>
	/// Counts the minitaurs.
	/// </summary>
	/// <returns>The minitaur count.</returns>
	/// <param name="map">Map.</param>
	public int MinitaurCount(string[][] map)
	{

		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("m"))
				{
					count++;
				}
			}
		}
		return count;	
	}

	/// <summary>
	/// Counts the ogres.
	/// </summary>
	/// <returns>The ogre count.</returns>
	/// <param name="map">Map.</param> 
	public int OgreCount(string[][] map)
	{

		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("o"))
				{
					count++;
				}
			}
		}
		return count;	
	}
	/// <summary>
	/// Counts the blobs.
	/// </summary>
	/// <returns>The blob count.</returns>
	/// <param name="map">Map.</param>	 
	public int BlobCount(string[][] map)
	{

		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("b"))
				{
					count++;
				}
			}
		}
		return count;	
	}
	/// <summary>
	/// Counts the Treasure.
	/// </summary>
	/// <returns>The treasure count.</returns>
	/// <param name="map">Map.</param> 
	public int TreasureCount(string[][] map)
	{

		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("T"))
				{
					count++;
				}
			}
		}
		return count;	
	}

	/// <summary>
	/// Counts the traps.
	/// </summary>
	/// <returns>The trap count.</returns>
	/// <param name="map">Map.</param>
	public int TrapCount(string[][] map)
	{

		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("t"))
				{
					count++;
				}
			}
		}
		return count;	
	}

	/// <summary>
	/// Portals the count.
	/// </summary>
	/// <returns>The count.</returns>
	/// <param name="map">Map.</param>
	public int PortalCount(string[][] map)
	{
		int count = 0;
		for (int i = 0; i<map.Length; i++)
		{
			for (int j = 0; j<map[i].Length; j++)
			{
				if (map[i][j].Equals("p"))
				{
					count++;
				}
			}
		}
		return count;
	}

	/// <summary>
	/// Finds the entrance.
	/// </summary>
	/// <returns>The entrance.</returns>
	/// <param name="map">Map.</param>
	public SimPoint FindEntrance(string[][] map)
	{
		SimPoint entrance = new SimPoint(0,0);
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				if (map[i][j].Equals("H"))
				{
					entrance = new SimPoint(j, i);
					break;
				}
			}
		}
		return entrance;
	}

	/// <summary>
	/// Finds the exit.
	/// </summary>
	/// <returns>The exit.</returns>
	/// <param name="map">Map.</param>
	public SimPoint FindExit(string[][] map)
	{
		SimPoint exit = new SimPoint(0, 0);
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				if (map[i][j].Equals("e"))
				{
					exit = new SimPoint(j, i);
					break;
				}
			}
		}
		return exit;	
	}

	/// <summary>
	/// Finds the count of potions that are "guarded" by any kind of monster. This creates an interesting dynamic we 
	/// would like to study, as having a potion near a monster increases the risk the player has to take to consume the potion
	/// </summary>
	/// <returns>The guarded potion count.</returns>
	/// <param name="map">Map.</param>
	public int GuardedPotions(string[][] map)
	{
		int count = 0;

		List<Pairing> XY = new List<Pairing>();
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				if (map[i][j].Equals("P"))
				{
					XY.Add(new Pairing(j, i));
				}
			}
		}
		// take our list and find shortest path between it and the hero
		// if the hero would ever have to travel through or next to a monster within 2 tiles of
		// the path, then this monster is counted as "guarding" the portion
		foreach (Pairing potion in XY)
		{
			// find path from hero to this point
			SimPoint pot = new SimPoint(potion.X, potion.Y);
			SimPoint entrance = FindEntrance(map);
			SimPoint[] path = FindPath(entrance, pot, map);
			for (int i = path.Length - 2; i < path.Length - 1; i++)
			{
				// check if a monster is on this tile
				if (IsMonster(path[i].X, path[i].Y, map)) {
					count++;
					break;
				}
			    // check if monster is next to this tile
				else if(IsMonster(path[i].X-1, path[i].Y, map) 
				        || IsMonster(path[i].X+1, path[i].Y, map)
				        || IsMonster(path[i].X, path[i].Y-1, map)
				        || IsMonster(path[i].X, path[i].Y+1, map)) {
					count++;
					break;
				}
					
			}
		}
		return count;
	}
	public int GuardedTreasures(string[][] map)
	{
		int count = 0;

		List<Pairing> XY = new List<Pairing>();
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				if (map[i][j].Equals("T"))
				{
					XY.Add(new Pairing(j, i));
				}
			}
		}
		foreach (Pairing treasure in XY)
		{
			// find path from hero to this point
			SimPoint tre = new SimPoint(treasure.X, treasure.Y);
			SimPoint entrance = FindEntrance(map);
			SimPoint[] path = FindPath(entrance, tre, map);
			for (int i = path.Length - 2; i < path.Length - 1; i++)
			{
				// check if a monster is on this tile
				if (IsMonster(path[i].X, path[i].Y, map))
				{
					count++;
					break;
				}
				// check if monster is next to this tile
				else if (IsMonster(path[i].X - 1, path[i].Y, map)
						|| IsMonster(path[i].X + 1, path[i].Y, map)
						|| IsMonster(path[i].X, path[i].Y - 1, map)
						|| IsMonster(path[i].X, path[i].Y + 1, map))
				{
					count++;
					break;
				}

			}
		}
		return count;	
	}

	public bool IsMonster(int X, int Y, string[][] map)
	{
		if (map[Y][X].Equals("M") || map[Y][X].Equals("R") || map[Y][X].Equals("o") || map[Y][X].Equals("b"))
		{
			//Console.WriteLine(map[Y][X]);
			return true;
		}
		return false;
	}
	/// <summary>
	/// Distances the between start and end.
	/// </summary>
	/// <returns>The <see cref="T:System.Int32"/>.</returns>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public int DistanceBetween(Pairing start, Pairing end)
	{
		Pairing[] rayTrace = RayTrace(start, end);
		return rayTrace.Length;	
	}

	/// <summary>
	/// Raycast the specified p1 and p2.
	/// </summary>
	/// <returns>The raycast.</returns>
	/// <param name="p1">P1.</param>
	/// <param name="p2">P2.</param>
	public Pairing[] RayTrace(Pairing p1, Pairing p2)
	{
		int x0 = p1.X;
		int y0 = p1.Y;
		int x1 = p2.X;
		int y1 = p2.Y;

		List<Pairing> trace = new List<Pairing>();

		int dx = Math.Abs(x1 - x0);
		int dy = Math.Abs(y1 - y0);
		int x = x0;
		int y = y0;
		int n = 1 + dx + dy;
		int x_inc = (x1 > x0) ? 1 : -1;
		int y_inc = (y1 > y0) ? 1 : -1;
		int error = dx - dy;
		dx *= 2;
		dy *= 2;

		for (int tiles = n; tiles > 0; tiles--)
		{
			trace.Add(new Pairing(x, y));
			if (error > 0)
			{
				x += x_inc;
				error -= dy;
			}
			else
			{
				y += y_inc;
				error += dx;
			}
		}

		Pairing[] result = trace.ToArray();
		return result;	
	}

	public bool LineOfSight(Pairing start, Pairing end, string[][] map)
	{
		bool result = true;
		Pairing[] rayTrace = RayTrace(start, end);
		for (int point = 0; point < rayTrace.Length; point++)
		{
			if (map[rayTrace[point].Y][rayTrace[point].X].Equals("X"))
			{
				result = false;
				break;
			}
		}
		return result;	
	}

	/// <summary>
	/// Counts the number of monsters on the shortest path.
	/// </summary>
	/// <returns>The monsters in shortest path.</returns>
	/// <param name="map">Map.</param>
	public int CountMonstersOnShortestPath(string[][] map)
	{
		SimPoint entrance = FindEntrance(map);
		SimPoint exit = FindExit(map);
		SimPoint[] path = FindPath(entrance, exit, map);

		List<Pairing> monstersOnPath = new List<Pairing>();

		foreach(SimPoint point in path)
		{
			// check if this point is a monster
			bool containsZero = false;
			bool containsOne = false;
			bool containsTwo = false;
			bool containsThree = false;
			bool containsFour = false;
			foreach (Pairing p in monstersOnPath)
			{
				// for zero
				if (p.X == point.X && p.Y == point.Y)
				{
					containsZero = true;
				}
				// for one
				if (p.X == point.X+1 && p.Y == point.Y)
				{
					containsOne = true;
				}
				// for two
				if (p.X == point.X-1 && p.Y == point.Y)
				{
					containsTwo = true;
				}
				// for three
				if (p.X == point.X && p.Y == point.Y+1)
				{
					containsThree = true;
				}
				if (p.X == point.X && p.Y == point.Y-1)
				{
					containsFour = true;
				}
			}
			if (IsMonster(point.X, point.Y, map) && !containsZero)
			{
				monstersOnPath.Add(new Pairing(point.X, point.Y));
			}
			if (IsMonster(point.X+1, point.Y, map) && !containsOne)
			{
				monstersOnPath.Add(new Pairing(point.X+1, point.Y));
			}
			if (IsMonster(point.X-1, point.Y, map) && !containsTwo)
			{
				monstersOnPath.Add(new Pairing(point.X-1, point.Y));
			}
			if (IsMonster(point.X, point.Y+1, map) && !containsThree)
			{
				monstersOnPath.Add(new Pairing(point.X, point.Y+1));
			}
			if (IsMonster(point.X, point.Y-1, map) && !containsFour)
			{
				monstersOnPath.Add(new Pairing(point.X, point.Y-1));
			}
		}

		// along the path, we want to see if we ever go through or 
		// pass by monsters
		return monstersOnPath.Count;
	}

	/// <summary>
	/// Cluster the paths of the map.
	/// </summary>
	/// <returns>The cluster.</returns>
	/// <param name="map">Map.</param>
	//public void Cluster(string[][] map)
	//{
	//	SimPoint ent = FindEntrance(map);
	//	Pairing entrance = new Pairing(ent.X, ent.Y);
	//	SimPoint ex = FindExit(map);
	//	Pairing exit = new Pairing(ent.X, ent.Y);
	//	// build the tree
	//	BFSTree tree = new BFSTree(entrance, exit, map);

	//	List<List<BFSNode>> paths = tree.Paths;
	//	double[,] distanceMatrix = new double[paths.Count, paths.Count];
	//	int i = 0;
	//	int j = 0;
	//	foreach (List<BFSNode> path1 in paths)
	//	{
	//		j = 0;
	//		foreach (List<BFSNode> path2 in paths)
	//		{
	//			int distance = CalculateLevenstein(path1, path2);
	//			distanceMatrix[i, j] = distance;
	//			j++;
	//		}
	//		i++;
	//	}

	//	alglib.clusterizerstate s;
	//	alglib.ahcreport rep;

	//	alglib.clusterizercreate(out s);
	//	alglib.clusterizersetdistances(s, distanceMatrix, true);
	//	alglib.clusterizerrunahc(s, out rep);
	//	int[,] instructions = rep.z;
	//	double[] distances = rep.mergedist;
	//	int number = rep.npoints;
	//	List<Cluster> clusters = new List<Cluster>();

	//	System.Console.WriteLine("{0}", alglib.ap.format(rep.z));
	//	// Init clusters now
	//	for (int p = 0; p < number; p++)
	//	{
	//		Cluster c = new Cluster();
	//		clusters.Add(c);
	//	}
	//	for (int k = 0; k < instructions.Length/2-1; k++)
	//	{
	//		Cluster parent = new Cluster();

	//		clusters[instructions[k, 0]].Parent = parent;
	//		clusters[instructions[k, 1]].Parent = parent;
	//		parent.Children.Add(clusters[instructions[k, 0]]);
	//		parent.Children.Add(clusters[instructions[k, 1]]);
	//		clusters.Add(parent);
	//		parent.Distance = distances[k];
	//	}
	//	Cluster root = clusters[clusters.Count - 1];
	//	int count = 1;
	//	foreach (double d in distances)
	//	{
	//		if (d > 7)
	//		{
	//			count++;
	//		}
	//	}

	//	//System.Console.WriteLine("{0}", alglib.ap.format(rep.p));
	//	Console.WriteLine("Paths: " + count);
	//}

	/// <summary>
	/// Calculates the levenstein distance.
	/// </summary>
	/// <returns>The levenstein distance.</returns>
	/// <param name="start">Start.</param>
	/// <param name="exit">Exit.</param>
	public int CalculateLevenstein(List<BFSNode> start, List<BFSNode> exit)
	{
		int distance = 0;

		// exit is longer than start
		if (start.Count < exit.Count)
		{
			for (int i = 0; i < start.Count; i++)
			{
				BFSNode a = start[i];
				BFSNode b = exit[i];
				// if this is not the same place, increment distance
				if (a.X != b.X || a.Y != b.Y)
				{
					distance++;
				}
			}
			// add difference in size to distance
			distance += (exit.Count - start.Count);
		}
		// start is longer than exit
		else if (start.Count > exit.Count)
		{
			for (int i = 0; i < exit.Count; i++)
			{
				BFSNode a = start[i];
				BFSNode b = exit[i];
				// if this is not the same place, increment distance
				if (a.X != b.X || a.Y != b.Y)
				{
					distance++;
				}
			}
			// add difference in size to distance
			distance += (start.Count - exit.Count);
		}
		// start and exit are the same length
		else
		{
			for (int i = 0; i < exit.Count; i++)
			{
				BFSNode a = start[i];
				BFSNode b = exit[i];
				// if this is not the same place, increment distance
				if (a.X != b.X || a.Y != b.Y)
				{
					distance++;
				}
			}
		}

		return distance;
	}
}

