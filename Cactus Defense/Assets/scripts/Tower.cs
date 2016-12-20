using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public GameObject explosionPrefab;
    public CactusController cactusController;
    public CactusController.CactusButton actionButton;

    private bool canFire = true;
    private ScoreManager scoreManager;

    // Use this for initialization
    void Start () {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
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
        if (scoreManager.useEnergy(100))
        {
            StartCoroutine(LockAndUnlockFire());
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
    }

    IEnumerator LockAndUnlockFire()
    {
        canFire = false;
        yield return new WaitForSeconds(0.9f);
        canFire = true;
    }
}
