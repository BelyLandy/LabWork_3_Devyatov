using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        MenuScene,
        GameScene
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}

