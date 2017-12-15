using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Vuforia;
using System;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour {


    public Transform[] path;
    float speed = 0.1f;
    float destroySpeed = 75;
    float destroySpin = 75;
    public float baseTurnRate = 1000f;
    public float turretTurnRate = 500f;

    private Vector3 targetDirection;
    public GameObject enemyTurret;
    public GameObject turret;
    public GameObject turretFirePos;
    public GameObject ImageTarget;

    public LayerMask rayMask;

    public GameObject spawnPrefab;
    //bool rayCastActive = true;
    [SerializeField] public bool tankMoving = false;//used for Vuforia tracking system
    public AudioSource audioTanks;

    public GameObject shotPrefab;
    private float reloadTime = 0.0f;

    bool tankDestroyed = false;
    int i;
    bool turretGoingUp = true;
    bool bodyGoingUp = true;
    public GameObject ARcamera;
    bool returnTicketTurret;
    bool returnTicketBody;
    public GameObject turretReturnLocation;
    public GameObject bodyReturnLocation;

    //Better way of doing this?
    public GameObject turretDead;
    [HideInInspector] public Quaternion turretRot;
    public GameObject bodyDead;
    [HideInInspector] public Quaternion bodyRot;

    public GameObject explosionPrefab;
    public ParticleSystem turretFire;
    public ParticleSystem bodySmoke;
    void Start() {
        //StartCoroutine(Followpath());  
    }

    IEnumerator Followpath()
    {
        foreach (Transform waypoint in path)
        {
            yield return StartCoroutine(Move(waypoint.localPosition, speed));
        }
    }

    IEnumerator Move(Vector3 destination, float speed)
    {
        //Rotate the base
        targetDirection = Vector3.Normalize(destination - transform.localPosition);
        float baseAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;// Angle around Y axis the base should rotate.
        Quaternion baseRotation = Quaternion.AngleAxis(baseAngle, Vector3.up);

        if (tankDestroyed == false)
        {
            while (Vector3.Distance(transform.localPosition, destination) > 0.01f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, baseRotation, baseTurnRate * Time.deltaTime);
                tankMoving = true;
                yield return null;
            }

            if (Vector3.Distance(transform.localPosition, destination) <= 0.01f)
            {
                tankMoving = false;
            }
        }
    }

