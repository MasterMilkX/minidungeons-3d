using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour {
	
	private bool isMinorShake = false;
	private bool isLongShake = false;
	private float baseX, baseY, baseZ;
	private float intensity;
	private int shakes = 0;

	void Start () 
	{
		baseX = transform.position.x;
		baseY = transform.position.y;
		baseZ = transform.position.z;

		intensity = 0.1f;
	}

	void Update () 
	{
		if(isLongShake || isMinorShake)
		{
			float randomShakeX = Random.Range(-intensity, intensity);
			float randomShakeY = Random.Range(-intensity, intensity);
			float randomShakeZ = Random.Range(0, intensity * 2);
			transform.position = new Vector3(baseX + randomShakeX, baseY + randomShakeY, baseZ + randomShakeZ);

			shakes--;

			if(shakes <= 0)
			{
				isLongShake = false;
				isMinorShake = false;
				transform.position = new Vector3(baseX, baseY, transform.position.z);
			}
		}
	}

	public void MinorShake(float in_intensity)
	{
		if(!isLongShake){
		isMinorShake = true;
		shakes = 10;
		intensity = in_intensity;
		}
	}

	public void LongShake(float in_intensity)
	{
		isLongShake = true;
		shakes = 100;
		intensity = in_intensity;
	}

}
