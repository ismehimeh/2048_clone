using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneButtonsController : MonoBehaviour
{

    [SerializeField]
    private Image soundImage;

    [SerializeField]
    private Sprite soundOnIcon;
    [SerializeField]
    private Sprite soundOffIcon;

    public void KeepGoingButtonTapped()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void NewGameButtonTapped()
    {
        PlayerPrefs.SetInt(PrefsStrings.isNewGame, 1);
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void SoundControlButtonTapped()
    {
        var isSoundOn = PlayerPrefs.GetInt(PrefsStrings.isSoundOn);

        if (isSoundOn == 0)
        {
            PlayerPrefs.SetInt(PrefsStrings.isSoundOn, 1);
            soundImage.sprite = soundOnIcon;
        }
        else
        {
            PlayerPrefs.SetInt(PrefsStrings.isSoundOn, 0);
            soundImage.sprite = soundOffIcon;
        }
    }
}