void Update()
    {
        //Tank Raycast
        Vector3 rayTankOrigin = transform.position;
        Vector3 rayTankDirection = -transform.up;
        Ray tankRayShot = new Ray(rayTankOrigin, rayTankDirection);

        RaycastHit rayInfoUnderMe;
        float lerpSpeed = 0.1f;

        if (Physics.Raycast(tankRayShot, out rayInfoUnderMe, 4000f))
        {
            Debug.DrawLine(rayTankOrigin, rayInfoUnderMe.point, Color.red);
            Debug.Log("HitPoint " + rayInfoUnderMe.point.y + " and im' hitting " + rayInfoUnderMe.collider.gameObject.name);
            //transform.localRotation= Quaternion.LookRotation(Vector3.ProjectOnPlane(rayInfoUnderMe.normal, transform.forward), rayInfoUnderMe.normal);

            Vector3 myNormal = Vector3.Lerp(-transform.up, rayInfoUnderMe.normal, lerpSpeed * Time.deltaTime);
            // find forward direction with new myNormal:
            var myForward = Vector3.Cross(transform.right, myNormal);
            // align character to the new myNormal while keeping the forward direction:
            var targetRot = Quaternion.LookRotation(myForward, myNormal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);


            //transform.rotation = Quaternion.FromToRotation(transform.up, rayInfoUnderMe.normal) * transform.rotation;
        }

        if (gameObject != null)
        {
            //Part of coroutine
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(Followpath());
                audioTanks.Play();
            }

            //Rotate the turret at detected direction: start with Local
            float distanceToEnemy = Vector3.Distance(turret.transform.position, enemyTurret.transform.position);
            //Debug.Log("turret parent is " + turret.transform.parent + " and the enemyTurret parent is " + enemyTurret.transform.parent);
            float visionRange = 150f;
            float shootingRange = 100;
            //Debug.Log(name + " tank is distance " + distanceToEnemy + "versus " + visionRange);

            RaycastHit targetHitInfo;
            Vector3 shootingRayOrigin = turretFirePos.transform.position;
            Vector3 shootingRayDirection = turretFirePos.transform.forward;
            Ray shootRay = new Ray(shootingRayOrigin, shootingRayDirection);
            //GameObject currentSpawnHit = Instantiate(spawnPrefab, shootingRayOrigin, Quaternion.identity);

            if (returnTicketTurret)
            {
                //Debug.Log("Turret Returning!!");
                StartCoroutine(returnFlightTurret(turretDead, turretRot));
            }
            if (returnTicketBody)
            {
                //Debug.Log("Body Returning!!");
                StartCoroutine(returnFlightBody(bodyDead, bodyRot));
            }


            if (tankDestroyed == false)
            {
                if (Physics.Raycast(shootRay, out targetHitInfo, visionRange, rayMask))//should get scale from hierarchy.image
                {
                    turret.transform.LookAt(enemyTurret.transform.position, transform.up);
                    if (distanceToEnemy > shootingRange)
                    {
                        Debug.Log(name + " in vision Range of " + targetHitInfo.collider.gameObject.name);
                        //Debug.Log("I'm hitting " + targetHitInfo.collider.gameObject.name);
                        Debug.DrawLine(shootingRayOrigin, targetHitInfo.point, Color.red);
                        //turret.transform.localRotation = Quaternion.identity;
                    }
                    else if (distanceToEnemy < shootingRange && reloadTime <= 0.0f)
                    {
                        GameObject shotGO = GameObject.Instantiate(shotPrefab, turretFirePos.transform.position, turretFirePos.transform.rotation);
                        i += 1;
                        Debug.Log(name + "  close enough to shoot at " + enemyTurret.name);
                        Debug.Log("number of laserbolts " + i);
                        shotGO.transform.parent = ImageTarget.transform;
                        reloadTime += 5f;
                        Debug.DrawLine(shootingRayOrigin, shootingRayOrigin + shootingRayDirection * 100.0f, Color.green);
                    }
                } else
                    Debug.DrawLine(shootingRayOrigin, shootingRayOrigin + shootingRayDirection * 100.0f, Color.red);

                if (reloadTime > 0.0f)
                {
                    reloadTime -= Time.deltaTime;
                }
            }
        }
    }


    public void destroyedTank(GameObject laserBoltImpacting)
    {
        shotMove explosionScript = laserBoltImpacting.GetComponent<shotMove>();
        if (explosionScript.turretHit != null)
        {
            //Debug.Log("This is Movement with " + gameObject + " reporting : Turret found !!" + explosionScript.turretHit);
        }
        else
        {
            //Debug.Log("This is Movement with " + gameObject + " reporting: Turret not found - " + explosionScript.turretHit);
        }

        GameObject turretToBlowUp = explosionScript.turretHit;
        GameObject tankToBlowUp = explosionScript.tankHit;
        GameObject bodyToBlowUp = explosionScript.bodyHit;

        tankDestroyed = true;//has to be here, else it tank that was hit doesn't stop

        float rotMinX = 0;
        float rotMaxX = 360;
        float rotMinY = 0;
        float rotMaxY = 360;
        float rotMinZ = 0;
        float rotMaxZ = 360;

        //Vector3 offsetDestroyTurretLocation = new Vector3(Random.Range(posMinX, posMaxX), Random.Range(posMinY,posMaxY), Random.Range(posMinZ,posMaxZ));

        Vector3 CameraOffset = new Vector3(5, 5, 5);
        Vector3 cameraTargetPositionForTurret = ARcamera.transform.position + CameraOffset;
        Vector3 targetTurretDirection = Vector3.Normalize(cameraTargetPositionForTurret - turretToBlowUp.transform.position);

        //Debug.Log("target Location " + destroyTargetTurretLocation + " current location" + turretToBlowUp.transform.localPosition);
        Quaternion DestroyTurretTargetRotation = Quaternion.Euler(new Vector3(Random.Range(rotMinX, rotMaxX), Random.Range(rotMinY, rotMaxY), Random.Range(rotMinZ, rotMaxZ)));

        //Vector3 offsetDestroyBodyLocation = new Vector3(Random.Range(posMinX, posMaxX), Random.Range(posMinY, posMaxY), Random.Range(posMinZ, posMaxZ));
        //Vector3 destroyTargetBodyLocation = bodyToBlowUp.transform.position + offsetDestroyBodyLocation;
        Vector3 cameraTargetPositionForBody = ARcamera.transform.position - CameraOffset;
        Vector3 targetBodyDirection = Vector3.Normalize(cameraTargetPositionForBody - CameraOffset - bodyToBlowUp.transform.position);
        Quaternion DestroyBodyTargetRotation = Quaternion.Euler(new Vector3(Random.Range(rotMinX, rotMaxX), Random.Range(rotMinY, rotMaxY), Random.Range(rotMinZ, rotMaxZ)));

        StartCoroutine(TurretDestructionAnim(turretToBlowUp, cameraTargetPositionForTurret, DestroyTurretTargetRotation));
        StartCoroutine(BodyDestructionAnim(bodyToBlowUp, cameraTargetPositionForBody, DestroyBodyTargetRotation));
    }

    IEnumerator TurretDestructionAnim(GameObject turretDestroyedAnimated, Vector3 cameraTargetPositionForTurret, Quaternion turretTargetRotation)
    {
        turretDead = turretDestroyedAnimated;
        turretRot = turretTargetRotation;
        GameObject turretExplosion;

        while (Vector3.Distance(turretDestroyedAnimated.transform.position, cameraTargetPositionForTurret) >= 50f && turretGoingUp)//WHY COULD I NOT DECLARE BOOL RETURN TICKET IN THIS IENUMERATOR?
        {
            turretDestroyedAnimated.transform.position = Vector3.MoveTowards(turretDestroyedAnimated.transform.position, cameraTargetPositionForTurret, destroySpeed * Time.deltaTime);
            turretDestroyedAnimated.transform.rotation = Quaternion.RotateTowards(turretDestroyedAnimated.transform.rotation, turretTargetRotation, destroySpin * Time.deltaTime);

            //Quaternion.Angle(q1,q2) which will return the difference in rotations, could be used up until within some close-enough threshold
            //Debug.Log("Turret Going Up Distance " + Vector3.Distance(turretDestroyedAnimated.transform.position, cameraTargetPositionForTurret));

            if (Vector3.Distance(turretDestroyedAnimated.transform.position, cameraTargetPositionForTurret) < 50f)
            {
                turretExplosion = Instantiate(explosionPrefab, turretDestroyedAnimated.transform.position, Quaternion.identity);
                turretExplosion.transform.parent = ImageTarget.transform;
                returnTicketTurret = true;
                turretGoingUp = false;
                turretFire.Play();
            }
            yield return null;
        }
    }

    IEnumerator BodyDestructionAnim(GameObject bodyDestroyedAnimated, Vector3 cameraTargetPositionForBody, Quaternion bodyTargetRotation)
    {
        bodyDead = bodyDestroyedAnimated;
        bodyRot = bodyTargetRotation;
        GameObject bodyExplosion;

        while (Vector3.Distance(bodyDestroyedAnimated.transform.position, cameraTargetPositionForBody) >= 50f && bodyGoingUp)
        {
            bodyDestroyedAnimated.transform.position = Vector3.MoveTowards(bodyDestroyedAnimated.transform.position, cameraTargetPositionForBody, destroySpeed * Time.deltaTime);
            bodyDestroyedAnimated.transform.rotation = Quaternion.RotateTowards(bodyDestroyedAnimated.transform.rotation, bodyTargetRotation, destroySpin * Time.deltaTime);

            //Debug.Log("Body Going Up Distance " + Vector3.Distance(bodyDestroyedAnimated.transform.position, cameraTargetPositionForBody));
            if (Vector3.Distance(bodyDestroyedAnimated.transform.position, cameraTargetPositionForBody) < 50f)
            {
                bodyExplosion = Instantiate(explosionPrefab, bodyDestroyedAnimated.transform.position, Quaternion.identity);
                bodyExplosion.transform.parent = ImageTarget.transform;
                returnTicketBody = true;
                bodyGoingUp = false;
                bodySmoke.Play();
            }
            yield return null;
        }
    }

    IEnumerator returnFlightTurret(GameObject turretDestroyedAnimated, Quaternion turretTargetRotation)
    {

        returnTicketTurret = false;
        while (Vector3.Distance(turretDestroyedAnimated.transform.position, turretReturnLocation.transform.position) >= 0.1f)
        {
            turretDestroyedAnimated.transform.position = Vector3.MoveTowards(turretDestroyedAnimated.transform.position, turretReturnLocation.transform.position, destroySpeed * Time.deltaTime);
            turretDestroyedAnimated.transform.rotation = Quaternion.RotateTowards(turretDestroyedAnimated.transform.rotation, turretTargetRotation, destroySpin * Time.deltaTime);
            //Debug.Log("Turret Distance left on return " + Vector3.Distance(turretDestroyedAnimated.transform.position, turretReturnLocation.transform.position));

            if (Vector3.Distance(turretDestroyedAnimated.transform.position, turretReturnLocation.transform.position) < 0.1f)
            {
                turretDestroyedAnimated.GetComponent<BoxCollider>().enabled = true;
                turretDestroyedAnimated.GetComponent<Rigidbody>().isKinematic = false;
                turretDestroyedAnimated.GetComponent<Rigidbody>().useGravity = true;
            }
            yield return null;
        }
    }

    IEnumerator returnFlightBody(GameObject bodyDestroyedAnimated, Quaternion bodyTargetRotation)
    {
        returnTicketBody = false;
        while (Vector3.Distance(bodyDestroyedAnimated.transform.position, bodyReturnLocation.transform.position) >= 0.1f)
        {
            bodyDestroyedAnimated.transform.position = Vector3.MoveTowards(bodyDestroyedAnimated.transform.position, bodyReturnLocation.transform.position, destroySpeed * Time.deltaTime);
            bodyDestroyedAnimated.transform.rotation = Quaternion.RotateTowards(bodyDestroyedAnimated.transform.rotation, bodyTargetRotation, destroySpin * Time.deltaTime);
            //Debug.Log("Body Distance left on return " + Vector3.Distance(bodyDestroyedAnimated.transform.position, bodyReturnLocation.transform.position));

            if (Vector3.Distance(bodyDestroyedAnimated.transform.position, bodyReturnLocation.transform.position) < 0.1f)
            {
                bodyDestroyedAnimated.GetComponent<BoxCollider>().enabled = true;
                bodyDestroyedAnimated.GetComponent<Rigidbody>().isKinematic = false;
                bodyDestroyedAnimated.GetComponent<Rigidbody>().useGravity = true;
            }
            yield return null;
        }
    }
}//Class Closing brace

