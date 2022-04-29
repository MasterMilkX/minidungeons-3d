using System;
using System.Collections;
using System.Collections.Generic;
public enum Direction
{
	Right,
	Left,
	Up,
	Down
}

public class DiggerGenerator : Creator
{
	/** SMART
	 * 	1: place the digger at a dungeon tile
		2: set helper variables Fr=0 and Fc=0
		3: for all possible room sizes:
		3: if a potential room will not intersect existing rooms:
		4: place the room
		5: Fr=1
		6: break from for loop
		7: for all possible corridors of any direction and length 3 to 7:
		8: if a potential corridor will not intersect existing rooms:
		9: place the corridor
		10: Fc=1
		11: break from for loop
		12:if Fr=1 or Fc=1:
		13: go to 2
	*/

	/*	BLIND
		1: initialize chance of changing direction Pc=5
		2: initialize chance of adding room Pr=5
		3: place the digger at a dungeon tile and randomize its direction
		4: dig along that direction
		5: roll a random number Nc between 0 and 100
		6: if Nc below Pc:
		7: randomize the agent’s direction
		8: set Pc=0
		9: else:
		10: set Pc=Pc+5
		11:roll a random number Nr between 0 and 100
		12:if Nr below Pr:
		13: randomize room width and room length between 3 and 7
		14: place room around current agent position
		14: set Pr=0
		15:else:
		16: set Pr=Pr+5
		17:if the dungeon is not large enough:
		18: go to step 4		*/

	/// <summary>
	/// Gets or sets the probability of the digger creating a room.
	/// </summary>
	/// <value>The probability of room generation.</value>
	public Double ProbRoom { get; set; }

	/// <summary>
	/// Gets or sets the probability of the digger changing direction.
	/// </summary>
	/// <value>The probability of directional change.</value>
	public Double ProbDirect { get; set; }

	/// <summary>
	/// Gets or sets the room width minimum.
	/// </summary>
	/// <value>The room width minimum.</value>
	public int RoomWidthMin { get; set; }

	/// <summary>
	/// Gets or sets the room width maximum.
	/// </summary>
	/// <value>The room width maximum.</value>
	public int RoomWidthMax { get; set; }

	/// <summary>
	/// Gets or sets the room height minimum.
	/// </summary>
	/// <value>The room height minimum.</value>
	public int RoomHeightMin { get; set; }

	/// <summary>
	/// Gets or sets the room height maximum.
	/// </summary>
	/// <value>The room height maximum.</value>
	public int RoomHeightMax { get; set; }

	/// <summary>
	/// Gets or sets the agent X Position.
	/// </summary>
	/// <value>The agent X Position.</value>
	public int AgentXPos { get; set; }

	/// <summary>
	/// Gets or sets the agent Y Position.
	/// </summary>
	/// <value>The agent Y Position.</value>
	public int AgentYPos { get; set; }

	/// <summary>
	/// Gets or sets the current direction.
	/// </summary>
	/// <value>The current direction the agent is going.</value>
	public Direction CurrentDirection { get; set; }

	/// <summary>
	/// Gets or sets the minimum size of the dungeon.
	/// </summary>
	/// <value>The minimum size of the dungeon.</value>
	public int MinDungeonSize { get; set; }

	/// <summary>
	/// Gets or sets the minitaur max.
	/// </summary>
	/// <value>The minitaur max.</value>
	public int MinitaurMax { get; set; }

	/// <summary>
	/// Gets or sets the minitaur count.
	/// </summary>
	/// <value>The minitaur count.</value>
	public int MinitaurCount { get; set; }

	/// <summary>
	/// Gets or sets the minitaur prob.
	/// </summary>
	/// <value>The minitaur prob.</value>
	public double MinitaurProb { get; set; }

	/// <summary>
	/// Gets or sets the melee monster max.
	/// </summary>
	/// <value>The melee monster max.</value>
	public int MeleeMonsterMax { get; set; }

	/// <summary>
	/// Gets or sets the melee monster count.
	/// </summary>
	/// <value>The melee monster count.</value>
	public int MeleeMonsterCount { get; set; }

	/// <summary>
	/// Gets or sets the melee monster prob.
	/// </summary>
	/// <value>The melee monster prob.</value>
	public double MeleeMonsterProb { get; set; }

	/// <summary>
	/// Gets or sets the ranged monster max.
	/// </summary>
	/// <value>The ranged monster max.</value>
	public int RangedMonsterMax { get; set; }

	/// <summary>
	/// Gets or sets the ranged monster count.
	/// </summary>
	/// <value>The ranged monster count.</value>
	public int RangedMonsterCount { get; set; }

