using NUnit.Framework;
using UnityEngine;
using FluentAssertions;

[TestFixture]
public class PlayerInputDataTests
{
    /// <summary>
    /// Проверяет, что конструктор правильно устанавливает вектор движения
    /// </summary>
    [Test]
    public void Constructor_SetsMovementInputCorrectly()
    {
        Vector2 expectedMovement = new Vector2(1, 2);
        bool fullscreen = true;

        var inputData = new PlayerInputData(
            expectedMovement,
            fullscreen);

        inputData.MovementInput.Should().Be(
            expectedMovement,
            "MovementInput должен совпадать с переданным значением");
    }

    /// <summary>
    /// Убеждаемся, что флаг полноэкранного режима корректно устанавливается в true
    /// </summary>
    [Test]
    public void Constructor_SetsIsFullscreenedOrWindowedCorrectly_WhenTrue()
    {
        Vector2 movement = new Vector2(0, 0);
        bool expectedFullscreen = true;

        var inputData = new PlayerInputData(
            movement,
            expectedFullscreen);

        inputData.IsFullscreenedOrWindowed.Should().Be(
            expectedFullscreen,
            "Флаг IsFullscreenedOrWindowed должен быть true, если передан true");
    }

    /// <summary>
    /// Проверяем, что если не передавать флаг полноэкранного режима, он по умолчанию будет false
    /// </summary>
    [Test]
    public void Constructor_SetsIsFullscreenedOrWindowedDefaultFalse_WhenNotProvided()
    {
        Vector2 movement = new Vector2(3, 4);
        bool expectedFullscreen = false;

        var inputData = new PlayerInputData(movement);

        inputData.IsFullscreenedOrWindowed.Should().Be(
            expectedFullscreen,
            "При отсутствии параметра флаг IsFullscreenedOrWindowed должен быть false по умолчанию");
    }
}