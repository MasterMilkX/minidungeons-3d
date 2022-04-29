using System;

public class GrammarGenerator : Creator
{
	/// <summary>
	/// Gets or sets the number of rooms to be created in the dungeon.
	/// </summary>
	/// <value>No of rooms.</value>
	public int NoOfRooms { get; set; }

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
	/// The map that the generator builds
	/// </summary>
	private String[][] map = new String[18][];

	/// <summary>
	/// The rng.
	/// </summary>
	private Random rng;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:AssemblyCSharp.GrammarGenerator"/> class.
	/// </summary>
	public GrammarGenerator()
	{ }

	/// <summary>
	/// Initialize this instance. Creates the map of walls for the generator to build.
	/// </summary>
	private void Initialize()
	{
		for (int i = 0; i < map.Length; i++) {
			map[i] = new String[8];
			for (int j = 0; j < map[i].Length; j++) {
				map[i][j] = "X";
			}
		}
		rng = new Random();
		MinDungeonSize = 75;
	}

	private void GaugeDifficulty()
	{
		NoOfRooms = rng.Next(2, 5);
		int tries = 0;

		//Console.WriteLine("NoOfRooms = " + NoOfRooms);

		for (int i = 0; i < NoOfRooms; i++) {
			do {
				PlotRoom(i);
				tries++;
			}
			while (map[AgentYPos][AgentXPos].Equals(" ") && tries < 100);

			int RoomDimensionX = rng.Next(2, 7);
			int RoomDimensionY = rng.Next(2, 7);

			int XLimit = (AgentXPos + RoomDimensionX < map[0].Length) ? AgentXPos + RoomDimensionX : map[0].Length;
			int YLimit = (AgentYPos + RoomDimensionY < map.Length) ? AgentYPos + RoomDimensionY : map.Length;

			for (AgentYPos = AgentYPos; AgentYPos < YLimit; AgentYPos++) {
				for (AgentXPos = AgentXPos; AgentXPos < XLimit; AgentXPos++) {
					OpenHere();
				}
				AgentXPos -= RoomDimensionX;
			}
		}
	}

	/// <summary>
	/// Generates the dungeon.
	/// </summary>
	private void GenerateDungeon()
	{
		PlotRoom();
		// put the entrance on top of the agent
		//PlopEntrance();

		// first randomize the initial direction
		CurrentDirection = (Direction)rng.Next(0, 4);
		// Move agent forward in current direction
		switch (CurrentDirection) {
			case Direction.Right:
				// make sure its possible to move right
				if (AgentXPos < map[0].Length - 1) {
					AgentXPos = AgentXPos + 1;
				}
				break;
			case Direction.Left:
				// make sure its possible to move right
				if (AgentXPos > 0) {
					AgentXPos = AgentXPos - 1;
				}
				break;
			case Direction.Up:
				// make sure its possible to move right
				if (AgentYPos > 0) {
					AgentYPos = AgentYPos - 1;
				}
				break;
			case Direction.Down:
				// make sure its possible to move right
				if (AgentYPos < map.Length - 1) {
					AgentYPos = AgentYPos + 1;
				}
				break;
		}
		OpenHere();
		int currentSize = CalculateDungeonSize();
		// initialize probabilities
		ProbDirect = 0.05;
		ProbRoom = 0.05;
		bool repickDirection = false;
		while (currentSize < MinDungeonSize) {
			// roll to see if we change direction
			Double roll = rng.NextDouble();
			//Console.WriteLine(roll + " : " + ProbDirect);
			if (repickDirection) {
				CurrentDirection = (Direction)rng.Next(0, 4);
				repickDirection = false;
			} else if (roll < ProbDirect) {
				ProbDirect = 0;
				CurrentDirection = (Direction)rng.Next(0, 4);
			}
			  // add 0.05 to the probability of changing direction
			  else {
				ProbDirect += 0.05;
			}
			// Move agent forward in current direction
			switch (CurrentDirection) {
				case Direction.Right:
					// make sure its possible to move right
					if (AgentXPos < map[0].Length - 1) {
						AgentXPos = AgentXPos + 1;
					} else {
						repickDirection = true;
					}
					break;
				case Direction.Left:
					// make sure its possible to move right
					if (AgentXPos > 0) {
						AgentXPos = AgentXPos - 1;
					} else {
						repickDirection = true;
					}
					break;
				case Direction.Up:
					// make sure its possible to move right
					if (AgentYPos > 0) {
						AgentYPos = AgentYPos - 1;
					} else {
						repickDirection = true;
					}
					break;
				case Direction.Down:
					// make sure its possible to move right
					if (AgentYPos < map.Length - 1) {
						AgentYPos = AgentYPos + 1;
					} else {
						repickDirection = true;
					}
					break;
			}

			if (!repickDirection) {
				OpenHere();
				currentSize = CalculateDungeonSize();
			}

		}
	}

