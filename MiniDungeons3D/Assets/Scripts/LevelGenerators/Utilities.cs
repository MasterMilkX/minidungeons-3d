using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


public static class Utilities
{
	static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
	[ThreadStatic]
	private static Random Local;
	public static int RandomSeed;
	class Position
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public static Random Random {
		
		get { return Local ?? (Local = new Random(RandomSeed = unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
	}

	/// <summary>
	/// Raycast the specified p1 and p2.
	/// </summary>
	/// <returns>The raycast.</returns>
	/// <param name="p1">P1.</param>
	/// <param name="p2">P2.</param>
	public static Pairing[] RayTrace(Pairing p1, Pairing p2)
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

		for (int tiles = n; tiles > 0; tiles--) {
			trace.Add(new Pairing(x, y));
			if (error > 0) {
				x += x_inc;
				error -= dy;
			} else {
				y += y_inc;
				error += dx;
			}
		}

		Pairing[] result = trace.ToArray();
		return result;
	}

	public static int DistanceBetween(Pairing start, Pairing end)
	{
		Pairing[] rayTrace = RayTrace(start, end);
		return rayTrace.Length;
	}

	public static bool LineOfSight(Pairing start, Pairing end, string[][] Map)
	{
		bool result = true;
		Pairing[] rayTrace = RayTrace(start, end);
		for (int point = 0; point < rayTrace.Length; point++) {
			if (Map[rayTrace[point].Y][rayTrace[point].X].Equals("X")) {
				result = false;
				break;
			}
		}
		return result;
	}
	public static string[][] CloneMap(string[][] o)
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
	public static int GetRandom(int limit)
	{
		byte[] box = new byte[1];
		do provider.GetBytes(box);
		while (!(box[0] < limit * (Byte.MaxValue / limit)));

		return box[0] % limit;
	}



	static void SwapListElements<T>(this IList<T> list, int swap1, int swap2)
	{
		T value = list[swap1];
		list[swap1] = list[swap2];
		list[swap2] = value;
	}

	public static void Shuffle<T>(IList<T> list)
	{

		int n = list.Count;
		while (n > 1) {
			int k = GetRandom(n);
			n--;
			SwapListElements(list, k, n);
		}
	}


	/// <summary>
	/// Connect disconnected rooms/components in the map.
	/// </summary>
	/// <returns>The map</returns>
	/// <param name="map">Map</param>
	public static String[][] ConnectComponents(String[][] map)
	{
		String[][] visited;
		int count;

		if (CheckDisconnectedComponents(map, out visited, out count)) {
			map = ConnectComponents(visited, count);
		}
		return map;
	}

	/// <summary>
	/// Checks whether there are disconnected rooms/components in the map.
	/// </summary>
	/// <returns>Presence of disconnected components</returns>
	/// <param name="map">Map</param>
	/// <param name="visited">Map with components labelled</param>
	/// <param name="count">Count of disconnected components</param>
	static bool CheckDisconnectedComponents(String[][] map, out String[][] visited, out int count)
	{
		int ROW = map.Length;
		int COL = map[0].Length;

		// Make a bool array to mark visited cells.
		// Initially all cells are unvisited
		visited = new String[ROW][];
		for (int i = 0; i < ROW; i++) {
			visited[i] = new String[COL];
			for (int j = 0; j < map[i].Length; j++) {
				visited[i][j] = "X";
			}
		}

		// Initialize count as 0 and travese through the all cells
		// of given matrix
		count = 0;
		String label = count.ToString();
		for (int i = 0; i < ROW; ++i)
			for (int j = 0; j < COL; ++j) {
				if (map[i][j].Equals(" ") && visited[i][j].Equals("X")) {
					label = (++count).ToString();
					DFS(map, i, j, visited, label);
				}
			}

		return count > 1;
	}

	static void DFS(String[][] map, int y, int x, String[][] visited, String label)
	{
		// These arrays are used to get row and column numbers
		// of 4 neighbors of a given cell
		int[] yNbr = new int[] { -1, 0, 0, 1 };
		int[] xNbr = new int[] { 0, -1, 1, 0 };

		// Mark this cell as visited
		visited[y][x] = label;

		// Recur for all connected neighbours
		for (int k = 0; k < 4; ++k)
			if (isSafe(map, y + yNbr[k], x + xNbr[k], visited))
				DFS(map, y + yNbr[k], x + xNbr[k], visited, label);

	}

	static bool isSafe(string[][] map, int y, int x, String[][] visited)
	{
		return (y >= 0) && (y < map.Length) &&
			(x >= 0) && (x < map[0].Length) &&
			(map[y][x].Equals(" ") && visited[y][x].Equals("X"));
	}

	static String[][] ConnectComponents(String[][] map, int count)
	{
		int ROW = map.Length;
		int COL = map[0].Length;
		Position[] labels = new Position[count];

		for (int i = 0; i < ROW; i++) {

			for (int j = 0; j < COL; j++) {

				if (!map[i][j].Equals("X")) {
					int index = Convert.ToInt32(map[i][j]);
					index -= 1;

					if (labels[index] == null)
						labels[index] = new Position(i, j);
				}
			}
		}

		for (int i = 0; i < labels.Length - 1; i++) {
			Position from = labels[i];
			Position to = labels[i + 1];

			if (Random.Next(0, 1) == 0) {
				PropagateHorizontally(map, from, to);
				PropagateVertically(map, from, to);
			} else {
				PropagateVertically(map, from, to);
				PropagateHorizontally(map, from, to);
			}
		}

		for (int i = 0; i < ROW; i++) {
			for (int j = 0; j < COL; j++) {
				if (!map[i][j].Equals("X"))
					map[i][j] = " ";
			}
		}

		return map;
	}

	static void PropagateHorizontally(String[][] map, Position from, Position to)
	{
		if (from.Y < to.Y)
			while (from.Y < to.Y)
				map[from.X][++(from.Y)] = " ";
		else
			while (from.Y > to.Y)
				map[from.X][--(from.Y)] = " ";
	}

	static void PropagateVertically(String[][] map, Position from, Position to)
	{
		if (from.X < to.X)
			while (from.X < to.X)
				map[++(from.X)][from.Y] = " ";
		else
			while (from.X > to.X)
				map[--(from.X)][from.Y] = " ";
	}


	public static int CountDeadEnds(String[][] map)
	{
		int noOfDeadEnds = 0;
		for (int i = 1; i < map.Length - 1; i++) {
			for (int j = 1; j < map[i].Length - 1; j++) {
				if (map[i][j] != "X") {
					int counter = map[i - 1][j] == "X" ? 1 : 0;
					counter = map[i + 1][j] == "X" ? counter + 1 : counter;
					counter = map[i][j - 1] == "X" ? counter + 1 : counter;
					counter = map[i][j + 1] == "X" ? counter + 1 : counter;

					if (counter >= 3) {
						noOfDeadEnds++;
					}
				}
			}
		}
		return noOfDeadEnds;
	}

	public static void CountDeadEndsByFile(String filePath, String[] folderNames)
	{
		String line;

		// Read the file and display it line by line.

		//String filePath = "/Bitbucket/minidungeonsdemo/minidungeonsdemo/Maps/";
		//String[] folderNames = { "Cellular Solo", "Cellular-Cellular", "Cellular-Constraint", "Cellular-Grammar", "Cross Cellular-Cellular", "Cross Cellular-Constraint", "Cross Cellular-Grammar", "Cross Digger-Cellular", "Cross Digger-Constraint", "Cross Digger-Grammar", "Cross Grammar-Cellular", "Cross Grammar-Constraint", "Cross Grammar-Grammar", "Digger Solo", "Digger-Cellular", "Digger-Constraint", "Digger-Grammar", "Grammar Solo", "Grammar-Cellular", "Grammar-Constraint", "Grammar-Grammar", "Single Cellular-Cellular", "Single Cellular-Constraint", "Single Cellular-Grammar", "Single Digger-Cellular", "Single Digger-Constraint", "Single Digger-Grammar", "Single Grammar-Cellular", "Single Grammar-Constraint", "Single Grammar-Grammar" };

		foreach (String folderName in folderNames) {
			var csv = new StringBuilder();
			for (int no = 0; no < 1000; no++) {
				StreamReader file =
						  new StreamReader(filePath + folderName + "/Map_" + no + ".txt");
				String[][] map = new String[20][];
				int counter = 0;
				while ((line = file.ReadLine()) != null) {
					int i = 0;
					map[counter] = new String[line.Length];
					foreach (char c in line.ToCharArray()) {
						map[counter][i] = c.ToString();
						i++;
					}
					counter++;
				}

				file.Close();

				//write in csv
				csv.AppendLine(CountDeadEnds(map).ToString());
			}
			File.WriteAllText(filePath + folderName + "/" + folderName + ".csv", csv.ToString());
		}
	}
	public static SimLevel BuildDatabase(string[][] map)
	{
		string mapString = "";
		// get the full level string
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				mapString += map[i][j];
			}
			if (i != map.Length - 1)
			{
				mapString += "\n";
			}
		}
		// make a simlevel out of this

		SimLevel level = new SimLevel(mapString);
		if (level == null)
		{
			return null;
		}
		PathDatabase.GameType = 1;
		PathDatabase.BigDatabase = new List<Dictionary<string, SimPoint[]>>();
		PathDatabase.levels = new ArrayList();
		PathDatabase.Farthest = new Dictionary<SimLevel, Pair>();

		PathDatabase.BuildDB(level);
		return level;
	}

	public static string MapToString(string[][] map)
	{
		string mapString = "";

		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				mapString += map[i][j];
			}
			mapString += "\n";
		}
		return mapString;	
	}
}


