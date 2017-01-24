using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour {
	

	//two endpos
	// four entry pos 
	// two middle pos 
	//all points
	private GameObject[,] allSpawnPoints = new GameObject[4,4];
	//way of enemy
	private Transform[] generatedWay;
	int randNr;
    //make public
    public float timeLeft;
    float timer;
    float shieldTimer = 2f;
    //prefab
    public GameObject enemyClonePrefab;
	//tatsächlicher Clone
	GameObject enemyClone;
	//1.enemy der unsichtbar sein soll/weg sein soll


	// Use this for initialization
	void Start () {
        //get all children / spawnpoints
        //wir setzen hierarchie der Wege
        int steps = allSpawnPoints.GetLength(0);
        int maxSpawns = allSpawnPoints.GetLength(1);
        for (int i = 0; i < steps; i++)
        {
            for(int o = 0; o < maxSpawns; o++)
            {
                Transform t = transform.FindChild("spawnpoint"+i+"-"+o);
                if (t == null)
                    break;
                allSpawnPoints[i, o] = t.gameObject;
            }
        }
        allSpawnPoints[0, 0] = transform.GetChild(0).gameObject;
        allSpawnPoints[0, 1] = transform.GetChild (1).gameObject;
        allSpawnPoints[0, 2] = transform.GetChild(2).gameObject;
        allSpawnPoints[0, 3] = transform.GetChild(3).gameObject;
        allSpawnPoints [1, 0] = transform.GetChild (4).gameObject;
		allSpawnPoints [1, 1] = transform.GetChild (5).gameObject;
		allSpawnPoints [2, 0] = transform.GetChild (6).gameObject;
		allSpawnPoints [2, 1] = transform.GetChild (7).gameObject;
        allSpawnPoints[3, 0] = transform.GetChild(8).gameObject;
        allSpawnPoints[3, 1] = transform.GetChild(9).gameObject;
       

        //for (int i = 0; i < transform.childCount; i++) {
        //	for (int j = 0; j < 2; j++) {
        //		allSpawnPoints [i] = transform.GetChild (i).gameObject;
        //		Debug.Log ("Got child number " + i);
        //	}
    }
		
	//spawnpt 1, 2 sind die oben ( evtl 3. hinzu) 
	//prüfen wenn man da ist geht man zum anderen, dann zu 3,4 dann zu rest!
	
	// Update is called once per frame
	void Update () {
        // we need to make 
        timer -= Time.deltaTime;
        shieldTimer -= Time.deltaTime;
		if (timer < 0) {
            //jedes Mal wenn Timer abläuft dann 
            //in diesem Spawnpoint einen neuen Enemyprefab instanziieren
            Transform spawnpoint = null;
            while(spawnpoint == null)
            {
                randNr = Random.Range(0, 4);
                spawnpoint = allSpawnPoints[0, randNr].transform;
            }
            
            generatedWay = new Transform[allSpawnPoints.GetLength(0)];
			generatedWay[0] = allSpawnPoints[0, randNr].transform;
			enemyClone = Instantiate(enemyClonePrefab, generatedWay[0].transform.position, Quaternion.identity) as GameObject;

            // Füge Shield zum Enemy hinzu
            if(shieldTimer <= 0)
            {
                enemyClone.GetComponent<enemy>().AddShield();
                // Alle 7 - 15 Sekunden neuen Enemy mit Shield spawnen
                shieldTimer = 7f + Random.value * 8;
            }

			timer = timeLeft;
		}

	}

	public GameObject[,] AllSpawnPoints {
		get {
			return allSpawnPoints;
		}
	}
	public Transform[] genWay{
		get{ 
			return generatedWay;
		}

	}

    public void setTimeLeft(float n)
    {
        timeLeft = n;
    }

}
