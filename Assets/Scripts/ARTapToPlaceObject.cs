using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject placementIndicator;

    [SerializeField] private ARSessionOrigin arOrigin;
    [SerializeField] private ARRaycastManager arRaycast;

    public GameObject objectToPlace;

    private Pose placementPose;
    private bool placementPoseIsValid;
    private bool spawned;

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if( Input.touchCount > 0)
		{
            var touch = Input.GetTouch(0);
            bool isOverUI = touch.position.IsPointOverUIObject();
            if (isOverUI) return;

            if (placementPoseIsValid && touch.phase == TouchPhase.Began && !spawned)
			{
                spawned = true;
                PlaceObject();
            }
		}
    }

	private void PlaceObject()
	{
        Instantiate(objectToPlace, placementPose.position + new Vector3(0,-11,5), placementPose.rotation);
	}

	private void UpdatePlacementPose()
	{
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycast.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
		if (placementPoseIsValid)
		{
            placementPose = hits[0].pose;

            var cameraForward = arOrigin.camera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
		}
	}

    private void UpdatePlacementIndicator()
    {
		if (placementPoseIsValid && !spawned)
		{
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
		}
        else if (spawned)
		{
            placementIndicator.SetActive(false);
        }
		else
		{
            placementIndicator.SetActive(false);
		}
    }
}
