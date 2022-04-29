using System;

public abstract class Furnisher
{
	public abstract string[][] GetMap();

	public abstract void GenerateMap();

	public abstract void PrintMap();

	public abstract void GenerateMap(string[][] map);

	public int GetSeed()
	{
		return Utilities.RandomSeed;	
	}
}

