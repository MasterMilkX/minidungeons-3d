using System;
using System.Collections.Generic;
public class BlobEntity : Entity
{

	public string[][] Map { get; set; }
	public Random rng { get; set; }
	public List<Entity> Potions { get; set; }
	public List<Entity> Blobs { get; set; }
	private List<Entity> BlobsAndPotions { get; set; }
    public BlobEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
		BlobsAndPotions = new List<Entity>();
    }

	public void CombineLists()
	{
		BlobsAndPotions.AddRange(Potions);
		BlobsAndPotions.AddRange(Blobs);
	}
    public override void MakeMove(string level)
    {
		// blobs treat potions and other blobs with equal priority
		// blobs want to find potions and other blobs, and try to maintain a distance of 4-8
		// tiles from them
		List<GameCell> moves = new List<GameCell>();
		Pairing me = new Pairing(Cell.X, Cell.Y);

		foreach (Entity ent in BlobsAndPotions)
		{
			Pairing you = new Pairing(ent.Cell.X, ent.Cell.Y);

			// check if we can see it
			if (!ent.Equals(this) && Utilities.LineOfSight(me, you, Map))
			{
				int distanceBefore = Utilities.DistanceBetween(me, you);
				// check if distance is greater than 5
				if (distanceBefore > 4)
				{
					// find the moves that close this gap
					if (!Map[Cell.Y - 1][Cell.X].Equals("X") && (Cell.Y - 1 != ent.Cell.Y || Cell.X != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X, Cell.Y - 1);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter < distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
						}
					}
					if (!Map[Cell.Y + 1][Cell.X].Equals("X") && (Cell.Y + 1 != ent.Cell.Y || Cell.X != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X, Cell.Y + 1);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter < distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
						}
					}
					if (!Map[Cell.Y][Cell.X - 1].Equals("X") && (Cell.Y != ent.Cell.Y || Cell.X - 1 != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X - 1, Cell.Y);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter < distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
						}
					}
					if (!Map[Cell.Y][Cell.X + 1].Equals("X") && (Cell.Y != ent.Cell.Y || Cell.X + 1 != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X + 1, Cell.Y);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter < distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
						}
					}

				}
				// check if distance is less than 9
				if (distanceBefore < 8)
				{
					// find the moves that make this bigger
					if (!Map[Cell.Y - 1][Cell.X].Equals("X") && (Cell.Y - 1 != ent.Cell.Y || Cell.X != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X, Cell.Y - 1);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter > distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
						}
					}
					if (!Map[Cell.Y + 1][Cell.X].Equals("X") && (Cell.Y + 1 != ent.Cell.Y || Cell.X != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X, Cell.Y + 1);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter > distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
						}
					}
					if (!Map[Cell.Y][Cell.X - 1].Equals("X") && (Cell.Y != ent.Cell.Y || Cell.X - 1 != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X - 1, Cell.Y);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter > distanceBefore)
						{
							moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
						}
					}
					if (!Map[Cell.Y][Cell.X + 1].Equals("X") && (Cell.Y != ent.Cell.Y || Cell.X + 1 != ent.Cell.X))
					{
						Pairing meAgain = new Pairing(Cell.X + 1, Cell.Y);
						int distanceAfter = Utilities.DistanceBetween(meAgain, you);
						if (distanceAfter > distanceBefore)
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
