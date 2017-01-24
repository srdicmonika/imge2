using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {


	//for cloning
	public GameObject enemiezPrefab;
    public GameObject shieldPrefab;
    GameObject shieldObject;
    bool shield = false;
	GameObject enemiezClones;
	//sets random spawnpoint
	public float speed;
	//for timer
	public float timeLeft;
	//sets random number
	int randNr;
	//array of all waypoints(set in inspector)
	public Transform[] waypointList;
	public int currentWayPoint=0;
	Transform targetWayPoint;
    //actual route of enemy

    //for the particlesystem effect
    public ParticleSystem particleD;
    public ParticleSystem particleT;

    private ScoreManager scoreManager;
	private spawnManager spawnmanager;

    private bool dead = false;
    private float deadAnimationTime = 2f;
    private float deadFor = 0f;


    private bool initialized = false;
    private bool flyingAway = false;
    // Use this for initialization
    void Start () {
		spawnmanager = GameObject.FindObjectOfType<spawnManager> ();
		//randNr = Random.Range (1, 2);
		//if (randNr == 1) {
		//	waypointList [randNr] = GameObject.Find ("spawnpoint1").transform.position;

		//if (randNr == 2) {
		//	waypointList [randNr] = GameObject.Find ("spawnpoint2").transform.position;

		int numSpawnpoints = 4;
		waypointList = spawnmanager.genWay;

		for (int i = 1; i < 4; i++) {
            GameObject g = null;
            Transform t = null;
            while (g == null)
            {
                g = spawnmanager.AllSpawnPoints[i, Random.Range(0, numSpawnpoints - 1)];
                if(g != null)
                    t = g.transform;
            }
                
            waypointList[i] = t;
		}

		targetWayPoint = waypointList [1];
        transform.LookAt(targetWayPoint);

        scoreManager = GameObject.FindObjectOfType<ScoreManager>();
        initialized = true;
    }
	
	// Update is called once per frame
	void Update () {
        //player moves towards tower 
        //transform.Translate(Vector3.left* speed * Time.deltaTime); automatic movement in one direction
        // check if we have somewere to walk
        if (dead)
        {
            deadFor += Time.deltaTime;
            float progress = deadFor / deadAnimationTime;

            if(shieldObject != null)
            {
                Color c = shieldObject.GetComponent<Renderer>().material.color;
                c.a = Mathf.Clamp(1 - progress, 0, 1);
                shieldObject.GetComponent<Renderer>().material.color = c;
            }
            
            if (progress >= 1)
            {
                Destroy(shieldObject);
                Destroy(gameObject);
            }
        }

        if(!flyingAway)
            walk();

	}

	void walk(){
		// rotate towards the target
		transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, speed*Time.deltaTime, 0.0f);

		// move towards the target
		//transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position,   speed*Time.deltaTime);
		transform.Translate(Vector3.forward * Time.deltaTime * speed);

		/*if(transform.position == targetWayPoint.position)
		{
			currentWayPoint++;
			Debug.Log ("reach");
			if (waypointList.Length > currentWayPoint) 
			{
				targetWayPoint = waypointList [currentWayPoint];
			}
		}*/
	} 


	void OnTriggerEnter(Collider other) {
        if(initialized)
        {
            if (other.gameObject.tag == "waypoint")
            {
                if (currentWayPoint + 1 < waypointList.Length)
                {
                    if (other.gameObject.transform == targetWayPoint || currentWayPoint == 0)
                        targetWayPoint = waypointList[++currentWayPoint];
                }
            }
            if ((other.gameObject.tag == "CactusTower"))
            {
                if (!shield)
                {
                Instantiate(particleD,transform.position,transform.rotation);
                    scoreManager.addScore(10);
                    RemoveEnemyAndShield();
                }
            }
            if ((other.gameObject.tag == "target"))
            {
                RemoveEnemyAndShield(true);
                scoreManager.decrementLife();
            }
            if (other.gameObject.tag == "GestureWave")
            {
                if (shield)
                {
                    DestroyShield();
                }
            }
            if (other.gameObject.tag == "SpecialMove")
            {
                scoreManager.addScore(10);
                flyingAway = true;
                Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
                rigid.isKinematic = false;
                rigid.useGravity = true;
                rigid.AddForce((Vector3.Normalize(transform.position - other.gameObject.transform.position) + Vector3.up) * 500);
                StartCoroutine(DestroyLater(5f));
            }

        }
    }

    IEnumerator DestroyLater(float time)
    {
        yield return new WaitForSeconds(time);
        RemoveEnemyAndShield();
    }

    private void RemoveEnemyAndShield(bool removeImmediately = false)
    { 
        if(shieldObject == null)
        {
            Destroy(gameObject);
        } else
        {
            if (removeImmediately)
            {
                Destroy(shieldObject);
                Destroy(gameObject);
            }
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            dead = true;
        }
        
    }

    public void AddShield()
    {
        shield = true;
        shieldObject = Instantiate(shieldPrefab, gameObject.transform.position + Vector3.forward * 0.5f, Quaternion.Euler(90, 0, 0), gameObject.transform);
    }

    public void DestroyShield()
    {
        GameObject emptyParentObject = new GameObject();
        emptyParentObject.transform.position = shieldObject.transform.position;
        shieldObject.transform.SetParent(emptyParentObject.transform);
        shield = false;
        shieldObject.GetComponent<Animator>().SetTrigger("Destroyed");
        //Destroy(shieldObject);
    }

}

