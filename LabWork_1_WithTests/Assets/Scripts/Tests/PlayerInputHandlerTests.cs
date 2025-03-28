using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

public class PlayerInputHandlerTests
{
    private GameObject _gameObject;
    private PlayerInputHandler _inputHandler;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _inputHandler = _gameObject.AddComponent<PlayerInputHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_gameObject);
    }

    /// <summary>
    /// Проверяет, что публичный метод TriggerInputEvent корректно вызывает событие ввода
    /// </summary>
    [Test]
    public void TriggerInputEvent_PublicMethod_ShouldInvokeOnInputChangedWithCorrectData()
    {
        PlayerInputData? receivedData = null;
        _inputHandler.OnInputChanged += data => receivedData = data;

        var expectedMovement = new Vector2(1f, 0f);
        var expectedFullscreen = true;
        var testData = new PlayerInputData(expectedMovement, expectedFullscreen);

        _inputHandler.TriggerInputEvent(testData);

        receivedData.Should().NotBeNull("событие должно быть вызвано");
        receivedData.Value.MovementInput.Should()
            .Be(expectedMovement, "движение должно совпадать с ожидаемым значением");
        receivedData.Value.IsFullscreenedOrWindowed.Should()
            .Be(expectedFullscreen, "флаг fullscreen должен быть равен true");
    }

    /// <summary>
    /// Проверяет работу приватного метода переключения полноэкранного режима
    /// </summary>
    [Test]
    public void OnFullscreenOrWindowed_PrivateMethod_ShouldTriggerEventWithFullscreenFlagTrue()
    {
        PlayerInputData? receivedData = null;
        _inputHandler.OnInputChanged += data => receivedData = data;

        var method =
            typeof(PlayerInputHandler).GetMethod("OnFullscreenOrWindowed",
                BindingFlags.Instance | BindingFlags.NonPublic);
        method.Should().NotBeNull("метод OnFullscreenOrWindowed должен быть доступен через рефлексию");

        method.Invoke(_inputHandler, null);

        receivedData.Should().NotBeNull("событие должно быть вызвано при переключении режима fullscreen/windowed");
        receivedData.Value.IsFullscreenedOrWindowed.Should()
            .BeTrue("флаг fullscreen должен быть установлен в true при вызове метода");
        receivedData.Value.MovementInput.Should().Be(Vector2.zero, "значение движения по умолчанию равно Vector2.zero");
    }
}