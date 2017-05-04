using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour, IMenuButton {

    public void Select()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    public void Deselect()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
    }

    public void OnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
