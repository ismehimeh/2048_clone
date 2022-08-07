using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MainLogicTests
{
    public List<List<CoreLogic.CellData>> convert(List<List<int>> field)
    {
        List<List<CoreLogic.CellData>> result = new();

        for(int i = 0; i < field.Count; i++)
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

    public List<CoreLogic.CellData> convert(List<int> field)
    {
        List<CoreLogic.CellData> result = new();

        for (int i = 0; i < field.Count; i++)
        {
            result.Add(new CoreLogic.CellData(field[i], false));
        }
        return result;
    }

    [Test]
    public void RotationForward()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
                                                            new List<int>{1, 2, 3},
                                                            new List<int>{4, 5, 6},
                                                            new List<int>{7, 8, 9}});

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
                                                            new List<int>{7, 4, 1},
                                                            new List<int>{8, 5, 2},
                                                            new List<int>{9, 6, 3}});
        List<List<CoreLogic.CellData>> result = sut.rotateList(field, true);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void RotationBackward()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
                                                            new List<int>{1, 2, 3},
                                                            new List<int>{4, 5, 6},
                                                            new List<int>{7, 8, 9}});

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
                                                            new List<int>{3, 6, 9},
                                                            new List<int>{2, 5, 8},
                                                            new List<int>{1, 4, 7}});
        List<List<CoreLogic.CellData>> result = sut.rotateList(field, false);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void Rotation222Backward()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
            new List<int> { 2, 0, 2, 0 },
            new List<int> { 2, 0, 2, 0 },
            new List<int> { 2, 0, 2, 0 },
            new List<int> { 0, 0, 0, 0 }
            });

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
            new List<int> { 0, 0, 0, 0 },
            new List<int> { 2, 2, 2, 0 },
            new List<int> { 0, 0, 0, 0 },
            new List<int> { 2, 2, 2, 0 }
            });

        List<List<CoreLogic.CellData>> result = sut.rotateList(field, false);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void SimpleMoveOf0002()
    {
        CoreLogic sut = new CoreLogic();

        List<CoreLogic.CellData> field =    convert(new List<int> { 0, 0, 0, 2 });
        List<CoreLogic.CellData> expected = convert(new List<int> { 2, 0, 0, 0 });

        List<CoreLogic.CellData> result = sut.makeMove(field);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void SimpleMoveOf0022()
    {
        CoreLogic sut = new CoreLogic();

        List<CoreLogic.CellData> field =    convert(new List<int> { 0, 0, 2, 2 });
        List<CoreLogic.CellData> expected = convert(new List<int> { 4, 0, 0, 0 });

        List<CoreLogic.CellData> result = sut.makeMove(field);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void SimpleMoveOf0222()
    {
        CoreLogic sut = new CoreLogic();

        List<CoreLogic.CellData> field =    convert(new List<int> { 0, 2, 2, 2 });
        List<CoreLogic.CellData> expected = convert(new List<int> { 4, 2, 0, 0 });

        List<CoreLogic.CellData> result = sut.makeMove(field);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void SimpleMoveOf2220()
    {
        CoreLogic sut = new CoreLogic();

        List<CoreLogic.CellData> field =    convert(new List<int> { 2, 2, 2, 0 });
        List<CoreLogic.CellData> expected = convert(new List<int> { 4, 2, 0, 0 });

        List<CoreLogic.CellData> result = sut.makeMove(field);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void SimpleMoveOf4422()
    {
        CoreLogic sut = new CoreLogic();

        List<CoreLogic.CellData> field =    convert(new List<int> { 4, 4, 2, 2 });
        List<CoreLogic.CellData> expected = convert(new List<int> { 8, 4, 0, 0 });

        List<CoreLogic.CellData> result = sut.makeMove(field);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void SimpleMoveOf2204()
    {
        CoreLogic sut = new CoreLogic();

        List<CoreLogic.CellData> field =    convert(new List<int> { 2, 2, 0, 4 });
        List<CoreLogic.CellData> expected = convert(new List<int> { 4, 4, 0, 0 });

        List<CoreLogic.CellData> result = sut.makeMove(field);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void MoveCollapse2and2and2ToLeft()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 2, 2, 2, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 2, 2, 2, 0 }
        });

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 4, 2, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 4, 2, 0, 0 }
        });

        List<List<CoreLogic.CellData>> result = sut.makeMove(field, Direction.Left);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void MoveCollapse2and2and2ToRight()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 2, 2, 2, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 2, 2, 2, 0 }
        });

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 2, 4 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 2, 4 }
        });

        List<List<CoreLogic.CellData>> result = sut.makeMove(field, Direction.Right);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void MoveCollapse2and2and2ToUp()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 2, 0, 2, 0 }
        });

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
        new List<int> { 4, 0, 4, 0 },
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 }
        });

        List<List<CoreLogic.CellData>> result = sut.makeMove(field, Direction.Up);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void MoveCollapse2and2and2ToDown()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 0, 0, 0, 0 }
        });

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 2, 0, 2, 0 },
        new List<int> { 4, 0, 4, 0 }
        });

        List<List<CoreLogic.CellData>> result = sut.makeMove(field, Direction.Down);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void MoveInpossibleToMoveFieldToLeft()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 },
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 }
        });

        List<List<CoreLogic.CellData>> expected = convert(new List<List<int>> {
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 },
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 }
        });

        List<List<CoreLogic.CellData>> result = sut.makeMove(field, Direction.Left);

        Assert.AreEqual(expected, result);
    }

    [Test]
    public void NotPossibleToMakeAMove()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 },
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 }
        });

        bool result = sut.isPossibleToMove(field);

        Assert.AreEqual(false, result);
    }

    [Test]
    public void IsPossibleToMakeAMove1()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 2, 4, 2, 4 },
        new List<int> { 4, 2, 4, 2 },
        new List<int> { 2, 4, 4, 4 },
        new List<int> { 4, 2, 4, 2 }
        });

        bool result = sut.isPossibleToMove(field);

        Assert.AreEqual(true, result);
    }

    [Test]
    public void IsPossibleToMakeAMove2()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 4, 0 },
        new List<int> { 0, 0, 0, 0 }
        });

        bool result = sut.isPossibleToMove(field);

        Assert.AreEqual(true, result);
    }

    [Test]
    public void IsScore4CalculatesCorrect()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 2, 0 },
        new List<int> { 0, 0, 2, 0 }
        });

        List<List<CoreLogic.CellData>> resultField = sut.makeMove(field, Direction.Down);
        var result = sut.GetScore(resultField);
        Assert.AreEqual(4, result);
    }

    [Test]
    public void IsScore8CalculatesCorrect()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 4, 0 },
        new List<int> { 0, 0, 4, 0 }
        });

        List<List<CoreLogic.CellData>> resultField = sut.makeMove(field, Direction.Down);
        var result = sut.GetScore(resultField);
        Assert.AreEqual(8, result);
    }

    [Test]
    public void IsScore1048CalculatesCorrect()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 8, 512, 4, 0 },
        new List<int> { 8, 512, 4, 0 }
        });

        List<List<CoreLogic.CellData>> resultField = sut.makeMove(field, Direction.Down);
        var result = sut.GetScore(resultField);
        Assert.AreEqual(1048, result);
    }

    [Test]
    public void IsScore0CalculatesCorrect()
    {
        CoreLogic sut = new CoreLogic();

        List<List<CoreLogic.CellData>> field = convert(new List<List<int>> {
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 0, 0, 0, 0 },
        new List<int> { 8, 0, 0, 0 },
        new List<int> { 0, 0, 4, 0 }
        });

        List<List<CoreLogic.CellData>> resultField = sut.makeMove(field, Direction.Down);
        var result = sut.GetScore(resultField);
        Assert.AreEqual(0, result);
    }
}
