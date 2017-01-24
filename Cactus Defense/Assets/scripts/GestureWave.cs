using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureWave : MonoBehaviour {

    public float lifeTime;
    public float fadeOutTime;
    public float endScale;
    Vector3 scaleFrom;
    Vector3 scaleTo;
    float startTime;
    float endTime;
    float matMaxAlpha;
    float endFadeOutTime;
    Renderer r;

	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
        startTime = Time.time;
        endTime = startTime + lifeTime;
        endFadeOutTime = endTime + fadeOutTime;
        scaleTo = new Vector3(endScale, endScale, endScale);
        scaleFrom = new Vector3() + transform.localScale;
        matMaxAlpha = r.material.color.a;
	}
	
	// Update is called once per frame
	void Update () {
        float timeNow = Time.time;
        if (endTime <= timeNow)
        {
            if (endFadeOutTime <= timeNow)
                Destroy(gameObject);

            Color color = r.material.color;
            color.a = Mathf.SmoothStep(matMaxAlpha, 0, (fadeOutTime - (endFadeOutTime - timeNow)) / fadeOutTime);
            r.material.color = color;
        }
        else
        {
            transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, Mathf.Pow((lifeTime - (endTime - timeNow)) / lifeTime, 0.4f));
        }
	}
}
