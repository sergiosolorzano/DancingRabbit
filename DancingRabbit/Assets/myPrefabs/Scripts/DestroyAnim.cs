using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnim : MonoBehaviour {
    Animator anim;
    //int destroyHash = Animator.StringToHash("tankDestroyed");

    
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AnimateDestruction()
    {
        /*Debug.Log("Now should do animation!!!!");
        anim = GetComponent<Animator>();
        //anim.SetBool(destroyHash, true);
        anim.Play("smallTank2TurretDestroyAnim");*/
        Debug.Log("I'm playing The ANIM!!");
        //anim.Play("smallTank2TurretDestroyAnim");
    }
}
