using System;
using System.Collections.Generic;
public class EntranceEntity : Entity
{

	public Random rng { get; set; }
    public Entity ExitEntity {get;set;}

    public EntranceEntity(GameCell[][] gb)
    {
        GameBoard = gb;
		rng = Utilities.Random;

    }

    public override void MakeMove(string level)
    {
        // find the shortest path to the exit. Move if that will move us farther away
        SimPoint me = new SimPoint(Cell.X, Cell.Y);
        SimPoint exit = new SimPoint(ExitEntity.Cell.X, ExitEntity.Cell.Y);
        int pathLength = PathDatabase.Lookup(me, exit, level).Length;

        List<GameCell> moves = new List<GameCell>();
        // now calculate out each of our four moves, and see if there are differences in pathLength
		if(!GameBoard[Cell.Y-1][Cell.X].IsWall && (Cell.Y - 1 != ExitEntity.Cell.Y || Cell.X != ExitEntity.Cell.X))
        {
            SimPoint newMe = new SimPoint(Cell.X, Cell.Y-1);
            int newPathLength = PathDatabase.Lookup(newMe, exit, level).Length;
            if(newPathLength > pathLength)
            {
				moves.Add(GameBoard[Cell.Y-1][Cell.X]);
            }
        }
        if(!GameBoard[Cell.Y+1][Cell.X].IsWall && (Cell.Y + 1 != ExitEntity.Cell.Y || Cell.X != ExitEntity.Cell.X))
        {
            SimPoint newMe = new SimPoint(Cell.X, Cell.Y+1);
            int newPathLength = PathDatabase.Lookup(newMe, exit, level).Length;
            if(newPathLength > pathLength)
            {
				moves.Add(GameBoard[Cell.Y+1][Cell.X]);
            }
        }
        if(!GameBoard[Cell.Y][Cell.X+1].IsWall && (Cell.Y != ExitEntity.Cell.Y || Cell.X + 1 != ExitEntity.Cell.X))
        {
            SimPoint newMe = new SimPoint(Cell.X+1, Cell.Y);
            int newPathLength = PathDatabase.Lookup(newMe, exit, level).Length;
            if(newPathLength > pathLength)
            {
                moves.Add(GameBoard[Cell.Y][Cell.X+1]);
            }
        }
        if(!GameBoard[Cell.Y][Cell.X-1].IsWall && (Cell.Y != ExitEntity.Cell.Y || Cell.X - 1 != ExitEntity.Cell.X))
        {
            SimPoint newMe = new SimPoint(Cell.X-1, Cell.Y);
            int newPathLength = PathDatabase.Lookup(newMe, exit, level).Length;
            if(newPathLength > pathLength)
            {
                moves.Add(GameBoard[Cell.Y][Cell.X-1]);
            }
        }

        // if we have moves to choose from, pick one
        if(moves.Count > 0)
        {
            // pick a random move
            int choice = rng.Next(0, moves.Count);
            Cell.Inhabitants.Remove(this);
            Cell = moves[choice];
            Cell.Inhabitants.Add(this);
        }
        // otherwise nothing happens and we dont make a move
    }



}
