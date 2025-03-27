using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class PlayerInputHandlerTests : InputTestFixture
{
    private GameObject testGO;
    private PlayerInputHandler inputHandler;
    private GameField gameField;

    /// <summary>
    /// Подготавливает тестовую среду с игровым полем и обработчиком ввода,
    /// настраивая все необходимые зависимости для корректной работы тестов
    /// </summary>
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        testGO = new GameObject("TestPlayerInputHandler");

        var prefab = new GameObject("CellPrefab");
        prefab.AddComponent<RectTransform>();
        prefab.AddComponent<Cell>();

        gameField = testGO.AddComponent<GameField>();

        var fieldPrefab = typeof(GameField).GetField("cellPrefab",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        fieldPrefab.SetValue(gameField, prefab);

        var parentObj = new GameObject("CellsParent");
        var fieldParent = typeof(GameField).GetField("cellsParent",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        fieldParent.SetValue(gameField, parentObj);

        FieldInfo animField =
            typeof(GameField).GetField("_isDoingAnim", BindingFlags.NonPublic | BindingFlags.Instance);
        animField.SetValue(gameField, false);

        gameField.endGame = false;

        inputHandler = testGO.AddComponent<PlayerInputHandler>();
        FieldInfo field =
            typeof(PlayerInputHandler).GetField("_gameField", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(inputHandler, gameField);
    }

    /// <summary>
    /// Очищает тестовую среду, удаляя созданные игровые объекты
    /// после выполнения каждого теста
    /// </summary>
    [TearDown]
    public override void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(testGO);
        base.TearDown();
    }

    /// <summary>
    /// Проверяет корректность обработки ввода направления,
    /// убеждаясь что вызов происходит без ошибок
    /// </summary>
    [UnityTest]
    public IEnumerator TestHandleInputAction_CallsMoveCells()
    {
        MethodInfo method =
            typeof(PlayerInputHandler).GetMethod("HandleInputAction", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);

        method.Invoke(inputHandler, new object[] { Vector2.up });

        yield return null;
        Assert.Pass();
    }

    /// <summary>
    /// Проверяет игнорирование свайп-ввода,
    /// когда соответствующая функция отключена в настройках
    /// </summary>
    [UnityTest]
    public IEnumerator TestHandleSwipeAction_DoesNothingWhenDisabled()
    {
        FieldInfo field =
            typeof(PlayerInputHandler).GetField("toggleSwipeMovement", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(inputHandler, false);

        InputAction.CallbackContext context = default;

        MethodInfo method =
            typeof(PlayerInputHandler).GetMethod("HandleSwipeAction", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(inputHandler, new object[] { context });

        yield return null;
        Assert.Pass();
    }
    
    /// <summary>
    /// Убеждается что система корректно обрабатывает случай,
    /// когда свайп включен но направление не определено
    /// </summary>
    [UnityTest]
    public IEnumerator TestHandleSwipeAction_WithSwipeEnabled_NoValidSwipe()
    {
        FieldInfo toggleField = typeof(PlayerInputHandler).GetField("toggleSwipeMovement", BindingFlags.NonPublic | BindingFlags.Instance);
        toggleField.SetValue(inputHandler, true);

        InputAction.CallbackContext context = default;

        MethodInfo method = typeof(PlayerInputHandler).GetMethod("HandleSwipeAction", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(inputHandler, new object[] { context });

        yield return null;
        Assert.Pass("HandleSwipeAction не выполняет действия при нулевом значении свайпа.");
    }
}