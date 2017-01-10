using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public CactusController.RVControl DoorKnob;
    float original;
    private CactusController cactusController;

    // Use this for initialization
    void Start () {
        //cactusController = FindObjectOfType<CactusController>();
        //original = transform.localScale.z;
        transform.localScale.Set(transform.localScale.x, transform.localScale.y, 0f);
	}
	
	// Update is called once per frame
	void Update () {
        //scale according to the slider position
        //transform.localScale.Set(transform.localScale.x, transform.localScale.y, original * cactusController.getRVValue(DoorKnob));
	}
}
