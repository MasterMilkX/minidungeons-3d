using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelGenerator : MonoBehaviour
{

	public GameObject wall;
	public GameObject floor;
	public GameObject ceiling;

	public Transform Level;
	public Transform Player;

	public int levelWidth = 10;
	public int levelHeight = 10;

	void Start(){
		PlaceAscMap(null);
	}

	// Generates a randomly made map
	public int[,] CreateRandomAscMap(){
		//make an empty map
		int[,] am = new int[levelHeight,levelWidth];
		for(int h=0;h<levelHeight;h++){
			for(int w=0;w<levelWidth;w++){
				am[h,w] = 0;
			}
		}

		//populate it
		for(int h=0;h<levelHeight;h++){
			for(int w=0;w<levelWidth;w++){
				if(h == 0 || w == 0 || h == levelHeight-1 || w == levelWidth-1){
					am[h,w] = 1;
				}else if(Random.Range(0.0f,1.0f) > 0.7){
					am[h,w] = 1;
				}
			}
		}

		return am;

	}

	//place prefabs on the game screen according to the map
	public void PlaceAscMap(int[] ascMap){
		List <Vector3> open_pos = new List<Vector3>();

		//place the floor
		for(int h=0;h<levelHeight;h++){
			for(int w=0;w<levelWidth;w++){
				GameObject newFloor = Instantiate(floor, new Vector3(w,-0.55f,h), Quaternion.identity);
				newFloor.transform.SetParent(Level);
			}
		}

		//place upside floor (ceiling)
		for(int h=0;h<levelHeight;h++){
			for(int w=0;w<levelWidth;w++){
				GameObject newFloor = Instantiate(ceiling, new Vector3(w,0.55f,h), Quaternion.identity * Quaternion.Euler(180,0,0));
				newFloor.transform.SetParent(Level);
			}
		}

		int[,] wallmap = CreateRandomAscMap();
		//place the floor
		for(int h=0;h<levelHeight;h++){
			for(int w=0;w<levelWidth;w++){
				if(wallmap[h,w] == 1){
					GameObject newWall = Instantiate(wall, new Vector3(w,0.0f,h), Quaternion.identity);
					newWall.transform.SetParent(Level);
				}else
					open_pos.Add(new Vector3(w,-0.55f,h));
			}
		}

		//place the player on a random open spot
		Player.position = open_pos[Random.Range(0,open_pos.Count)];

	}

}