	/// <summary>
	/// Gets or sets the ranged monster prob.
	/// </summary>
	/// <value>The ranged monster prob.</value>
	public double RangedMonsterProb { get; set; }

	/// <summary>
	/// Gets or sets the ogre max.
	/// </summary>
	/// <value>The ogre max.</value>
	public int OgreMax { get; set; }

	/// <summary>
	/// Gets or sets the ogre count.
	/// </summary>
	/// <value>The ogre count.</value>
	public int OgreCount { get; set; }

	/// <summary>
	/// Gets or sets the ogre prob.
	/// </summary>
	/// <value>The ogre prob.</value>
	public double OgreProb { get; set; }

	/// <summary>
	/// Gets or sets the blob max.
	/// </summary>
	/// <value>The blob max.</value>
	public int BlobMax { get; set; }

	/// <summary>
	/// Gets or sets the BLOB count.
	/// </summary>
	/// <value>The BLOB count.</value>
	public int BlobCount { get; set; }

	/// <summary>
	/// Gets or sets the blop prob.
	/// </summary>
	/// <value>The blop prob.</value>
	public double BlobProb { get; set; }

	/// <summary>
	/// Gets or sets the potion max.
	/// </summary>
	/// <value>The potion max.</value>
	public int PotionMax { get; set; }

	/// <summary>
	/// Gets or sets the potion count.
	/// </summary>
	/// <value>The potion count.</value>
	public int PotionCount { get; set; }

	/// <summary>
	/// Gets or sets the potion prob.
	/// </summary>
	/// <value>The potion prob.</value>
	public double PotionProb { get; set; }

	/// <summary>
	/// Gets or sets the treasure max.
	/// </summary>
	/// <value>The treasure max.</value>
	public int TreasureMax { get; set; }

	/// <summary>
	/// Gets or sets the treasure count.
	/// </summary>
	/// <value>The treasure count.</value>
	public int TreasureCount { get; set; }

	/// <summary>
	/// Gets or sets the treasure prob.
	/// </summary>
	/// <value>The treasure prob.</value>
	public double TreasureProb { get; set; }

	/// <summary>
	/// Gets or sets the trap max.
	/// </summary>
	/// <value>The trap max.</value>
	public int TrapMax { get; set; }

	/// <summary>
	/// Gets or sets the trap count.
	/// </summary>
	/// <value>The trap count.</value>
	public int TrapCount { get; set; }

	/// <summary>
	/// Gets or sets the trap prob.
	/// </summary>
	/// <value>The trap prob.</value>
	public double TrapProb { get; set; }

	/// <summary>
	/// The map that the generator builds
	/// </summary>
	private String[][] map = new String[18][];

	/// <summary>
	/// The rng.
	/// </summary>
	private Random rng;

	/// <summary>
	/// The entrance is plopped.
	/// </summary>
	private bool EntranceIsPlopped;

	/// <summary>
	/// The exit is plopped.
	/// </summary>
	private bool ExitIsPlopped;

