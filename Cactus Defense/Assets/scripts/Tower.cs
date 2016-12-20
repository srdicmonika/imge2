using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public GameObject explosionPrefab;
    public CactusController cactusController;
    public CactusController.CactusButton actionButton;

    private bool canFire = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (cactusController.buttonPressed(actionButton) && canFire)
        {
            Fire();
        }
	}

    void Fire()
    {
        StartCoroutine(LockAndUnlockFire());
        Instantiate(explosionPrefab,transform.position, transform.rotation);
    }

    IEnumerator LockAndUnlockFire()
    {
        canFire = false;
        yield return new WaitForSeconds(0.9f);
        canFire = true;
    }
}
