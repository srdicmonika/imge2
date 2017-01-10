using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public int scr;
    public int enrg;
    public int life;
    private CactusController cactusController;
    public CactusController.RVControl energyChargeControl;

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
        cactusController = FindObjectOfType<CactusController>();
        lastRotateValue = cactusController.getRVValue(energyChargeControl);
    }

    void Update()
    {
        checkEnergyCharge();
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
        }
    }

    public void addScore(int amount)
    {
        scr += amount;
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
        enrg += amount;
    }
}
