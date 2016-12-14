using UnityEngine;
using System.Collections.Generic;
using System;

// Hidden Markov Model

public class HMM {
	
	private int N; // number of states
	private int M; // number of observables
	private int jumplimit; // jumplimit
	private float[] pi; // Initial probabilities
	private float[,] aij; // Transition probabilities
	private float[,] bik; // Observation probabilities


	public HMM(int n, int m, int j) {
		N = n;
		M = m;
		jumplimit = j;

		pi = new float[N];
		aij = new float[N,N];
		bik = new float[N,M];

		initialize();
		// debugPrintState();
	}

	// Note that we use a left-to-right HMM
	// Start state is always 0 and the state can only move to larger indices
	// Jumplimit tell how far we can skip
	public void initialize() {

		// Start probability is 1 for state 0
		pi[0] = 1.0f;
		for (int i=1; i<N; i++) {
			pi[i] = 0.0f;
		}

		// Observation probabilities are equal for all states and observables
		for (int i=0; i<N; i++) {
			for (int k=0; k<M; k++) {
				bik[i,k] = 1.0f / (float)M;
			}
		}

		// Initialize the probability matrix
		// Out edges are next state and states within jump range
		// All edges have equal probability
		for (int i=0; i<N; i++) {			
			int jumpIndex = Math.Min(i+1+jumplimit,N-1);
			int neighSize = jumpIndex-i+1;

			for (int j=i; j<=jumpIndex; j++) {
				aij[i,j] = 1.0f/((float)neighSize);
			}
			for (int j=jumpIndex+1; j<N; j++) {
				aij[i,j] = 0.0f;
			}
		}
	}

	// Train a given set of gestures
	public void train(List<List<int>> trainingData) {
		// Compute new matrices Aij and Bik
		float[,] aij_p = new float[N,N];
		float[,] bik_p = new float[N,M];

		// Compute new transition probabilities
		for (int i=0; i<N; i++) {
			for (int j=0; j<N; j++) {
				float num = 0.0f;
				float denom = 0.0f;

				foreach (List<int> observation in trainingData) {
					float[,] fwd = forwardProc(observation);
					float[,] bwd = backwardProc(observation);
					float prob = GetProbability(observation);

					float inner_num = 0.0f;
					float inner_denom = 0.0f;

					for (int x=0; x<observation.Count-1; x++) {
						inner_num += fwd[i,x]*aij[i,j]*bik[j,observation[x+1]]*bwd[j,x+1];
						inner_denom += fwd[i,x]*bwd[i,x];
					}
					num += (1/prob)*inner_num;
					denom += (1/prob)*inner_denom;
				}
				aij_p[i,j] = num/denom;
			}
		}

		// Compute new emission probabilities
		for (int i=0; i<N; i++) {
			for (int k=0; k<M; k++) {
				float num = 0.0f;
				float denom = 0.0f;

				foreach (List<int> observation in trainingData) {
					float[,] fwd = forwardProc(observation);
					float[,] bwd = backwardProc(observation);
					float prob = GetProbability(observation);

					float inner_num = 0.0f;
					float inner_denom = 0.0f;

					for (int x=0; x<observation.Count-1; x++) {
						if (observation[x] == k) {
							inner_num += fwd[i,x]*bwd[i,x];
						}
						inner_denom += fwd[i,x]*bwd[i,x];
					}
					num += (1/prob)*inner_num;
					denom += (1/prob)*inner_denom;
				}
				bik_p[i,k] = num / denom;
			}
		}

		// Set new matrices
		aij = aij_p;
		bik = bik_p;
	}

	// HMM forward computation
	public float[,] forwardProc(List<int> observation) {
		float[,] f = new float[N,observation.Count];

		for (int i=0; i<N; i++) {
			f[i,0] = pi[i] * bik[i,observation[0]];
		}

		for (int x=1; x<observation.Count; x++) {
			for (int i=0; i<N; i++) {
				float sum = 0.0f;
				for (int j=0; j<N; j++) {
					sum += f[j,x-1]*aij[j,i];
				}
				f[i,x] = sum * bik[i,observation[x]];
			}
		}

		return f;
	}

	// HMM backward computation
	public float[,] backwardProc(List<int> observation) {
		int O = observation.Count;
		float[,] bwd = new float[N,O];

		for(int i=0; i<N; i++) {
			bwd[i,O-1] = 1.0f;
		}

		for (int x=O-2; x>=0; x--) {
			for (int i=0; i<N; i++) {
				bwd[i,x] = 0.0f;
				for (int j=0; j<N; j++) {
					bwd[i,x] += (bwd[j,x+1]*aij[i,j]*bik[j,observation[x+1]]);
				}
			}
		}

		return bwd;
	}

	// Copmpute the probability of a given sequence for this HMM
	// We basically compute the forward matrix and sum over all states
	public float GetProbability(List<int> observation) {

		float prob = 0.0f;

		// compute forward matrix
		float[,] forward = forwardProc(observation);

		// Sum over all states
		for (int i=0; i<N; i++) {
			prob += forward[i,observation.Count-1];
		}

		return prob;
	}

	// Some debug putput
	private void debugPrintState() {
		string res = "";
		res += "N: "+N+"\n";
		res += "M: "+M+"\n";
		for (int i=0; i<N; i++) {
			res += "P"+i+": "+pi[i].ToString()+"\n";
		}
		for (int i=0; i<N; i++) {
			for (int k=0; k<M; k++) {
				res += "B_"+i+"_"+k+": "+bik[i,k].ToString()+"\n";
			}
		}
		for (int i=0; i<N; i++) {
			for (int j=0; j<N; j++) {
				res += "A_"+i+"_"+j+": "+aij[i,j].ToString()+"\n";
			}
		}
		Debug.Log(res);
	}

}