//GameObject theBolt = GameObject.Find("laserbolt");
//shotMove explosionMechanics = theBolt.GetComponent<shotMove>();
//Debug.Log("I am on the switch script" + "destroyed item to spawn where " + explosionMechanics.tankToDestroy.name + " was");
//GameObject destroyedTank = Instantiate(destroyedTankPrefab, explosionMechanics.tankToDestroy.transform.position, Quaternion.identity);
//GameObject destroyedTank = Instantiate(destroyedTankPrefab, turret.transform.position, Quaternion.identity);
//destroyedTank.transform.parent = ImageTarget.transform;
//Debug.Log("Ive switched to Movement and I'm spawning the destroyed tanked" + destroyedTank);

//GameObject firstTankTurret = GameObject.Find("TurretForMove");
//DestroyAnim destroyAnimation = firstTankTurret.GetComponent<DestroyAnim>();
//anim.SetTrigger("tankDestroyed");
//destroyAnimation.AnimateDestruction();
//float baseAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;// Angle around Y axis the base should rotate.
//Quaternion baseRotation = Quaternion.AngleAxis(baseAngle, Vector3.up);
//transform.localRotation = Quaternion.RotateTowards(transform.localRotation, baseRotation, baseTurnRate * Time.deltaTime);*/

//Debug.Log("Now destroying " + shotPrefab);
//Debug.Log("Now destroying " + gameObject);
//Destroy(gameObject);
//turret.transform.LookAt(new Vector3(turret.transform.position.x*100,transform.position.y*100,transform.position.z)); 
//turret.transform.LookAt(turret.transform.forward, Vector3.up);
//Debug.Log("I'm at Quaternion");
//GameObject laserBoltImpacting = GameObject.FindGameObjectWithTag ("laserBolt");

