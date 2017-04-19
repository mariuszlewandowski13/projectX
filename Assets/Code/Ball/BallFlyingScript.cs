using UnityEngine;

public class BallFlyingScript : MonoBehaviour
{

    private object collisionLock = new object();
    private bool collide;

    public GameObject myOldParent;

    private void OnTriggerEnter(Collider other)
    {
        lock (collisionLock)
        {
            if (!collide)
            {
                //Debug.Log(other.name);
                if (other.tag == "Ball")
                {
                    GameObject.Find("GameManager").GetComponent<GameManagerScript>().AddNewBallFromPlayer(gameObject, other.gameObject);
                    collide = true;
                }
            }

        }

    }

    private void Update()
    {
        if (collide)
        {
            DestroyImmediate(GetComponent<Rigidbody>());
            Destroy(this);
        }
        if (myOldParent != null && Vector3.Distance(gameObject.transform.position, myOldParent.transform.position) > 30.0f)
        {
            Destroy(gameObject);
        }

    }
}