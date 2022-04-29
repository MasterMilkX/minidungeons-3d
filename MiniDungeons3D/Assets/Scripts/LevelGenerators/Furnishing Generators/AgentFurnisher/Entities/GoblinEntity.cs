using System;
using System.Collections.Generic;

public class GoblinEntity : Entity
{
	public List<GoblinEntity> Goblins { get; set; }
	public string[][] Map { get; set; }
	public Random rng { get; set;}

    public GoblinEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
    }
	/// <summary>
	/// Makes the move.
	/// </summary>
	/// <param name="level">Level.</param>
    public override void MakeMove(string level)
    {
		// goblins don't want to be near other golbins. 
		//If it sees any, it will try to move away
		List<GameCell> moves = new List<GameCell>();
		Pairing me = new Pairing(Cell.X, Cell.Y);
		SimPoint meAgain = new SimPoint(Cell.X, Cell.Y);
		// if we see any monsters, move away from them
		List<GoblinEntity> ISeeYou = new List<GoblinEntity>();
		foreach (GoblinEntity goblin in Goblins)
		{
			if (!goblin.Equals(this))
			{
				Pairing you = new Pairing(goblin.Cell.X, goblin.Cell.Y);
				if (Utilities.LineOfSight(me, you, Map) && Utilities.DistanceBetween(me, you) < 6)
				{
					ISeeYou.Add(goblin);
				}
			}
		}

		foreach (GoblinEntity goblin in ISeeYou)
		{
			Pairing you = new Pairing(goblin.Cell.X, goblin.Cell.Y);
			int distanceBefore = Utilities.DistanceBetween(me, you);
			// check around this entity to see if moving either
			// 1. removes them from sight
			// 2. adds distance between them
			if (!Map[Cell.Y - 1][Cell.X].Equals("X"))
			{
				Pairing me2 = new Pairing(Cell.X, Cell.Y-1);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (!Utilities.LineOfSight(me2, you, Map) || distanceAfter > distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
				}

			}
			if(!Map[Cell.Y + 1][Cell.X].Equals("X"))
			{
				Pairing me2 = new Pairing(Cell.X, Cell.Y + 1);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (!Utilities.LineOfSight(me2, you, Map) || distanceAfter > distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
				}

			}
			if (!Map[Cell.Y][Cell.X - 1].Equals("X"))
			{
				Pairing me2 = new Pairing(Cell.X - 1, Cell.Y);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (!Utilities.LineOfSight(me2, you, Map) || distanceAfter > distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
				}

			}
			if (!Map[Cell.Y][Cell.X + 1].Equals("X"))
			{
				Pairing me2 = new Pairing(Cell.X + 1, Cell.Y);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (!Utilities.LineOfSight(me2, you, Map) || distanceAfter > distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
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
