using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldController : MonoBehaviour
{

    public GameObject gameField;
    public GameObject cellsContainer;
    public GameObject cellPrefab;
    public float gap = 0.15f;

    private CoreLogic logic;
    private List<List<CoreLogic.CellData>> field;

    private List<GameObject> valuedCells = new();

    // Start is called before the first frame update
    void Start()
    {
        logic = ScriptableObject.CreateInstance<CoreLogic>();
        field = logic.getStartField();

        buildFieldBackground();
        renderField();
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoveBeenMade = false;
        List<List<CoreLogic.CellData>> previousField = new();
        for (int i = 0; i < field.Count; i++)
        {
            previousField.Add(field[i]);
        }

        if (Input.GetKeyDown("left"))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Left);
            isMoveBeenMade = true;
        }

        if (Input.GetKeyDown("up"))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Up);
            isMoveBeenMade = true;
        }

        if (Input.GetKeyDown("right"))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Right);
            isMoveBeenMade = true;
        }

        if (Input.GetKeyDown("down"))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Down);
            isMoveBeenMade = true;
        }

        if (!isMoveBeenMade)
            return;

        if (logic.ListsEqual(field, previousField))
        {
            // if out move didn't change something, then not generate new element
            return;
        }

        clearFieldFromNewbiemarks();

        (int, int)? newCellPosition = logic.getPositionForNewCell(field);
        if (!newCellPosition.HasValue)
        {
            Debug.Log("You loose!");
            return;
        }

        int newCellValue = logic.getNewCellValue();
        CoreLogic.CellData newCell = new CoreLogic.CellData(newCellValue, false);
        field[newCellPosition.GetValueOrDefault().Item1][newCellPosition.GetValueOrDefault().Item2] = newCell;

        renderField();

        if (!logic.isPossibleToMove(field))
        {
            Debug.Log("You loose!");
            return;
        }
    }

    private void buildFieldBackground()
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
                cell.GetComponent<CellController>().SetValue(0);
                cell.transform.parent = cellsContainer.transform;

                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);

                cell.transform.localPosition =
                    new Vector3(gap * (j + 1) + cellWidth * j, gap * (i + 1) + cellHeight * i) +
                    new Vector3(cellWidth / 2, cellHeight / 2);
            }
        }
    }

    private void renderField()
    {
        foreach(GameObject cell in valuedCells)
        {
            Destroy(cell);
        }
        valuedCells.Clear();

        float gameFieldWidth = gameField.GetComponent<SpriteRenderer>().bounds.size.x;
        float gameFieldHeight = gameField.GetComponent<SpriteRenderer>().bounds.size.y;

        float cellWidth = (gameFieldWidth - 5 * gap) / 4;
        float cellHeight = (gameFieldHeight - 5 * gap) / 4;

        List<List<CoreLogic.CellData>> reversedField = new();
        for(int i = field.Count - 1; i >= 0; i--)
        {
            reversedField.Add(field[i]);
        }

        for (int i = 0; i < reversedField.Count; i++ )
        {
            for (int j = 0; j < reversedField[i].Count; j++)
            {
                GameObject cell = Instantiate(cellPrefab);
                cell.GetComponent<CellController>().SetValue(reversedField[i][j].value);
                cell.transform.parent = cellsContainer.transform;

                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);

                cell.transform.localPosition =
                    new Vector3(gap * (j + 1) + cellWidth * j, gap * (i + 1) + cellHeight * i) +
                    new Vector3(cellWidth / 2, cellHeight / 2);

                valuedCells.Add(cell);
            }
        }
    }

    private void clearFieldFromNewbiemarks()
    {
        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                field[i][j] = new CoreLogic.CellData(field[i][j].value, false);
            }
        }
    }
}
