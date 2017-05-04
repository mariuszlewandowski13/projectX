using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScript : MonoBehaviour, IMenuButton {

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
        AnimationsManager.ClearAnimations();
        SceneManager.LoadScene("Level" + (LevelManager.actualLevel + 1).ToString());
    }
}