	public List<int[]> RoomDimensions { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:AssemblyCSharp.DiggerGenerator"/> class.
	/// </summary>
	public DiggerGenerator()
	{

	}

	/// <summary>
	/// Initialize this instance. Creates the map of walls for the digger to dig.
	/// </summary>
	private void Initialize()
	{
		map = new String[18][];
		for (int i = 0; i < map.Length; i++)
		{
			map[i] = new String[8];
			for (int j = 0; j < map[i].Length; j++)
			{
				map[i][j] = "X";
			}
		}
		rng = Utilities.Random;
		MinDungeonSize = rng.Next(75, 95);
		EntranceIsPlopped = false;
		ExitIsPlopped = false;

		// set up probabilities
		MinitaurProb 		= 0.01;
		MeleeMonsterProb 	= 0.04;
		RangedMonsterProb 	= 0.04;
		OgreProb 			= 0.04;
		BlobProb 			= 0.04;
		TreasureProb 		= 0.05;
		PotionProb 			= 0.05;
		TrapProb 			= 0.05;

		// set up max counts
		MinitaurMax 		= 1;
		MeleeMonsterMax 	= 3;
		RangedMonsterMax 	= 2;
		OgreMax 			= 1;
		BlobMax 			= 1;	
		TreasureMax 		= 4;
		PotionMax 			= 4;
		TrapMax 			= 1;
	}

	/// <summary>
	/// Places the agent somewhere randomly in the map.
	/// </summary>
	private void PlaceAgent()
	{
		// Randomly places the agent on the map
		AgentYPos = rng.Next(0, map.Length);
		AgentXPos = rng.Next(0, map[0].Length);

		DigHere();
		//Console.WriteLine("Initialization:");
		//PrintMap();
	}

	/// <summary>
	/// Digs a hole in the map at the agent's current location.
	/// </summary>
	private void DigHere()
	{

		if (map[AgentYPos][AgentXPos].Equals("X"))
		{
			map[AgentYPos][AgentXPos] = " ";
		}
		//PrintMap();
		//Console.WriteLine("");
	}

	/// <summary>
	/// Generates the map.
	/// </summary>
	public override void GenerateMap()
	{
        Initialize();
		PlaceAgent();
		GenerateDungeon();
		CloseDungeon();
		//Console.WriteLine("Done");
		//PrintMap();
	}
	public void GenerateMap(int size)
	{
        Initialize();
		if (size != -1)
		{
			MinDungeonSize = size;
		}
		else
		{
			MinDungeonSize = rng.Next(55, 95);
		}
		PlaceAgent();
		GenerateDungeon();
		CloseDungeon();
	}
	/// <summary>
	/// Generates the dungeon.
	/// </summary>
	private void GenerateDungeon()
	{
		// put the entrance on top of the agent
		//PlopEntrance();

		// first randomize the initial direction
		CurrentDirection = (Direction) rng.Next(0, 4);
		// Move agent forward in current direction
		switch (CurrentDirection) 
		{
			case Direction.Right:
				// make sure its possible to move Right
				if (AgentXPos < map[0].Length - 1)
				{
					AgentXPos = AgentXPos + 1;
				}
				break;
			case Direction.Left:
				// make sure its possible to move Left
				if (AgentXPos > 0)
				{
					AgentXPos = AgentXPos - 1;
				}
				break;
			case Direction.Up:
				// make sure its possible to move Up
				if (AgentYPos > 0)
				{
					AgentYPos = AgentYPos - 1;
				}
				break;
			case Direction.Down:
				// make sure its possible to move Down
				if (AgentYPos < map.Length - 1)
				{
					AgentYPos = AgentYPos + 1;
				}
				break;
		}

		//map[AgentYPos][AgentXPos] = "S";
		
		DigHere();
		int currentSize = CalculateDungeonSize();
		// initialize probabilities
		ProbDirect = 0.05;
		ProbRoom = 0.05;
		bool repickDirection = false;
		while (currentSize < MinDungeonSize)
		{
			// roll to see if we change direction
			Double roll = rng.NextDouble();
			if (repickDirection)
			{
				CurrentDirection = (Direction)rng.Next(0, 4);
				repickDirection = false;
			}
			else if (roll < ProbDirect)
			{
				ProbDirect = 0;
				CurrentDirection = (Direction)rng.Next(0, 4);
			}
			// add 0.05 to the probability of changing direction
			else
			{
				ProbDirect += 0.05;
			}	
			// Move agent forward in current direction
			switch (CurrentDirection)
			{
				case Direction.Right :
					// make sure its possible to move right
					if (AgentXPos < map[0].Length - 1)
					{
						AgentXPos = AgentXPos + 1;
					}
					else
					{
						repickDirection = true;
					}
					break;
				case Direction.Left:
					// make sure its possible to move right
					if (AgentXPos > 0)
					{
						AgentXPos = AgentXPos - 1;
					}
					else
					{
						repickDirection = true;
					}
					break;
				case Direction.Up:
					// make sure its possible to move right
					if (AgentYPos > 0)
					{
						AgentYPos = AgentYPos - 1;
					}
					else
					{
						repickDirection = true;
					}
					break;
				case Direction.Down:
					// make sure its possible to move right
					if (AgentYPos < map.Length - 1)
					{
						AgentYPos = AgentYPos + 1;
					}
					else
					{
						repickDirection = true;
					}
					break;
			}
			// if we dont have to repick direction, we can dig
			if (!repickDirection)
			{
				// dig right here
				DigHere();

				// populate tile
				//PopulateSquare();

				// roll to see if a room is made
				roll = rng.NextDouble();
				if (roll < ProbRoom)
				{
					ProbRoom = 0;
					// make a new room of random size
					int xSize = rng.Next(RoomWidthMin, RoomWidthMax + 1);
					int ySize = rng.Next(RoomHeightMin, RoomHeightMax + 1);

				// TODO: build the room in these dimensions with the agent as center

				}
				else
				{
					ProbRoom += 0.05;
				}

				// recalculate dungeon size
				currentSize = CalculateDungeonSize();
			}
		}
		//PlopExit();
	}
	/// <summary>
	/// Closes the dungeon up by surrounding with an outside wall.
	/// </summary>
	private void CloseDungeon()
	{
		String[][] mapCopy = new String[20][];
		for (int i = 0; i < mapCopy.Length; i++)
		{
			mapCopy[i] = new String[10];
		}

		// copy the map over
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				mapCopy[i+1][j+1] = map[i][j];
			}
		}

