using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DisableYellowShapes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // To disable all plane visualizations at runtime:
        foreach (var plane in Object.FindObjectsByType<ARPlane>(FindObjectsSortMode.None))
        {
            var visualizer = plane.GetComponent<ARPlaneMeshVisualizer>();
            if (visualizer != null)
                visualizer.enabled = false;
        }
    }
}
