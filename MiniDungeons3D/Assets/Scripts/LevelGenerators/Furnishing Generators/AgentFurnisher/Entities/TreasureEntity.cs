using System;
using System.Collections.Generic;

public class TreasureEntity : Entity
{
	public Random rng { get; set; }
	public string[][] Map { get; set; }
	public List<GoblinEntity> Goblins { get; set; }

    public TreasureEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
    }

    public override void MakeMove(string level)
    {
		List<GameCell> moves = new List<GameCell>();
		Pairing me = new Pairing(Cell.X, Cell.Y);
		SimPoint meAgain = new SimPoint(Cell.X, Cell.Y);
		// if we are next to a monster, dont move
		bool nextTo = false;
		if (!nextTo && GameBoard[Cell.Y - 1][Cell.X].Inhabitants.Count > 0)
		{
			foreach (Entity e in GameBoard[Cell.Y - 1][Cell.X].Inhabitants)
			{
				if (e is GoblinEntity)
				{
					nextTo = true;
					break;
				}
			}
		}
		else if (!nextTo && GameBoard[Cell.Y + 1][Cell.X].Inhabitants.Count > 0)
		{
			foreach (Entity e in GameBoard[Cell.Y + 1][Cell.X].Inhabitants)
			{
				if (e is GoblinEntity)
				{
					nextTo = true;
					break;
				}
			}
		}
		else if (!nextTo && GameBoard[Cell.Y][Cell.X - 1].Inhabitants.Count > 0)
		{
			foreach (Entity e in GameBoard[Cell.Y][Cell.X - 1].Inhabitants)
			{
				if (e is GoblinEntity)
				{
					nextTo = true;
					break;
				}
			}
		}
		else if (!nextTo && GameBoard[Cell.Y][Cell.X + 1].Inhabitants.Count > 0)
		{
			foreach (Entity e in GameBoard[Cell.Y][Cell.X + 1].Inhabitants)
			{
				if (e is GoblinEntity)
				{
					nextTo = true;
					break;
				}
			}
		}
		else
		{
			// line of sight search for all melee monsters. If we can see it, move towards it.
			foreach (GoblinEntity goblin in Goblins)
			{
				Pairing you = new Pairing(goblin.Cell.X, goblin.Cell.Y);
				if (((Cell.Y != goblin.Cell.Y && Cell.X != goblin.Cell.X)) && Utilities.LineOfSight(me, you, Map))
				{
					// get the path length to this goblin
					SimPoint youAgain = new SimPoint(goblin.Cell.X, goblin.Cell.Y);
					int pathLength = PathDatabase.Lookup(meAgain, youAgain, level).Length;

					// see which direction would result in being closer to it
					if (!GameBoard[Cell.Y - 1][Cell.X].IsWall && (Cell.Y - 1 != goblin.Cell.Y || Cell.X != goblin.Cell.X))
					{
						SimPoint newMe = new SimPoint(Cell.X, Cell.Y - 1);
						int newPathLength = PathDatabase.Lookup(newMe, youAgain, level).Length;
						if (newPathLength < pathLength)
						{
							moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
						}
					}

					if (!GameBoard[Cell.Y + 1][Cell.X].IsWall && (Cell.Y + 1 != goblin.Cell.Y || Cell.X != goblin.Cell.X))
					{
						SimPoint newMe = new SimPoint(Cell.X, Cell.Y + 1);
						int newPathLength = PathDatabase.Lookup(newMe, youAgain, level).Length;
						if (newPathLength < pathLength)
						{
							moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
						}
					}
					if (!GameBoard[Cell.Y][Cell.X - 1].IsWall && (Cell.Y != goblin.Cell.Y || Cell.X - 1 != goblin.Cell.X))
					{
						SimPoint newMe = new SimPoint(Cell.X - 1, Cell.Y);
						int newPathLength = PathDatabase.Lookup(newMe, youAgain, level).Length;
						if (newPathLength < pathLength)
						{
							moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
						}
					}
					if (!GameBoard[Cell.Y][Cell.X + 1].IsWall && (Cell.Y != goblin.Cell.Y || Cell.X + 1 != goblin.Cell.X))
					{
						SimPoint newMe = new SimPoint(Cell.X + 1, Cell.Y);
						int newPathLength = PathDatabase.Lookup(newMe, youAgain, level).Length;
						if (newPathLength < pathLength)
						{
							moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
						}
					}
				}
			}
		}
        // if we have moves to choose from, pick one
        if(moves.Count > 0)
        {
            // pick a random move
            int choice = rng.Next(0, moves.Count);
			Cell.Inhabitants.Remove(this);
			Cell = moves [choice];
			Cell.Inhabitants.Add(this); 
	 	}
    }
}
