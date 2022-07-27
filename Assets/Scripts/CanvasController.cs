using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{

    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        //Setup();

        //var res = Screen.currentResolution;

        //var gameTitle = CreateGameTitleRectangle();
        //gameTitle.GetComponent<RectTransform>().SetParent(gameObject.transform);
        //gameTitle.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, 0.7f);
        //gameTitle.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.95f);
        //gameTitle.GetComponent<RectTransform>().
    }

    private void Setup()
    {
        gameObject.AddComponent<Canvas>();
        gameObject.name = "UI";
        canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
    }

    private GameObject CreateGameTitleRectangle()
    {
        var view = new GameObject();
        view.AddComponent<Image>();

        var background = view.GetComponent<Image>();
        background.color = new Color32(225, 197, 91, 255);

        return view;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
