using System;
using System.Collections;
using System.Collections.Generic;

public class GrammarFurnisher : Furnisher
{

	/// <summary>
	/// The rng.
	/// </summary>
	Random random;

	/// <summary>
	/// Gets or sets the map.
	/// </summary>
	/// <value>The map.</value>
	public string[][] Map { get; set; }

	/// <summary>
	/// Gets or sets the entrance point.
	/// </summary>
	/// <value>The entrance point.</value>
	public Pairing EntrancePoint { get; set; }

	/// <summary>
	/// Gets or sets the exit point.
	/// </summary>
	/// <value>The exit point.</value>
	public Pairing ExitPoint { get; set; }

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
	/// Gets or sets the poition pairings list.
	/// </summary>
	/// <value>The poition pairings list.</value>
	public List<Pairing> PotionPairings { get; set; }

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
	/// Gets or sets the treasure pairings list.
	/// </summary>
	/// <value>The treasure pairings list.</value>
	public List<Pairing> TreasurePairings { get; set; }

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
	/// Gets or sets the path to exit.
	/// </summary>
	/// <value>The path to exit.</value>
	public SimPoint[] PathToExit { get; set; }

	/// <summary>
	/// Gets or sets the level.
	/// </summary>
	/// <value>The level.</value>
	public SimLevel Level { get; set; }

	/// <summary>
	/// The index of the entrance.
	/// </summary>
	private int entranceIndex;
	/// <summary>
	/// The index of the exit.
	/// </summary>
	private int exitIndex;

	public GrammarFurnisher()
	{

	}

	public GrammarFurnisher(string[][] m)
	{
		this.Map = m;
	}


	public void Init()
	{
		this.random = Utilities.Random;
		this.PotionPairings = new List<Pairing>();
		this.TreasurePairings = new List<Pairing>();

		this.TreasureCount = this.OgreCount = this.MeleeMonsterCount = this.RangedMonsterCount = this.TrapCount = this.PotionCount = this.BlobCount = this.MinitaurCount = 0;

		this.TreasureMax = this.random.Next(2, 6);
		this.OgreMax = this.random.Next(1, 3);
		this.MeleeMonsterMax = this.random.Next(2, 4);
		this.RangedMonsterMax = this.random.Next(1, 3);
		this.TrapMax = 2;
		this.PotionMax = this.random.Next(4, 8);
		this.BlobMax = this.random.Next(1, 3);
		this.MinitaurMax = this.random.Next(1, 3);

	}

	public override void GenerateMap()
	{
		Init();
		//Console.WriteLine("Begin:");
		//PrintMap();
		//Console.WriteLine("");
		FindPathToExit();
		PlaceEntrance();
		PlaceExit();

		//PrintMap();
		//Console.WriteLine("");

		PlaceTreasures();
		PlaceOgre();
		PlaceMelee();
		PlaceRanged();

  //      PrintMap();
		//Console.WriteLine("");

		PlaceTrap();
		PlacePotions();
		PlaceBlobs();
		PlacePortals();
		PlaceMinitaur();

  //      PrintMap();
		//Console.WriteLine("");

	}

	public override void GenerateMap(string[][] map)
	{
		this.Map = map;
		GenerateMap();
	}

	/// <summary>
	/// Puts the map in string format.
	/// </summary>
	/// <returns>The to string.</returns>
	public string MapToString()
	{
		string mapString = "";

		for (int i = 0; i < this.Map.Length; i++) {
			for (int j = 0; j < this.Map[i].Length; j++) {
				mapString += this.Map[i][j];
			}
			mapString += "\n";
		}
		return mapString;
	}

	/// <summary>
	/// Finds the path to exit.
	/// </summary>
	/// <returns>The path to exit.</returns>
	public int FindPathToExit()
	{
		string mapString = "";
		// get the full level string
		for (int i = 0; i < this.Map.Length; i++) {
			for (int j = 0; j < this.Map[i].Length; j++) {
				mapString += this.Map[i][j];
			}
			if (i != this.Map.Length - 1) {
				mapString += "\n";
			}
		}
		// make a simlevel out of this

		SimLevel level = new SimLevel(mapString);
		if (level == null) {
			return 0;
		}
		PathDatabase.GameType = 1;
		PathDatabase.BigDatabase = new List<Dictionary<string, SimPoint[]>>();
		PathDatabase.levels = new ArrayList();
		PathDatabase.Farthest = new Dictionary<SimLevel, Pair>();

		PathDatabase.BuildDB(level);

		this.PathToExit = PathDatabase.GetLongestPath(level);
		if (this.PathToExit == null) {
			return 1;
		}
		return 2;
	}

