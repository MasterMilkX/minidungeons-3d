using System;
using System.Collections.Generic;
public class RangedGoblinEntity : Entity
{
	public List<GoblinEntity> Goblins { get; set; }
	public List<RangedGoblinEntity> RangedGoblins { get; set; }
	public string[][] Map { get; set; }
	public Random rng { get; set; }

    public RangedGoblinEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
    }

    public override void MakeMove(string level)
    {
		// attempt to get close to Goblins, but avoid each other
		// goblins don't want to be near other golbins. 
		//If it sees any, it will try to move away
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
			// if we see any monsters, move away from them
			List<RangedGoblinEntity> ISeeYou = new List<RangedGoblinEntity>();
			List<GoblinEntity> ISeeYouTwo = new List<GoblinEntity>();
			foreach (RangedGoblinEntity goblin in RangedGoblins)
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

			foreach (GoblinEntity goblin in Goblins)
			{
				Pairing you = new Pairing(goblin.Cell.X, goblin.Cell.Y);
				if (Utilities.LineOfSight(me, you, Map))
				{
					ISeeYouTwo.Add(goblin);
				}
			}
			foreach (RangedGoblinEntity goblin in ISeeYou)
			{
				Pairing you = new Pairing(goblin.Cell.X, goblin.Cell.Y);
				int distanceBefore = Utilities.DistanceBetween(me, you);
				// check around this entity to see if moving either
				// 1. removes them from sight
				// 2. adds distance between them
				if (!Map[Cell.Y - 1][Cell.X].Equals("X"))
				{
					Pairing me2 = new Pairing(Cell.X, Cell.Y - 1);
					int distanceAfter = Utilities.DistanceBetween(me2, you);
					if (!Utilities.LineOfSight(me2, you, Map) || distanceAfter > distanceBefore)
					{
						moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
					}

				}
				if (!Map[Cell.Y + 1][Cell.X].Equals("X"))
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

			foreach (GoblinEntity goblin in ISeeYouTwo)
			{
				Pairing you = new Pairing(goblin.Cell.X, goblin.Cell.Y);
				int distanceBefore = Utilities.DistanceBetween(me, you);
				// check around this entity to see if moving either
				// 1. removes them from sight
				// 2. adds distance between them
				if (!Map[Cell.Y - 1][Cell.X].Equals("X") && (Cell.Y - 1 != goblin.Cell.Y || Cell.X != goblin.Cell.X))
				{
					Pairing me2 = new Pairing(Cell.X, Cell.Y - 1);
					int distanceAfter = Utilities.DistanceBetween(me2, you);
					if (distanceAfter < distanceBefore)
					{
						moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
					}

				}
				if (!Map[Cell.Y + 1][Cell.X].Equals("X") && (Cell.Y + 1 != goblin.Cell.Y || Cell.X != goblin.Cell.X))
				{
					Pairing me2 = new Pairing(Cell.X, Cell.Y + 1);
					int distanceAfter = Utilities.DistanceBetween(me2, you);
					if (distanceAfter < distanceBefore)
					{
						moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
					}

				}
				if (!Map[Cell.Y][Cell.X - 1].Equals("X") && (Cell.Y != goblin.Cell.Y || Cell.X - 1 != goblin.Cell.X))
				{
					Pairing me2 = new Pairing(Cell.X - 1, Cell.Y);
					int distanceAfter = Utilities.DistanceBetween(me2, you);
					if (distanceAfter < distanceBefore)
					{
						moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
					}

				}
				if (!Map[Cell.Y][Cell.X + 1].Equals("X") && (Cell.Y != goblin.Cell.Y || Cell.X + 1 != goblin.Cell.X))
				{
					Pairing me2 = new Pairing(Cell.X + 1, Cell.Y);
					int distanceAfter = Utilities.DistanceBetween(me2, you);
					if (distanceAfter < distanceBefore)
					{
						moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
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
