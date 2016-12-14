using UnityEngine;
using System.Collections.Generic;

// This is a collection of named gesture models
// Note this class also handles the acutual Bayesian classification

public class GestureLibrary {

	// Directory to store the gesture models by name
	private Dictionary<string, GestureModel> dict = new Dictionary<string, GestureModel>();

	public GestureLibrary() {
	}

	// Implement [] operator to access the gesture models by name (read/write)
	public GestureModel this[string name] {
		get {return dict[name];}
		set {dict[name] = value;}
	}

	// Delete a specific gestrue
	public void Remove(string name) {
		dict.Remove(name);
	}

	// Test is a gesture with a certain name is present
	public bool Contains(string name) {
		return dict.ContainsKey(name);
	}


	// Bayesian classifier
	public string Classify(Gesture g) {

		// Compute the sum of all conditional probabilities for the current library and the observed gesture
		float sum = 0.0f;
		foreach (string gestureName in dict.Keys) {
			sum += dict[gestureName].DefaultProbability * dict[gestureName].MatchGesture(g);
		}

		// if the sum is not larger than 0, the gesture is not recognized
		if (sum <= 0) {
			return "";
		}

		// serarch for the best match and store some values
		string bestMatch = "";
		float bestRecogProb = -1.0f;

		float bestMatchProb = -1.0f;
		float bestModelProb = -1.0f;

		foreach (string gestureName in dict.Keys) {
			// Get the probability of the gesture model itself and the conditional observation probability
			float match = dict[gestureName].MatchGesture(g);
			float model = dict[gestureName].DefaultProbability;

			// Check if conditional model probability is lager than best one so fat
			if ((match*model)/sum > bestRecogProb) {
				bestMatchProb = match;
				bestModelProb = model;
				bestRecogProb = (match*model)/sum;
				bestMatch = gestureName;
			}
		}

		// If none of the probabilities are zero, we have found a match
		if (bestRecogProb>0 && bestMatchProb>0 && bestModelProb>0) {
			return bestMatch;
		}

		// Otherwise not
		return "";

	}

}