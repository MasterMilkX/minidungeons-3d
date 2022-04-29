using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CellularAutomataGenerator : Creator {

	// the random variable	
	Random rand = new Random();
	
	// The map array
	public int[,] Map;
	
	// getters and setters for the properties width - height - wall percentage
	public int MapWidth { get; set; }
	public int MapHeight { get; set; }
	public int PercentAreWalls { get; set; }

	// constructor that randomly fills a map with walls
	public CellularAutomataGenerator(bool random)
	{
		MapWidth = 10;
		MapHeight = 20;
		PercentAreWalls = 40;

		// randomly fill
		RandomFillMap();
		// apply cellular Automata
		MakeCave(); // this only gives us a big room 
		if (random)
		{
			RandomFillMap(); // to have a better map randomly fill it again
		}
		// print the map
		PrintMap();
	}


	public override void GenerateMap()
	{
		bool random = false;
		MapWidth = 10;
		MapHeight = 20;
		PercentAreWalls = 40;

		// randomly fill
		RandomFillMap();
		// apply cellular Automata
		MakeCave(); // this only gives us a big room 
		if (random)
		{
			RandomFillMap(); // to have a better map randomly fill it again
		}
	}
	public void MakeCave() // The cellular Automata starts here
	{
		// loop through to create the cave based on wall logic
		for (int column = 0, row = 0; row <= MapHeight - 1; row++)
		{
			for (column = 0; column <= MapWidth - 1; column++)
			{
				Map[column, row] = WallLogicPlacment(column, row);
			}
		}
	}

	public int WallLogicPlacment(int x, int y)
	{   // check the number of walls around it's neighbors [top - bottom - left  - right]
		int numberOfWalls = GetNeighborWalls(x, y, 1, 1);


		if (Map[x, y] == 1)
		{
			if (numberOfWalls >= 4)
			{
				return 1;
			}
			if (numberOfWalls < 2)
			{
				return 0;
			}

		}
		else
		{
			if (numberOfWalls >= 5)
			{
				return 1;
			}
		}
		return 0;
	}

	public int GetNeighborWalls(int x, int y, int offsetX, int offsetY)
	{
		// return the number of walls around the cell 

		// the left neighbor 	
		int startX = x - offsetX;
		// the bottom neighbor
		int startY = y - offsetY;
		// the right neighbor
		int endX = x + offsetX;
		// the top neighbor 
		int endY = y + offsetY;

		int iX = startX;
		int iY = startY;

		int wallCounter = 0;

		for (iY = startY; iY <= endY; iY++)
		{
			for (iX = startX; iX <= endX; iX++)
			{
				if (!(iX == x && iY == y))
				{
					if (CheckIfWall(iX, iY))
					{
						wallCounter += 1;
					}
				}
			}
		}
		return wallCounter;
	}

	bool CheckIfWall(int x, int y)
	{
		// Consider out-of-bound a wall
		if (IsOutOfBounds(x, y))
		{
			return true;
		}

		if (Map[x, y] == 1)
		{
			return true;
		}

		if (Map[x, y] == 0)
		{
			return false;
		}
		return false;
	}

	bool IsOutOfBounds(int x, int y)
	{
		if (x < 0 || y < 0)
		{
			return true;
		}
		else if (x > MapWidth - 1 || y > MapHeight - 1)
		{
			return true;
		}
		return false;
	}

	public override void PrintMap()
	{
		Console.Write(MapToString());
	}

	string MapToString()
	{
		string returnString = "";

		List<string> mapSymbols = new List<string>();
		mapSymbols.Add(" ");
		mapSymbols.Add("X");

		for (int column = 0, row = 0; row < MapHeight; row++)
		{
			for (column = 0; column < MapWidth; column++)
			{
				returnString += mapSymbols[Map[column, row]];
			}
			returnString += Environment.NewLine;
		}
		return returnString;
	}

	/// <summary>
	/// Gets the map.
	/// </summary>
	/// <returns>The map.</returns>
	public override string[][] GetMap()
	{
		string returnString = MapToString();
		// split string into rows
		string[] rows = returnString.Split(Environment.NewLine.ToCharArray());

		string[][] returnMap = new string[MapHeight][];

		for (int i = 0; i < MapHeight; i++)
		{
			returnMap[i] = new[] { rows[i] };
		}


		return returnMap;
	}

	public void BlankMap()
	{   // clear the map and return an empty one
		for (int column = 0, row = 0; row < MapHeight; row++)
		{
			for (column = 0; column < MapWidth; column++)
			{
				Map[column, row] = 0;
			}
		}
	}


	public void RandomFillMap()
	{
		// New/empty map
		Map = new int[MapWidth, MapHeight];

		int mapMiddle = 0; // Temperary variable
		for (int column = 0, row = 0; row < MapHeight; row++)
		{
			for (column = 0; column < MapWidth; column++)
			{
				// if they're in the corners have them be a wall [border]
				if (column == 0)
				{
					Map[column, row] = 1;
				}
				else if (row == 0)
				{
					Map[column, row] = 1;
				}
				else if (column == MapWidth - 1)
				{
					Map[column, row] = 1;
				}
				else if (row == MapHeight - 1)
				{
					Map[column, row] = 1;
				}
				// otherwise, fill with a wall a random percent of the time
				else
				{
					mapMiddle = (MapHeight / 2);

					if (row == mapMiddle)
					{
						Map[column, row] = 0;
					}
					else
					{
						Map[column, row] = RandomPercent(PercentAreWalls);
					}
				}
			}
		}
	}

	int RandomPercent(int percent)
	{
		if (percent >= rand.Next(1, 101))
		{
			return 1;
		}
		return 0;
	}

	// constructor to tweak the valus if wanted
	public CellularAutomataGenerator(int mapWidth, int mapHeight, int[,] map, int percentWalls = 40)
	{
		this.MapWidth = mapWidth;
		this.MapHeight = mapHeight;
		this.PercentAreWalls = percentWalls;
		this.Map = new int[this.MapWidth, this.MapHeight];
		this.Map = map;
	}
}