/*public getChildrenByTag(string childTag)
{
    GameObject childName;
    Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
    foreach (Transform child in allChildren)
    {
        if (child.gameObject.tag == childTag)
        {
            childName = child.gameObject.name;
        }
    }
    return childName;
}*/
/*CODE TO GET BODY CHILD, NOT QUITE WORKING//getChildrenByTag(transform.tag tankBody);//CAN'T GET THIS TO WORK
        GameObject baseToBlowUp;
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.tag == "tankBody")
            {
                baseToBlowUp = child.gameObject;
            }
        }*/
//turretToBlowUp.transform.SetParent(ImageTarget.transform);


/*Debug.Log("tankDestruction location before " + turretDestroyedAnimated.transform.localPosition);
        Debug.Log("tankDestruction location target " + turretTargetLocation);
        Debug.Log("destroyspeed " + destroySpeed);*/
//turretDestroyedAnimated.transform.localPosition = Vector3.MoveTowards(turretDestroyedAnimated.transform.localPosition, turretTargetLocation, destroySpeed * Time.deltaTime);//THIS WORKED
//Vector3 brokenTurretLocalTargetLocation = turretToBlowUp.transform.localPosition + offsetDestroyTurretLocation;//THIS WORKED

/*Quaternion destroyTurretRotation = Quaternion.Euler(new Vector3(150, 150f, 150f));
   Debug.Log("tankDestruction rotation before " + turretDestroyedAnimated.transform.localRotation);
   turretDestroyedAnimated.transform.localRotation = Quaternion.RotateTowards(transform.localRotation, destroyTurretRotation, destroySpeed * Time.deltaTime);
   Debug.Log("tankDestruction rotation done " + turretDestroyedAnimated.transform.localRotation);
   Debug.Log("tankDestruction rotation target " + destroyTurretRotation);
   */

