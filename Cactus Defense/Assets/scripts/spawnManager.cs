using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour {
	

	//two endpos
	// four entry pos 
	// two middle pos 
	//all points
	private GameObject[,] allSpawnPoints = new GameObject[3,2];
	//way of enemy
	private Transform[] generatedWay;
	int randNr; 
	float timeLeft =5;
	//prefab
	public GameObject enemyClonePrefab;
	//tatsächlicher Clone
	GameObject enemyClone;
	//1.enemy der unsichtbar sein soll/weg sein soll


	// Use this for initialization
	void Start () {
		//get all children / spawnpoints
		//wir setzen hierarchie der Wege
		allSpawnPoints [0, 0] = transform.GetChild (0).gameObject;
		allSpawnPoints[0, 1] = transform.GetChild (1).gameObject;
		allSpawnPoints [1, 0] = transform.GetChild (2).gameObject;
		allSpawnPoints [1, 1] = transform.GetChild (3).gameObject;
		allSpawnPoints [2, 0] = transform.GetChild (4).gameObject;
		allSpawnPoints [2, 1] = transform.GetChild (4).gameObject;
		//for (int i = 0; i < transform.childCount; i++) {
		//	for (int j = 0; j < 2; j++) {
		//		allSpawnPoints [i] = transform.GetChild (i).gameObject;
		//		Debug.Log ("Got child number " + i);
		//	}
		}
		
	//spawnpt 1, 2 sind die oben ( evtl 3. hinzu) 
	//prüfen wenn man da ist geht man zum anderen, dann zu 3,4 dann zu rest!
	
	// Update is called once per frame
	void Update () {
		// we need to make 
		timeLeft -= Time.deltaTime;
		if (timeLeft < 0) {
			//jedes Mal wenn Timer abläuft dann 
			randNr = Random.Range (0, 2);
			//in diesem Spawnpoint einen neuen Enemyprefab instanziieren
			generatedWay = new Transform[allSpawnPoints.Length];
			generatedWay[0] = allSpawnPoints[0, randNr].transform;
			enemyClone = Instantiate(enemyClonePrefab, generatedWay[0].transform.position, Quaternion.identity) as GameObject;

			timeLeft = 4;
		}

	}

	public GameObject[,] AllSpawnPoints {
		get {
			return allSpawnPoints;
		}
	}
	public Transform[] genWay{
		get{ 
			return generatedWay;
		}

	}
}
