using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A Gesture is a list of recorded acceleration values

public class Gesture : IEnumerable<Vector3> {
	
	private List<Vector3> vectors = new List<Vector3>();	

	// We cap the gesture after this many data points
	private static int MAX_LEN = 1000;

	// needed to behave like an enumeratable class
	public IEnumerator<Vector3> GetEnumerator() {
		return vectors.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	// the length of the gesture
	public int Length {
		get { return vectors.Count; }
	}
	
	// implement the [] opertator to access the individual elements
	public Vector3 this[int i] {
		get { return vectors[i]; }
	}

	// add a new vector
	public void Add(Vector3 v) {
		if (vectors.Count >= MAX_LEN)
			return;

		vectors.Add(v);
	}

	// Debug print output
	public override string ToString() {
		string res = "Gesture: ";
		foreach (Vector3 v in vectors) {
			res += v.ToString();
			res += " ; ";
		}
		return res;
	}

	// empty public constructor
	public Gesture() {
		
	}
	
}
