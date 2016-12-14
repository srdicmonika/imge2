using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour {

	public GameObject enemiezPrefab;
	GameObject enemiezClones;
	public float timeLeft;
	//sets random spawnpoint
	int randNr;
	//sets target of enemies, where they want to go!
	GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeLeft -= Time.deltaTime;
		if ( timeLeft < 0 )
		{
			timeLeft = 4;
			randNr = Random.Range (1, 3);
			Debug.Log (randNr);
			if (randNr == 1) {
				enemiezClones = Instantiate (enemiezPrefab, GameObject.Find("spawnpoint1").transform.position, Quaternion.identity) as GameObject;
			} 
			if (randNr == 2) {
				enemiezClones = Instantiate (enemiezPrefab,GameObject.Find("spawnpoint2").transform.position, Quaternion.identity) as GameObject;
			}
		}
	}
}
