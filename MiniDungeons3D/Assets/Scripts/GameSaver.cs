using System;
using UnityEngine;
public class GameSaver
{
	public static int Progress;

	public static void SaveGame()
	{
		PlayerPrefs.SetInt("Progress", Progress);
	}

	public static void LoadGame()
	{
		Progress = PlayerPrefs.GetInt("Progress", 1);
	}

	public static void IncremenentProgress(int Prog)
	{
		if(Prog == Progress)
		Progress += 1;
		SaveGame();
	}

	public static void Reset()
	{
		Progress = 1;
		PlayerPrefs.SetInt("Progress", 1);
	}
}

