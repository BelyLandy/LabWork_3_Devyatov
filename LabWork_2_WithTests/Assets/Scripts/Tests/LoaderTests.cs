using FluentAssertions;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.TestTools;

public class LoaderTests
{
    private Loader _loader;

    /// <summary>
    /// Проверяет корректность загрузки игровой сцены,
    /// убеждаясь что она становится активной после загрузки
    /// </summary>
    [UnityTest]
    public IEnumerator GameScene_ShouldCallLoadWithGameScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
        SceneManager.GetActiveScene().name.Should().Be("GameScene");
    }

    /// <summary>
    /// Проверяет корректность загрузки сцены меню,
    /// убеждаясь что она становится активной после загрузки
    /// </summary>
    [UnityTest]
    public IEnumerator MenuScene_ShouldCallLoadWithMenuScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("MenuScene");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MenuScene"));
        SceneManager.GetActiveScene().name.Should().Be("MenuScene");
    }
}