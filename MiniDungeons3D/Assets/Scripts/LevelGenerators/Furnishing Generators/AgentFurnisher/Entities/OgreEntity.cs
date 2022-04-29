using System;
using System.Collections.Generic;
public class OgreEntity : Entity
{
	public Random rng { get; set; }
	public string[][] Map { get; set; }
	public List<OgreEntity> Ogres;
	public List<TreasureEntity> Treasures;

    public OgreEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
    }

    public override void MakeMove(string level)
    {
		// if we see a treasure, move within 4 tiles of it
		// move away from other ogres
		List<GameCell> moves = new List<GameCell>();
		Pairing me = new Pairing(Cell.X, Cell.Y);
		// if we see any ogres, move away from them (if distance < 6 tiles)
		List<OgreEntity> ISeeYou = new List<OgreEntity>();
		List<TreasureEntity> ISeeYouTwo = new List<TreasureEntity>();
		foreach (OgreEntity ogre in Ogres)
		{
			if (!ogre.Equals(this))
			{
				Pairing you = new Pairing(ogre.Cell.X, ogre.Cell.Y);
				if (Utilities.LineOfSight(me, you, Map) && Utilities.DistanceBetween(me, you) < 6)
				{
					ISeeYou.Add(ogre);
				}
			}
		}
		// if we see any treasures, move towards them (if distance > 4 tiles)
		foreach (TreasureEntity treasure in Treasures)
		{
			Pairing you = new Pairing(treasure.Cell.X, treasure.Cell.Y);
			if (Utilities.LineOfSight(me, you, Map) && Utilities.DistanceBetween(me, you) > 4)
			{
				ISeeYouTwo.Add(treasure);
			}
		}
		// move away from ogres we see
		foreach (OgreEntity ogre in ISeeYou)
		{
			Pairing you = new Pairing(ogre.Cell.X, ogre.Cell.Y);
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
		// move towards treasure we see
		foreach (TreasureEntity treasure in ISeeYouTwo)
		{
			Pairing you = new Pairing(treasure.Cell.X, treasure.Cell.Y);
			int distanceBefore = Utilities.DistanceBetween(me, you);
			// check around this entity to see if moving either
			// 1. removes them from sight
			// 2. adds distance between them
			if (!Map[Cell.Y - 1][Cell.X].Equals("X") && (Cell.Y - 1 != treasure.Cell.Y || Cell.X != treasure.Cell.X))
			{
				Pairing me2 = new Pairing(Cell.X, Cell.Y - 1);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (distanceAfter<distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
				}

			}
			if (!Map[Cell.Y + 1][Cell.X].Equals("X") && (Cell.Y + 1 != treasure.Cell.Y || Cell.X != treasure.Cell.X))
			{
				Pairing me2 = new Pairing(Cell.X, Cell.Y + 1);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (distanceAfter<distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
				}

			}
			if (!Map[Cell.Y][Cell.X - 1].Equals("X") && (Cell.Y != treasure.Cell.Y || Cell.X - 1 != treasure.Cell.X))
			{
				Pairing me2 = new Pairing(Cell.X - 1, Cell.Y);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (distanceAfter<distanceBefore)
				{
					moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
				}

			}
			if (!Map[Cell.Y][Cell.X + 1].Equals("X") && (Cell.Y != treasure.Cell.Y || Cell.X + 1 != treasure.Cell.X))
			{
				Pairing me2 = new Pairing(Cell.X + 1, Cell.Y);
				int distanceAfter = Utilities.DistanceBetween(me2, you);
				if (distanceAfter<distanceBefore)
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
		// otherwise nothing happens and we dont make a move
    }
}
