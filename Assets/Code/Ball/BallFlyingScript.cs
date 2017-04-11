using UnityEngine;

public class BallFlyingScript : MonoBehaviour {

    private object collisionLock = new object();
    private bool collide;

    private void OnTriggerEnter(Collider other)
    {
        lock (collisionLock)
        {
            if (!collide)
            {
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
        
    }
}
