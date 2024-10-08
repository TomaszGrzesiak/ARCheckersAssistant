using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using TMPro;


public class ColorDetectionOpenCvSharp : MonoBehaviour
{
    public RawImage cameraDisplay;  // UI element to display the camera feed
    public TextMeshProUGUI colorHexText;       // Text UI to display the hex code
    private WebCamTexture webCamTexture;  // For accessing the camera feed
    private Mat frame;  // OpenCvSharp Mat to hold the camera frame
    private Texture2D texture;  // Texture to display in the RawImage

    void Start()
    {
        // Start the camera
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
        
        // Create a texture to display the camera feed
        texture = new Texture2D(webCamTexture.width, webCamTexture.height);
        cameraDisplay.texture = texture;
    }

    void Update()
    {
        // Update the camera feed only if there's a new frame
        if (webCamTexture.didUpdateThisFrame)
        {
            // Convert WebCamTexture to OpenCvSharp Mat
            frame = OpenCvSharp.Unity.TextureToMat(webCamTexture);

            if (frame != null)
            {
                // Get the center pixel color
                Vec3b centerColor = GetCenterPixelColor(frame);

                // Convert the color to hex code
                string hexColor = ColorToHex(centerColor);

                // Display the hex code in the UI text element
                colorHexText.text = "Hex Code: " + hexColor;

                // Convert the Mat back to Texture2D and display it in the RawImage
                texture = OpenCvSharp.Unity.MatToTexture(frame, texture);
            }
        }
    }

    // Function to get the color of the pixel at the center of the frame
    private Vec3b GetCenterPixelColor(Mat frame)
    {
        int centerX = frame.Width / 2;
        int centerY = frame.Height / 2;

        // Access the pixel at the center of the frame
        Vec3b color = frame.At<Vec3b>(centerY, centerX);
        return color;
    }

    // Helper function to convert Vec3b (BGR) to a hex color string
    private string ColorToHex(Vec3b color)
    {
        // Vec3b in OpenCV uses BGR (Blue, Green, Red)
        int blue = color.Item0;
        int green = color.Item1;
        int red = color.Item2;

        // Convert to hex string
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    void OnDestroy()
    {
        // Stop the camera when the script is destroyed
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}
