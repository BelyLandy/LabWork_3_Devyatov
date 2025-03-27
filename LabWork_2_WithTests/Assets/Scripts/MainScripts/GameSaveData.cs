using System;

/// <summary>
/// Класс сохраняемых данных.
/// </summary>
[Serializable]
public class GameSaveData
{
    public int[,] CellValues;
    public int currScore;
    public int hightScore;
}