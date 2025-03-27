using FluentAssertions;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.TestTools;

public class SceneLoaderTests
{
    /// <summary>
    /// Проверяет корректность загрузки сцены главного меню
    /// и убеждается, что она становится активной после завершения загрузки
    /// </summary>
    [UnityTest]
    public IEnumerator Load_MenuScene_ShouldLoadCorrectScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("MenuScene");
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MenuScene"));
        SceneManager.GetActiveScene().name.Should().Be("MenuScene");
    }

    /// <summary>
    /// Проверяет корректность загрузки игровой сцены
    /// и убеждается, что она становится активной после завершения загрузки
    /// </summary>
    [UnityTest]
    public IEnumerator Load_GameScene_ShouldLoadCorrectScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        SceneManager.GetActiveScene().name.Should().Be("GameScene");
    }
}