using System;
using System.Collections;
using System.Collections.Generic;

public class AgentFurnisher : Furnisher
{
	
	public SimLevel Level { get; set; }
	public GameCell[][] GameBoard {get;set;}

	public List<Entity> EntityList { get; set; }
	public EntranceEntity Entrance;
	public ExitEntity Exit;
	public List<GoblinEntity> Goblins { get; set; }
	public List<RangedGoblinEntity> RangedGoblins { get; set; }
	public List<Entity> Potions { get; set; }
	public List<Entity> Blobs { get; set; }
	public List<TreasureEntity> Treasures { get; set; }
	public List<OgreEntity> Ogres { get; set; }
	/// <summary>
	/// The rng.
	/// </summary>
	System.Random rng;

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
	/// Initializes a new instance of the <see cref="T:AgentFurnisher"/> class.
	/// </summary>
	public AgentFurnisher()
	{
		rng = Utilities.Random;

		MinitaurCount = 0;
		MeleeMonsterCount = 0;
		RangedMonsterCount = 0;
		OgreCount = 0;
		BlobCount = 0;

		TreasureCount = 0;
		PotionCount = 0;
		TrapCount = 0;

		MeleeMonsterMax = rng.Next(2, 4);
		RangedMonsterMax = rng.Next(1,3);
		MinitaurMax = rng.Next(1, 2);
		BlobMax = 3;
		OgreMax = 2;

		TreasureMax = rng.Next(2, 6);
		PotionMax = rng.Next(4, 8);
		TrapMax = 2;

		Goblins = new List<GoblinEntity>();
		RangedGoblins = new List<RangedGoblinEntity>();
		Potions = new List<Entity>();
		Blobs = new List<Entity>();
		Treasures = new List<TreasureEntity>();
		Ogres = new List<OgreEntity>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:AgentFurnisher"/> class.
	/// </summary>
	/// <param name="map">Map.</param>
	public AgentFurnisher(string[][] map)
	{
		Map = map;
		rng = new Random();

		MinitaurCount = 0;
		MeleeMonsterCount = 0;
		RangedMonsterCount = 0;
		OgreCount = 0;
		BlobCount = 0;

		TreasureCount = 0;
		PotionCount = 0;
		TrapCount = 0;

		MeleeMonsterMax = rng.Next(2, 4);
		RangedMonsterMax = rng.Next(1,3);
		MinitaurMax = rng.Next(1, 2);
		BlobMax = 3;
		OgreMax = 2;

		TreasureMax = rng.Next(2, 6);
		PotionMax = rng.Next(4, 8);
		TrapMax = 2;

		Goblins = new List<GoblinEntity>();
		RangedGoblins = new List<RangedGoblinEntity>();
		Potions = new List<Entity>();
		Blobs = new List<Entity>();
		Treasures = new List<TreasureEntity>();
		Ogres = new List<OgreEntity>();
	}
	public void Init()
	{
		rng = new Random();

		MinitaurCount = 0;
		MeleeMonsterCount = 0;
		RangedMonsterCount = 0;
		OgreCount = 0;
		BlobCount = 0;

		TreasureCount = 0;
		PotionCount = 0;
		TrapCount = 0;

		MeleeMonsterMax = rng.Next(2, 4);
		RangedMonsterMax = rng.Next(1,3);
		MinitaurMax = rng.Next(1, 2);
		BlobMax = 3;
		OgreMax = 2;

		TreasureMax = rng.Next(2, 6);
		PotionMax = rng.Next(4, 8);
		TrapMax = 2;

		Goblins = new List<GoblinEntity>();
		RangedGoblins = new List<RangedGoblinEntity>();
		Potions = new List<Entity>();
		Blobs = new List<Entity>();
		Treasures = new List<TreasureEntity>();
		Ogres = new List<OgreEntity>();
	}
	public void Init(string[][] map)
	{
		Map = map;
		rng = new Random();

		MinitaurCount = 0;
		MeleeMonsterCount = 0;
		RangedMonsterCount = 0;
		OgreCount = 0;
		BlobCount = 0;

		TreasureCount = 0;
		PotionCount = 0;
		TrapCount = 0;

		MeleeMonsterMax = rng.Next(2, 4);
		RangedMonsterMax = rng.Next(1,3);
		MinitaurMax = rng.Next(1, 2);
		BlobMax = 3;
		OgreMax = 2;

		TreasureMax = rng.Next(2, 6);
		PotionMax = rng.Next(4, 8);
		TrapMax = 2;

		Goblins = new List<GoblinEntity>();
		RangedGoblins = new List<RangedGoblinEntity>();
		Potions = new List<Entity>();
		Blobs = new List<Entity>();
		Treasures = new List<TreasureEntity>();
		Ogres = new List<OgreEntity>();
	}
	public void Init(string[][] map, int minitaurMax, int meleeMonsterMax, int rangedMonsterMax,
								 int ogreMax, int blobMax, int treasureMax, int potionMax, int trapMax)
	{
		Map = map;
		rng = new Random();

		MinitaurCount = 0;
		MeleeMonsterCount = 0;
		RangedMonsterCount = 0;
		OgreCount = 0;
		BlobCount = 0;

		TreasureCount = 0;
		PotionCount = 0;
		TrapCount = 0;

		if (meleeMonsterMax != -1)
		{
			MeleeMonsterMax = meleeMonsterMax;
		}
		else
		{
			MeleeMonsterMax = rng.Next(1, 4);
		}

		if (rangedMonsterMax != -1)
		{
			RangedMonsterMax = rangedMonsterMax;
		}
		else
		{
			RangedMonsterMax = rng.Next(1,3);
		}

		if (minitaurMax != -1)
		{
			MinitaurMax = minitaurMax;
		}
		else
		{
			MinitaurMax = rng.Next(1, 2);
		}

		if (blobMax != -1)
		{
			BlobMax = blobMax;
		}
		else
		{
			BlobMax = rng.Next(1, 3);
		}
		if (ogreMax != -1)
		{
			OgreMax = ogreMax;
		}
		else
		{
			OgreMax = rng.Next(1, 2);
		}
		if (treasureMax != -1)
		{
			TreasureMax = treasureMax;
		}
		else
		{
			TreasureMax = rng.Next(2, 6);
		}
		if (potionMax != -1)
		{

			PotionMax = potionMax;
		}
		else
		{
			PotionMax = rng.Next(4, 8);
		}
		if (trapMax != -1)
		{
			TrapMax = trapMax;
		}
		else
		{
			TrapMax = rng.Next(1, 4);
		}
		Goblins = new List<GoblinEntity>();
		RangedGoblins = new List<RangedGoblinEntity>();
		Potions = new List<Entity>();
		Blobs = new List<Entity>();
		Treasures = new List<TreasureEntity>();
		Ogres = new List<OgreEntity>();	
	}

	/// <summary>
	/// Prints the map.
	/// </summary>
	public override void PrintMap()
	{
		for (int i = 0; i < Map.Length; i++)
		{
			for (int j = 0; j < Map[i].Length; j++)
			{
				Console.Write(Map[i][j]);
			}
			Console.WriteLine("");
		}
		Console.WriteLine("");
		Console.WriteLine("");	

	}

	/// <summary>
	/// Generates the map.
	/// </summary>
	public override void GenerateMap()
	{
		Init();
        PlayGame(Map);
	}

	/// <summary>
	/// Generates the map.
	/// </summary>
	/// <param name="map">Map.</param>
	public override void GenerateMap(string[][] map)
	{
		Init(map);
		Map = map;
		PlayGame(map);
	}

	public void GenerateMap(string[][] map, int minitaurMax, int meleeMonsterMax, int rangedMonsterMax, 
	                                 int ogreMax, int blobMax, int treasureMax, int potionMax, int trapMax)
	{
		Init(map, minitaurMax, meleeMonsterMax, rangedMonsterMax, ogreMax, blobMax, treasureMax, potionMax, trapMax);
		Map = map;
		PlayGame(map);
	}

	/// <summary>
	/// Gets the map.
	/// </summary>
	/// <returns>The map.</returns>
	public override string[][] GetMap()
	{
		return Map;
	}

	/// <summary>
	/// Initializes the game.
	/// </summary>
	public void InitializeGame()
	{
		EntityList = new List<Entity>();

		GameBoard = new GameCell[20][];
		for(int i = 0; i < GameBoard.Length; i++)
		{
			GameBoard[i] = new GameCell[10];
		}
		MapBoard();
		CreateEntities();

		Utilities.BuildDatabase(Map);

	}

	/// <summary>
	/// Maps the board.
	/// </summary>
	private void MapBoard()
	{
		for(int i = 0; i < Map.Length; i++)
		{
			for(int j = 0; j < Map[i].Length; j++)
			{
				GameBoard[i][j] = new GameCell(j, i);

				if(Map[i][j].Equals("X"))
				{
					// put x in the GameBoard
					GameBoard[i][j].IsWall = true;
				}
			}
		}

	}

	/// <summary>
	/// Plays the game to place entities.
	/// </summary>
	/// <param name="map">Map.</param>
	public void PlayGame(string[][] map)
	{

		InitializeGame();
                    ConvertGameBoard();

		Console.WriteLine("Begin:");
		PrintMap();
		Console.WriteLine("");

		ConvertGameBoard();

		//PrintMap();
		// play for 45 turns
		for(int i = 0; i < 45; i++)
		{
			foreach(Entity e in EntityList)
			{
				e.MakeMove(Utilities.MapToString(map));
			}
			ConvertGameBoard();
			PrintMap();
		}
		CorrectMap();
		// convert GameBoard into a string[][]
		ConvertGameBoard();
		CorrectPortals();


	}

	/// <summary>
	/// Converts the game board.
	/// </summary>
	private void ConvertGameBoard()
	{
		for (int i = 0; i < GameBoard.Length; i++)
		{
			for (int j = 0; j < GameBoard[i].Length; j++)
			{
				if (GameBoard[i][j].Inhabitants.Count > 0)
				{
					Map[i][j] = ConvertEntityToString(GameBoard[i][j].Inhabitants[0]);
				}
				else if (!GameBoard[i][j].IsWall)
				{
					Map[i][j] = " ";
				}
			}
		}
	}

	/// <summary>
	/// Converts the entity to string.
	/// </summary>
	/// <returns>The entity to string.</returns>
	/// <param name="e">E.</param>
	private String ConvertEntityToString(Entity e)
	{
		string r = "";
		if (e is EntranceEntity)
		{
			r = "H";
		}
		else if (e is ExitEntity)
		{
			r = "e";
		}
		else if (e is TreasureEntity)
		{
			r = "T";
		}
		else if (e is PotionEntity)
		{
			r = "P";
		}
		else if (e is PortalEntity)
		{
			r = "p";
		}
		else if (e is TrapEntity)
		{
			r = "t";
		}
		else if (e is GoblinEntity)
		{
			r = "M";
		}
		else if (e is RangedGoblinEntity)
		{
			r = "R";
		}
		else if (e is BlobEntity)
		{
			r = "b";
		}
		else if (e is OgreEntity)
		{
			r = "o";
		}
		else if (e is MinitaurEntity)
		{
			r = "m";
		}
		else if (e is PortalEntity)
		{
			r = "p";
		}
		return r;
	}
	/// <summary>
	/// Creates the entities.
	/// </summary>

	public void CreateEntities()
	{
		// make entrance Entity
		EntranceEntity entrance = new EntranceEntity(GameBoard);
		EntityList.Add(entrance);
		RandomlyPlace(entrance);
		Entrance = entrance;

		ExitEntity exit = new ExitEntity(GameBoard);
		EntityList.Add(exit);
		RandomlyPlace(exit);
		Exit = exit;

		// give exit to entrance and entrance to exit
		entrance.ExitEntity = exit;
		exit.EntranceEntity = entrance;

		for (int i = 0; i < MeleeMonsterMax; i++)
		{
			GoblinEntity g = new GoblinEntity(GameBoard);
			g.Map = Map;
			EntityList.Add(g);
			RandomlyPlace(g);
			Goblins.Add(g);
		}
		foreach(GoblinEntity goblin in Goblins) 
		{
			goblin.Goblins = Goblins;
		}
		// create treasures
		for(int i = 0; i<TreasureMax; i++)
		{
			TreasureEntity t = new TreasureEntity(GameBoard);
			t.Map = Map;
			EntityList.Add(t);
			RandomlyPlace(t);
			t.Goblins = Goblins;
			Treasures.Add(t);
		}
		for (int i = 0; i < RangedMonsterMax; i++)
		{
			RangedGoblinEntity r = new RangedGoblinEntity(GameBoard);
			r.Map = Map;
			EntityList.Add(r);
			RandomlyPlace(r);
			r.Goblins = Goblins;
			RangedGoblins.Add(r);
		}
		foreach (RangedGoblinEntity r in RangedGoblins)
		{
			r.RangedGoblins = RangedGoblins;
		}

		for (int i = 0; i < PotionMax; i++)
		{
			PotionEntity p = new PotionEntity(GameBoard);
			p.Map = Map;
			EntityList.Add(p);
			RandomlyPlace(p);
			Potions.Add(p);
		}

		for (int i = 0; i < BlobMax; i++)
		{
			BlobEntity b = new BlobEntity(GameBoard);
			b.Map = Map;
			EntityList.Add(b);
			RandomlyPlace(b);
			Blobs.Add(b);
			b.Potions = Potions;
		}
		foreach (BlobEntity blob in Blobs)
		{
			blob.Blobs = Blobs;
			blob.CombineLists();
		}

		// make the two portals
		PortalEntity p1 = new PortalEntity(GameBoard);
		PortalEntity p2 = new PortalEntity(GameBoard);
		p1.Map = Map;
		p2.Map = Map;

		p1.EntranceEntity = entrance;
		p2.EntranceEntity = entrance;
		p1.ExitEntity = exit;
		p2.ExitEntity = exit;
		p1.OtherPortal = p2;
		p2.OtherPortal = p1;

		RandomlyPlace(p1);
		RandomlyPlace(p2);
		EntityList.Add(p1);
		EntityList.Add(p2);


		for (int i = 0; i < OgreMax; i++)
		{
			OgreEntity o = new OgreEntity(GameBoard);
			o.Map = Map;
			Ogres.Add(o);
			o.Treasures = Treasures;
			RandomlyPlace(o);
			EntityList.Add(o);
		}
		foreach (OgreEntity o in Ogres)
		{
			o.Ogres = Ogres;
		}

		for (int i = 0; i < MinitaurMax; i++)
		{
			MinitaurEntity m = new MinitaurEntity(GameBoard);
			m.Map = Map;
			m.EntranceEntity = entrance;
			m.ExitEntity = exit;
			RandomlyPlace(m);
			EntityList.Add(m);
		}
		for (int i = 0; i < TrapMax; i++)
		{
			TrapEntity t = new TrapEntity(GameBoard);
			t.Map = Map;
			t.Treasures = Treasures;
			t.Goblins = Goblins;
			RandomlyPlace(t);
			EntityList.Add(t);
		}
	}

	/// <summary>
	/// Randomly places the entity.
	/// </summary>
	/// <param name="e">the entity to be placed</param>
	private void RandomlyPlace(Entity e)
	{
		List<Pairing> goodSpots = new List<Pairing>();
		for(int i = 0; i < Map.Length; i++)
		{
			for(int j = 0; j < Map[i].Length; j++)
			{
				if(!Map[i][j].Equals("X") && GameBoard[i][j].Inhabitants.Count < 1)
				{
					goodSpots.Add(new Pairing(j, i));
				}
			}
		}
		int s = rng.Next(0, goodSpots.Count);
		Pairing spot = goodSpots[s];
		GameBoard[spot.Y][spot.X].Inhabitants.Add(e);
		e.Cell = GameBoard[spot.Y][spot.X];
	}

	/// <summary>
	/// Corrects the map. We can't allow tiles to have multiple inhabitants, so we must
	/// move inhabitants off tiles if they share
	/// </summary>
	private void CorrectMap()
	{
		ConvertGameBoard();
		for (int i = 0; i < GameBoard.Length; i++)
		{
			for (int j = 0; j < GameBoard[i].Length; j++)
			{
				if (GameBoard[i][j].Inhabitants.Count > 1)
				{
					// put the entrance or exit inhabitant at the beginning
					if (GameBoard[i][j].Inhabitants.Contains(Entrance)) 
					{
						int index = GameBoard[i][j].Inhabitants.IndexOf(Entrance);
						// swap this index with the first item
						Entity swap = GameBoard[i][j].Inhabitants[0];
						GameBoard[i][j].Inhabitants[0] = Entrance;
						GameBoard[i][j].Inhabitants[index] = swap;
					}
					if (GameBoard[i][j].Inhabitants.Contains(Exit)) 
					{
						int index = GameBoard[i][j].Inhabitants.IndexOf(Exit);
						// swap this index with the first item
						Entity swap = GameBoard[i][j].Inhabitants[0];
						GameBoard[i][j].Inhabitants[0] = Exit;
						GameBoard[i][j].Inhabitants[index] = swap;
					}
					for (int k = 1; k < GameBoard[i][j].Inhabitants.Count; k++)
					{
						Entity e = GameBoard[i][j].Inhabitants[k];
						// do BFS to find another location for every inhabitant over 1
						BFSTree tree = new BFSTree(Map);
						BFSNode root = tree.BuildTree(new Pairing(j, i), " ", 5);

						//swap location, look through all the potential spots we found and pick
						BFSNode exit = tree.Exits[rng.Next(0, tree.Exits.Count)];
						GameBoard[i][j].Inhabitants.Remove(e);
						e.Cell = GameBoard[exit.Y][exit.X];
						GameBoard[exit.Y][exit.X].Inhabitants.Add(e);
                        ConvertGameBoard();
					}
				}
			}
		}
	}

	/// <summary>
	/// If after CorrectMap() is run we have an uneven number of portals, delete the other portal
	/// </summary>
	private void CorrectPortals()
	{
		int portalCount = 0;
		int x = 0;
		int y = 0;
		for (int i = 0; i < Map.Length; i++)
		{
			for (int j = 0; j < Map[i].Length; j++)
			{
				if (Map[i][j].Equals("p"))
				{
					portalCount++;
					x = j;
					y = i;
				}
			}
		}
		if (portalCount < 2)
		{
			Map[y][x] = " ";
		}
	}
		

}
