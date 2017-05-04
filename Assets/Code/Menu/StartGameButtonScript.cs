using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButtonScript : MonoBehaviour, IMenuButton {

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
        LevelManager.Clear();
        AnimationsManager.ClearAnimations();
        SceneManager.LoadScene("Level1");
    }
}
