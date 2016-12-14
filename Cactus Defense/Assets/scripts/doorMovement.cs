using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorMovement : MonoBehaviour {

	//to change speed of doors
	public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.M)) {
			//bewegung nach hinten
			transform.Translate (Vector3.right* speed * Time.deltaTime);
		} if (Input.GetKey (KeyCode.N)) {
			transform.Translate (Vector3.left* speed * Time.deltaTime);
		}
	}
}
