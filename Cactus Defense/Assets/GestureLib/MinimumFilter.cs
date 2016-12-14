using UnityEngine;

// Filter that removes all acceleration vectors that are below a minimum threshold
// This removes sections of no movement

public class MinimumFilter : IFilter {

	public MinimumFilter() {

	}

	public bool FilterVector(ref Vector3 v) {

		if (v.sqrMagnitude < 1.0f)
			return false;

		return true;
	}
}