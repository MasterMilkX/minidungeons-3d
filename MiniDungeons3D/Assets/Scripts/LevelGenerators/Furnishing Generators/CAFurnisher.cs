using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CAFurnisher : Furnisher {

	// the map we will populate
		public String[][] Map { get; set; }

		public int width { get; set; }
		public int height { get; set; }

		// the random variable
		Random random = new Random();

		// pistions of the floors in the map
		public List<int> x = new List<int>();
		public List<int> y = new List<int>();

		// Entrance and Exit variables
		public int ExitMax { get; set; }
		public int ExitCount { get; set; }

		public int EntranceMax { get; set; }
		public int EntranceCount { get; set; }

		public int MinimumDistanceBetweenEntranceAndExit { get; set; }
		public int entranceY;
		public int entranceX;
		public int exitY;
		public int exitX;
		int distance;

		// The items to place
		// The monsters
		public int MinitaurMax { get; set; }
		public int MinitaurCount { get; set; }
		public double MinitaurProbability { get; set; }

		public int MeleeMax { get; set; }
		public int MeleeCount { get; set; }
		public double MeleeProbability { get; set; }
		public List<int> MaleeX = new List<int>();
		public List<int> MaleeY = new List<int>();

		public List<int> portal1X = new List<int>();
		public List<int> portal1Y = new List<int>();
		public List<int> portal2X = new List<int>();
		public List<int> portal2Y = new List<int>();

		public int RangeMonsterMax { get; set; }
		public int RangeMonsterCount { get; set; }
		public double RangeMonsterProbability { get; set; }

		public int OgreMonsterMax { get; set; }
		public int OgreMonsterCount { get; set; }
		public double OgreMonsterProbability { get; set; }

		public int BlobMax { get; set; }
		public int BlobCount { get; set; }
		public double BlobProbability { get; set; }

		// The Objects
		public int PotionMax { get; set; }
		public int PotionCount { get; set; }
		public double PotionProbability { get; set; }

		public int TreasureMax { get; set; }
		public int TreasureCount { get; set; }
		public double TreasureProbability { get; set; }

		public int TrapMax { get; set; }
		public int TrapCount { get; set; }
		public double TrapProbability { get; set; }

		public int PortalMax { get; set; }
		public int PortalCount { get; set; }
		public double PortalProbability { get; set; }

		// the plotting and rule to check
		public enum Plot
		{
			Treasure,
			Minitaur,
			Trap,
			Potion,
			Blob,
			Ogre,
			Range,
			Melee,
			Portal
		};


		public void Initializer()
		{
			// dimention variables
			this.width = 10;
			this.height = 20;

      this.Map = new String[this.height][];

			// Entrance and Exit variables
			ExitMax = 1;
			EntranceMax = 1;
			ExitCount = 0;
			EntranceCount = 0;
			MinimumDistanceBetweenEntranceAndExit = 15;

			// Monster variables
			MinitaurCount = 0;
			MeleeCount = 0;
			RangeMonsterCount = 0;
			OgreMonsterCount = 0;
			BlobCount = 0;

			MeleeMax = random.Next(2, 4);
			RangeMonsterMax = random.Next(1, 3);
			MinitaurMax = 1;
			BlobMax = 3;
			OgreMonsterMax = 1;

			// object variables
			TreasureCount = 0;
			PotionCount = 0;
			TrapCount = 0;
			PortalCount = 0;

			TreasureMax = random.Next(2, 6);
			PotionMax = random.Next(4, 8);
			TrapMax = 2;
			PortalMax = 2;

			entranceY = 0;
			entranceX = 0;
			exitY = 0;
			exitX = 0;
			distance = 0;
		}

		public CAFurnisher()
		{
      Initializer();
		}

		public CAFurnisher(String[][] map)
		{
			Initializer();
			this.Map = map;
		}

		public override void GenerateMap()
		{
			Initializer();

			// Get floor positions
			FloorPositions(this.Map);

			// start furnishing
			CAFurnishing();

			FixPortals();
			//this.Map = GetMap();
		}

	public void FixPortals()
	{
		int firstX = 0;
		int firstY = 0;
		bool firstExists = false;
		bool secondExists = false;
		for (int i = 0; i < Map.Length; i++)
		{
			for (int j = 0; j < Map[i].Length; j++)
			{
				if (Map[i][j].Equals("p") && !firstExists)
				{
					firstExists = true;
					firstX = j;
					firstY = i;
				}
				else if (Map[i][j].Equals("p") && !secondExists)
				{
					secondExists = true;
				}
			}
		}

		if (firstExists && !secondExists)
		{
			Map[firstY][firstX] = " ";
		}
	}
		public override void GenerateMap(String[][] map)
		{
			Initializer();

			this.Map = map;

			// Get floor positions
			FloorPositions(this.Map);

			// start furnishing
			CAFurnishing();
			

			//this.Map = GetMap();
			
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
		/// A fucntion that grabs the position of the floors
		/// so we wont constantly traverse the 2d array when searching
		/// we just use the open area positions
		/// so when it incounters a floor it stores its location
		/// </summary>
		public void FloorPositions(String[][] map)
		{
			// first clear the list
			ClearPositionOfRoomList();

			// go through the map and save the floor positions
			for (int y = 0; y < this.height; y++)
			{
				for (int x = 0; x < this.width; x++)
				{
					if (map[y][x] == " ")
					{
						this.x.Add(x);
						this.y.Add(y);
					}
				}

			}
		}

		/// <summary>
		/// Clears the position of room list. Since the room keep updating
		/// </summary>
		public void ClearPositionOfRoomList()
		{
			this.x.Clear();
			this.y.Clear();

			this.portal1Y.Clear();
			this.portal1X.Clear();
			this.portal2X.Clear();
			this.portal2Y.Clear();
		}

		public void CAFurnishing()
		{
		//Console.WriteLine("Begin");
		//	PrintMap();
		//Console.WriteLine("");
			// plot the exit and entrance
			PlotEntranceAndExit();
		//PrintMap();
		//Console.WriteLine("");
			// plot the treature
			ImplementCAFurnisher(Plot.Treasure);

			// plot the potion
			ImplementCAFurnisher(Plot.Potion);

			// plot the melee mosters
			ImplementCAFurnisher(Plot.Melee);

			// plot the melee mosters
			ImplementCAFurnisher(Plot.Range);

		//PrintMap();
		//Console.WriteLine("");

			// plot the melee mosters
			ImplementCAFurnisher(Plot.Blob);

			// plot the org mosters
			ImplementCAFurnisher(Plot.Ogre);

			// plot the Minitaur mosters
			ImplementCAFurnisher(Plot.Trap);

			// plot the Minitaur mosters
			ImplementCAFurnisher(Plot.Minitaur);

			// plot the Minitaur mosters
			ImplementCAFurnisher(Plot.Portal);

		//PrintMap();
		//Console.WriteLine("");

		}

		public void PlotEntranceAndExit()
		{
			// The contrlling conditions
			bool findLocationForBoth = false;
			bool findEntance = false;
			bool findExit = false;

			// entrance and exit positions
			int entreX = 0;
			int entreY = 0;
			int exitX = 0;
			int exitY = 0;


			while (!findLocationForBoth)
			{
				// first placement
				for (int i = 0; i < this.x.Count; i++)
				{
					if (random.Next(0, 101) < 10)
					{
						if (!findEntance && !findExit)
						{
							int choice = random.Next(0, 101);
							if (choice < 50)
							{
								// grab the position of the floor
								entreX = this.x[i];
								entreY = this.y[i];
								findEntance = true;
								break; // exit the loop

							}
							else
							{
								// grab the position of the floor
								exitX = this.x[i];
								exitY = this.y[i];
								findExit = true;
								break; // exit the loop
							}
						}
					}
				}

				// second placment
				for (int i = 0; i < this.x.Count; i++)
				{
					this.distance = Math.Abs(this.y[i] - this.entranceY);

					if (this.distance >= MinimumDistanceBetweenEntranceAndExit && random.Next(0, 101) < 30)
					{
						if (!findExit)
						{
							// grab the position of the floor
							exitX = this.x[i];
							exitY = this.y[i];
							findExit = true;
							break; // exit the loop
						}

						if (!findEntance)
						{
							// grab the position of the floor
							entreX = this.x[i];
							entreY = this.y[i];
							findEntance = true;
							break; // exit the loop
						}
					}
				}

				if (findExit && findEntance)
				{
					findLocationForBoth = true;

					// assign the new locations
					this.Map[entreY][entreX] = "H";
					this.Map[exitY][exitX] = "e";

					// increment the counters
					this.ExitCount++;
					this.EntranceCount++;
					break; // exit the loop
				}
				else
				{
					int entrance = random.Next(0, this.x.Count / 4);
					// assign the new locations
					this.Map[this.y[entrance]][this.x[entrance]] = "H";

					int exit = random.Next(this.x.Count / 5, this.x.Count);
          this.Map[this.y[exit]][this.x[exit]] = "e";

					findLocationForBoth = true;

					// increment the counters
					this.ExitCount++;
					this.EntranceCount++;
					break; // exit the loop

					// reset
					//findExit = false;
					//findEntance = false;
				}
			}

			this.entranceX = entreX;
			this.entranceY = entreY;
			this.exitX = exitX;
			this.exitY = exitY;
		}

		/// <summary>
		/// Implements the CA Furnisher.
		/// It travers the map and based on what we're plotting
		/// ot users the rule to save the possable plotting positions
		/// as based on the plotting item it counts neighbors in diferent ways
		/// </summary>
		public void ImplementCAFurnisher(Plot item)
		{
			// makes a copy of the map so we can manipulate the
			// temporary array that holds the new states of the cells
			String[][] newCell = new String[height][];

			//initialize the jagged array
			for (int i = 0; i < height; i++)
			{
				newCell[i] = new String[10];
			}

			newCell = NewMapCopy(this.Map);

			// clear the positions since the map changed
			FloorPositions(newCell);

			// loop over every cell in the grid
			for (int y = 1; y < this.height - 1; y++)
			{
				for (int x = 1; x < this.width - 1; x++)
				{

					// grab the number of neighbor walls
					int countNeighbors = CountNeighborWalls(newCell, y, x, item);

					// check if it's a floor
					if (newCell[y][x] == " ")
					{
						// if what we're trying to plot is a tresure check this rule
						if (item == Plot.Treasure)
						{

							// Treasure rule shoud have a neighborhood of walls at least 3
							// if so marke it as a possable position for the reasure
							if (countNeighbors >= 3)
							{
								this.x.Add(x);
								this.y.Add(y);
							}

						}

						// if what we're trying to plot is a potions check this rule
						if (item == Plot.Potion)
						{
							// Potion rule shoud have a neighborhood of walls at most 3
							// if so marke it as a possable position for the reasure
							if (countNeighbors <= 3 && countNeighbors >= 0)
							{
								this.x.Add(x);
								this.y.Add(y);
							}
						}

						// if what we're trying to plot is a Malee monster check this rule
						if (item == Plot.Melee)
						{
							// Malee rule shoud have a neighborhood of walls at least 7
							// if so marke it as a possable position for the reasure
							if (countNeighbors >= 7)
							{
								this.x.Add(x);
								this.y.Add(y);
							}
						}

						if (item == Plot.Range)
						{
							// Range rule shoud have a neighborhood of Malee at least 1
							// if so marke it as a possable position for the reasure
							if (countNeighbors >= 1)
							{
								this.x.Add(x);
								this.y.Add(y);
							}
						}

						if (item == Plot.Blob)
						{
							// blobs want to be around potions within a reasnoable distance
							if (countNeighbors >= 1)
							{
								this.x.Add(x);
								this.y.Add(y);
							}
						}

						if (item == Plot.Ogre)
						{
							// ogres like to be around open spaces
							if (countNeighbors < 1)
							{
								this.x.Add(x);
								this.y.Add(y);
							}
						}

						if (item == Plot.Trap)
						{
							// traps like to be around populated areas
							if (countNeighbors >= 5)
							{
								this.x.Add(x);
								this.y.Add(y);
							}
						}

						// if (item == Plot.Portal)
						// {
						// 	// if an entrance or an exit if around you save the area
						// 	if (countNeighbors == 1)
						// 	{
						// 		this.x.Add(x);
						// 		this.y.Add(y);
						// 	}
						// }

					}

				}
			}


			// start plotting the original map
			StartPlotting(newCell, item);
		}

		/// <summary>
		/// The poltter function
		/// it checks the maps value [based on the labled number]
		/// and checks if we can place it still
		/// if so it plots the item there based on a random position
		/// </summary>
		public void StartPlotting(String[][] map, Plot item)
		{

			// loop over every cell in the grid
			for (int i = 1; i < this.y.Count - 1; i++)
			{
				if (item == Plot.Treasure && this.TreasureCount < this.TreasureMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
					this.Map[this.y[randomPosition]][this.x[randomPosition]] = "T";
					this.TreasureCount++;
				}

				else if (item == Plot.Potion && this.PotionCount < this.PotionMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "P";
					this.PotionCount++;
				}

				else if (item == Plot.Melee && this.MeleeCount < this.MeleeMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "M";
					this.MeleeCount++;
				}

				else if (item == Plot.Range && this.RangeMonsterCount < this.RangeMonsterMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "R";
					this.RangeMonsterCount++;
				}

				else if (item == Plot.Blob && this.BlobCount < this.BlobMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "b";
					this.BlobCount++;
				}

				else if (item == Plot.Ogre && this.OgreMonsterCount < this.OgreMonsterMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "o";
					this.OgreMonsterCount++;
				}

				else if (item == Plot.Minitaur && this.MinitaurCount < this.MinitaurMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "m";
					this.MinitaurCount++;
				}

				else if (item == Plot.Trap && this.TrapCount < this.TrapMax)
				{
					// pick a random spot
					int randomPosition = random.Next(0, this.y.Count);
                    this.Map[this.y[randomPosition]][this.x[randomPosition]] = "t";
					this.TrapCount++;
				}

				else if (item == Plot.Portal && this.PortalCount < this.PortalMax)
				{

					//if there are no points to plot in then don't plot any portals
					if (this.portal1Y.Count == 0 || this.portal2Y.Count == 0 || this.portal1X.Count == 0 || this.portal2X.Count == 0)
					{
						break;
					}
					else
					{
						if (this.portal1Y.Count > 0 && this.portal2Y.Count > 0 && this.portal1X.Count > 0 &&  this.portal2X.Count > 0) {
							// pick a random spot
							int randomFirstPortal = random.Next(0, this.portal1Y.Count);
							// plot the first portal
							this.Map[this.portal1Y[randomFirstPortal]][this.portal1X[randomFirstPortal]] = "p";
                    		this.PortalCount++;

							int randomSecondPortal = random.Next(0, this.portal2Y.Count);
							// plot the first portal
							this.Map[this.portal2Y[randomSecondPortal]][this.portal2X[randomSecondPortal]] = "p";
                    		this.PortalCount++;
						} else {
							break;
						}
					}
				}

			}
		}


		public int CountNeighborWalls(String[][] map, int y, int x, Plot item)
		{
			// varable that holds the number of walls in the neighborhood
			int neighbors = 0;

			// if we're plotting treasures or potions count this way
			if (item == Plot.Treasure || item == Plot.Potion)
			{
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (i == 0 && j == 0) { continue; }
						else if (map[y + i][x + j] == "X") { neighbors += 1; }

					}
				}
			}

			// if we're plotting a malee monster count this way
			if (item == Plot.Melee)
			{
				// if we can check over three tiles from all sides
				// then if we encounter a malee don't count a wall
				// and decrease from the counter to avoide having
				// malee monsters next to each other
				if (y >= 3 && x >= 3 && x <= 6 && y <= 16)
				{
					for (int i = -3; i <= 3; i++)
					{
						for (int j = -3; j <= 3; j++)
						{
							if (i == 0 && j == 0) { continue; }
							else if (map[y + i][x + j] == "M") { neighbors -= 1; }
							else if (map[y + i][x + j] == "X") { neighbors += 1; }

						}
					}
				}
				// if we're on the edges and can't count three tiles away count the normal way
				else
				{
					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							if (i == 0 && j == 0) { continue; }
							else if (map[y + i][x + j] == "M") { neighbors -= 1; }
							else if (map[y + i][x + j] == "X") { neighbors += 1; }
						}
					}
				}

			}

			// if we're ploting range mosters count this way
			if (item == Plot.Range)
			{
				// range want to be around a malee so if we encounter one
				// increase the neighborhood
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (i == 0 && j == 0) { continue; }
						else if (map[y + i][x + j] == "M") { neighbors += 1; }
					}
				}
			}

			// if we're ploting blob mosters count this way
			if (item == Plot.Blob)
			{
				// check around the for potions and if
				// it's at least 3 steps away increase the neighborhood
				if (y >= 4 && y <= 15)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (j == 0) { continue; }
						else if (map[y+j][x] == "P") { neighbors += 1; }
					}
				}

			}

			// if we're plotting ogres or traps count this way
			if (item == Plot.Ogre || item == Plot.Trap)
			{
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (i == 0 && j == 0) { continue; }
						else if (map[y + i][x + j] == "X" || map[y + i][x + j] == "H"
								 || map[y + i][x + j] == "e" || map[y + i][x + j] == "T"
								 || map[y + i][x + j] == "P" ||map[y + i][x + j] == "M"
								 || map[y + i][x + j] == "R" || map[y + i][x + j] == "b") { neighbors += 1; }

					}
				}
			}

			// if we're looking for portal look around you
			// for empty areas at the top of teh map
			// and the low half of themap and
			// mark their spots as posable portals
			if (item == Plot.Portal)
			{
				for (int j = 1; j< this.height - 1; j++)
			{
				for (int i = 1; i< this.width - 1; i++)
				{
					if (map[j][i] == " " && j < 5)
					{
						int offSet = random.Next(3, 9);
						if ((i + offSet) > this.width - 1 || (i + offSet) < 0 || j + offSet > this.height - 1 || j + offSet < 0)
						{
							continue;
						}
						else
						{
							if (map[j + offSet][i + offSet] == " ")
							{
								portal1X.Add(i + offSet);
								portal1Y.Add(j + offSet);
							}

						}
					}
					else if(map[j][i] == " " && (j >= 10 && j < this.height - 1))
					{
						int offSet = random.Next(1, 9);
						if ((i + offSet) > this.width - 1 || (i + offSet) < 0 || j + offSet > this.height - 1 || j + offSet < 0)
						{
							continue;
						}
						else
						{
							if (map[j + offSet][i + offSet] == " ")
							{
								portal2X.Add(i + offSet);
								portal2Y.Add(j + offSet);
							}

						}
					}
				}
			}
		}


			// if we're plotting a minitaur plot this way
			if (item == Plot.Minitaur)
			{
				int offSet = random.Next(4, 9);

				if (this.entranceX - offSet < 0 || this.entranceX + offSet > this.width - 1 || this.entranceY - offSet < 0 || this.entranceY + offSet > this.height - 1)
				{
					offSet = random.Next(4, 9);
				}

				else
				{
					if (random.Next(0, 101) < 50 && this.entranceX - offSet > 2)
					{
						if (map[y][x - offSet] == " ")
						{
							map[y][x - offSet] = "m";
							this.x.Add(x + offSet);
							this.y.Add(y);
						}

					}

					else if (random.Next(0, 101) < 50 && (this.entranceX + offSet) < this.width - 2)
					{
						if (map[y][x + offSet] == " ")
						{
							map[y][x + offSet] = "m";
							this.x.Add(x + offSet);
							this.y.Add(y);
						}
					}

					else if (random.Next(0, 101) < 50 && (this.entranceY + offSet) < this.height - 4)
					{
						// the offset error is triggered here
						if ((y + offSet) < (this.height - 4) || (y + offSet) > (this.height - 4))
						{
							if (map[y][x] == " ")
							{
								map[y][x] = "m";
								this.x.Add(x);
								this.y.Add(y);
							}
						}
						else
						{
							if (map[y + offSet][x] == " ")
							{
								map[y + offSet][x] = "m";
								this.x.Add(x);
								this.y.Add(y + offSet);
							}
						}

					}
					else if (this.entranceY - offSet < 0)
					{
						if (map[y + offSet][x] == " ")
						{
							map[y + offSet][x] = "m";
							this.x.Add(x);
							this.y.Add(y - offSet);
						}
					}

				}
			}

			return neighbors;
		}

		public String[][] NewMapCopy(string[][] map)
		{
			// temporary array that holds the new states of the cells
			String[][] newCell = new String[height][];

			//initialize the jagged array
			for (int i = 0; i<height; i++)
			{
				newCell[i] = new String[10];
			}

			// coppy the map into the temporary array
			for (int y = 0; y < this.height; y++)
			{
				for (int x = 0; x < this.width; x++)
				{
					newCell[y][x] = map[y][x];
				}
			}

			return newCell;
		}
}
