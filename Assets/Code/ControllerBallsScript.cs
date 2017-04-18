using UnityEngine;

public class ControllerBallsScript : MonoBehaviour {

    public GameObject ballPrefab;

    private GameObject myBall;

    public Transform startPosition;

    private float forceMultiplier = 100.0f;

    private bool ballToSpawn;

    private float lastShotTime;

    private float timeFromShootToSpawn = 0.5f;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private bool active;

    private bool isPointing;

    private Vector3 hitPoint;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        CreateNewBall();
    }

    void Update()
    {
        bool isTriggerDown = controller.GetPressDown(triggerButton);
        bool isGripDown = controller.GetPressDown(gripButton);
        RaycastHit hit;
            Ray ray = new Ray(startPosition.position, startPosition.forward);
            Physics.Raycast(ray, out hit);

            if (hit.transform != null && (hit.transform.tag == "Board" || hit.transform.tag == "Ball"))
            {
                isPointing = true;
                hitPoint = hit.point;
                CursorOn();

                if (isTriggerDown && ShootTheBall(hit.point))
                    {
                        SetTimeForSpawningNewBall();
                    }
            }
            else if (isPointing)
            {
                CursorOff();
                isPointing = false;
            }

        if (ballToSpawn && (Time.time - lastShotTime) > timeFromShootToSpawn)
        {
            CreateNewBall();
            ballToSpawn = false;
        }

        if (isGripDown)
        {
            PlayStopGame();
        }
    }

    private void PlayStopGame()
    {
        GameManagerScript.playing = !GameManagerScript.playing;
    }

    private void CreateNewBall()
    {
        myBall = Instantiate(ballPrefab, startPosition.position, new Quaternion());
        myBall.GetComponent<BallScript>().SetBallObject(ApplicationData.RandomNewColor()/*Color.black*/);
        myBall.transform.parent = startPosition;
    }

    private bool ShootTheBall(Vector3 destination)
    {
        if (myBall != null)
        {
            myBall.transform.parent = null;
            Vector3 force = destination - startPosition.position;
            myBall.AddComponent<BallFlyingScript>();
            Rigidbody rigid = myBall.AddComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.isKinematic = false;
            rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigid.AddForce(force * forceMultiplier, ForceMode.Force);
            myBall = null;
            return true;
        }

        return false;
    }

    private void SetTimeForSpawningNewBall()
    {
        lastShotTime = Time.time;
        ballToSpawn = true;
    }

    private void CursorOn()
    {
        active = true;
        Vector3[] points = new Vector3[] { (myBall != null ? startPosition.position : transform.position), hitPoint};
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPositions(points);
    }
    private void CursorOff()
    {
        if (active)
        {
            active = false;
            GetComponent<LineRenderer>().positionCount = 0;
        }

    }

}