//turretDestroyedAnimated.transform.localPosition= turretTargetLocation;
//Debug.Log("tankDestruction location done " + turretDestroyedAnimated.transform.localPosition);
//transform.localRotation = Quaternion.RotateTowards(transform.localRotation, baseRotation, baseTurnRate * Time.deltaTime);

//turretDestroyedAnimated.transform.Rotate(destroySpeed * Time.deltaTime, destroySpeed * Time.deltaTime, destroySpeed * Time.deltaTime);
//transform.RotateAround(turretDestroyedAnimated.transform.position, transform.up, Time.deltaTime * 90f);
//tankDestroyedAnimated.transform.localRotation = destroyTurretRotation;


//explosionScript.turretHit.transform.position = Vector3.MoveTowards(transform.position, brokenTurretLocation, destroySpeed * Time.deltaTime);

//Vector3 brokenBodyLocation = explosionScript.bodyHit.transform.position += Vector3.right * 2.0f;
//Quaternion destroyTurretRotation = Quaternion.AngleAxis(30f, Vector3.right);

//Debug.Log("Rotating " + explosionScript.turretHit + " the parent is " + explosionScript.turretHit.transform.parent);
//Debug.Log("rotation of " + explosionScript.turretHit + " before whose parent is " + explosionScript.turretHit.transform.parent + " is " + explosionScript.turretHit.transform.eulerAngles.x + " " + explosionScript.turretHit.transform.eulerAngles.y + " " + explosionScript.turretHit.transform.eulerAngles.z);

//explosionScript.turretHit.transform.rotation = Quaternion.Slerp(transform.localRotation, destroyTurretRotation, destroySpeed * Time.deltaTime);

//Debug.Log("rotation of " + explosionScript.turretHit + " after whose parent is " + explosionScript.turretHit.transform.parent + " is " + explosionScript.turretHit.transform.eulerAngles.x + " " + explosionScript.turretHit.transform.eulerAngles.y + " " + explosionScript.turretHit.transform.eulerAngles.z);



//Destruction latest coded
//while (Vector3.Distance(turretDestroyedAnimated.transform.localPosition,turretTargetLocation)>0.01f)
//while (turretDestroyedAnimated.transform.localRotation != turretTargetRotation && bodyDestroyedAnimated.transform.localRotation != turretTargetRotation)

