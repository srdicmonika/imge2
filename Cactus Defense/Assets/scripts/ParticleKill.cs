using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKill : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        StartCoroutine(Kill());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
