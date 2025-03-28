using NUnit.Framework;
using UnityEngine;
using FluentAssertions;

public class CellTests
{
    /// <summary>
    /// Проверяет, что ячейка правильно инициализирует позицию и значение при создании
    /// </summary>
    [Test]
    public void Constructor_InitializesPositionAndValue()
    {
        Vector2Int initialPosition = new Vector2Int(1, 2);
        int initialValue = 4;

        Cell cell = new Cell(initialPosition, initialValue);

        cell.Position.Should().Be(initialPosition, "Позиция не инициализирована корректно");
        cell.Value.Should().Be(initialValue, "Значение не инициализировано корректно");
    }

    /// <summary>
    /// Убеждаемся, что при изменении значения ячейки вызывается событие OnValueChanged
    /// </summary>
    [Test]
    public void ValueProperty_InvokesOnValueChanged_WhenValueChanges()
    {
        Cell cell = new Cell(new Vector2Int(0, 0), 2);
        bool eventInvoked = false;
        int eventValue = 0;
        cell.OnValueChanged += (newValue) =>
        {
            eventInvoked = true;
            eventValue = newValue;
        };

        cell.Value = 8;

        eventInvoked.Should().BeTrue("Событие OnValueChanged не было вызвано при изменении значения");
        eventValue.Should().Be(8, "Переданное значение в событии неверное");
    }

    /// <summary>
    /// Проверяет, что событие не вызывается, если новое значение совпадает с текущим
    /// </summary>
    [Test]
    public void ValueProperty_DoesNotInvokeOnValueChanged_WhenValueUnchanged()
    {
        Cell cell = new Cell(new Vector2Int(0, 0), 2);
        bool eventInvoked = false;
        cell.OnValueChanged += (newValue) => eventInvoked = true;

        cell.Value = 2;

        eventInvoked.Should().BeFalse("Событие OnValueChanged было вызвано, несмотря на отсутствие изменения значения");
    }

    /// <summary>
    /// Проверяет корректность работы события изменения позиции ячейки
    /// </summary>
    [Test]
    public void PositionProperty_InvokesOnPositionChanged_WhenPositionChanges()
    {
        Cell cell = new Cell(new Vector2Int(0, 0), 2);
        bool eventInvoked = false;
        Vector2Int newPositionFromEvent = Vector2Int.zero;
        cell.OnPositionChanged += (newPos) =>
        {
            eventInvoked = true;
            newPositionFromEvent = newPos;
        };

        Vector2Int newPosition = new Vector2Int(3, 4);
        cell.Position = newPosition;

        eventInvoked.Should().BeTrue("Событие OnPositionChanged не было вызвано при изменении позиции");
        newPositionFromEvent.Should().Be(newPosition, "Переданная позиция в событии неверная");
    }

    /// <summary>
    /// Убеждаемся, что событие позиции не вызывается без реального изменения позиции
    /// </summary>
    [Test]
    public void PositionProperty_DoesNotInvokeOnPositionChanged_WhenPositionUnchanged()
    {
        Vector2Int initialPosition = new Vector2Int(5, 5);
        Cell cell = new Cell(initialPosition, 2);
        bool eventInvoked = false;
        cell.OnPositionChanged += (newPos) => eventInvoked = true;

        cell.Position = initialPosition;

        eventInvoked.Should().BeFalse("Событие OnPositionChanged было вызвано, несмотря на отсутствие изменения позиции");
    }

    /// <summary>
    /// Тестирует корректность работы флага слияния ячейки
    /// </summary>
    [Test]
    public void MergedProperty_SetAndGet_WorkCorrectly()
    {
        Cell cell = new Cell(new Vector2Int(0, 0), 2);

        cell.Merged = true;
        cell.Merged.Should().BeTrue("Свойство Merged не вернуло ожидаемое значение после установки true");

        cell.Merged = false;
        cell.Merged.Should().BeFalse("Свойство Merged не вернуло ожидаемое значение после установки false");
    }
}