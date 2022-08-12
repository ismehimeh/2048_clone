using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private float updateInterval = 0.1f;

    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float fps;

    private TMP_Text label;


    private void Awake()
    {
        label = GetComponent<TMP_Text>();    
    }

    private void Start()
    {
        timeleft = updateInterval;
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fps = (accum / frames);
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;

            label.text = fps.ToString();
        }
    }
}
