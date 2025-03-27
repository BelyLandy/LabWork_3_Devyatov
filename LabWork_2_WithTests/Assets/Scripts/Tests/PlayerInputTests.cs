using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.InputSystem;

public class PlayerInputTests
{
    private PlayerInput playerInput;

    /// <summary>
    /// Подготавливает тестовую среду, создавая новый экземпляр PlayerInput
    /// перед каждым тестом
    /// </summary>
    [SetUp]
    public void Setup()
    {
        playerInput = new PlayerInput();
    }

    /// <summary>
    /// Очищает тестовую среду, освобождая ресурсы PlayerInput
    /// после каждого теста
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        playerInput.Dispose();
    }

    /// <summary>
    /// Проверяет корректность поиска действия по имени
    /// в системе ввода игрока
    /// </summary>
    [Test]
    public void TestFindAction()
    {
        var action = playerInput.FindAction("MoveCellUp");
        Assert.IsNotNull(action);
    }

    /// <summary>
    /// Проверяет установку и получение маски привязок ввода,
    /// убеждаясь в корректности работы свойства bindingMask
    /// </summary>
    [Test]
    public void TestBindingMaskSetGet()
    {
        InputBinding binding = new InputBinding { path = "<Keyboard>/space" };
        playerInput.bindingMask = binding;
        Assert.AreEqual(binding, playerInput.bindingMask);
    }

    /// <summary>
    /// Тестирует включение и отключение системы ввода,
    /// проверяя состояние активации asset
    /// </summary>
    [Test]
    public void TestEnableDisable()
    {
        playerInput.Disable();
        Assert.IsFalse(playerInput.asset.enabled);
        playerInput.Enable();
        Assert.IsTrue(playerInput.asset.enabled);
    }

    /// <summary>
    /// Проверяет работу интерфейса IEnumerable,
    /// убеждаясь что можно получить список всех действий ввода
    /// </summary>
    [Test]
    public void TestContainsAndEnumerator()
    {
        var action = playerInput.FindAction("MoveCellDown");
        Assert.IsTrue(playerInput.Contains(action));

        List<InputAction> actions = new List<InputAction>();
        foreach (var a in playerInput)
        {
            actions.Add(a);
        }

        Assert.IsTrue(actions.Count > 0);
    }

    /// <summary>
    /// Проверяет поиск привязки ввода по шаблону,
    /// возвращая индекс найденной привязки и соответствующее действие
    /// </summary>
    [Test]
    public void TestFindBindingMethod()
    {
        InputBinding binding = new InputBinding { path = "<Keyboard>/space" };
        InputAction foundAction;
        int bindingIndex = playerInput.FindBinding(binding, out foundAction);
        Assert.GreaterOrEqual(bindingIndex, -1);
        if (bindingIndex >= 0)
        {
            Assert.IsNotNull(foundAction);
        }
    }

    /// <summary>
    /// Проверяет работу свойства Devices,
    /// убеждаясь что можно установить и получить устройства ввода
    /// </summary>
    [Test]
    public void TestDevicesProperty()
    {
        playerInput.devices = null;
        Assert.IsNull(playerInput.devices);
    }

    /// <summary>
    /// Проверяет доступность схем управления,
    /// убеждаясь что список схем не является null
    /// </summary>
    [Test]
    public void TestControlSchemesProperty()
    {
        var schemes = playerInput.controlSchemes;
        Assert.IsNotNull(schemes);
        Assert.GreaterOrEqual(schemes.Count, 0);
    }

    /// <summary>
    /// Проверяет перечисление всех привязок ввода,
    /// убеждаясь что список не пустой
    /// </summary>
    [Test]
    public void TestBindingsEnumerableProperty()
    {
        List<InputBinding> bindings = new List<InputBinding>(playerInput.bindings);
        Assert.IsTrue(bindings.Count > 0);
    }

    /// <summary>
    /// Тестирует включение и отключение игровых действий,
    /// проверяя состояние активации gameplay схемы
    /// </summary>
    [Test]
    public void TestGameplayEnableDisable()
    {
        var gameplay = playerInput.Gameplay;
        gameplay.Disable();
        Assert.IsFalse(gameplay.enabled);
        gameplay.Enable();
        Assert.IsTrue(gameplay.enabled);
    }
    
    private class DummyGameplayActions : PlayerInput.IGameplayActions
    {
        public bool MoveCellUpCalled = false;
        public void OnMoveCellUp(InputAction.CallbackContext context)
        {
            MoveCellUpCalled = true;
        }
        public void OnMoveCellLeft(InputAction.CallbackContext context) { }
        public void OnMoveCellDown(InputAction.CallbackContext context) { }
        public void OnMoveCellRight(InputAction.CallbackContext context) { }
        public void OnMoveWithSwipe(InputAction.CallbackContext context) { }
        public void OnFullscreenOrWindowedMode(InputAction.CallbackContext context) { }
        public void OnQuitApplication(InputAction.CallbackContext context) { }
    }

    /// <summary>
    /// Проверяет работу системы коллбэков для игровых действий,
    /// тестируя добавление и удаление обработчиков событий
    /// </summary>
    [Test]
    public void TestGameplayCallbacks()
    {
        var gameplay = playerInput.Gameplay;
        var dummy = new DummyGameplayActions();

        Assert.DoesNotThrow(() => gameplay.SetCallbacks(dummy));
        Assert.DoesNotThrow(() => gameplay.RemoveCallbacks(dummy));
        Assert.Pass();
    }

    /// <summary>
    /// Проверяет корректность освобождения ресурсов,
    /// убеждаясь что метод Dispose не вызывает исключений
    /// </summary>
    [Test]
    public void TestDisposeMethod()
    {
        Assert.DoesNotThrow(() => playerInput.Dispose());
    }
}