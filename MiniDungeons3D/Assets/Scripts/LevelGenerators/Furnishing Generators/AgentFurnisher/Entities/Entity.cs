using System;

public abstract class Entity
{
    public GameCell[][] GameBoard {get;set;}
    public GameCell Cell {get;set;}
    public abstract void MakeMove(string level);

}
