using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{

    [SerializeField]
    private TMP_Text scoreLabel;

    void Start()
    {
        scoreLabel.text = PlayerPrefs.GetInt("score").ToString();
    }
}
