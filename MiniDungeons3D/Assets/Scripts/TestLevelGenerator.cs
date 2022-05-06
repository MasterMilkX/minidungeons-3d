using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelGenerator : MonoBehaviour
{

	public HumanGameManager3D hgm3d;

	void Start(){
		string[][] map = CreateRandomAscMap();


		//SimLevel level = new SimLevel(mapstring);

		hgm3d = GetComponent<HumanGameManager3D>();
		hgm3d.LoadLevel(map);

	}

	// Generates a randomly made map
	public string[][] CreateRandomAscMap(){
        //make an empty map
        DiggerGenerator gen = new DiggerGenerator();
		gen.GenerateMap();
		string[][] map = gen.GetMap();

		ConstrainFurnisher furnisher = new ConstrainFurnisher(map);
		furnisher.GenerateMap();

		map = furnisher.GetMap();
		return map;
	}

}
