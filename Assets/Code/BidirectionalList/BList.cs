using UnityEngine;

public class BList  {

    private BListObject first;
    private BListObject actual;
    private BListObject last;

    public GameObject Last
    {
       get {
            if (last != null)
            {
                return last.value;
            }
            else return null;
        }
    }

    public GameObject First
    {
        get
        {
            if (first != null)
            {
                return first.value;
            }
            else return null;
        }
    }

    public int count;

    public BList()
    {
        count = 0;
    }

    public GameObject Next()
    {
        if (actual != null)
        {
            actual = actual.rightNeighbour;
            if (actual != null)
            {
                return actual.value;
            }
        }
        return null;
    }

    public GameObject Previous()
    {
        if (actual != null)
        {
            actual = actual.leftNeighbour;
            if (actual != null)
            {
                return actual.value;
            }
        }
        return null;
    }

    public BListObject NextBListObject()
    {
        if (actual != null)
        {
            actual = actual.rightNeighbour;
            return actual;
        }
        return null;
    }

    public BListObject PreviousBListObject()
    {
        if (actual != null)
        {
            actual = actual.leftNeighbour;
            return actual;
        }
        return null;
    }

    public GameObject InitEnumerationFromLeft()
    {
        actual = last;
        if (actual != null)
        {
            return actual.value;
        }
        else return null;
        
    }

    public GameObject InitEnumerationFromRight()
    {
        actual = first;
        if (actual != null)
        {
            return actual.value;
        }
        else return null;
    }

    public BListObject InitEnumerationFromLeftBListObject()
    {
        actual = last;
        return actual;
    }

    public BListObject InitEnumerationFromRightBListObject()
    {
        actual = first;
        return actual;
    }

    public void AppendLast(GameObject value)
    {
        BListObject newListObject = new BListObject();
        newListObject.value = value;

        if (first == null)
        {
            first = newListObject;
            last = newListObject;
            count++;
            return;
        }

        if (count == 1)
        {
            first.leftNeighbour = newListObject;
            newListObject.rightNeighbour = first;
            last = newListObject;
            count++;
            return;
        }

        last.leftNeighbour = newListObject;
        newListObject.rightNeighbour = last;
        last = newListObject;
        count++;
    }

    public void Insert(GameObject objectToInsert, GameObject neighbour, bool neighbourIsRight)
    {
        BListObject newObj = new BListObject();
        newObj.value = objectToInsert;

        BListObject obj = InitEnumerationFromLeftBListObject();
        do {
            if (obj.value == neighbour) break;
        } while ((obj = NextBListObject()) != null );

        if (neighbourIsRight)
        {
            newObj.leftNeighbour = obj.leftNeighbour;
            newObj.rightNeighbour = obj;

            if (obj.leftNeighbour != null)
            {
                obj.leftNeighbour.rightNeighbour = newObj;
            }

            obj.leftNeighbour = newObj;
        }
        else {
            newObj.leftNeighbour = obj;
            newObj.rightNeighbour = obj.rightNeighbour;

            if (obj.rightNeighbour != null)
            {
                obj.rightNeighbour.leftNeighbour = newObj;
            }

            obj.rightNeighbour = newObj;
        }

    }

    public BListObject Find(GameObject obj)
    {
        BListObject bListObj = InitEnumerationFromLeftBListObject();
        do {
            if (bListObj.value == obj) return bListObj;
        } while ((bListObj = NextBListObject()) != null);

        return null;
    }
}


