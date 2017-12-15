using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public Transform[] path2;
    public float speed2;
    public float turnRate2 = 3f;

    private Vector3 direction2;
    private GameObject node;

    void Start()
    {

        //StartCoroutine(Followpath2());
    }

    IEnumerator Followpath2()
    {
        foreach (Transform waypoint in path2)
        {
            //Debug.Log(waypoint.name);
            //node = waypoint.GetComponent//IN  WORKS!!!!!!
            yield return StartCoroutine(Move2(waypoint.position, speed2));
        }
    }

    IEnumerator Move2(Vector3 destination, float speed)
    {
        while (transform.position != destination)
        {
            Vector3 direction2 = destination - transform.position;
            //Debug.Log("Before:" + direction.x + " " + direction.y + " " + direction.z);

            //direction2.x = transform.position.x;
            //direction2.y = transform.position.y;
            //direction2.z = transform.position.z;

            //Debug.Log("After: " + direction.x + " " + direction.y + " " + direction.z);
            Quaternion targetRotation = Quaternion.LookRotation(direction2);
            Vector3 eurlers = targetRotation.eulerAngles;
            //Debug.Log("direction:" + direction + "targetRotation: " + targetRotation);

            transform.position = Vector3.MoveTowards(transform.position, destination, speed2 * Time.deltaTime);
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnRate2);
            //transform.Rotate(Vector3.up, eurlers.y);
            transform.LookAt(destination);
            yield return null;
        }
    }

    void Update()
    {
        /*//Manual movement
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 direction = input.normalized;
        Vector3 velocity = direction * speed;
        Vector3 moveAmount = velocity * Time.deltaTime;

        transform.position += moveAmount;*/

        //Part of coroutine
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Followpath2());
        }



        //transform.position+= Vector3.up* speed* Time.deltaTime;
        //Debug.Log(transform.position);


    }
}
