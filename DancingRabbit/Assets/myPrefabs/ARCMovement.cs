using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCMovement : MonoBehaviour
{
    public Transform[] path;
    public float speed;
    public float turnRate;

    private Vector3 direction;
    public GameObject tank;

    void Start()
    {

        //StartCoroutine(Followpath());
    }

    IEnumerator Followpath()
    {
        foreach (Transform waypoint in path)
        {
            //Debug.Log(waypoint.name);
            //node = waypoint.GetComponent//IN  WORKS!!!!!!

            yield return StartCoroutine(Move(waypoint.localPosition, speed));
        }
    }
    
    IEnumerator Move(Vector3 destination, float speed)
    {
        /*Vector3 direction = destination - tank.transform.localPosition;
        Debug.Log("Direction: " + direction);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Debug.Log("targetRotation: " + targetRotation);
        Debug.Log("Current Rotation:" + tank.transform.rotation);
        //direction.x = tank.transform.localPosition.x;
        //direction.y = tank.transform.localPosition.y;
        //direction.z = tank.transform.localPosition.z;
        tank.transform.rotation = Quaternion.Lerp(tank.transform.localRotation, targetRotation, Time.deltaTime * turnRate);
        Debug.Log("EndLine Rotation: " + tank.transform.rotation);*/

        /*float ang = Vector3.Angle(transform.position, destination);
        //float ang = AngleBetweenTwoVectors(destination, transform.position);
        transform.Rotate(0f, ang, 0f);
        Debug.Log(ang);*/

        Vector3 targetDirection = Vector3.Normalize(destination - tank.transform.localPosition);
        float baseAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;// Angle around Y axis the base should rotate.
        Debug.Log("baseAngle : " + baseAngle);
        Debug.Log("rotation before : " + tank.transform.rotation);
        Quaternion baseRotation = Quaternion.AngleAxis(baseAngle, Vector3.up);
        Debug.Log("baseRotation in Quaternion " + baseRotation);
        Debug.Log("rotation after : " + tank.transform.rotation);
        Debug.Log("From " + tank.transform.localRotation + "To " + baseRotation + "turnRate " + turnRate) ;
        tank.transform.localRotation= Quaternion.RotateTowards(tank.transform.localRotation, baseRotation, turnRate * Time.deltaTime);
        


        float lengthXZ = Mathf.Sqrt(targetDirection.x * targetDirection.x + targetDirection.z * targetDirection.z);// Hypotenusae of the x z triangle
        float headAngle = (Mathf.Atan2(lengthXZ, targetDirection.y) * Mathf.Rad2Deg) - 90f; // Angle around X axis the head should rotate.
        
        Quaternion headRotation = Quaternion.AngleAxis(headAngle, Vector3.right);

        

        while (tank.transform.localPosition != destination)
        {

            //Debug.Log("Before:" + direction.x + " " + direction.y + " " + direction.z);

            //direction.x = transform.localPosition.x;
            //direction.y = transform.localPosition.y;
            //direction.z = transform.localPosition.z;

            //Debug.Log("After: " + direction.x + " " + direction.y + " " + direction.z);


            //Debug.Log("direction:" + direction + "targetRotation: " + targetRotation);

            tank.transform.localPosition = Vector3.MoveTowards(tank.transform.localPosition, destination, speed * Time.deltaTime);

            //transform.rotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * turnRate);

            /*Vector3 lookAtPosLocalised = transform.InverseTransformVector(destination);
            lookAtPosLocalised.y = 0;
            transform.LookAt(destination); ;*/

            /*targetRotation.y = transform.localRotation.y;
            targetRotation.x = transform.localRotation.x;
            //targetRotation.z = transform.rotation.z;*/
            //transform.localRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnRate);





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
            StartCoroutine(Followpath());
        }



        //transform.position+= Vector3.up* speed* Time.deltaTime;
        //Debug.Log(transform.position);


    }
}