	/// <summary>
	/// Places the entrance.
	/// RULE: Any possible empty tile can be the Entrance.
	/// </summary>
	public void PlaceEntrance()
	{
		this.entranceIndex = this.random.Next(0, 8);
		SimPoint Entrance = this.PathToExit[this.entranceIndex];
		Map[Entrance.Y][Entrance.X] = "H";
		this.EntrancePoint = new Pairing(Entrance.X, Entrance.Y);
	}

	/// <summary>
	/// Places the exit.
	/// RULE: The exit likes to be as far as possible from the Entrance
	/// </summary>
	public void PlaceExit()
	{
		this.exitIndex = this.random.Next(this.PathToExit.Length - 6, this.PathToExit.Length - 1);
		SimPoint Exit = this.PathToExit[this.exitIndex];
		Map[Exit.Y][Exit.X] = "e";
		this.ExitPoint = new Pairing(Exit.X, Exit.Y);
	}

	/// <summary>
	/// Places the treasures. 
	/// RULE: Treasures like to be in places with 3 walls
	/// </summary>
	public void PlaceTreasures()
	{
		// lists for x and y coordinates Pairings
		List<Pairing> Treasures = new List<Pairing>();
		List<Pairing> Backup = new List<Pairing>();
		// look around the map for dead-ends and place treasures there
		for (int i = 1; i < Map.Length - 1; i++) {
			for (int j = 1; j < Map[i].Length - 1; j++) {
				int count = 0;
				// check if space is empty
				if (Map[i][j].Equals(" ")) {
					// check if at least 3 neighbors are walls
					if (Map[i - 1][j].Equals("X")) {
						count++;
					}
					if (Map[i + 1][j].Equals("X")) {
						count++;
					}
					if (Map[i][j - 1].Equals("X")) {
						count++;
					}
					if (Map[i][j + 1].Equals("X")) {
						count++;
					}
				}
				if (count > 2) {
					// since the spot has >3 walls, mark as a potential treasure spot

					Treasures.Add(new Pairing(j, i));
				}
				// add to backuplist
				else if (count > 1) {
					Backup.Add(new Pairing(j, i));
				}
			}
		}

		// make sure we had >1 potential spot for treasure
		if (Treasures.Count > 0) {
			for (int i = 0; i < Treasures.Count; i++) {
				// make sure we are under the treasure threashold and we still have potential spots
				if (TreasureCount < TreasureMax && Treasures.Count > 0) {
					// pick a random spot
					int index = random.Next(Treasures.Count);
					Map[Treasures[index].Y][Treasures[index].X] = "T";
					TreasureCount++;
					TreasurePairings.Add(Treasures[index]);
				}
			}
		}
		// if we exhausted all 3 wall treasure spots, do some 2 wall ones
		if (TreasureCount < TreasureMax) {
			foreach (Pairing point in Backup) {
				Treasures.Add(point);
			}
		}
		// make sure we had >1 potential spot for treasure
		if (Treasures.Count > 0) {
			for (int i = 0; i < Treasures.Count; i++) {
				// make sure we are under the treasure threashold and we still have potential spots
				if (TreasureCount < TreasureMax && Treasures.Count > 0) {
					// pick a random spot
					int index = random.Next(Treasures.Count);
					Map[Treasures[index].Y][Treasures[index].X] = "T";
					TreasureCount++;
					TreasurePairings.Add(Treasures[index]);
				}
			}
		}
	}

