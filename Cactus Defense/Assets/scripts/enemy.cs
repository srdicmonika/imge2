﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {


	//for cloning
	public GameObject enemiezPrefab;
	GameObject enemiezClones;
	//sets random spawnpoint
	public float speed;
	//for timer
	public float timeLeft;
	//sets random number
	int randNr;
	//array of all waypoints(set in inspector)
	public Transform[] waypointList;
	public int currentWayPoint=0;
	Transform targetWayPoint;
    //actual route of enemy

    private ScoreManager scoreManager;
	private spawnManager spawnmanager;
	// Use this for initialization
	void Start () {
		spawnmanager = GameObject.FindObjectOfType<spawnManager> ();
		//randNr = Random.Range (1, 2);
		//if (randNr == 1) {
		//	waypointList [randNr] = GameObject.Find ("spawnpoint1").transform.position;

		//if (randNr == 2) {
		//	waypointList [randNr] = GameObject.Find ("spawnpoint2").transform.position;

		int numSpawnpoints = 2;
		waypointList = spawnmanager.genWay;

		for (int i = 1; i < 3; i++) {
			waypointList[i] = spawnmanager.AllSpawnPoints[i, Random.Range(0,numSpawnpoints)].transform;
		}

		targetWayPoint = waypointList [1];

        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

    }
	
	// Update is called once per frame
	void Update () {
		//player moves towards tower 
		//transform.Translate(Vector3.left* speed * Time.deltaTime); automatic movement in one direction
		// check if we have somewere to walk

		walk();

	}

	void walk(){
		// rotate towards the target
		transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, speed*Time.deltaTime, 0.0f);

		// move towards the target
		//transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position,   speed*Time.deltaTime);
		transform.Translate(Vector3.forward * Time.deltaTime * speed);

		if(transform.position == targetWayPoint.position)
		{
			currentWayPoint++;
			Debug.Log ("reach");
			if (waypointList.Length > currentWayPoint) 
			{
				targetWayPoint = waypointList [currentWayPoint];
			}
		}
	} 


	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "waypoint") 
		{
			targetWayPoint = waypointList [++currentWayPoint];
		}
		if ((other.gameObject.tag == "CactusTower")) {
            scoreManager.addScore(10);
			Destroy (gameObject);
		}
		if ((other.gameObject.tag == "target")) {
			Destroy (gameObject);
		}
	}
}

