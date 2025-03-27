using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CellAnimationsTests
{
    private GameObject cellObject;
    private Cell cell;

    /// <summary>
    /// Подготавливает тестовую среду перед каждым тестом:
    /// - Создаёт тестовую ячейку с RectTransform
    /// - Добавляет AudioListener при его отсутствии
    /// </summary>
    [SetUp]
    public void Setup()
    {
        cellObject = new GameObject("CellAnimTest", typeof(RectTransform));
        cell = cellObject.AddComponent<Cell>();
        cell.CellView = cellObject;

        var listenerObj = GameObject.FindObjectOfType<AudioListener>();
        if (listenerObj == null)
        {
            new GameObject("AudioListener", typeof(AudioListener));
        }
    }

    /// <summary>
    /// Очищает тестовую среду после каждого теста:
    /// - Удаляет созданный объект ячейки
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(cellObject);
    }

    /// <summary>
    /// Проверяет плавность анимации перемещения ячейки:
    /// - Из начальной позиции (0,0) в новую позицию (1,1)
    /// - Убеждается, что конечная позиция соответствует ожидаемой
    /// </summary>
    [UnityTest]
    public IEnumerator TestCellMovementAnim()
    {
        cell.Position = new Vector2Int(0, 0);
        Vector2Int newPos = new Vector2Int(1, 1);
        yield return CellAnimations.CellMovementAnim(cell, newPos);
        RectTransform rt = cell.CellView.GetComponent<RectTransform>();
        Vector2 expected = Cell.GetPos(newPos);

        Assert.AreEqual(expected.x, rt.anchoredPosition.x, 0.001f);
        Assert.AreEqual(expected.y, rt.anchoredPosition.y, 0.001f);
    }

    /// <summary>
    /// Проверяет анимацию появления новой ячейки:
    /// - Начинает с нулевого масштаба
    /// - Проверяет, что после анимации масштаб становится (1,1,1)
    /// </summary>
    [UnityTest]
    public IEnumerator TestCellGenerateAnim()
    {
        RectTransform rt = cellObject.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        yield return CellAnimations.CellGenerateAnim(cell);
        Assert.That(rt.localScale.x, Is.EqualTo(1f).Within(0.001f));
        Assert.That(rt.localScale.y, Is.EqualTo(1f).Within(0.001f));
        Assert.That(rt.localScale.z, Is.EqualTo(1f).Within(0.001f));
    }

    /// <summary>
    /// Проверяет анимацию слияния двух ячеек:
    /// - Создаёт исходную и целевую ячейки
    /// - Запускает анимацию слияния
    /// - Проверяет уничтожение исходной ячейки
    /// - Убеждается, что целевая ячейка имеет нормальный масштаб после подпрыгивания
    /// </summary>
    [UnityTest]
    public IEnumerator TestCellMergedAnim()
    {
        GameObject oldObj = new GameObject("OldCell", typeof(RectTransform), typeof(AudioSource));
        GameObject newObj = new GameObject("NewCell", typeof(RectTransform), typeof(AudioSource));

        Cell oldCell = oldObj.AddComponent<Cell>();
        Cell newCell = newObj.AddComponent<Cell>();
        oldCell.CellView = oldObj;
        newCell.CellView = newObj;

        oldObj.GetComponent<RectTransform>().localScale = Vector3.one;
        oldObj.GetComponent<RectTransform>().localScale = Vector3.one;
        newObj.GetComponent<RectTransform>().localScale = Vector3.one;

        yield return CellAnimations.CellMergedAnim(oldCell, newCell);

        Assert.IsTrue(oldCell == null || oldObj == null);

        Vector3 finalScale = newObj.GetComponent<RectTransform>().localScale;
        Assert.That(finalScale.x, Is.EqualTo(1f).Within(0.1f));
        Assert.That(finalScale.y, Is.EqualTo(1f).Within(0.1f));
        Assert.That(finalScale.z, Is.EqualTo(1f).Within(0.1f));
    }
}