//ROTATION AROUND GROUND MESH
/*RaycastHit rayInfoUnderMe;
    RaycastHit rayInfoInFrontOfMe;
    float angleToRotate;

    //Turret RayCast
    Vector3 rayOrigin = turretFirePos.transform.position;
    Vector3 rayDirection = -turretFirePos.transform.up;
    Ray turretRayShot = new Ray(rayOrigin, rayDirection);

    //Tank Raycast
    Vector3 rayTankOrigin = transform.position;
    Vector3 rayTankDirection = -transform.up;
    Ray tankRayShot = new Ray(rayTankOrigin, rayTankDirection);

    if (Physics.Raycast(turretRayShot, out rayInfoInFrontOfMe, 4f * 100.0f))
    {
        Debug.DrawLine(rayOrigin, rayInfoInFrontOfMe.point, Color.red);
        if (Physics.Raycast(tankRayShot, out rayInfoUnderMe, 4f*100f))
        {
            Debug.DrawLine(rayTankOrigin, rayInfoUnderMe.point, Color.green);
            float pointUnderMe = rayInfoUnderMe.point.y;
            float pointInFrontOfMe = rayInfoInFrontOfMe.point.y;
            float tankLength = 1 / 7;
            float hypotenuse = Mathf.Sqrt((Mathf.Pow(pointUnderMe - pointInFrontOfMe, 2) + Mathf.Pow(tankLength, 2)));
            angleToRotate = Mathf.Acos((Mathf.Pow(pointUnderMe - pointInFrontOfMe, 2) + Mathf.Pow(tankLength, 2) - Mathf.Pow(hypotenuse, 2)) / (2 * (pointUnderMe - pointInFrontOfMe) * hypotenuse));


            if (Mathf.Abs(pointUnderMe - pointInFrontOfMe) > 0.05f)
            {
                Debug.Log("going down");
                Quaternion walkRotation = Quaternion.AngleAxis(angleToRotate, Vector3.up);
                transform.Rotate(Vector3.right * angleToRotate);
                //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, walkRotation, baseTurnRate * Time.deltaTime);
                Debug.Log("DOWN " + "pointUnderMe" + pointUnderMe + " pointInFrontOfMe " + pointInFrontOfMe + "pointUnderMe - pointInFrontOfMe " + (pointUnderMe - pointInFrontOfMe) + " hypotenuse " + hypotenuse + " angleToRate " + angleToRotate + " Quaterinon " + walkRotation + rayInfoInFrontOfMe.collider.gameObject);
            }
            else if (pointUnderMe - pointInFrontOfMe< -0.05f)
            {
                Debug.Log("going up");
                Quaternion walkRotation = Quaternion.AngleAxis(angleToRotate, Vector3.up);
                transform.Rotate(Vector3.right * angleToRotate);
                //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, walkRotation, baseTurnRate * Time.deltaTime);
                Debug.Log("UP " + "pointUnderMe" + pointUnderMe + " pointInFrontOfMe " + pointInFrontOfMe + "pointUnderMe - pointInFrontOfMe " + (pointUnderMe - pointInFrontOfMe) + " hypotenuse " + hypotenuse + " angleToRate " + angleToRotate + " Quaterinon " + walkRotation + rayInfoInFrontOfMe.collider.gameObject);
            }
            else
            {
                transform.Rotate(Vector3.zero);
                Debug.Log("FLAT " + "pointUnderMe" + pointUnderMe + " pointInFrontOfMe " + "pointUnderMe - pointInFrontOfMe " + (pointUnderMe - pointInFrontOfMe) + " hypotenuse " + hypotenuse + " NO ROTATION " + rayInfoInFrontOfMe.collider.gameObject);
            }
        }	
    }

    //Composite normal for tank movement on terrain
    Physics.Raycast(backLeft.position + Vector3.up, Vector3.down, out bl);
    Physics.Raycast(backRight.position + Vector3.up, Vector3.down, out br);
    Physics.Raycast(frontLeft.position + Vector3.up, Vector3.down, out fl);
    Physics.Raycast(frontRight.position + Vector3.up, Vector3.down, out fr);

    //Connect Raycasts
    Vector3 connectingVector1 = br.point - bl.point;//A
    Vector3 connectingVector2 = fr.point-br.point;//B
    Vector3 connectingVector3 = fl.point-fr.point;//C
    Vector3 connectingVector4 = bl.point-fl.point;//D

    //Get the normal at each corner
    Vector3 crossBA = Vector3.Cross(connectingVector2, connectingVector1);
    Vector3 crossCB = Vector3.Cross(connectingVector3, connectingVector2);
    Vector3 crossDC = Vector3.Cross(connectingVector4, connectingVector3);
    Vector3 crossAD = Vector3.Cross(connectingVector1, connectingVector4);

    Debug.DrawRay(br.point, Vector3.up);
    Debug.DrawRay(bl.point, Vector3.up);
    Debug.DrawRay(fr.point, Vector3.up);
    Debug.DrawRay(fl.point, Vector3.up);

    //Calculate composite normal
    transform.up = (crossBA + crossCB + crossDC + crossAD).normalized;

    //SphereCast
    RaycastHit hit;
    if(Physics.SphereCast(transform.localPosition, 0.5f, -transform.up, out hit, 0.1f))
    {
        Debug.Log("Sphere has collided " + hit.point);
        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal), Time.deltaTime * 5.0f);
    }*/

//Tank Raycast for rotation on mountains: WORKED
/*Vector3 rayOrigin = turretFirePos.transform.position;
Vector3 rayDirection = -turretFirePos.transform.up;
Ray turretRayShot = new Ray(rayOrigin, rayDirection);*/
