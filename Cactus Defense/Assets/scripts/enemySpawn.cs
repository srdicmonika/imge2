using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemiez : MonoBehaviour {

	public GameObject enemiezPrefab;
	GameObject enemiezClones;
	public float timeLeft;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeLeft -= Time.deltaTime;
		if ( timeLeft < 0 )
		{
			timeLeft = 4;
			enemiezClones = Instantiate (enemiezPrefab, transform.position, Quaternion.identity) as GameObject;
		}
	}
}
