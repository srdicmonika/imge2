using UnityEngine;
using System.Collections;
using System.IO.Ports;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CactusController : MonoBehaviour {

    public enum RVControl
    {
        RV1 = 0,
        RV2 = 1,
        RV3 = 2,
        RV4 = 3
    }

    public enum CactusButton
    {
        SW1 = 0x40,
        SW2 = 0x80,
        SW3 = 0x100,
        SW4 = 0x200,
        SW5 = 0x400,
        SW6 = 0x800,

        RIGHT = SW5,
        LEFT = SW6,
        FIRE = SW3
    }

    public enum LED
    {
        BLUE = 0,
        RED = 1,
        YELLOW = 2,
        GREEN = 3
    }

    public enum Acceleration
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    // Setting to reduce amount of polling (simply deactivate features you dont need!)
    public bool EnableAcceleration, EnableButtons, EnableSliders, EnableSound;

    private SerialPort stream = new SerialPort("COM3", 115200);
    private string receivedData = "EMPTY";
    private StreamProcessor sp = new StreamProcessor();
    private Text receivedDataText;
    // Values of the sliders in [0, 1]
    private float[] rvValues;
    // Pressed buttons (use bitmask to check for pressed buttons)
    private int cactusButtonVal;
    // Remaining rumble time
    private float rumbleDuration;
    // You know what it is
    private int rumbleIntensity = 0;
    // Time passed since last rumble
    private float lastRumbleChange = 0;
    // false if rumble change needs to be applied in Update
    private bool rumbleChangeApplied = true;
    // Holds the current LED status {0 = off, 1 = on}
    private int[] ledStatus = new int[] { 0, 0, 0, 0 };
    // The currect acceleration vector
    private Vector3 acceleration;
    // The volume measured by the microphone
    private float rmsVolume;
    

    // ----------------------------- Gesture Detection ----------------------------------
    // The last gesture detected by the stream processor
    private Gesture gesture;
    // The library of gesture models and the name of the currently selected gesture
    private string gestureName = "Unnamed";
    private GestureLibrary gestureLibrary = new GestureLibrary();
    // Methods that will be called upon recognized gestures
    private Action<string> gestureCallbacks;


    // Use this for initialization
    void Start () {
        if (!stream.IsOpen)
        {
            stream.Open(); //Open the Serial Stream.
            Debug.Log("Cactus Controller Serial Port opened.");

            if(EnableAcceleration)
                PollAcceleration();
        }

        // Load Gestures
        string[] gestureNames = { "universal" };
        foreach(string gestureName in gestureNames)
        {
            string fileName = "Assets\\Gestures\\gesture_" + gestureName + ".xml"; // Application.persistentDataPath+"/gesture_"+gestureName+".xml";
            Debug.Log("Loading from: " + fileName);
            gestureLibrary[gestureName] = new GestureModel();
            gestureLibrary[gestureName].load(fileName);
        }
    }
	
    void FixedUpdate()
    {
        if(EnableAcceleration)
            PollAcceleration();
    }

	// Update is called once per frame
	void Update () {

        if (EnableSliders)
        {
            // Poll slider values
            stream.Write("4");
            receivedData = stream.ReadLine();
            string[] splitReceived = receivedData.Split(' ');
            rvValues = new float[] {
                analogToFloat(System.Convert.ToInt32(splitReceived[1], 16), 0xfff),
                analogToFloat(System.Convert.ToInt32(splitReceived[2], 16), 0xfff),
                analogToFloat(System.Convert.ToInt32(splitReceived[3], 16), 0xfff),
                analogToFloat(System.Convert.ToInt32(splitReceived[4], 16), 0xfff)
            };
        }

        if (EnableButtons)
        {
            // Poll pressed buttons
            stream.Write("1");
            string cactusData = stream.ReadLine();
            cactusButtonVal = System.Convert.ToInt32(cactusData, 16);
        }

        // Poll Volume
        if (EnableSound)
        {
            rmsVolume = PollVolume();
        }

        // Apply rumble 
        if (!rumbleChangeApplied && Time.fixedTime - lastRumbleChange >= 0.05f)
        {
            applyRumble();
        }

        // If still rumbling, check how long it still rumbles
        if (rumbleDuration > 0) {
            rumbleDuration -= Time.deltaTime;
            // If duration has expired, stop rumbling
            if (rumbleDuration <= 0) {
                setRumble(0, 0);
            }
        }   
    }

    private float PollVolume()
    {
        float volume = 0f;
        stream.Write("s");
        receivedData = stream.ReadLine();
        string[] splitData = receivedData.Split(' ');
        if(splitData[0] == "RMS:")
        {
            volume = float.Parse(splitData[1], System.Globalization.CultureInfo.InvariantCulture);
            Debug.Log(String.Format("Lautstärke: {0}", volume));
        }
        return volume;
    }

    private void PollAcceleration()
    {
        stream.Write("a");
        receivedData = stream.ReadLine();
        string[] splitData = receivedData.Split(' ');
        if (splitData[0] == "Accel:")
        {
            int accelX = System.Convert.ToInt32(splitData[1], 16);
            int accelY = System.Convert.ToInt32(splitData[2], 16);
            int accelZ = System.Convert.ToInt32(splitData[3], 16);

            if (accelX > 127)
                accelX -= 256;

            if (accelY > 127)
                accelY -= 256;

            if (accelZ > 127)
                accelZ -= 256;

            float gravity = 0.507f;

            /*acceleration[(int)Acceleration.X] = analogToFloat(accelX, 128) / gravity;
            acceleration[(int)Acceleration.Y] = analogToFloat(accelY, 128) / gravity;
            acceleration[(int)Acceleration.Z] = analogToFloat(accelZ, 128) / gravity;*/

            // Create new dierction vector
            // NOTE: We need to flip x and z axes to be compatible to Unity coordinate system
            acceleration = new Vector3(-analogToFloat(accelY, 128), analogToFloat(accelZ, 128), -analogToFloat(accelX, 128));

            // Process this new vector
            sp.ProcessRawMeasurement(acceleration);

            // if a gesture is available from the stream processor
            if (sp.GestureIsValid)
            {

                // we copy it
                // Note this resets also GestureIsValid
                gesture = sp.Gesture;

                // Try to classify the gestrue by the libraray
                // Log this result
                string name = gestureLibrary.Classify(gesture);
                
                if (name != "")
                {
                    Debug.Log("Classified as: " + name);
                    if(gestureCallbacks != null)
                        gestureCallbacks(name);
                }
                    
                //Debug.Log(String.Format("X: {0}, Y: {1}, Z: {2}", getAcceleration(Acceleration.X), getAcceleration(Acceleration.Y), getAcceleration(Acceleration.Z)));
                //tanax = 360f/(2*(float)Math.PI)*(float)Math.Atan(getAcceleration(Acceleration.X) / getAcceleration(Acceleration.Z));
                //tanay = 360f / (2 * (float)Math.PI) * (float)Math.Atan(getAcceleration(Acceleration.Y) / getAcceleration(Acceleration.Z);)
            }
        }
        
    }
    
    private float analogToFloat(int value, int maxVal)
    {
        return (float)value / maxVal;
    }

    public float getAcceleration(Acceleration a)
    {
        return acceleration[(int)a];
    }

    public float getRVValue(RVControl control)
    {
        return rvValues[(int)control];
    }

    public float getVolume()
    {
        return rmsVolume;
    }

    public bool buttonPressed(CactusButton button)
    {
        return (cactusButtonVal & (int)button) != 0;
    }

    public void setRumble(int intensity, float duration)
    {
        setRumble(intensity);
        rumbleDuration = duration;
    }

    public void setRumble(int intensity)
    {
        rumbleIntensity = intensity;

        if (rumbleIntensity > 1000)
            rumbleIntensity = 1000;
        else if (rumbleIntensity < 0)
            rumbleIntensity = 0;

        rumbleChangeApplied = false;
    }

    public void applyRumble()
    {
        stream.Write("m " + rumbleIntensity + "\r\n");
        string response = stream.ReadLine();
        if(response == "Done.")
        {

        }
        lastRumbleChange = Time.fixedTime;
        rumbleChangeApplied = true;
    }

    public void setLED(LED led, bool on)
    {
        int val = on ? 1 : 0;
        if (ledStatus[(int)led] != val)
        {
            stream.Write("l " + (int)led + " " + val + "\r\n");
            string response = stream.ReadLine();
            if (response == "Done.")
                ledStatus[(int)led] = val;
        }
    }

    public void toggleLED(LED led)
    {
        bool on = ledStatus[(int)led] == 1 ? false : true;
        setLED(led, on);
    }

    public void addGestureCallback(Action<string> callback)
    {
        gestureCallbacks += callback;
    }

}
