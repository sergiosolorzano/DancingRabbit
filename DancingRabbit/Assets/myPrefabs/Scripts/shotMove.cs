using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotMove : MonoBehaviour {
    public GameObject explosionPrefab;
    public GameObject ImageTargetAssigned;
    [HideInInspector] public GameObject tankToDestroy;
    [HideInInspector] bool noMoreShooting = false;
    //public LayerMask tank;
    [HideInInspector] public GameObject tankHit;
    [HideInInspector] public GameObject turretHit;
    [HideInInspector] public GameObject bodyHit;

    // Use this for initialization
    public void Start () {
        GameObject imgTgt = GameObject.Find("ImageTarget");
        ImageTargetAssigned = imgTgt;
    }

    // Update is called once per frame
    public void Update () {
        if(gameObject != null)
        transform.position += transform.forward * 10.0f * Time.deltaTime;//may need to use local position for vuforia
    }

    void OnTriggerEnter(Collider other){
        GameObject explosion;
        
        if (other.gameObject.layer == LayerMask.NameToLayer("tank"))
            {
            GameObject hitGO= other.gameObject;
            GameObject firstParentHitGO = hitGO.transform.parent.gameObject;
            //GameObject tankParentHitGO = firstParentHitGO.transform.parent.gameObject;
            
            if (hitGO.gameObject.name=="smallTankB" || hitGO.gameObject.name=="smallTank2B")
            {
                Debug.Log("Object Hit is " + hitGO);
                //Debug.Log("Destroying " + hitGO.name);
                explosion = Instantiate(explosionPrefab, hitGO.transform.position, Quaternion.Euler(-90,0,0));
                explosion.transform.parent = ImageTargetAssigned.transform;//EASIER WAY TO CALL THE PUBLIC OBJECT IMAGETARGET I CREATED ON MOVEMENT?
                //explosion.transform.localRotation = Quaternion(-90,, 0, 9);
                Movement destroyTankObject =  hitGO.GetComponent<Movement>();
                tankHit = hitGO;
                turretHit = hitGO.transform.Find("TurretForMove").gameObject;
                bodyHit = hitGO.transform.Find("body").gameObject;
                
                //turretHit.transform.rotation = Random.rotation;

                if (turretHit != null)
                {
                    Debug.Log("This is shotMove with " + gameObject + " reporting : Turret found !!" + turretHit + " whose parent is " + turretHit.transform.parent);
                }
                else
                {
                    Debug.Log("This is shotMove with " + gameObject + " reporting: Turret not found - " + turretHit);
                }
                destroyTankObject.destroyedTank(gameObject);
                Destroy(gameObject);
                //noMoreShooting = true;

            } else if (hitGO.gameObject.name=="body" || hitGO.gameObject.name == "TurretForMove")
            {
                Debug.Log("Object Hit is " + hitGO + "parent To Destroy is " + firstParentHitGO);
                //Debug.Log("Destroying " + tankParentHitGO.name);
                explosion = Instantiate(explosionPrefab, firstParentHitGO.transform.position, Quaternion.identity);
                explosion.transform.parent = ImageTargetAssigned.transform;
                Movement destroyTankObject = firstParentHitGO.GetComponent<Movement>();
                tankHit = firstParentHitGO;
                turretHit = firstParentHitGO.transform.Find("TurretForMove").gameObject;//find turretformove child 
                bodyHit = firstParentHitGO.transform.Find("body").gameObject;

                if (turretHit != null)
                {
                    Debug.Log("This is shotMove with " + gameObject + " reporting : Turret found !!" + turretHit);
                }
                else
                {
                    Debug.Log("This is shotMove with " + gameObject + " reporting: Turret not found - " + turretHit);
                }
                destroyTankObject.destroyedTank(gameObject);
                Destroy(gameObject);
            }
        }
    }
}
