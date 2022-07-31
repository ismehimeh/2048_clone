using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsController : MonoBehaviour
{
    public void tappedTryAgainButton()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
