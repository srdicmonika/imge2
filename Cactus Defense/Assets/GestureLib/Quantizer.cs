
using UnityEngine;
using System;
using System.Collections.Generic;

// The quantizer converts acceleration vectors into symbols

public class Quantizer {

	// The number k of cluster centers we have
	public static int CLUSTER_COUNT = 14;

	// Store the positions of the cluster centers
	private Vector3[] clusterCenters = new Vector3[CLUSTER_COUNT];

	// Implement [] operator to access the cluster centers
	// This is mainly used for the ball visualization
	public Vector3 this[int index] {
		get { return clusterCenters[index]; }
	}
	
	// The radius of the sphere on which the cluster centers lie
	// Currently not used
	public float Radius { get; private set; }
	
	public Quantizer () {
		Radius = 1.0f;
		ResetConstellation();
	}


	// Initialize the custer centers into two great circles on the unit sphere
	public void ResetConstellation() {
		// X/Z Circle
		clusterCenters[0]  = new Vector3(Mathf.Cos(0.0f*Mathf.PI/4.0f), 0.0f, Mathf.Sin (0.0f*Mathf.PI/1.0f));		
		clusterCenters[1]  = new Vector3(Mathf.Cos(1.0f*Mathf.PI/4.0f), 0.0f, Mathf.Sin (1.0f*Mathf.PI/4.0f));		
		clusterCenters[2]  = new Vector3(Mathf.Cos(1.0f*Mathf.PI/2.0f), 0.0f, Mathf.Sin (1.0f*Mathf.PI/2.0f));		
		clusterCenters[3]  = new Vector3(Mathf.Cos(3.0f*Mathf.PI/4.0f), 0.0f, Mathf.Sin (3.0f*Mathf.PI/4.0f));		
		clusterCenters[4]  = new Vector3(Mathf.Cos(1.0f*Mathf.PI/1.0f), 0.0f, Mathf.Sin (1.0f*Mathf.PI/1.0f));		
		clusterCenters[5]  = new Vector3(Mathf.Cos(5.0f*Mathf.PI/4.0f), 0.0f, Mathf.Sin (5.0f*Mathf.PI/4.0f));		
		clusterCenters[6]  = new Vector3(Mathf.Cos(3.0f*Mathf.PI/2.0f), 0.0f, Mathf.Sin (3.0f*Mathf.PI/2.0f));		
		clusterCenters[7]  = new Vector3(Mathf.Cos(7.0f*Mathf.PI/4.0f), 0.0f, Mathf.Sin (7.0f*Mathf.PI/4.0f));		
		
		// Y/Z Circle (1/2Pi and 3/2Pi would be duplicates of above)
		clusterCenters[8]  = new Vector3(0.0f, Mathf.Cos(0.0f*Mathf.PI/1.0f), Mathf.Sin (0.0f*Mathf.PI/1.0f));		
		clusterCenters[9]  = new Vector3(0.0f, Mathf.Cos(1.0f*Mathf.PI/4.0f), Mathf.Sin (1.0f*Mathf.PI/4.0f));		
		clusterCenters[10] = new Vector3(0.0f, Mathf.Cos(3.0f*Mathf.PI/4.0f), Mathf.Sin (3.0f*Mathf.PI/4.0f));		
		clusterCenters[11] = new Vector3(0.0f, Mathf.Cos(1.0f*Mathf.PI/1.0f), Mathf.Sin (1.0f*Mathf.PI/1.0f));				
		clusterCenters[12] = new Vector3(0.0f, Mathf.Cos(5.0f*Mathf.PI/4.0f), Mathf.Sin (5.0f*Mathf.PI/4.0f));				
		clusterCenters[13] = new Vector3(0.0f, Mathf.Cos(7.0f*Mathf.PI/4.0f), Mathf.Sin (7.0f*Mathf.PI/4.0f));		
	}

	// Quantize a vector into an integer
	public int GetNearestIdx(Vector3 v) {
		float sqDist = Mathf.Infinity;
		int resIdx = -1;
		
		// We look at each cluster center and find the one which is nearest to the acceleration vector
		for (int i=0;i<14;i++) {
			Vector3 diff = clusterCenters[i] - v;

			// Note we can comprare squared magnitues directly, since squaring is monotonic
			if (diff.sqrMagnitude < sqDist) {
				sqDist = diff.sqrMagnitude;
				resIdx = i;
			}
		}
				
		return resIdx;
	}	

	// Quantize a complete gesture by quantizing each vector		
	public List<int> QuantizeGesture(Gesture g) {
		List<int> res = new List<int>();

		foreach (Vector3 v in g) {
			res.Add(GetNearestIdx(v));
		}
		return res;
	}

	// Recompute the cluster centroids given a set of gestures
	// Note this does only a single iteration
	public float RecomputeCentroids(List<Gesture> gestures) {

		List<int> symbols = new List<int>();
		List<Vector3> vectors = new List<Vector3>();
		float sqDiff = 0.0f;


		// Find the minimum and maximum accleration in all the gestures
		float min = Mathf.Infinity;
		float max = 0.0f;

		// Build a single list of acceleration vectors, combining all input gestures
		foreach (Gesture g in gestures) {
			symbols.AddRange(QuantizeGesture(g));

			// Also find minimum and maximum
			foreach (Vector3 e in g) {
				vectors.Add(e);
				if (min > e.sqrMagnitude) {
					min = e.sqrMagnitude;
				}
				if (max < e.sqrMagnitude) {
					max = e.sqrMagnitude;
				}
			}
		}

		// Compute the average and use this as the radius of all the cluster centers
		// Note: Currently not
		float avg = (min+max)/2.0f;
		// Radius = avg;

		// Run over all symbols
		for (int k=0; k<14; k++) {

			// Compute new centroid
			Vector3 newCentroid = new Vector3(0.0f, 0.0f, 0.0f);
			int count=0;

			// Run over all the vectors in the input sequence
			for (int i=0; i<symbols.Count; i++) {
				Vector3 e = vectors[i];

				// if the current vector is quantized as the current symbol
				if (symbols[i] == k) {
					// add this vector to the computation of the new centroid
					newCentroid += e;
					count++;
				}
			}

			// If there were some vectors in this cluster
			if (count != 0) {
				// Compute the new cluster center as the mean of all the contributing vectors
				newCentroid /= (float)count;
				// newCentroid /= avg;
				// newCentroid.Normalize();
				Vector3 diff = clusterCenters[k]-newCentroid;

				// Note: we also compute the overall difference in distances to the old
				// cluster center constelation on the fly
				sqDiff += diff.sqrMagnitude;

				// Set new cluster center
				clusterCenters[k] = newCentroid;
			}
		}
		return sqDiff;
	}

	// Compute new constellation for a given set of gestures
	public void ComputeCentroids(List<Gesture> gestures) {
		// Reset into initial constellation
		ResetConstellation();

		float sqDiff = Mathf.Infinity;
		float eps = 0.1f;
		int iterations = 0;

		// Iterate over recomputations until the constallation is stable
		while ((sqDiff > eps) && (iterations < 1000)) {
			sqDiff = RecomputeCentroids(gestures);
			iterations++;
		}
	}
}
