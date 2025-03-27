using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;

[TestFixture]
public class SaveLoadManagerTests
{
    private string testSavePath;

    /// <summary>
    /// Подготавливает тестовую среду перед каждым тестом,
    /// устанавливая путь для временного файла сохранения
    /// </summary>
    [SetUp]
    public void Setup()
    {
        testSavePath = Path.Combine(Application.persistentDataPath, "test_save.dat");
    }

    /// <summary>
    /// Очищает тестовую среду после каждого теста,
    /// удаляя временный файл сохранения если он существует
    /// </summary>
    [TearDown]
    public void Cleanup()
    {
        if (File.Exists(testSavePath))
        {
            File.Delete(testSavePath);
        }
    }

    /// <summary>
    /// Проверяет обработку случая, когда файл сохранения не существует,
    /// ожидая что метод вернет null
    /// </summary>
    [Test]
    public void TestLoadGame_FileNotExist_ReturnsNull()
    {
        var result = SaveLoadManager.LoadGame(testSavePath, 4);
        Assert.IsNull(result);
    }

    /// <summary>
    /// Проверяет обработку поврежденного файла сохранения,
    /// ожидая сообщение об ошибке и возврат null
    /// </summary>
    [Test]
    public void TestLoadGame_CorruptedFile_ThrowsException()
    {
        string corruptedFilePath = "test_corrupted.dat";
        File.WriteAllText(corruptedFilePath, "corrupt data");

        LogAssert.Expect(LogType.Error,
            "Ошибка! Загрузка игры прервана! End of Stream encountered before parsing was completed.");

        var result = SaveLoadManager.LoadGame(corruptedFilePath, 4);
        Assert.IsNull(result);
    }

    /// <summary>
    /// Проверяет обработку несоответствия размеров сетки,
    /// ожидая создание нового массива с правильными размерами
    /// </summary>
    [Test]
    public void TestLoadGame_GridSizeMismatch_CreatesNewArray()
    {
        var data = new GameSaveData { CellValues = new int[3, 3] };
        SaveLoadManager.SaveGame(testSavePath, data, 3, new List<Cell>(), 0);

        var loaded = SaveLoadManager.LoadGame(testSavePath, 4);
        Assert.AreEqual(4, loaded.CellValues.GetLength(0));
        Assert.AreEqual(4, loaded.CellValues.GetLength(1));
    }

    /// <summary>
    /// Проверяет обработку заблокированного файла,
    /// ожидая сообщение об ошибке при попытке сохранения
    /// </summary>
    [Test]
    public void TestSaveGame_FileLocked_LogsError()
    {
        string savePath = "test_save.dat";
        var data = new GameSaveData();
        var cellGameObject = new GameObject();
        var cell = cellGameObject.AddComponent<Cell>();
        cell.Position = new Vector2Int(0, 0);
        cell.Value = 2;

        var cells = new List<Cell> { cell };
        string fullSavePath = Path.GetFullPath(savePath);

        LogAssert.NoUnexpectedReceived();
        LogAssert.Expect(LogType.Error,
            $"Ошибка! Неправильное сохранение игры: Sharing violation on path {fullSavePath}");

        using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            SaveLoadManager.SaveGame(savePath, data, 4, cells, 10);
        }

        LogAssert.NoUnexpectedReceived();
        UnityEngine.Object.DestroyImmediate(cellGameObject);
    }

    /// <summary>
    /// Проверяет успешное сохранение и загрузку данных,
    /// убеждаясь что файл создается и данные загружаются корректно
    /// </summary>
    [Test]
    public void TestSaveGame_SuccessfullySaves()
    {
        var data = new GameSaveData();
        var cells = new List<Cell> { new Cell { Position = new Vector2Int(0, 0), Value = 2 } };

        SaveLoadManager.SaveGame(testSavePath, data, 4, cells, 50);

        Assert.IsTrue(File.Exists(testSavePath));
        var loadedData = SaveLoadManager.LoadGame(testSavePath, 4);

        Assert.IsNotNull(loadedData);
        Assert.AreEqual(50, loadedData.currScore);
        Assert.AreEqual(2, loadedData.CellValues[0, 0]);
    }
}