
using UnityEngine;

public class BallReturningScript : MonoBehaviour {

    private object collisionLock = new object();
    private bool collide;

    private void OnTriggerEnter(Collider other)
    {
        lock (collisionLock)
        {
            if (!collide)
            {
                if (other.tag == "Ball" && other.GetComponent<BallScript>().ballObj.forwardBackward != GetComponent<BallScript>().ballObj.forwardBackward)
                {
                    GameObject.Find("GameManager").GetComponent<GameManagerScript>().ChangeBallDirection(gameObject);
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

    }
}
