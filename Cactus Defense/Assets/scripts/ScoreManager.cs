using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {
    public int scr;
    public int enrg;
    public int life;
    private CactusController cactusController;
    public CactusController.RVControl energyChargeControl;
    public ParticleSystem particle;
	private int secretEnergyCount = 400;

    //spawn manager, to change spawn times
    public spawnManager spawner;
    float startTime;
    bool cap = false;

    //level counter
    private int lvl;

    // Wie viel ändert der Drehknopf Wert momentan (Änderung pro Sekunde)?
    private float energyChangeRate = 0;
    private float lastRotateValue;

    void OnGUI()
    {

        GUI.Box(new Rect(30, Screen.height/4- 10, 100, 30), "Score: " + scr);

        GUI.Box(new Rect(30, Screen.height * 2 / 4 - 10, 100, 30),"Energy:" + enrg);

        GUI.Box(new Rect(30, Screen.height * 3 / 4 - 10, 100, 30), "Life: " + life);

    }

    void Start()
    {
        spawner = FindObjectOfType<spawnManager>();
        startTime = spawner.timeLeft;
        cactusController = FindObjectOfType<CactusController>();
        lastRotateValue = cactusController.getRVValue(energyChargeControl);
		Reset();
    }

    void Update()
    {
        checkEnergyCharge();
    }

	void Reset(){
		scr = 0;
		enrg = 100;
		life = 15;
		cactusController.ResetLEDs();
	}

    private void checkEnergyCharge()
    {
        float newRotateValue = cactusController.getRVValue(energyChargeControl);
        float energyChangeRate = Mathf.Abs(lastRotateValue - newRotateValue) / Time.deltaTime;
        lastRotateValue = newRotateValue;
        if (energyChangeRate > 2)
        {
            //addEnergy((int)(energyChangeRate * 2));
            addEnergy((int)(energyChangeRate * 0.3));
            Instantiate(particle);
        }
    }

    public void addScore(int amount)
    {
        scr += amount;
        
        if (!cap && scr > lvl *100)
            increaseLvl();
    }

    public bool useEnergy(int amount)
    {
        if (enrg < amount)
            return false;

        enrg -= amount;
        return true;
    }

    public void addEnergy(int amount)
    {
		if (enrg < 100) {
			int oldEnrg = enrg;
			enrg += amount;
			if (enrg > 100)
				enrg = 100;

			int changed = enrg - oldEnrg;
				//secretEnergyCount += amount;
				//Debug.Log ("secretVar:"+secretEnergyCount);
			secretEnergyCount+= changed;
			if (secretEnergyCount >= 100) {
				cactusController.setLED (CactusController.LED.BLUE, true);
			}
			if (secretEnergyCount >= 200) {
				cactusController.setLED (CactusController.LED.RED, true);
			}
			if (secretEnergyCount >= 300) {
				cactusController.setLED (CactusController.LED.YELLOW, true);
			}
			if (secretEnergyCount >= 400) {
				cactusController.setLED (CactusController.LED.GREEN, true);
			}

			//secretEnergyCount = (100- enrg);
			//Debug.Log ("Amount"+amount);
			//secretEnergyCount += amount;
			Debug.Log ("SecretVar"+secretEnergyCount);
			Debug.Log ("Etatsächliche Energie"+enrg);
		}

    }
    public void decrementLife()
    {
        if (life == 0)
            //load scene
            SceneManager.LoadScene(2);
         life--;
    }
    public int getEnergy()
    {
        return enrg;
    }
    public void increaseLvl()
    {
        lvl++;
        spawner.setTimeLeft(startTime - lvl * 0.2f);
        if (spawner.timeLeft <= 0.4f)
            cap = true;
        Debug.Log("LevelUp, Time: "+spawner.timeLeft);
    }

	public bool SuperMove(){
		return secretEnergyCount >= 400;
	}

	public void ResetSecretEnergy(){
		secretEnergyCount = 0;
	}
}
