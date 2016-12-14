using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemiez : MonoBehaviour {

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
			//Object prefab = AssetDatabase.LoadAssetAtPath("Assets/prefabs.enemiez", typeof(GameObject));
			//GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
			//clone.transform.position = Vector3.one;
		}
	}
}
