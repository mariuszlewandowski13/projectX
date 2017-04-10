using UnityEngine;

public class BallFlyingScript : MonoBehaviour {

    private object collisionLock = new object();
    private bool collide;

    private void OnCollisionEnter(Collision other)
    {
        lock (collisionLock)
        {
            if (!collide)
            {
                if (other.collider.tag == "Ball")
                {
                    GameObject.Find("GameManager").GetComponent<GameManagerScript>().AddNewBallFromPlayer(gameObject, other.gameObject);
                    collide = true;
                    Destroy(this);
                }
            }
            
        }
        
    }
}
