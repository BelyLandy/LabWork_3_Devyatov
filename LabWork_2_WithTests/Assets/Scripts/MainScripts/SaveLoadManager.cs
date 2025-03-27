using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadManager
{
    /// <summary>
    /// Загружает сохранённые данные игры.
    /// </summary>
    /// <param name="savePath">Путь к файлу сохранения.</param>
    /// <param name="gridSize">Размер сетки (для создания массива значений ячеек).</param>
    /// <returns>Возвращает объект GameSaveData или null, если файл отсутствует или произошла ошибка.</returns>
    public static GameSaveData LoadGame(string savePath, int gridSize)
    {
        if (File.Exists(savePath))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using FileStream stream = new FileStream(savePath, FileMode.Open);
                GameSaveData data = (GameSaveData)formatter.Deserialize(stream);
                // Если размер сетки изменился, создаём новый массив (опционально)
                if (data.CellValues.GetLength(0) != gridSize || data.CellValues.GetLength(1) != gridSize)
                {
                    data.CellValues = new int[gridSize, gridSize];
                }
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка! Загрузка игры прервана! " + e.Message);
            }
        }
        return null;
    }
    
    /// <summary>
    /// Сохраняет данные игры.
    /// </summary>
    /// <param name="savePath">Путь к файлу сохранения.</param>
    /// <param name="data">Данные игры.</param>
    /// <param name="gridSize">Размер сетки.</param>
    /// <param name="cells">Список ячеек на поле.</param>
    /// <param name="currentScore">Текущий счёт.</param>
    public static void SaveGame(string savePath, GameSaveData data, int gridSize, List<Cell> cells, int currentScore)
    {
        data.currScore = currentScore;
        data.CellValues = new int[gridSize, gridSize];
        foreach (Cell cell in cells)
        {
            data.CellValues[cell.Position.x, cell.Position.y] = cell.Value;
        }
        
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream stream = new FileStream(savePath, FileMode.Create);
            formatter.Serialize(stream, data);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка! Неправильное сохранение игры: " + e.Message);
        }
    }
}