	/// <summary>
	/// Places the ogres.
	/// RULE: Ogres like being within eyesight of treasure within 8 tiles, but at least 4 tiles away
	/// but they don't want to see each other at all within 8 tiles
	/// </summary>
	public void PlaceOgre()
	{
		List<Pairing> Ogres = new List<Pairing>();
		foreach (Pairing treasure in TreasurePairings) {
			for (int i = 0; i < Map.Length; i++) {
				for (int j = 0; j < Map[i].Length; j++) {
					if (Map[i][j].Equals(" ")) {
						Pairing point = new Pairing(j, i);
						int distance = Utilities.DistanceBetween(point, treasure);
						bool canSee = Utilities.LineOfSight(point, treasure, Map);
						bool canSeeOgre = false;
						int distanceOgre = int.MaxValue;

						for (int k = 0; k < Ogres.Count; k++) {

							Pairing anotherOgre = Ogres[k];
							canSeeOgre = Utilities.LineOfSight(point, anotherOgre, Map);
							distanceOgre = Utilities.DistanceBetween(point, anotherOgre);
							if (canSeeOgre && distanceOgre < 6)
								break;
						}

						if (distance > 4 && distance < 9 && canSee && !canSeeOgre && distanceOgre > 6) {
							Ogres.Add(point);
						}
					}
				}
			}
		}

		for (OgreCount = 0; OgreCount < OgreMax && Ogres.Count > 0; OgreCount++) {

			int index = random.Next(Ogres.Count);
			Map[Ogres[index].Y][Ogres[index].X] = "o";
			Ogres.RemoveAt(index);
		}


	}

	/// <summary>
	/// Places the melee monsters.
	/// RULE: Melee monsters like to be with their back to a wall,
	/// AND not around each other (within 3 tiles)
	/// </summary>
	public void PlaceMelee()
	{
		// lists for x and y coordinates, mapped by index
		List<Pairing> MeleeMonsters = new List<Pairing>();
		// look around the map for walled places. These are the potential initial spots for melee monsters
		for (int i = 1; i < Map.Length - 1; i++) {
			for (int j = 1; j < Map[i].Length - 1; j++) {

				// check if space is empty
				if (Map[i][j].Equals(" ")) {
					// check if one neighbor is a wall
					if (Map[i - 1][j].Equals("X") || Map[i + 1][j].Equals("X")
						|| Map[i][j - 1].Equals("X") || Map[i][j + 1].Equals("X")) {

						// since the spot has a wall and no monsters around
						// mark as a potential monster spot
						MeleeMonsters.Add(new Pairing(j, i));
					}

				}

			}
		}

		for (int MeleeMonsterCount = 0; MeleeMonsterCount < MeleeMonsterMax && MeleeMonsters.Count > 0; MeleeMonsterCount++) {

			// pick a random spot
			int rand = random.Next(MeleeMonsters.Count);

			if (Map[MeleeMonsters[rand].Y - 1][MeleeMonsters[rand].X].Equals("M") || Map[MeleeMonsters[rand].Y + 1][MeleeMonsters[rand].X].Equals("M")
				|| Map[MeleeMonsters[rand].Y][MeleeMonsters[rand].X - 1].Equals("M") || Map[MeleeMonsters[rand].Y][MeleeMonsters[rand].X + 1].Equals("M")) {
				// put this in the list to remove from the mapping, since it has monsters around
				MeleeMonsterCount--;
			} else {
				Map[MeleeMonsters[rand].Y][MeleeMonsters[rand].X] = "M";
			}
			//Remove the position.
			MeleeMonsters.RemoveAt(rand);
		}

	}

	/// <summary>
	/// Places the ranged.
	/// RULE: Ranged monsters like to be around a melee monster for protection,
	/// </summary>
	public void PlaceRanged()
	{
		// lists for x and y coordinates, mapped by index
		List<Pairing> RangeMonsters = new List<Pairing>();
		// look around the map for places near melee monsters
		for (int i = 1; i < Map.Length - 1; i++) {
			for (int j = 1; j < Map[i].Length - 1; j++) {

				// check if space is empty
				if (Map[i][j].Equals(" ") && (Map[i - 1][j].Equals("M") || Map[i + 1][j].Equals("M")
											  || Map[i][j - 1].Equals("M") || Map[i][j + 1].Equals("M"))) {

					// mark as a potential monster spot
					RangeMonsters.Add(new Pairing(j, i));
				}
			}
		}

		for (RangedMonsterCount = 0; RangedMonsterCount < RangedMonsterMax && RangeMonsters.Count > 0; RangedMonsterCount++) {

			// pick a random spot
			int rand = random.Next(RangeMonsters.Count);
			Map[RangeMonsters[rand].Y][RangeMonsters[rand].X] = "R";
			RangeMonsters.RemoveAt(rand);
		}

	}

