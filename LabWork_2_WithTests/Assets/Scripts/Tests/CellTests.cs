using NUnit.Framework;
using UnityEngine;

public class CellTests
{
    private GameObject cellObject;
    private Cell cell;
    private bool valueChangedFired;
    private bool posChangedFired;
    private Vector2Int newPosition;

    [SetUp]
    public void Setup()
    {
        cellObject = new GameObject("CellTest");
        cellObject.AddComponent<RectTransform>();
        cell = cellObject.AddComponent<Cell>();
        cell.CellView = cellObject; // имитация CellView, чтобы не было Null

        valueChangedFired = false;
        posChangedFired = false;

        cell.OnValueChanged += (val) => valueChangedFired = true;
        cell.OnPositionChanged += (pos) =>
        {
            posChangedFired = true;
            newPosition = pos;
        };
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(cellObject);
    }

    [Test]
    public void TestValueChangeEvent()
    {
        cell.Value = 2;
        Assert.IsTrue(valueChangedFired);
        Assert.AreEqual(2, cell.Value);
    }

    [Test]
    public void TestPositionChangeEvent()
    {
        Vector2Int pos = new Vector2Int(1, 1);
        cell.Position = pos;
        Assert.IsTrue(posChangedFired);
        Assert.AreEqual(pos, newPosition);
    }

    [Test]
    public void TestGetPosCalculation()
    {
        Vector2Int pos = new Vector2Int(2, 3);
        Vector2 worldPos = Cell.GetPos(pos);
        // x = 2*144 - 208.5 = 79.5, y = 3*144 - 318 = 114
        Assert.AreEqual(79.5f, worldPos.x, 0.0001f);
        Assert.AreEqual(114f, worldPos.y, 0.0001f);
    }
}