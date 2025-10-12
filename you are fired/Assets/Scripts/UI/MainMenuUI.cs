using UnityEngine;
public class MainMenuUI : MonoBehaviour
{
    public void OnStartGame() => Services.Game.StartFromLevel(0);
    public void OnQuit() => Application.Quit();
}

