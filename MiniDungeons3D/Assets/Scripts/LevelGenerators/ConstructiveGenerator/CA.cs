using System.Collections;
using System.Collections.Generic;
using System;


public class CA : Creator{

	// getters and setters for the properties width - height - wall percentage
	public int width { get; set; }
	public int height { get; set; }
	public int ifWallPercentage { get; set; }
	public int ifFloorPercentage { get; set; }
	public int wallProbability { get; set; }

	// The grid of cells
	public String[][] Map { get; set; }
	public String[][] newCell { get; set; }
	// the random variable
	Random random = new Random();

	// Thw wall and room counts
	public float TotalCellsInGrid { get; set; }
	public float floorPercentage { get; set; }
	public float wallPercentage { get; set; }

	// pistions of the floors in the map
	public List<int> x = new List<int>();
	public List<int> y = new List<int>();

	// offseting position for adjustment
	int[] posableOffsets = { -1, 0, +1 };

	public CA()
	{
		width = 10;
		height = 20;
		wallProbability = 45;
		ifWallPercentage = 4;
		ifFloorPercentage = 5;
		wallPercentage = 0;
		floorPercentage = 0;
		TotalCellsInGrid = this.width* this.height;
		this.Map = new String[height][];
	}

	public void Initialize()
	{
		width = 10;
		height = 20;
		wallProbability = 45;
		ifWallPercentage = 4;
		ifFloorPercentage = 5;
		wallPercentage = 0;
		floorPercentage = 0;
		TotalCellsInGrid = this.width* this.height;
		this.Map = new String[height][];

		//initialize the jagged array
		for (int i = 0; i < height; i++)
		{
			this.Map[i] = new String[10];
		}

	}

