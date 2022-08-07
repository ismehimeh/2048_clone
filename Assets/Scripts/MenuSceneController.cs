using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneController : MonoBehaviour
{
    [SerializeField]
    private Image soundImage;

    [SerializeField]
    private Sprite soundOnIcon;
    [SerializeField]
    private Sprite soundOffIcon;

    // Start is called before the first frame update
    void Start()
    {
        var isSoundOn = PlayerPrefs.GetInt("isSoundOn");

        if (isSoundOn == 1)
        {
            soundImage.sprite = soundOnIcon;
        }
        else
        {
            soundImage.sprite = soundOffIcon;
        }
    }
}
