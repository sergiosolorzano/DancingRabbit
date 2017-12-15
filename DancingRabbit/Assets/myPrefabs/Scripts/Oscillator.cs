using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {
    public Transform center;
    public float degreesPerSecond = -1;
    private Vector3 v;
    
    // Use this for initialization
    void Start () {
        v = transform.position - center.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        v = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, Vector3.up) * v;
        transform.position = center.transform.position + v;
        //transform.Rotate(v*Time.deltaTime);
        //transform.Rotate(Vector3.up, 90);

        //transform.rotation=
        Debug.Log(v);
	}
}
