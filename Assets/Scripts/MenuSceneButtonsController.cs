using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuSceneButtonsController : MonoBehaviour
{
    public void KeepGoingButtonTapped()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void NewGameButtonTapped()
    {
        PlayerPrefs.SetInt("new game", 1);
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
