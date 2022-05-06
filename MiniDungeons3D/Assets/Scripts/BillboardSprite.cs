using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Transform faceTarget;


    private void Start()
    {
        try
        {
            faceTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }
        catch
        {

        }
    }
    void Update()
    {
        //always face the target
        if(faceTarget == null)
            faceTarget = GameObject.FindGameObjectWithTag("Player").transform;
        if (faceTarget != null)
       		transform.eulerAngles = faceTarget.eulerAngles;
    }
}
