using UnityEngine;

public class BListObject  {

    public GameObject value;
    public BListObject leftNeighbour;
    public BListObject rightNeighbour;
    public bool mustBeCenterPoint = false;// helper for finding sequences
}
