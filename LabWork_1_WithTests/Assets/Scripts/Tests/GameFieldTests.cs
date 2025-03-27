using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameFieldTests
{
    private GameObject gameFieldGO;
    private GameField gameField;
    private GameObject cellPrefab;
    private GameObject cellParentGO;
    
    /// <summary>
    /// Подготавливает тестовую среду перед каждым тестом:
    /// - Создает игровое поле 2x2
    /// - Настраивает префаб клетки с размером 100 единиц
    /// - Инициализирует детерминированный Random
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        gameFieldGO = new GameObject("GameFieldGO");
        gameField = gameFieldGO.AddComponent<GameField>();
        
        SetPrivateField(gameField, "gridWidth", 2);
        SetPrivateField(gameField, "gridHeight", 2);
        
        cellPrefab = new GameObject("CellPrefab");
        var cellView = cellPrefab.AddComponent<CellView>();
        cellView.cellSize = 100f;
        cellPrefab.SetActive(false);
        
        cellParentGO = new GameObject("CellParent");
        Transform cellParent = cellParentGO.transform;
        
        SetPrivateField(gameField, "cellPrefab", cellPrefab);
        SetPrivateField(gameField, "cellParent", cellParent);
        
        UnityEngine.Random.InitState(0);
    }

    /// <summary>
    /// Очищает тестовую среду после каждого теста,
    /// удаляя все созданные игровые объекты
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(gameFieldGO);
        GameObject.DestroyImmediate(cellPrefab);
        GameObject.DestroyImmediate(cellParentGO);
    }

    /// <summary>
    /// Проверяет, что после вызова Start игровое поле инициализировано,
    /// и создана ровно одна клетка с ненулевым значением.
    /// </summary>
    [Test]
    public void Start_InitializesGridAndCreatesOneCell()
    {
        // Act: Вызываем Start. Метод Start вызывает InitializeGrid и CreateCell.
        gameField.SendMessage("Start");

        // Получаем приватное поле _cells через рефлексию.
        var cells = GetPrivateField<List<Cell>>(gameField, "_cells");
        cells.Should().NotBeNull("Поле _cells должно быть инициализировано");
        cells.Count.Should().Be(4, "При размерах 2х2 должно быть 4 ячейки");

        // Среди ячеек ровно одна должна быть заполнена (значение != 0).
        int filledCount = 0;
        foreach (var cell in cells)
        {
            if (cell.Value != 0)
                filledCount++;
        }
        filledCount.Should().Be(1, "После вызова CreateCell ровно одна ячейка должна быть заполнена");

        // Проверяем, что в контейнере появилось созданное представление клетки.
        cellParentGO.transform.childCount.Should().Be(1, "Должен быть создан ровно один экземпляр префаба");
    }

    /// <summary>
    /// Проверяет, что метод CreateCell устанавливает значение клетки в 1 или 2,
    /// а также создает объект с нужной позицией с учётом cellSize.
    /// </summary>
    [Test]
    public void CreateCell_SetsCellValueAndInstantiatesPrefab()
    {
        gameField.SendMessage("Start");
        
        var cells = GetPrivateField<List<Cell>>(gameField, "_cells");
        
        Cell createdCell = cells.Find(c => c.Value != 0);
        createdCell.Should().NotBeNull("Должна быть создана ячейка с ненулевым значением");
        
        (createdCell.Value == 1 || createdCell.Value == 2)
            .Should().BeTrue("Значение созданной ячейки должно быть 1 (90% вероятности) или 2 (10%)");
        
        Transform instantiated = cellParentGO.transform.GetChild(0);
        var cellViewComp = instantiated.GetComponent<CellView>();
        float cellSize = cellViewComp.cellSize;
        
        Vector3 expectedPos = new Vector3(
            createdCell.Position.x * cellSize,
            createdCell.Position.y * cellSize, 0);
        instantiated.localPosition.Should().Be(expectedPos,
            "Позиция созданного объекта должна соответствовать позиции ячейки, умноженной на cellSize");
    }

    /// <summary>
    /// Проверяет ветку, когда свободных позиций для создания клетки нет.
    /// Для этого заполняем все ячейки ненулевыми значениями и вызываем CreateCell.
    /// </summary>
    [Test]
    public void CreateCell_NoEmptyPosition_LogsMessageAndDoesNotInstantiate()
    {
        gameField.SendMessage("Start");
        
        var cells = GetPrivateField<List<Cell>>(gameField, "_cells");
        foreach (var cell in cells)
        {
            cell.Value = 1;
        }
        
        foreach (Transform child in cellParentGO.transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        cellParentGO.transform.childCount.Should().Be(
            0,
            "Контейнер должен быть пуст после удаления всех детей");
        
        LogAssert.Expect(
            UnityEngine.LogType.Log,
            "Нет свободных позиций для создания клетки.");
        
        MethodInfo createCellMethod = typeof(GameField).GetMethod(
            "CreateCell",
            BindingFlags.NonPublic | BindingFlags.Instance);
        createCellMethod.Invoke(gameField, null);
        
        cellParentGO.transform.childCount.Should().Be(
            0,
            "При отсутствии свободных позиций новый объект не должен быть создан");
    }

    #region Вспомогательные методы рефлексии

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
        {
            throw new Exception($"Поле {fieldName} не найдено в типе {obj.GetType().Name}");
        }
        return (T)field.GetValue(obj);
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName, 
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
        {
            throw new Exception($"Поле {fieldName} не найдено в типе {obj.GetType().Name}");
        }
        field.SetValue(obj, value);
    }

    #endregion
}
