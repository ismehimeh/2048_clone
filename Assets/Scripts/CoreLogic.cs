using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoreLogic : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down };

    public struct CellData: IEquatable<CellData>
    {
        public int value;
        public bool isNew;

        public CellData(int value, bool isNew)
        {
            this.value = value;
            this.isNew = isNew;
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
                bool canMoveToCell =    !fieldCopy[g - 1].isNew && (
                                        fieldCopy[g - 1].value == 0 ||
                                        fieldCopy[g - 1].value == fieldCopy[g].value);
                if (canMoveToCell)
                {
                    bool isNewCell = fieldCopy[g - 1].value != 0;
                    CellData newCell = new CellData(fieldCopy[g].value + fieldCopy[g - 1].value, isNewCell);
                    fieldCopy[g - 1] = newCell;
                    fieldCopy[g] = new CellData(0, false);
                }
            }

        }

        return fieldCopy;
    }

    // there is one major movement: from to the left
    // another moves are made by rotating or reversing a list
    public List<List<CellData>> makeMove(List<List<CellData>> field, Direction direction)
    {
        List<List<CellData>> fieldCopy = new List<List<CellData>>(field);
        switch(direction)
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
}
