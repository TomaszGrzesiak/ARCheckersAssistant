using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageOutlineTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;
    private LineRenderer lineRenderer;
    public GameObject lineObject;
    public TextMeshProUGUI info;

    void Awake()
    {
        // Find the ARTrackedImageManager component in the scene
        trackedImageManager = Object.FindAnyObjectByType<ARTrackedImageManager>();
        lineRenderer = lineObject.GetComponent<LineRenderer>();

        // Set the number of positions for the LineRenderer (4 points + loop closure)
        lineRenderer.positionCount = 5;
        lineRenderer.loop = true;
        info.text = "searchin'";
    }

    void OnEnable()
    {
        // Subscribe to the trackablesChanged event using AddListener
        trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
        info.text = "ENABLED'";
    }

    void OnDisable()
    {
        // Unsubscribe from the trackablesChanged event
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        info.text = "DISABLED'";
    }

    // This method will be called when AR Tracked Images are added, updated, or removed
    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // Handle added tracked images
        foreach (var addedImage in eventArgs.added)
        {
            DrawOutline(addedImage);
        }

        // Handle updated tracked images
        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                DrawOutline(updatedImage);
                info.text = "board found";
            }
            else
            {
                lineRenderer.enabled = false;  // Disable line if the image is no longer tracked
                info.text = "nyrandu ;c";
            }
        }

        // Handle removed tracked images
        foreach (var removedImage in eventArgs.removed)
        {
            lineRenderer.enabled = false;  // Disable the outline when the image is removed
        }
    }

    // Method to draw an outline around the tracked image
    void DrawOutline(ARTrackedImage trackedImage)
    {
        // Get the size of the tracked image
        Vector2 size = trackedImage.size;

        // Calculate the corner points of the image
        Vector3 topLeft = trackedImage.transform.TransformPoint(new Vector3(-size.x / 2, 0, size.y / 2));
        Vector3 topRight = trackedImage.transform.TransformPoint(new Vector3(size.x / 2, 0, size.y / 2));
        Vector3 bottomRight = trackedImage.transform.TransformPoint(new Vector3(size.x / 2, 0, -size.y / 2));
        Vector3 bottomLeft = trackedImage.transform.TransformPoint(new Vector3(-size.x / 2, 0, -size.y / 2));

        // Assign the corner points to the LineRenderer to form the outline
        lineRenderer.SetPosition(0, topLeft);
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, bottomRight);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, topLeft);  // Close the loop

        // Enable the line renderer to display the outline
        lineRenderer.enabled = true;
    }
}