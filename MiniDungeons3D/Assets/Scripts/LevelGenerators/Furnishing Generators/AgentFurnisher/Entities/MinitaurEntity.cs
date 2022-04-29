using System;
using System.Collections.Generic;

public class MinitaurEntity : Entity
{
	public string[][] Map { get; set; }
	public Random rng { get; set; }
	public Entity ExitEntity { get; set; }
	public Entity EntranceEntity { get; set; }

    public MinitaurEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;
    }

    public override void MakeMove(string level)
    {
		List<GameCell> moves = new List<GameCell>();
		SimPoint me = new SimPoint(Cell.X, Cell.Y);

		// be as far from the entrance and the exit that you can
		// if you are less than 7 tiles to the entrance or the exit, look for tiles that move you away
		SimPoint entrancePoint = new SimPoint(EntranceEntity.Cell.X, EntranceEntity.Cell.Y);
		int entrancePathLength = 0;

		if (me.X != entrancePoint.X && me.Y != entrancePoint.Y)
		{
			entrancePathLength = PathDatabase.Lookup(me, entrancePoint, level).Length;
		}
		if (entrancePathLength< 7)
		{
			if(!GameBoard[Cell.Y - 1][Cell.X].IsWall && (Cell.Y - 1 != EntranceEntity.Cell.Y && Cell.X != EntranceEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X, Cell.Y - 1);
				int newPathLength = PathDatabase.Lookup(newMe, entrancePoint, level).Length;
				if(newPathLength > entrancePathLength)
	            {
					moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
	            }
	        }
			if(!GameBoard[Cell.Y + 1][Cell.X].IsWall && (Cell.Y + 1 != EntranceEntity.Cell.Y && Cell.X != EntranceEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X, Cell.Y + 1);
				int newPathLength = PathDatabase.Lookup(newMe, entrancePoint, level).Length;
				if(newPathLength > entrancePathLength)
	            {
					moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
	            }
	        }
			if(!GameBoard[Cell.Y][Cell.X + 1].IsWall && (Cell.Y != EntranceEntity.Cell.Y && Cell.X + 1 != EntranceEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X + 1, Cell.Y);
				int newPathLength = PathDatabase.Lookup(newMe, entrancePoint, level).Length;
				if(newPathLength > entrancePathLength)
	            {
	                moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
	            }
	        }
			if(!GameBoard[Cell.Y][Cell.X - 1].IsWall && (Cell.Y != EntranceEntity.Cell.Y && Cell.X - 1 != EntranceEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X - 1, Cell.Y);
				int newPathLength = PathDatabase.Lookup(newMe, entrancePoint, level).Length;
				if(newPathLength > entrancePathLength)
	            {
	                moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
	            }
	        }
		}
		// if you are less than 7 tiles from the exit, look for tiles that move you away
		SimPoint exitPoint = new SimPoint(ExitEntity.Cell.X, ExitEntity.Cell.Y);
		int exitPathLength = 0;

		if (me.X != exitPoint.X && me.Y != exitPoint.Y)
		{
			exitPathLength = PathDatabase.Lookup(me, exitPoint, level).Length;
		}
		if (entrancePathLength< 7)
		{
			if(!GameBoard[Cell.Y - 1][Cell.X].IsWall && (Cell.Y - 1 != ExitEntity.Cell.Y || Cell.X != ExitEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X, Cell.Y - 1);
				int newPathLength = PathDatabase.Lookup(newMe, exitPoint, level).Length;
				if(newPathLength > exitPathLength)
	            {
					moves.Add(GameBoard[Cell.Y - 1][Cell.X]);
	            }
	        }
			if(!GameBoard[Cell.Y + 1][Cell.X].IsWall && (Cell.Y + 1 != ExitEntity.Cell.Y || Cell.X != ExitEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X, Cell.Y + 1);
				int newPathLength = PathDatabase.Lookup(newMe, exitPoint, level).Length;
				if(newPathLength > exitPathLength)
	            {
					moves.Add(GameBoard[Cell.Y + 1][Cell.X]);
	            }
	        }
			if(!GameBoard[Cell.Y][Cell.X + 1].IsWall && (Cell.Y != ExitEntity.Cell.Y || Cell.X + 1 != ExitEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X + 1, Cell.Y);
				int newPathLength = PathDatabase.Lookup(newMe, exitPoint, level).Length;
				if(newPathLength > exitPathLength)
	            {
	                moves.Add(GameBoard[Cell.Y][Cell.X + 1]);
	            }
	        }
			if(!GameBoard[Cell.Y][Cell.X - 1].IsWall && (Cell.Y != ExitEntity.Cell.Y || Cell.X - 1 != ExitEntity.Cell.X))
	        {
	            SimPoint newMe = new SimPoint(Cell.X - 1, Cell.Y);
				int newPathLength = PathDatabase.Lookup(newMe, exitPoint, level).Length;
				if(newPathLength > exitPathLength)
	            {
	                moves.Add(GameBoard[Cell.Y][Cell.X - 1]);
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
