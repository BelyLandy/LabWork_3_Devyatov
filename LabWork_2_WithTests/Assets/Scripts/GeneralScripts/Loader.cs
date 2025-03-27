using UnityEngine;
using static SceneLoader;

public class Loader : MonoBehaviour
{
    public void MenuScene()
    {
        Load(Scene.MenuScene);
    }
    
    public void GameScene()
    {
        Load(Scene.GameScene);
    }
}
