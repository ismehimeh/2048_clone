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

    private readonly List<GameObject> valuedCells = new();

    float gameFieldWidth;
    float gameFieldHeight;
    float cellWidth;
    float cellHeight;

    // Start is called before the first frame update
    void Start()
    {
        gameFieldWidth = gameField.GetComponent<SpriteRenderer>().bounds.size.x;
        gameFieldHeight = gameField.GetComponent<SpriteRenderer>().bounds.size.y;

        cellWidth = (gameFieldWidth - 5 * gap) / 4;
        cellHeight = (gameFieldHeight - 5 * gap) / 4;

        logic = ScriptableObject.CreateInstance<CoreLogic>();
        field = logic.getStartField();

        BuuildFieldBackground();
        RenderField();
    }

    // Update is called once per frame
    void Update()
    {
        List<List<CoreLogic.CellData>> fieldBeforeMove = new List<List<CoreLogic.CellData>>(field);

        if (!CatchMove()) return;

        if (logic.ListsEqual(field, fieldBeforeMove))
        {
            // if out move didn't change something, then not generate new element
            return;
        }

        AddNewCell();
        RenderField();

        if (!logic.isPossibleToMove(field))
        {
            Debug.Log("You loose!");
            return;
        }
    }

    private bool CatchMove()
    {
        bool isMoveBeenMade = false;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Left);
            isMoveBeenMade = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Up);
            isMoveBeenMade = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Right);
            isMoveBeenMade = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            field = logic.makeMove(field, CoreLogic.Direction.Down);
            isMoveBeenMade = true;
        }

        return isMoveBeenMade;
    }

    private void AddNewCell()
    {
        (int, int)? newCellPosition = logic.getPositionForNewCell(field);
        if (!newCellPosition.HasValue)
        {
            Debug.Log("You loose!");
            return;
        }

        int newCellValue = logic.getNewCellValue();
        var newCell = new CoreLogic.CellData(newCellValue, false)
        {
            isNew = true
        };
        field[newCellPosition.GetValueOrDefault().Item1][newCellPosition.GetValueOrDefault().Item2] = newCell;
    }

    private void BuuildFieldBackground()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var cell = Instantiate(cellPrefab);
                cell.GetComponent<CellController>().SetValue(0);
                cell.transform.parent = cellsContainer.transform;
                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                cell.transform.localPosition = PositionForCell(i, j);
            }
        }
    }

    private Vector3 PositionForCell(int i, int j)
    {
        return new Vector3(gap * (j + 1) + cellWidth * j, gap * (i + 1) + cellHeight * i) +
                    new Vector3(cellWidth / 2, cellHeight / 2);
    }

    private Vector3 PositionForCellReversed(int i, int j, List<List<CoreLogic.CellData>> field)
    {
        var iReversed = field.Count - 1 - i;
        return new Vector3(gap * (j + 1) + cellWidth * j, gap * (iReversed + 1) + cellHeight * iReversed) +
                    new Vector3(cellWidth / 2, cellHeight / 2);
    }

    private void RenderField()
    {
        foreach(var cell in valuedCells)
        {
            Destroy(cell);
        }
        valuedCells.Clear();
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject cell = Instantiate(cellPrefab);
                cell.GetComponent<CellController>().SetValue(field[i][j].value);
                cell.transform.parent = cellsContainer.transform;
                cell.transform.localScale = new Vector3(cellWidth, cellHeight, 1);
                cell.transform.localPosition = PositionForCellReversed(i, j, field);

                valuedCells.Add(cell);

                if (field[i][j].isNew)
                {
                    cell.GetComponent<CellController>().playPopUpAnimation();
                }

                PerformMovementAnimation(cell, field[i][j], i, j, field);
            }
        }
    }

    private void PerformMovementAnimation(  GameObject cell,
                                            CoreLogic.CellData cellData,
                                            int i,
                                            int j,
                                            List<List<CoreLogic.CellData>> field)
    {
        if (cellData.previousIndex.HasValue && cellData.previousIndex != (i, j))
        {
            var iPrevious = cellData.previousIndex.Value.Item1;
            var jPrevious = cellData.previousIndex.Value.Item2;

            var previousPosition = PositionForCellReversed(iPrevious, jPrevious, field);

            var newPosition = PositionForCellReversed(i, j, field);
            cell.GetComponent<CellController>().playAnimationOfMove(previousPosition, newPosition);
        }
    }
}
