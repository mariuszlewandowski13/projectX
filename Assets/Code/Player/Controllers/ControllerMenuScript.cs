using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMenuScript : MonoBehaviour {

    public Transform rayStartPosition;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private bool active;

    private bool isPointing;

    private Vector3 hitPoint;

    private IMenuButton lastButton;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
        bool isTriggerDown = controller.GetPressDown(triggerButton);
        RaycastHit hit;
        Ray ray = new Ray(rayStartPosition.position, transform.forward);
        Physics.Raycast(ray, out hit);

        if (hit.transform != null && (hit.transform.tag == "MenuButton"))
        {
            isPointing = true;
            hitPoint = hit.point;
            CursorOn();

            if (hit.transform.GetComponent<IMenuButton>() != null)
            {
                if (lastButton != null) lastButton.Deselect();
                lastButton = hit.transform.GetComponent<IMenuButton>();
                lastButton.Select();
            }

            if (isTriggerDown)
            {
                lastButton.OnClick();
            }

        }
        else if (isPointing)
        {
            CursorOff();
            isPointing = false;
            if (lastButton != null)
            {
                lastButton.Deselect();
                lastButton = null;
            }

        }
    }

    private void CursorOn()
    {
        active = true;
        Vector3[] points = new Vector3[] { transform.position, hitPoint };
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