	/// <summary>
	/// Places the traps.
	/// RULE: Traps like to be somewhere along the shortest path, or very close to it
	/// </summary>
	public void PlaceTrap()
	{
		// lists for x and y coordinates, mapped by index
		List<Pairing> Traps = new List<Pairing>();
		// get a list of the points in the shortest path
		for (int i = entranceIndex + 1; i < exitIndex - 1; i++) {
			if (Map[PathToExit[i].Y][PathToExit[i].X].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X, PathToExit[i].Y));
			}
			// also add potions just along the shortest path on the sides
			if (Map[PathToExit[i].Y - 1][PathToExit[i].X].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X, PathToExit[i].Y - 1));
			}
			if (Map[PathToExit[i].Y][PathToExit[i].X - 1].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X - 1, PathToExit[i].Y));
			}

			if (Map[PathToExit[i].Y + 1][PathToExit[i].X].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X, PathToExit[i].Y + 1));
			}

			if (Map[PathToExit[i].Y][PathToExit[i].X + 1].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X + 1, PathToExit[i].Y));
			}

			// also add diagonals

			if (Map[PathToExit[i].Y - 1][PathToExit[i].X - 1].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X - 1, PathToExit[i].Y - 1));
			}

			if (Map[PathToExit[i].Y - 1][PathToExit[i].X + 1].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X + 1, PathToExit[i].Y - 1));
			}

			if (Map[PathToExit[i].Y + 1][PathToExit[i].X + 1].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X + 1, PathToExit[i].Y + 1));
			}

			if (Map[PathToExit[i].Y + 1][PathToExit[i].X - 1].Equals(" ")) {
				Traps.Add(new Pairing(PathToExit[i].X - 1, PathToExit[i].Y + 1));
			}
		}

		for (int TrapCount = 0; TrapCount < TrapMax && Traps.Count > 0; TrapCount++) {
			int index = random.Next(Traps.Count);
			Map[Traps[index].Y][Traps[index].X] = "t";
			Traps.RemoveAt(index);
		}

	}

	/// <summary>
	/// Places the potions
	/// RULE: Potions don't care where they live, they will scatter themselves across the map
	/// </summary>
	public void PlacePotions()
	{
		// lists for x and y coordinates, mapped by index
		List<Pairing> Potions = new List<Pairing>();


		// get all the points that are blank
		for (int i = 0; i < Map.Length; i++) {
			for (int j = 0; j < Map[i].Length; j++) {
				if (Map[i][j].Equals(" ")) {
					Potions.Add(new Pairing(j, i));
				}
			}
		}


		for (PotionCount = 0; PotionCount < PotionMax && Potions.Count > 0; PotionCount++) {
			int index = random.Next(0, Potions.Count);
			Map[Potions[index].Y][Potions[index].X] = "P";
			PotionPairings.Add(new Pairing(Potions[index].X, Potions[index].Y));
			Potions.RemoveAt(index);
		}
	}

	/// <summary>
	/// Places the blobs.
	/// RULE: Blobs like to be within 5 tiles of potions, but not nearer than 2 tiles
	/// </summary>
	public void PlaceBlobs()
	{
		// lists for x and y coordinates, mapped by index
		List<Pairing> Blobs = new List<Pairing>();


		foreach (Pairing potion in PotionPairings) {
			for (int i = 0; i < Map.Length; i++) {
				for (int j = 0; j < Map[i].Length; j++) {
					if (Map[i][j].Equals(" ")) {
						Pairing point = new Pairing(j, i);
						bool canSee = Utilities.LineOfSight(point, potion, Map);
						int distance = Utilities.DistanceBetween(point, potion);
						if (distance > 4 && distance < 8 && canSee) {
							Blobs.Add(point);
						}
					}
				}
			}
		}

		for (BlobCount = 0; BlobCount < BlobMax && Blobs.Count > 0; BlobCount++) {
			int rand = random.Next(Blobs.Count);
			Pairing point = Blobs[rand];
			Boolean add = true;
			foreach (Pairing potion in PotionPairings) {
				int distance = Utilities.DistanceBetween(point, potion);
				if (distance < 4) {
					add = false;
					BlobCount--;
					break;
				}
			}
			if (add)
				Map[point.Y][point.X] = "b";

			Blobs.Remove(point);
		}

	}

	/// <summary>
	/// Places the portals.
	/// RULE: Portals like to be somewhere around the entrance and exit (5 to 10 tiles)
	/// but they also have to be a minimal distance of 10 tiles away to make any sense
	/// </summary>
	public void PlacePortals()
	{
		Pairing entrancePortal = null;

		// place the first portal
		PlacePortal(ref entrancePortal);

		// do again for second portal, this time towards the exit
		PlacePortal(ref entrancePortal);

	}

	private void PlacePortal(ref Pairing entrancePortal)
	{
		List<Pairing> Portals = new List<Pairing>();
		for (int i = 1; i < Map.Length - 1; i++) {
			for (int j = 1; j < Map[i].Length - 1; j++) {
				if (Map[i][j].Equals(" ")) {
					// make sure this is 5 - 10 tiles from the exit and 10 tiles from the other portal
					Pairing point = new Pairing(j, i);
					if (entrancePortal == null) {
						int distance = Utilities.DistanceBetween(point, EntrancePoint);
						if (distance > 5 && distance < 10) {
							Portals.Add(point);
						}
					} else {
						int distanceExit = Utilities.DistanceBetween(point, ExitPoint);
						int distancePortal = Utilities.DistanceBetween(point, entrancePortal);
						if (distanceExit > 5 && distanceExit < 10 && distancePortal > 10) {
							Portals.Add(point);
						}
					}
				}
			}
		}

		if (Portals.Count > 0) {
			int index = this.random.Next(Portals.Count);
			Map[Portals[index].Y][Portals[index].X] = "p";
			entrancePortal = Portals[index];
		} else if (entrancePortal != null) {
			Map[entrancePortal.Y][entrancePortal.X] = " ";
		}
	}



	/// <summary>
	/// Places the minitaurs.
	/// RULE: A minitaur wants to be somewhere within 8 tiles of the entrance, but at least 4 tiles away
	/// </summary>
	public void PlaceMinitaur()
	{
		List<Pairing> XY = new List<Pairing>();
		for (int i = 1; i < Map.Length - 1; i++) {
			for (int j = 1; j < Map[i].Length - 1; j++) {
				if (Map[i][j].Equals(" ")) {
					for (int k = 4; k < i; k++) {
						if (Map[i - k][j].Equals("H")) {
							XY.Add(new Pairing(j, i));
						}
					}
					for (int k = 4; k < 9 - i; k++) {
						if (Map[i + k][j].Equals("H")) {
							XY.Add(new Pairing(j, i));
						}
					}
					for (int k = 4; k < j; k++) {
						if (Map[i][j - k].Equals("H")) {
							XY.Add(new Pairing(j, i));
						}
					}
					for (int k = 4; k < 9 - j; k++) {
						if (Map[i][j + k].Equals("H")) {
							XY.Add(new Pairing(j, i));
						}
					}

				}
			}
		}


		for (int MinitaurCount = 0; MinitaurCount < MinitaurMax && XY.Count > 0; MinitaurCount++) {
			int index = random.Next(XY.Count);
			Map[XY[index].Y][XY[index].X] = "m";

			XY.RemoveAt(index);
		}
	}

	/// <summary>
	/// Prints the map.
	/// </summary>
	public override void PrintMap()
	{
		for (int i = 0; i < Map.Length; i++) {
			for (int j = 0; j < Map[i].Length; j++) {
				Console.Write(Map[i][j]);
			}
			Console.WriteLine("");
		}
		Console.WriteLine("");
		Console.WriteLine("");
	}

	/// <summary>
	/// Gets the map.
	/// </summary>
	/// <returns>The map.</returns>
	public override string[][] GetMap()
	{
		return Map;
	}





}

