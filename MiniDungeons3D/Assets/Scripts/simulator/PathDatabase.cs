using System;
using System.Collections;
using System.Collections.Generic;

public class Pair
{
	public SimLevel Level { get; set; }
	public SimPoint[] LongestPath { get; set; }
	public Pair(SimLevel level, SimPoint[] lp)
	{
		Level = level;
		LongestPath = lp;
	}

}
public class PathDatabase {
	// database of SimPoints
	//	public static Dictionary<int, SimPoint[]> db = new Dictionary<int, SimPoint[]>();
	//	public List<Pairing> db1 = new List<Pairing> ();
	//	public static List<HashSet<Pairing>> BigDatabase { get; set; }
	public static Dictionary<SimLevel, Pair> Farthest { get; set; }
	public static SimPoint[] longestPath { get; set; }
	public static List<Dictionary<string, SimPoint[]>> BigDatabase { get; set; }
	public static ArrayList levels { get; set; }
	public static int GameType { get; set; }

	public static void BuildDB (SimLevel level) {
		if(GameType == 1) {
			var stopwatch = new System.Diagnostics.Stopwatch();
			Dictionary<string, SimPoint[]> db2 = new Dictionary<string, SimPoint[]> ();
			BigDatabase.Add (db2);
			stopwatch.Start();
			for (int i = 0; i < level.BaseMap.GetLength (0); i++) {
				for (int j = 0; j < level.BaseMap.GetLength (1); j++) {
					if (level.BaseMap[i,j].TileType != TileTypes.wall) {
						for (int k = 0; k < level.BaseMap.GetLength (0); k++) {
							for (int l = 0; l < level.BaseMap.GetLength (1); l++) {
								if (level.BaseMap [k, l].TileType != TileTypes.wall && level.BaseMap [i, j] != level.BaseMap [k, l]) {
                                    try
                                    {
										SimPoint[] path = level.AStar(level.BaseMap[i, j].Point, level.BaseMap[k, l].Point);

										/*// is there a path already in here?
										if (Farthest.ContainsKey(level))
										{
											// check if the path in there is longer
											Pair check;
											if (Farthest.TryGetValue(level, out check))
											{
												if (check.LongestPath.Length < path.Length)
												{
													check.LongestPath = path;
												}
											}
										}
										// there isnt a path, make this the farthest path
										else
										{
											Pair pr = new Pair(level, path);
											Farthest.Add(level, pr);
										}*/
										if (longestPath is null)
										{
											longestPath = path;
										}
										else if (path.Length > longestPath.Length)
										{
											longestPath = path;
										}
										//Pair```ing pairing = new Pairing(level.BaseMap[i, j].Point, level.BaseMap[k,l].Point, path);
										string temp = level.BaseMap[i, j].Point.ToString() + level.BaseMap[k, l].Point.ToString();
										db2.Add(temp, path);
									}
                                    catch (Exception ex)
                                    {
										// Console.WriteLine("(" + i + " ," + j + ") is not traversible from (" + k + ", " + l + ")");
                                    }
									
								}
							}
						}
					}
				}
			}
			stopwatch.Stop ();
			float timeTaken = (float)stopwatch.ElapsedMilliseconds;
		}
//		Console.Write ("Path Database initialized in " + timeTaken + " ms\n");
	}
	public static SimPoint[] Lookup(SimPoint start, SimPoint end, string level) {
//		Pairing p = new Pairing(start, end);
		if (GameType == 1) {
			int index = 0;
			foreach (string l in levels) {
				if (l.Equals (level)) {
					index = levels.IndexOf (l);
				} 
			}
			string keyCheck = start.ToString () + end.ToString ();
			SimPoint[] output;
			if (BigDatabase [index].TryGetValue (keyCheck, out output)) {
				return output;		
			} else {
				// we couldnt find the point in the database, this is strange, but we will cover our bases
				SimLevel temp = new SimLevel (level);
//			Console.WriteLine ("Wut");
				return temp.AStar (start, end);
			}

		} else {
			SimLevel temp = new SimLevel (level);
			//			Console.WriteLine ("Wut");
			return temp.AStar (start, end);
		}

	}

	public static SimPoint[] Lookup(SimPoint start, SimPoint end, SimLevel level)
	{
		//		Pairing p = new Pairing(start, end);
		if (GameType == 1)
		{
			int index = 0;
			foreach (string l in levels)
			{
				if (l.Equals(level))
				{
					index = levels.IndexOf(l);
				}
			}
			string keyCheck = start.ToString() + end.ToString();
			SimPoint[] output;
			if (BigDatabase[index].TryGetValue(keyCheck, out output))
			{
				return output;
			}
			else
			{
				// we couldnt find the point in the database, this is strange, but we will cover our bases
				//			Console.WriteLine ("Wut");
				return level.AStar(start, end);
			}

		}
		else
		{
			//			Console.WriteLine ("Wut");
			return level.AStar(start, end);
		}
	}



	public static SimPoint[] GetLongestPath(SimLevel level)
	{
		/*		Pair output;
		 * if (Farthest.L)
		{
			return output.LongestPath;
		}
		else
		{
			return null;
		}*/
		return longestPath;
	}

}


