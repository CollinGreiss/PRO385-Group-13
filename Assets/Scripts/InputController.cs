using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceObject : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject gameBoard;

    public ARPlaneManager planeManager;

    bool isPlacing = false;
    bool placed = false;

    void Start()
    {

        raycastManager ??= GetComponent<ARRaycastManager>();

    }

    void Update()
    {

        if (raycastManager == null) return;

        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {

            Vector2 touchPos = Touchscreen.current.touches[0].position.ReadValue();
            if (!isPlacing && !placed)
            {

                isPlacing = true;
                PlaceObject(touchPos);
                return;

            }

            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log("Hit: " + hit.transform.name);
                GameManager.Instance.CheckHit(hit);

            }
            else Debug.Log("No hit detected");

        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {

            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (!isPlacing && !placed)
            {
                isPlacing = true;
                PlaceObject(mousePos);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
                GameManager.Instance.CheckHit(hit);
            else Debug.Log("No hit detected");

        }
    
    }


    void PlaceObject(Vector2 position)
    {

        if (placed || !raycastManager || gameBoard == null) return;


        //Debug.Log(placed);
        var rayHits = new List<ARRaycastHit>();
        raycastManager.Raycast(position, rayHits, TrackableType.PlaneWithinPolygon);

        if (rayHits.Count > 0)
        {

            placed = true;
            Vector3 hitPosePosition = rayHits[0].pose.position;
            Quaternion hitPoseRotation = rayHits[0].pose.rotation;
            Instantiate(gameBoard, hitPosePosition, hitPoseRotation);

            GameManager.Instance.IsReadyToStart();

            DisablePlaneDetection();

        }

        StartCoroutine(SetPlacingToFalseWithDelay());

    }

    void DisablePlaneDetection()
    {
        planeManager.enabled = false;

        foreach (var plane in planeManager.trackables)
        {
            // Hide plane GameObject
            plane.gameObject.SetActive(false);

            // OR: If that doesn't do it, surgically disable visuals
            var meshVisualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (meshVisualizer != null) meshVisualizer.enabled = false;

            var renderer = plane.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = false;

            var lineRenderer = plane.GetComponent<LineRenderer>();
            if (lineRenderer != null) lineRenderer.enabled = false;

            // You can also zero out the mesh if you're desperate
            var meshFilter = plane.GetComponent<MeshFilter>();
            if (meshFilter != null) meshFilter.mesh = null;
        }
    }

    IEnumerator SetPlacingToFalseWithDelay()
    {

        yield return new WaitForSeconds(0.25f);
        isPlacing = false;

    }

}