using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {

	public int speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//player moves towards tower 
		transform.Translate(Vector3.left* speed * Time.deltaTime);
	}
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "CactusTower") {
			Destroy (gameObject);
		}
	}
}