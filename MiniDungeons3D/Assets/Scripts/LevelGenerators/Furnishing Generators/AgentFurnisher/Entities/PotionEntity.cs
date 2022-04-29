using System;
using System.Collections.Generic;

public class PotionEntity : Entity
{
	public string[][] Map { get; set; }
	public Random rng { get; set; }

    public PotionEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
    }

    public override void MakeMove(string level)
    {
		// potions randomly move around the map
		List<GameCell> moves = new List<GameCell>();
		if (!Map[Cell.Y - 1][Cell.X].Equals("X"))
		{
			moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
		}
		if (!Map[Cell.Y + 1][Cell.X].Equals("X"))
		{
			moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
		}
		if (!Map[Cell.Y][Cell.X - 1].Equals("X"))
		{
			moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
		}
		if (!Map[Cell.Y][Cell.X + 1].Equals("X"))
		{
			moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
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
