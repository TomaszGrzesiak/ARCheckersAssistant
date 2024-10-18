using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ImageOutlineTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;
    public GameObject linePrefab;  // Prefab containing LineRenderer
    public TextMeshProUGUI info;

    // Dictionary to store LineRenderers for each tracked image
    private Dictionary<ARTrackedImage, LineRenderer> lineRenderers = new Dictionary<ARTrackedImage, LineRenderer>();

    void Awake()
    {
        // Find the ARTrackedImageManager component in the scene
        trackedImageManager = Object.FindAnyObjectByType<ARTrackedImageManager>();
        info.text = "searchin'";
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackablesChanged;
        info.text = "ENABLED";
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackablesChanged;
        info.text = "DISABLED";
    }

    // This method will be called when AR Tracked Images are added, updated, or removed
    void OnTrackablesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle added tracked images
        foreach (var addedImage in eventArgs.added)
        {
            // Create a new LineRenderer for the added image
            GameObject newLineObject = Instantiate(linePrefab);
            LineRenderer newLineRenderer = newLineObject.GetComponent<LineRenderer>();

            // Set positions for the LineRenderer (4 points + loop closure)
            newLineRenderer.positionCount = 5;
            newLineRenderer.loop = true;

            // Store the new LineRenderer in the dictionary
            lineRenderers[addedImage] = newLineRenderer;

            // Draw the outline
            DrawOutline(addedImage, newLineRenderer);
        }

        // Handle updated tracked images
        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                // Draw the outline for the updated image
                if (lineRenderers.ContainsKey(updatedImage))
                {
                    DrawOutline(updatedImage, lineRenderers[updatedImage]);
                    info.text = "board found";
                }
            }
            else
            {
                // Disable LineRenderer if the image is not tracked
                if (lineRenderers.ContainsKey(updatedImage))
                {
                    lineRenderers[updatedImage].enabled = false;
                    info.text = "nyrandu ;c";
                }
            }
        }

        // Handle removed tracked images
        foreach (var removedImage in eventArgs.removed)
        {
            // Disable and destroy the LineRenderer for the removed image
            if (lineRenderers.ContainsKey(removedImage))
            {
                lineRenderers[removedImage].enabled = false;
                Destroy(lineRenderers[removedImage].gameObject);
                lineRenderers.Remove(removedImage);
            }
        }
    }

    // Method to draw an outline around the tracked image
    void DrawOutline(ARTrackedImage trackedImage, LineRenderer lineRenderer)
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

        // Enable the LineRenderer to display the outline
        lineRenderer.enabled = true;
    }
}
