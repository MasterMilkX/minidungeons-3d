using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResultCompare : MonoBehaviour {

	string runnerData,monsterKillerData,treasureCollectorData,complitionistData,resultCompare;


	public string Compare(){

		resultCompare = null;
		//get the  test result data
		runnerData = Variables.runner;
		monsterKillerData = Variables.monsterKiller;
		treasureCollectorData = Variables.treasureCollertor;
		complitionistData = Variables.complitionist;
		if (runnerData != null && monsterKillerData != null && treasureCollectorData != null && complitionistData != null) {
			// split the string 
			string[] runner = runnerData.Split (',');
			string[] monsterKiller = monsterKillerData.Split (',');
			string[] treasureCollector = treasureCollectorData.Split (',');
			string[] complitionist = complitionistData.Split (',');

			// if both runner and monster killer kills same number of monster
			if ((runner [13] == monsterKiller [13]) && runner [13] != "0" && monsterKiller [13] != "0") {
				resultCompare = resultCompare + "Good level";
			}

			// if both runner and treasure collector collects same amount of treasure
			if ((runner [15] == treasureCollector [15]) && runner [15] != "0" && treasureCollector [15] != "0") {
				resultCompare = resultCompare + "Treasures are on the shortest path";
			}

			// take the hero alive variables from all test data into a string
			string temp = runner [11] + " " + monsterKiller [11] + " " + treasureCollector [11] + " " + complitionist [11];

			// if the strinf contains 3 or more false
			if (temp.Count (x => x == 'F') >= 3) {
				resultCompare = resultCompare + "Too tough level"; 
			}
		}
		return resultCompare;
	}

}
