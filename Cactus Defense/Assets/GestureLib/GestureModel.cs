using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml.Serialization;
using System.IO;


// The gesture model is the model description of a gesture togethwe with the set of training
// data from which it was derived

public class GestureModel {

	// Collection of training data gestrues
	public GestureCollection gc = new GestureCollection();

	// The quantizer that translates acceleration vectors into symbols
	public Quantizer q = new Quantizer();

	// A Hidden Markov Model for teh symbol sequences
	// Parameters are number of states, number of input symbols (= number of quantization clusters) and 
	// jump distance (internal HMM parameter)
	public HMM hmm = new HMM(5,14,3);
	
	// A priory probability for this gesture as derived from the training data
	public float DefaultProbability = 0.0f;


	public GestureModel() {
	}

	// Convert the gesture collection into XML representation
	public string ToXML() {
		XmlSerializer serializer = new XmlSerializer(typeof(GestureCollection));
		StringWriter sW = new StringWriter();
		serializer.Serialize(sW, gc);
		sW.Close();

		return sW.ToString();		
	}

	public void save(string fileName) {
		string xml = ToXML();
		StreamWriter fileWriter = File.CreateText(fileName);
		fileWriter.Write(xml);
		fileWriter.Close();		
	}

	// Convert an XML representation back into a set of training data
	public void FromXML(string xml) {
		XmlSerializer deserializer = new XmlSerializer(typeof(GestureCollection));
		TextReader tR = new StringReader(xml);
		gc = (GestureCollection)deserializer.Deserialize(tR);
		tR.Close();

		// trigger training for this gesture with the new data
		Update();
	}

	public void load(string fileName) {
		StreamReader fileRead = File.OpenText(fileName);
		FromXML(fileRead.ReadToEnd());
		fileRead.Close();
	}

	// Add a gesture to the training set
	public void AddTrainingGesture(Gesture g) {
		gc.gestures.Add(g);
	}

	// Recompute the quantization clusters
	public void RecomputeClusters() {
		q.ComputeCentroids(gc.gestures);
	}

	// Complete trainging for this gesture model
	// Consists of computing quatization clusters and training the HMM
	public void Update() {
		RecomputeClusters();
		TrainHMM();
	}

	// Train the Hidden Markov Model
	public void TrainHMM() {
		// First reset it back to default values
		hmm.initialize();

		// Construct a list of symbol sequences representing all the training gestures
		List<List<int>> seqs = new List<List<int>>();
		foreach (Gesture g in gc.gestures) {
			seqs.Add(q.QuantizeGesture(g));
		}

		// train these equences
		hmm.train(seqs);

		// recompute the a priori probability
		// just compute the mean of all the probabilities of the training gestures
		DefaultProbability = 0.0f;
		foreach (Gesture g in gc.gestures) {
			DefaultProbability += MatchGesture(g);
		}
		DefaultProbability /= gc.gestures.Count;
	}

	// Get the probability of a given observation sequence for this gesture
	public float MatchGesture(Gesture g) {

		return hmm.GetProbability(q.QuantizeGesture(g));
	}

	// Just some debug output
	public void DebugLogAllTrainingGestures() {
		foreach (Gesture g in gc.gestures) {
			string tmp = "";
			List<int> sym = q.QuantizeGesture(g);
			foreach (int i in sym) {
				tmp += i;
				tmp += " ";
			}

			Debug.Log(tmp);			
		}
	}
}