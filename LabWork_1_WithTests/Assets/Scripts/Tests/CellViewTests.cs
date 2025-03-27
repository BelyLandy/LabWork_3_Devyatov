using NUnit.Framework;
using UnityEngine;
using TMPro;
using FluentAssertions;

public class CellViewTests
{
    private GameObject gameObject;
    private CellView cellView;
    private Cell cell;
    private TextMeshProUGUI tmp;

    /// <summary>
    /// Подготавливает тестовую среду с клеткой и текстовым компонентом
    /// перед каждым тестом
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("CellViewGO");
        cellView = gameObject.AddComponent<CellView>();

        GameObject tmpObject = new GameObject("TMP");
        tmp = tmpObject.AddComponent<TextMeshProUGUI>();
        cellView.valueText = tmp;

        cellView.cellSize = 100f;
        cell = new Cell(new Vector2Int(2, 3), 3);
    }

    /// <summary>
    /// Очищает тестовую среду, удаляя созданные объекты
    /// после каждого теста
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
        if (tmp != null)
        {
            Object.DestroyImmediate(tmp.gameObject);
        }
    }

    /// <summary>
    /// Проверяет корректность инициализации значений и позиции клетки
    /// при первом создании
    /// </summary>
    [Test]
    public void Init_Should_UpdateValueAndPosition()
    {
        cellView.Init(cell);

        tmp.text.Should().Be(Mathf.Pow(2, cell.Value).ToString());
        gameObject.transform.position.Should().Be(new Vector3(
            cell.Position.x * cellView.cellSize,
            cell.Position.y * cellView.cellSize,
            0));
    }

    /// <summary>
    /// Проверяет автоматическое обновление текста
    /// при изменении значения клетки
    /// </summary>
    [Test]
    public void CellValueChange_Should_UpdateText()
    {
        cellView.Init(cell);
        cell.Value = 4;

        tmp.text.Should().Be(Mathf.Pow(2, 4).ToString());
    }

    /// <summary>
    /// Проверяет автоматическое перемещение объекта
    /// при изменении позиции клетки
    /// </summary>
    [Test]
    public void CellPositionChange_Should_UpdateTransformPosition()
    {
        cellView.Init(cell);
        Vector2Int newPosition = new Vector2Int(5, 6);
        cell.Position = newPosition;

        gameObject.transform.position.Should().Be(new Vector3(
            newPosition.x * cellView.cellSize,
            newPosition.y * cellView.cellSize,
            0));
    }

    /// <summary>
    /// Проверяет отписку от событий клетки
    /// при деактивации компонента
    /// </summary>
    [Test]
    public void OnDisable_Should_UnsubscribeFromCellEvents()
    {
        cellView.Init(cell);
        string currentText = tmp.text;
        Vector3 currentPosition = gameObject.transform.position;

        cellView.SendMessage("OnDisable");
        cell.Value = 10;
        cell.Position = new Vector2Int(10, 10);

        tmp.text.Should().Be(currentText);
        gameObject.transform.position.Should().Be(currentPosition);
    }

    /// <summary>
    /// Проверяет устойчивость к отсутствию текстового компонента,
    /// убеждаясь что базовые функции продолжают работать
    /// </summary>
    [Test]
    public void Init_ShouldNotThrow_WhenValueTextIsNull()
    {
        cellView.valueText = null;

        System.Action act = () => cellView.Init(cell);
        act.Should().NotThrow();
        gameObject.transform.position.Should().Be(new Vector3(
            cell.Position.x * cellView.cellSize,
            cell.Position.y * cellView.cellSize,
            0));
    }
}