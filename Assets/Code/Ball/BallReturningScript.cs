using UnityEngine;

[DisallowMultipleComponent]
public class BallReturningScript : MonoBehaviour {

    private object collisionLock = new object();
    private bool collide;

    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Other: " + other.name + " , my name: " + gameObject.name);
        lock (collisionLock)
        {
            if (!collide)
            {
                if (other.tag == "Ball" && ((other.GetComponent<BallScript>().ballObj.forwardBackward != GetComponent<BallScript>().ballObj.forwardBackward) || (other.GetComponent<BallReturningScript>() != null)))
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
            //Debug.Log(gameObject.name + "    " + Time.time);
        }

    }
}
