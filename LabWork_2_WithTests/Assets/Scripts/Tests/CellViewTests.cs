using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellViewTests
{
    private GameObject cellViewRoot;
    private GameObject textObj;
    private GameObject imageObj;
    private CellView cellView;
    private Cell cell;
    private TextMeshProUGUI tmp;
    private Image img;

    /// <summary>
    /// Подготавливает тестовую среду перед каждым тестом:
    /// - Создаёт родительский объект и дочерние элементы для текста и изображения
    /// - Настраивает компоненты CellView и Cell
    /// - Связывает приватные поля через Reflection
    /// </summary>
    [SetUp]
    public void Setup()
    {
        cellViewRoot = new GameObject("CellViewTest", typeof(RectTransform));

        textObj = new GameObject("TMP", typeof(RectTransform));
        textObj.transform.SetParent(cellViewRoot.transform, false);
        tmp = textObj.AddComponent<TextMeshProUGUI>();

        imageObj = new GameObject("Image", typeof(RectTransform));
        imageObj.transform.SetParent(cellViewRoot.transform, false);
        img = imageObj.AddComponent<Image>();

        cellView = cellViewRoot.AddComponent<CellView>();
        cell = cellViewRoot.AddComponent<Cell>();
        cell.CellView = cellViewRoot;

        FieldInfo fiTxt = typeof(CellView).GetField("_cellValueTxt", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo fiImg = typeof(CellView).GetField("_cellImg", BindingFlags.NonPublic | BindingFlags.Instance);
        fiTxt.SetValue(cellView, tmp);
        fiImg.SetValue(cellView, img);
    }

    /// <summary>
    /// Очищает тестовую среду после каждого теста:
    /// - Удаляет созданные игровые объекты
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(cellViewRoot);
    }

    /// <summary>
    /// Проверяет корректность обновления значения и цвета ячейки:
    /// - Устанавливает тестовое значение
    /// - Проверяет отображение текста
    /// - Убеждается, что цвет установлен корректно
    /// </summary>
    [Test]
    public void TestUpdateValueAndColor()
    {
        cell.Value = 5;
        cellView.Init(cell);

        Assert.AreEqual("32", tmp.text);
        Assert.AreNotEqual(Color.clear, img.color);
    }

    /// <summary>
    /// Проверяет корректность обновления позиции ячейки:
    /// - Устанавливает тестовую позицию
    /// - Сравнивает фактическую позицию с ожидаемой
    /// - Учитывает погрешность при сравнении координат
    /// </summary>
    [Test]
    public void TestUpdatePosition()
    {
        cell.Position = new Vector2Int(2, 2);
        cellView.Init(cell);

        var rt = cellViewRoot.GetComponent<RectTransform>();
        Vector2 expected = Cell.GetPos(new Vector2Int(2, 2));
        Assert.That(rt.anchoredPosition.x, Is.EqualTo(expected.x).Within(0.001f));
        Assert.That(rt.anchoredPosition.y, Is.EqualTo(expected.y).Within(0.001f));
    }
}