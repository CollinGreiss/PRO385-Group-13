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

    bool isPlacing = false;
    bool placed = false;

    void Start() {

        raycastManager ??= GetComponent<ARRaycastManager>();

    }

    void Update() {
        
        if (raycastManager == null) return;

        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began && !isPlacing) {
                    
            isPlacing = true;
            Vector2 touchPos = Touchscreen.current.touches[0].position.ReadValue();
            PlaceObject(touchPos);

        } else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !isPlacing) {
                    
            isPlacing = true;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            PlaceObject(mousePos);

        }

    }


    void PlaceObject(Vector2 position) {

        if (placed || !raycastManager || gameBoard == null) return;


        //Debug.Log(placed);
        var rayHits = new List<ARRaycastHit>();
        raycastManager.Raycast(position, rayHits, TrackableType.PlaneWithinPolygon);

        if (rayHits.Count > 0)
        {

            //placed = true;
            Vector3 hitPosePosition = rayHits[0].pose.position;
            Quaternion hitPoseRotation = rayHits[0].pose.rotation;
            Instantiate(gameBoard, hitPosePosition, hitPoseRotation);
            
            GameManager.Instance.IsReadyToStart();

        }

        StartCoroutine(SetPlacingToFalseWithDelay());

    }

    IEnumerator SetPlacingToFalseWithDelay() {
        
        yield return new WaitForSeconds(0.25f);
        isPlacing = false;

    }

}