	/// <summary>
	/// Randomly fill the 2D int array with walls based on the percentage specified in the varaible
	/// wall percentage. Make sure to fill the corners with walls
	/// </summary>
	public void RandomFill()
	{

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// if the cell is in the corner, make sure they're walls
				if (x == 0) { Map[y][x] = "X"; }
				else if (y == 0) { Map[y][x] = "X"; }
				else if (x == (width - 1)) { Map[y][x] = "X"; }
				else if (y == (height - 1)) { Map[y][x] = "X"; }
				// otherwise fill it with walls based on a percentage [ in our code it's 45% ]
				else
				{
					if (random.Next(1, 101) < wallProbability) { Map[y][x] = "X"; }
					else { Map[y][x] = " "; }
				}
			}
		}
	}

	public void CAGeneration(int numberOfGens)
	{
			// initialize a map
			Initialize();

			// randomize the map with walls
			RandomFill();

		//Console.WriteLine("Random Fill:");
		//	PrintMap();
		//Console.WriteLine("");
			// run CA a number of times [based on the numberOfGenerations variable]
			for (int i = 0; i < numberOfGens; i++)
			{
				this.Map = ImplementCARuleset();
                //PrintMap();
			//Console.WriteLine("");

			}

			// make sure the map is good
			Restraint();

			// clean map
			ConnectIslands();

			// clear list
			ClearPositionOfRoomList();
	}

	/// <summary>
	/// A new rulset to generate CA caves
	/// </summary>
	public String[][] ImplementCARuleset()
	{
		// Reset wall and floor counter
		ResetWallsAndFloors();

		// temporary array that holds the new states of the cells
					this.newCell = new String[height][];

		//initialize the jagged array
		for (int i = 0; i < height; i++)
		{
						this.newCell[i] = new String[10];
		}


		// initialize the new cell grid with floors
		// make sure that the edges are walls
		for (int y = 0; y<height; y++)
		{
			for (int x = 0; x<width; x++)
			{
				// if the cell is in the corner, make sure they're walls
				if (x == 0) { this.newCell[y][x] = "X"; wallPercentage += 1;}
				else if (y == 0) { this.newCell[y][x] = "X"; wallPercentage += 1;}
				else if (x == (width - 1)) { this.newCell[y][x] = "X"; wallPercentage += 1;}
				else if (y == (height - 1)) { this.newCell[y][x] = "X"; wallPercentage += 1;}
				// otherwise fill it with walls based on a percentage [ in our code it's 45% ]
				else
				{
					this.newCell[y][x] = " ";
				}
			}
		}


		// loop over every cell in the grid
		for (int y = 1; y < height - 1; y++)
		{
			for (int x = 1; x < width - 1; x++)
			{
				// iterate over each cell and implement the new ruleset
				Rules(newCell, y, x);
			}
		}

		return newCell;
	}


	/// <summary>
	/// A new ruleset for the walls to test different maps
	/// here the rules are as follows:
	/// [if the cell is a wall] and
	/// 1. for a 3x3 grid if it's serounded by at least 5 walls it becomes a wall
	/// 2. if iy has exactly 2 neighboring walls it becomes a wall
	/// 3. otherwise it becomes a floor
	/// [if the cell is a floor]
	///  it stays a floor
	/// </summary>
	public void Rules(string[][] newCells, int y, int x)
	{
		// grab the number of neighbor walls
		int countNeighbors = CountNeighborWalls(y, x);

		// the rules that depicts if we get a wall or not based on the 4-5 method
		// if the cell is a wall
		if (this.Map[y][x] == "X")
		{
			if (countNeighbors >= 5) { newCells[y][x] = "X"; wallPercentage += 1; }
			else if (countNeighbors == 2) { newCells[y][x] = "X"; wallPercentage += 1; }
			else
			{
				newCells[y][x] = " ";
				floorPercentage += 1;

				// add the point to the list of floor position
				this.x.Add(x);
				this.y.Add(y);
			}
		}
		//otherwise if its a floor then
		else
		{
			newCells[y][x] = " ";
			floorPercentage += 1;

			// add the point to the list of floor position
			this.x.Add(x);
			this.y.Add(y);

		}
	}

	/// <summary>
	/// function to count the number of walls around a cell
	/// the function checks all it's 8 neighbors and sees how
	/// many walls are there
	/// ____________________________________________________
	/// |              |                 |                 |
	/// | x -1, y -1   |    x , y -1     | x +1, y -1      |
	/// |              |                 |                 |
	/// ____________________________________________________
	/// |              |                 |                 |
	/// | x -1, y 	   |    x , y	     | x +1, y	       |
	/// |              |                 |                 |
	/// ____________________________________________________
	/// |              |                 |                 |
	/// | x -1, y +1   |    x , y +1     | x +1, y +1      |
	/// |              |                 |                 |
	/// ____________________________________________________
	/// </summary>
	public int CountNeighborWalls(int y, int x)
	{
		// varable that holds the number of walls in the neighborhood
		int neighbors = 0;

		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i == 0 && j == 0) { continue; }
				if (this.Map[y+i][x+j] == "X")
				{
					neighbors += 1;
				}
			}
		}

		return neighbors;
	}

	public void Restraint()
	{
		// Check the percentages to make sure we get a good map
		bool seed = true;
		int loopCounter = 0;
		while (seed)
		{
			loopCounter += 1;

			if (floorPercentage > 75 /*&& CountConnectedComponents(this.Cell) < 35*/) { seed = !seed; break;}
			else {
				// randomlly fill the grid with walls
				RandomFill();
				// perform the CA rules on the grid map
				CAGeneration(1);
			}
		}


	}

	public void ConnectIslands()
	{

		for (int i = 0; i < this.x.Count; i++)
		{
			// add the rulset to connect the islands
			ConnectingRules(this.Map, this.y[i], this.x[i]);
		}
	}

	/// <summary>
	/// Connecting islands to the map
	/// this function solves the isolated islands by
	/// checking the neighbors of a cell
	/// but in this case it doesn't check all 8 neighbors but rather 4
	/// since the player doesn't move diagonaly
	/// it counts the neighborhood floors if we have more than two move on
	/// otherwise turn the current cell to a wall
	/// </summary>
	public void ConnectingRules(String[][] cell, int y, int x)
	{
		// grab the number of neighbor walls
		int countNeighbors = CountNeighborRooms(y, x);

		// if I have less that 1 room then it means I am surrounded by walls
		// so pick a random directio and turn it to an empty room
		if (countNeighbors < 2)
		{
			int xOffset = 0;
			int yOffset = 0;
			bool keepGoing = true;

			// get a random off set to turn into a room
			while (keepGoing)
			{
				xOffset = posableOffsets[random.Next(0, 3)];
				yOffset = posableOffsets[random.Next(0, 3)];

				if (xOffset == 0 && yOffset == 0) { continue; }
				else { keepGoing = false; }
			}

			//cell[x + xOffset, y + yOffset] = 0;
			cell[y][x] = "X";

		}
	}

	/// <summary>
	/// function to count the number of rooms around an emppty cell
	/// We will check teh four neighbors not 8
	/// since in this game the player moves in four directions
	/// so no need to check the diagonal movment
	/// </summary>
	public int CountNeighborRooms(int y, int x)
	{
		// varable that holds the number of walls in the neighborhood
		int neighbors = 0;

		// corner variables
		int top = y - 1;
		int bottom = y + 1;
		int right = x + 1;
		int left = x - 1;

		if (this.Map[top][x] == " ") { neighbors += 1; }
		if (this.Map[bottom][x] == " ") { neighbors += 1; }
		if (this.Map[y][left] == " ") { neighbors += 1; }
		if (this.Map[y][right] == " ") { neighbors += 1; }

		return neighbors;
	}


	/// <summary>
	/// Resets the walls and floors. For counting
	/// </summary>
	public void ResetWallsAndFloors()
	{
		this.wallPercentage = 0;
		this.floorPercentage = 0;
	}


	public override void GenerateMap()
	{
		// CA generator
		//do {
		//    	  CAGeneration(1);
		//}
		//while (!Utilities.CheckConnectivity(Map));
		CAGeneration(1);
		Map = Utilities.ConnectComponents(Map);
	}


	/// <summary>
	/// A function that turns the binary 2D array into a
	/// string array of 'X's and ' ' charachters
	/// </summary>
	public override string[][] GetMap()
	{

		return this.Map;
	}


	public override void PrintMap()
	{
		for (int i = 0; i < this.Map.Length; i++)
		{
			for (int j = 0; j < this.Map[i].Length; j++)
			{
				Console.Write(this.Map[i][j]);
			}
			Console.WriteLine("");
		}
		Console.WriteLine("");
		Console.WriteLine("");
	}


	/// <summary>
	/// Clears the position of room list. Since the room keep updating
	/// </summary>
	public void ClearPositionOfRoomList()
	{
		this.x.Clear();
		this.y.Clear();
	}



