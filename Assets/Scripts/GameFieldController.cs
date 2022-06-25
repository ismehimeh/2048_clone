using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldController : MonoBehaviour
{

    public GameObject gameField;
    public GameObject cellsContainer;
    public GameObject cellPrefab;
    public float gap = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        float gameFieldWidth = gameField.GetComponent<SpriteRenderer>().bounds.size.x;
        float gameFieldHeight = gameField.GetComponent<SpriteRenderer>().bounds.size.y;

        float cellWidth = (gameFieldWidth - 5 * gap) / 4;
        float cellHeight = (gameFieldHeight - 5 * gap) / 4;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject cell = Instantiate(cellPrefab);
                cell.transform.parent = cellsContainer.transform;

                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);

                cell.transform.localPosition =
                    new Vector3(gap * (j + 1) + cellWidth * j, gap * (i + 1) + cellHeight * i) +
                    new Vector3(cellWidth / 2, cellHeight / 2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
