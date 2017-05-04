using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameButtonScript : MonoBehaviour, IMenuButton {

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
        Application.Quit();
    }
}
