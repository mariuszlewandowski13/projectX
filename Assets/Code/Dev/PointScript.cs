using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PointScript : MonoBehaviour {

    public GameObject pointToInstantiate;
    public Transform board;
    public string levelName;
    public List<GameObject> points;
    public List<Vector3> positions;
    private LineRenderer lineRend;

    private void Start()
    {
        points = new List<GameObject>();
        positions = new List<Vector3>();
        lineRend = GetComponent<LineRenderer>();
    }

    void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if (hit.transform != null && hit.transform == board)
            {
               points.Add( Instantiate(pointToInstantiate, hit.point, new Quaternion()));
                positions.Add(hit.point);
                DrawLine();
            }
                
        }
        if (Input.GetKeyDown("s"))
        {
            SavePoints();
        }
    }

    private void SavePoints()
    {
        string [] str = new string[1] { ""};
        foreach (GameObject obj in points)
        {
            if (obj != null)
            {
                str[0] += ("new Vector3(" + obj.transform.position.x + "f, " + obj.transform.position.y + "f, " + obj.transform.position.z + "f), ");
                File.WriteAllLines(Application.dataPath + "/" + levelName + ".txt", str);
            }
        }
    }

    private void DrawLine()
    {
        lineRend.positionCount = positions.Count;
        lineRend.SetPositions(positions.ToArray());
    }
}
