using UnityEngine;

// Just an interface for filter classes

interface IFilter {

	// called with a reference to a 3 vector (that may be modified according to the filtering)
	// returns if the current vector is still "interesting" or not
	bool FilterVector(ref Vector3 v);
}