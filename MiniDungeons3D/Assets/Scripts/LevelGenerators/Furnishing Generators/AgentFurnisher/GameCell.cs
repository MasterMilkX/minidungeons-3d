using System;
using System.Collections;
using System.Collections.Generic;
public class GameCell
{
    public List<Entity> Inhabitants {get;set;}
    public int X {get;set;}
    public int Y {get;set;}

    public bool IsWall {get;set;}
    public GameCell(int x, int y)
    {
		X = x;
		Y = y;
        IsWall = false;
        Inhabitants = new List<Entity>();
    }
}
