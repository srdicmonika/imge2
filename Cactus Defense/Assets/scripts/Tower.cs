﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public GameObject explosionPrefab;
    public CactusController cactusController;
    public CactusController.CactusButton actionButton;
    public GameObject gestureWavePrefab;
    public string gestureName;

    private bool canFire = true;
    private ScoreManager scoreManager;

    private TowerTimer timer;

    // Use this for initialization
    void Start () {
        timer = GameObject.Find("TowerTimer").GetComponent<TowerTimer>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        if(gestureName != "" && gestureWavePrefab != null)
        {
            cactusController.addGestureCallback(gestureCallback);
            StartCoroutine(TestWave());
        }
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
        if (timer.getReady() && scoreManager.useEnergy(10))
        {
            timer.setReady(false);
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

    IEnumerator TestWave()
    {
        yield return new WaitForSeconds(3.5f);
        gestureCallback("whip");
    }

    void gestureCallback(string gesture)
    {
        if(gesture == gestureName)
        {
            Instantiate(gestureWavePrefab, transform.position, Quaternion.identity);
        }
    }
}