// THE CONNECTED COMPONENT PART

/// <summary>
/// Check if i can iclude the count or not [based on visited]
/// </summary>
public bool CanCheckCell(int[,] cell, int row, int col, bool[,] visited) {
	// row is in range, and th ecolumn is in range
	// and teh value of the cell is 0 [floor] and it hasn't been visited yet
	return (row >= 0) && (row < this.height) &&
			 (col >= 0) && (col < this.width) &&
			 (cell[row,col] == 0 && !visited[row,col]);
}

/// <summary>
/// A function that performs a depth first search
/// it considers the 8 neighbors as adjacent cells
/// </summary>
public void DFS(int[,] cell, int row, int col, bool[,] visited) {
	// variables that will be used to locate the newbors positions
	int[] rowNumber = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };
	int[] colNumber = new int[] { -1,  0,  1,-1, 1,-1, 0, 1 };

	// mark the cell as visited
	visited[row, col] = true;

	// repeat for all connected neighbors
	for (int i = 0; i < 8; ++i) {
		if (CanCheckCell(cell, row + rowNumber[i], col + colNumber[i], visited))
		{ DFS(cell, row + rowNumber[i], col + colNumber[i], visited);}
	}

}

public int CountConnectedComponents(int[,] cells) {
	// create a boolean array that keep strack of visited cells
	bool[,] visited = new bool[this.width, this.height];

	// initialize count as 0 before traversing through the cells
	int count = 0;
	for (int y = 1; y < this.height; y++)
	{
		for (int x = 1; x < this.width; x++)
		{
			// if a cell with a value of 0 [floor] is not visited
			// then new island is found
			// visit all cells in this island and incremant the count

			if (cells[x, y] == 0 && !visited[x, y])
			{
				DFS(cells, x, y, visited);
				++count;
			}
		}
	}

	return count;
}

}
