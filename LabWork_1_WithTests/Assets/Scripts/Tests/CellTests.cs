using NUnit.Framework;
using UnityEngine;
using FluentAssertions;

public class CellTests
{
    [Test]
    public void Constructor_InitializesPositionAndValue()
    {
        // Arrange
        Vector2Int initialPosition = new Vector2Int(1, 2);
        int initialValue = 4;

        // Act
        Cell cell = new Cell(initialPosition, initialValue);

        // Assert
        cell.Position.Should().Be(initialPosition, "Позиция не инициализирована корректно");
        cell.Value.Should().Be(initialValue, "Значение не инициализировано корректно");
    }

    [Test]
    public void ValueProperty_InvokesOnValueChanged_WhenValueChanges()
    {
        // Arrange
        Cell cell = new Cell(new Vector2Int(0, 0), 2);
        bool eventInvoked = false;
        int eventValue = 0;
        cell.OnValueChanged += (newValue) =>
        {
            eventInvoked = true;
            eventValue = newValue;
        };

        // Act
        cell.Value = 8;

        // Assert
        eventInvoked.Should().BeTrue("Событие OnValueChanged не было вызвано при изменении значения");
        eventValue.Should().Be(8, "Переданное значение в событии неверное");
    }

    [Test]
    public void ValueProperty_DoesNotInvokeOnValueChanged_WhenValueUnchanged()
    {
        // Arrange
        Cell cell = new Cell(new Vector2Int(0, 0), 2);
        bool eventInvoked = false;
        cell.OnValueChanged += (newValue) => eventInvoked = true;

        // Act
        cell.Value = 2; // то же самое значение

        // Assert
        eventInvoked.Should().BeFalse("Событие OnValueChanged было вызвано, несмотря на отсутствие изменения значения");
    }

    [Test]
    public void PositionProperty_InvokesOnPositionChanged_WhenPositionChanges()
    {
        // Arrange
        Cell cell = new Cell(new Vector2Int(0, 0), 2);
        bool eventInvoked = false;
        Vector2Int newPositionFromEvent = Vector2Int.zero;
        cell.OnPositionChanged += (newPos) =>
        {
            eventInvoked = true;
            newPositionFromEvent = newPos;
        };

        // Act
        Vector2Int newPosition = new Vector2Int(3, 4);
        cell.Position = newPosition;

        // Assert
        eventInvoked.Should().BeTrue("Событие OnPositionChanged не было вызвано при изменении позиции");
        newPositionFromEvent.Should().Be(newPosition, "Переданная позиция в событии неверная");
    }

    [Test]
    public void PositionProperty_DoesNotInvokeOnPositionChanged_WhenPositionUnchanged()
    {
        // Arrange
        Vector2Int initialPosition = new Vector2Int(5, 5);
        Cell cell = new Cell(initialPosition, 2);
        bool eventInvoked = false;
        cell.OnPositionChanged += (newPos) => eventInvoked = true;

        // Act
        cell.Position = initialPosition; // то же самое значение

        // Assert
        eventInvoked.Should().BeFalse("Событие OnPositionChanged было вызвано, несмотря на отсутствие изменения позиции");
    }

    [Test]
    public void MergedProperty_SetAndGet_WorkCorrectly()
    {
        // Arrange
        Cell cell = new Cell(new Vector2Int(0, 0), 2);

        // Act & Assert
        cell.Merged = true;
        cell.Merged.Should().BeTrue("Свойство Merged не вернуло ожидаемое значение после установки true");

        cell.Merged = false;
        cell.Merged.Should().BeFalse("Свойство Merged не вернуло ожидаемое значение после установки false");
    }
}
