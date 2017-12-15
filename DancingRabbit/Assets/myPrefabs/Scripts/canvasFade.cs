using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasFade : MonoBehaviour {
    public CanvasGroup myUI;
    float fadeSpeed = 1.0f;
    public GameObject myShip;

    private void Awake()
    {
        myUI.alpha = 0;
        myUI = GetComponent<CanvasGroup>();   
    }
    
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(myShip.GetComponent<shipMovement>().UIreadyToFade)
        {
            myUI.alpha += Time.deltaTime * fadeSpeed;
        }
	}
}
