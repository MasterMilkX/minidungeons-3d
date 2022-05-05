using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Transform faceTarget;

    void Update()
    {
        //always face the target
        if(faceTarget != null)
       		transform.eulerAngles = faceTarget.eulerAngles;
    }
}
