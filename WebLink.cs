using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebLink : MonoBehaviour
{
    public string url = "";
    
    public void OpenURL()
    {
        Application.OpenURL(url);
        Debug.Log("is this working?");
    }
}
