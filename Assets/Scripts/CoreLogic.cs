using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CoreLogic : ScriptableObject
{
    public enum Direction { Left, Right, Up, Down };

    public class CellData: IEquatable<CellData>
    {
        public int value;
        public bool isCollapsedFromOthers;
        public bool isNew;
        public (int, int)? previousIndex;

        public CellData(int value, bool isCollapsedFromOthers)
        {
            this.value = value;
            this.isCollapsedFromOthers = isCollapsedFromOthers;
            isNew = false;
            previousIndex = null;
        }

        public bool Equals(CellData other)
        {
            return this.value == other.value;
        }
    }

    public List<CellData> makeMove(List<CellData> field)
    {
        List<CellData> fieldCopy = new List<CellData>(field);

        for (int i = 1; i < fieldCopy.Count; i++)
        {
            if (fieldCopy[i].value == 0)
                continue;
            for (int g = i; g > 0; g--)
            {
                bool canMoveToCell =    !fieldCopy[g - 1].isCollapsedFromOthers && (
                                        fieldCopy[g - 1].value == 0 ||
                                        fieldCopy[g - 1].value == fieldCopy[g].value);
                if (canMoveToCell)
                {
                    bool isCollapsedFromOthers = fieldCopy[g - 1].value != 0;
                    CellData newCell = new CellData(fieldCopy[g].value + fieldCopy[g - 1].value, isCollapsedFromOthers);
                    newCell.previousIndex = fieldCopy[g].previousIndex;

                    fieldCopy[g - 1] = newCell;
                    fieldCopy[g] = new CellData(0, false);
                }
            }

        }

        return fieldCopy;
    }

    // переношу эти методы сюда
    private void markCellsCurrentPositions(List<List<CellData>> field)
    {
        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                if (field[i][j].value != 0) field[i][j].previousIndex = (i, j);
            }
        }
    }

    private void
        clearFieldFromMarks(List<List<CellData>> field)
    {
        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                field[i][j] = new CoreLogic.CellData(field[i][j].value, false)
                {
                    previousIndex = null
                };
            }
        }
    }

    // there is one major movement: from to the left
    // another moves are made by rotating or reversing a list
    public List<List<CellData>> makeMove(List<List<CellData>> field, Direction direction)
    {
        List<List<CellData>> fieldCopy = new List<List<CellData>>(field);
        clearFieldFromMarks(fieldCopy);
        markCellsCurrentPositions(fieldCopy);
        switch (direction)
        {
            case Direction.Left:
                for(int j = 0; j < fieldCopy.Count; j++)
                {
                    fieldCopy[j] = makeMove(fieldCopy[j]);
                }
                break;
            case Direction.Right:
                for (int j = 0; j < fieldCopy.Count; j++)
                {
                    List<CellData> fieldCopyReversed = new List<CellData>(fieldCopy[j]);
                    fieldCopyReversed.Reverse();
                    fieldCopyReversed = makeMove(fieldCopyReversed);
                    fieldCopyReversed.Reverse();
                    fieldCopy[j] = fieldCopyReversed;
                }
                break;
            case Direction.Up:
                fieldCopy = rotateList(field, false);
                for (int j = 0; j < fieldCopy.Count; j++)
                {
                    fieldCopy[j] = makeMove(fieldCopy[j]);
                }
                fieldCopy = rotateList(fieldCopy, true);
                break;
            case Direction.Down:
                fieldCopy = rotateList(field, true);
                for (int j = 0; j < fieldCopy.Count; j++)
                {
                    fieldCopy[j] = makeMove(fieldCopy[j]);
                }
                fieldCopy = rotateList(fieldCopy, false);
                break;
        }

        return fieldCopy;
    }

    // need to process a case where field is not rectangle
    public List<List<CellData>> rotateList(List<List<CellData>> field, bool rotateForwards)
    {
        if (rotateForwards)
        {
            List<List<CellData>> result = new();

            for (int i = 0; i < field[0].Count; i++)
            {
                List<CellData> row = new();
                for (int j = field.Count - 1; j >= 0; j--)
                {
                    row.Add(field[j][i]);
                }
                result.Add(row);
            }
            return result;
        } else
        {
            List<List<CellData>> result = new();

            for (int i = field[0].Count - 1; i >= 0; i--)
            {
                List<CellData> row = new();
                for (int j = 0; j < field.Count; j++)
                {
                    row.Add(field[j][i]);
                }
                result.Add(row);
            }
            return result;
        }
    }

    public bool isPossibleToMove(List<List<CellData>> field)
    {
        List<List<CellData>> fieldCopy = new List<List<CellData>>(field);
        foreach (Direction d in Enum.GetValues(typeof(Direction)))
        {
            List<List<CellData>> movedField = makeMove(fieldCopy, d);
            if (!ListsEqual(fieldCopy, movedField))
                return true;
        }
        return false;
    }

    public bool ListsEqual(List<List<CellData>> lhs, List<List<CellData>> rhs)
    {
        if (lhs.Count != rhs.Count) return false;
        for( int i = 0; i < lhs.Count; i++)
        {
            if (!lhs[i].SequenceEqual(rhs[i])) return false;
        }
        return true;
    }

    public (int, int)? getPositionForNewCell(List<List<CellData>> field)
    {
        List<(int, int)> emptyCells = new();
        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                if (field[i][j].value == 0) emptyCells.Add((i, j));
            }
        }
        int randomIndex = UnityEngine.Random.Range(0, emptyCells.Count - 1);

        return emptyCells[randomIndex];
    }

    public int getNewCellValue()
    {
        int randomNumber = UnityEngine.Random.Range(1, 10);
        if (randomNumber == 1)
        {
            return 4;
        } else
        {
            return 2;
        }
    }

    public List<List<CellData>> getStartField()
    {
        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 }
        });

        (int, int)? firstCellIndex = getPositionForNewCell(field);
        if (!firstCellIndex.HasValue) return field;
        int firstCellValue = getNewCellValue();
        field[firstCellIndex.GetValueOrDefault().Item1][firstCellIndex.GetValueOrDefault().Item2] = new CellData(firstCellValue, false);

        (int, int)? secondtCellIndex = getPositionForNewCell(field);
        if (!secondtCellIndex.HasValue) return field;
        int secondCellValue = getNewCellValue();
        field[secondtCellIndex.GetValueOrDefault().Item1][secondtCellIndex.GetValueOrDefault().Item2] = new CellData(secondCellValue, false);

        return field;
    }

    private List<List<CoreLogic.CellData>> convert(List<List<int>> field)
    {
        List<List<CoreLogic.CellData>> result = new();

        for (int i = 0; i < field.Count; i++)
        {
            List<CoreLogic.CellData> row = new();
            for (int j = 0; j < field[i].Count; j++)
            {
                row.Add(new CoreLogic.CellData(field[i][j], false));
            }
            result.Add(row);
        }
        return result;
    }

    private List<CoreLogic.CellData> convert(List<int> field)
    {
        List<CoreLogic.CellData> result = new();

        for (int i = 0; i < field.Count; i++)
        {
            result.Add(new CoreLogic.CellData(field[i], false));
        }
        return result;
    }
}
