using UnityEngine;

public class WebLink : MonoBehaviour
{
    /*
     * Класс, необходимый для кнопки перехода по ссылке.
     * Хранит URL и осуществляет переход к данному веб-ресурсу
     */
    public string url = "";
    
    public void OpenURL()
    {
        Application.OpenURL(url);
        Debug.Log("is this working?");
    }
}
