using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The stream processor handles the input stream of acceleration values
// It applies filtering and determines when new gestures are beginning and
// when they stop

public class StreamProcessor {

	// The time no motion is detected that ends a gesture
	private static float GESTURE_TIMEOUT = 0.2f;

	// Store current gesture to which new data will be added
	private Gesture workGesture;

	// Store last valid gesture that can be queried
	private Gesture lastGesture;
	private bool gestureIsValid;

	// Get the last valid gesture
	// Note this automatically resets the isValid flag
	public Gesture Gesture {
		get { gestureIsValid = false; return lastGesture; }
	}

	// Check if a valid gesture is available
	public bool GestureIsValid {
		get { return gestureIsValid; }
	}

	// We detect the end of a gesture by observing no motion of some time
	// Store the last timestamp of when motion was observed
	private float lastValidVectorTimestamp = 0.0f;
	// Are we currently moving or not
	private bool isInMotion = false;

	// List of filters that we apply to all incoming vectors
	private List<IFilter> filterList = new List<IFilter>();

	public StreamProcessor () {

		// Create a new gesture to collect input values
		workGesture = new Gesture();

		// Initialize the list of input filters
		filterList.Add(new MinimumFilter());
		filterList.Add(new SimilarityFilter());

	}

	// Hanlde a new input vector
	public void ProcessRawMeasurement(Vector3 input) {
		// Store a local copy that can be modified by the filters
		Vector3 v = input;

		// Store the timestamp when it arrvied
		float timestamp = Time.realtimeSinceStartup;

		// While the input vector is still interesting process it by
		// the successive filters
		bool valid = true;
		foreach (IFilter f in filterList) {
			valid = valid && f.FilterVector(ref v);
		}

		// if the vector is not intersting but we were in motion till now
		// check if the time delay is too large
		// Stop the gesture if it is
		if (!valid && isInMotion) {
			float timeDiff = timestamp - lastValidVectorTimestamp;
			if (timeDiff > GESTURE_TIMEOUT) {
				isInMotion = false;
				Debug.Log("Motion stopped");

				gestureIsValid = true;
				lastGesture = workGesture;
			}
		} else if (valid) {
			// IF it is intresting, add the measurement to the current working gesture

			// Also update the timestamp when we last saw an interesting vector
			lastValidVectorTimestamp = timestamp;

			if (!isInMotion) {
				isInMotion = true;
				Debug.Log("Motion started");				
				workGesture = new Gesture();
			}
			workGesture.Add(v);
		}
		
	}

}

