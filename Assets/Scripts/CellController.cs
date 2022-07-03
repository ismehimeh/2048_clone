using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CellController : MonoBehaviour
{
    public TextMeshProUGUI label;
   
    private SpriteRenderer spriteRenderer;

    private Color color0 = new Color32(199, 192, 182, 255);
    private Color color2 = new Color32(236, 228, 219, 255);
    private Color color4 = new Color32(234, 224, 204, 255);
    private Color color8 = new Color32(231, 179, 129, 255);
    private Color color16 = new Color32(231, 153, 107, 255);
    private Color color32 = new Color32(230, 129, 102, 255);
    private Color color64 = new Color32(229, 104, 71, 255);
    private Color color128 = new Color32(232, 207, 126, 255);
    private Color color256 = new Color32(232, 202, 114, 255);
    private Color color512 = new Color32(231, 200, 101, 255);
    private Color color1024 = new Color32(231, 197, 89, 255);
    private Color color2048 = new Color32(230, 194, 80, 255);
    private Color colorGreaterThan2048 = new Color32(234, 61, 50, 255);

    private Color textColor_2_4 = new Color32(117, 110, 102, 255);
    private Color textColor_biggerThan_2_4 = new Color32(248, 246, 242, 255);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(int value)
    {
        label.gameObject.SetActive(value != 0);
        label.text = value.ToString();

        switch(value)
        {
            case 2:
            case 4:
                label.color = textColor_2_4;
                break;
            default:
                label.color = textColor_biggerThan_2_4;
                break;
        }

        switch (value)
        {
            case 0:
                spriteRenderer.color = color0;
                break;
            case 2:
                spriteRenderer.color = color2;
                break;
            case 4:
                spriteRenderer.color = color4;
                break;
            case 8:
                spriteRenderer.color = color8;
                break;
            case 16:
                spriteRenderer.color = color16;
                break;
            case 32:
                spriteRenderer.color = color32;
                break;
            case 64:
                spriteRenderer.color = color64;
                break;
            case 128:
                spriteRenderer.color = color128;
                break;
            case 256:
                spriteRenderer.color = color256;
                break;
            case 512:
                spriteRenderer.color = color512;
                break;
            case 1024:
                spriteRenderer.color = color1024;
                break;
            case 2048:
                spriteRenderer.color = color2048;
                break;
            default:
                spriteRenderer.color = colorGreaterThan2048;
                break;
        }
    }
}
