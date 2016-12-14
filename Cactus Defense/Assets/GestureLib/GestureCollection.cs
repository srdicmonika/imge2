using UnityEngine;
using System.Collections.Generic;

// This is a collecttion of gestures constituating one set of training data
// This class is basically what is saved and loaded from disk

// We just encapsulate the name and the list of gestures

public class GestureCollection {
	public string Name = "Unnamed";
	public List<Gesture> gestures = new List<Gesture>();

	public GestureCollection() {		
	}

	public GestureCollection(string name) {
		Name = name;
	}
}