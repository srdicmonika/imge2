using UnityEngine;

// Simple filter that prunes vectors that are too similar

public class SimilarityFilter : IFilter {

	private Vector3 lastVector = new Vector3(0,0,0);

	public SimilarityFilter() {

	}

	public bool FilterVector(ref Vector3 v) {

		// Compare with last "intersting" vector
		Vector3 diff = lastVector - v;

		// If difference is too small we ignore this one
		if (diff.sqrMagnitude < 0.2f)
			return false;

		// Else we store it as the next comparison
		lastVector = v;
		return true;
	}
}