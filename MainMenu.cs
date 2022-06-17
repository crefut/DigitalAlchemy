using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /*
     * Класс, отвечающий за старт игры и выход из нее.
     */
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
