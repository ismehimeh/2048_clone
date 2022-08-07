using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneAudioController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var isSoundOn = PlayerPrefs.GetInt("isSoundOn");

        if (isSoundOn == 1)
            GetComponent<AudioSource>().Play();
    }
}