		// fill in the top and bottom wall
		for (int j = 0; j < mapCopy[0].Length; j++)
		{
			mapCopy[0][j] = "X";
			mapCopy[19][j] = "X";
		}

		// fill in the sides
		for (int i = 0; i < mapCopy.Length; i++)
		{
			mapCopy[i][0] = "X";
			mapCopy[i][9] = "X";
		}
		map = mapCopy;
	}

	/// <summary>
	/// Calculates the size of the dungeon.
	/// </summary>
	/// <returns>The dungeon size.</returns>
	public int CalculateDungeonSize()
	{
		int size = 0;

		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[0].Length; j++)
			{
				if (map[i][j].Equals(" "))
				{
					size++;
				}
			}
		}
		return size;
	}


	override public string[][] GetMap()
	{
		return map;
	}
	/// <summary>
	/// Prints the map.
	/// </summary>
	public override void PrintMap()
	{
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[i].Length; j++)
			{
				Console.Write(map[i][j]);
			}
			Console.WriteLine("");
		}
	}

	/// <summary>
	/// Populates the square with Monsters, Potions, Traps, Exit and Entrance, and Teleporters.
	/// </summary>
	private void PopulateSquare()
	{
		// roll to see what kind of tile this will be
		double roll = rng.NextDouble();
		if (roll < MinitaurProb)
		{
			PlopMinitaur();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb)
		{
			PlopMonsterMelee();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb + RangedMonsterProb)
		{
			PlopMonsterRanged();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb + RangedMonsterProb + OgreProb)
		{
			PlopOgre();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb + RangedMonsterProb + OgreProb
				+ BlobProb)
		{
			PlopBlob();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb + RangedMonsterProb + OgreProb
				+ BlobProb + TreasureProb)
		{
			PlopTreasure();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb + RangedMonsterProb + OgreProb
				+ BlobProb + TreasureProb + PotionProb)
		{
			PlopPotion();
		}
		else if (roll < MinitaurProb + MeleeMonsterProb + RangedMonsterProb + OgreProb
		         + BlobProb + TreasureProb + PotionProb + TrapProb)
		{
			PlopTrap();
		}
	}

	/// <summary>
	/// Populates the current tile with the entrance.
	/// </summary>
	private void PlopEntrance()
	{
		
		map[AgentYPos][AgentXPos] = "H";
		EntranceIsPlopped = true;
	}

	/// <summary>
	/// Populates the current tile with the exit.
	/// </summary>
	private void PlopExit()
	{
		if (!map[AgentYPos][AgentXPos].Equals("E"))
		{
			map[AgentYPos][AgentXPos] = "e";
			ExitIsPlopped = true;
		}
	}

	/// <summary>
	/// Populates the current tile with the minitaur.
	/// </summary>
	private void PlopMinitaur()
	{
		if (MinitaurCount < MinitaurMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "m";
			MinitaurCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with a melee monster
	/// </summary>
	private void PlopMonsterMelee()
	{
		if (MeleeMonsterCount < MeleeMonsterMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "M";
			MeleeMonsterCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with a ranged monster
	/// </summary>
	private void PlopMonsterRanged()
	{
		if (RangedMonsterCount < RangedMonsterMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "R";
			RangedMonsterCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with an ogre
	/// </summary>
	private void PlopOgre()
	{
		if (OgreCount < OgreMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "o";
			OgreCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with a blob
	/// </summary>
	private void PlopBlob()
	{
		if (BlobCount < BlobMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "b";
			BlobCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with a treasure
	/// </summary>
	private void PlopTreasure()
	{
		if (TreasureCount < TreasureMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "T";
			TreasureCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with a potion
	/// </summary>
	private void PlopPotion()
	{
		if (PotionCount < PotionMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "P";
			PotionCount++;
		}
	}

	/// <summary>
	/// Populates the current tile with a trap
	/// </summary>
	private void PlopTrap()
	{
		if (TrapCount < TrapMax && map[AgentYPos][AgentXPos].Equals(" "))
		{
			map[AgentYPos][AgentXPos] = "t";
			TrapCount++;
		}
	}
}

