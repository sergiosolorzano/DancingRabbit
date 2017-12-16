using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class presentCollision : MonoBehaviour {

    public GameObject ground;
    public Rigidbody rbBox;
    public Rigidbody rbTop;

   /*
    void onCollisionEnter(presentCollision col)
    {
        if(col.gameObject.name==ground.name)
        {
            rbBox.isKinematic = true;
            rbTop.useGravity = false;
            rbBox.useGravity = false;
        }
    }*/
    private void Start()
    {
        {
            rbBox = GetComponent<Rigidbody>();
        }
    }
}