	/// <summary>
	/// Generates the starting the point of the room somewhere randomly in the map.
	/// </summary>
	private void PlotRoom(int RoomNo)
	{
		if (RoomNo % 2 == 0) {
			// Randomly places the agent on the map
			AgentYPos = rng.Next(0, map.Length / 2);
			AgentXPos = rng.Next(0, map[0].Length);
		} else {
			// Randomly places the agent on the map
			AgentYPos = rng.Next(map.Length / 2, map.Length);
			AgentXPos = rng.Next(0, map[0].Length);
		}


	}

	/// <summary>
	/// Generates the starting the point of the room somewhere randomly in the map.
	/// </summary>
	private void PlotRoom()
	{
		// Randomly places the agent on the map
		AgentYPos = rng.Next(0, map.Length);
		AgentXPos = rng.Next(0, map[0].Length);

	}

	/// <summary>
	/// Opens a room in the map at the agent's current location.
	/// </summary>
	private void OpenHere()
	{
		if (map[AgentYPos][AgentXPos].Equals("X")) {
			map[AgentYPos][AgentXPos] = " ";
		}
	}

	/// <summary>
	/// Calculates the size of the dungeon.
	/// </summary>
	/// <returns>The dungeon size.</returns>
	public int CalculateDungeonSize()
	{
		int size = 0;

		for (int i = 0; i < map.Length; i++) {
			for (int j = 0; j < map[0].Length; j++) {
				if (map[i][j].Equals(" ")) {
					size++;
				}
			}
		}
		return size;
	}

	/// <summary>
	/// Closes the dungeon up by surrounding with an outside wall.
	/// </summary>
	private void CloseDungeon()
	{
		String[][] mapCopy = new String[20][];
		for (int i = 0; i < mapCopy.Length; i++) {
			mapCopy[i] = new String[10];
		}

		// copy the map over
		for (int i = 1; i < mapCopy.Length - 1; i++) {
			for (int j = 1; j < mapCopy[i].Length - 1; j++) {
				mapCopy[i][j] = map[i - 1][j - 1];
			}
		}

		// fill in the top and bottom wall
		for (int j = 0; j < mapCopy[0].Length; j++) {
			mapCopy[0][j] = "X";
			mapCopy[19][j] = "X";
		}

		// fill in the sides
		for (int i = 0; i < mapCopy.Length; i++) {
			mapCopy[i][0] = "X";
			mapCopy[i][9] = "X";
		}
		map = mapCopy;
	}

	public override string[][] GetMap()
	{
		return map;
	}

	public override void GenerateMap()
	{
		Initialize();
		GaugeDifficulty();
		GenerateDungeon();
		CloseDungeon();
		map = Utilities.ConnectComponents(map);
	}

	/// <summary>
	/// Prints the map.
	/// </summary>
	public override void PrintMap()
	{
		for (int i = 0; i < map.Length; i++) {
			for (int j = 0; j < map[i].Length; j++) {
				Console.Write(map[i][j]);
			}
			Console.WriteLine("");
		}
		Console.WriteLine("");
		Console.WriteLine("");
	}


}
