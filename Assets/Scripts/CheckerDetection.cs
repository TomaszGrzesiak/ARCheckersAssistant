using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using OpenCvSharp;
using System;
using Unity.Collections;
using TMPro;

[RequireComponent(typeof(ARCameraManager))]
public class CircleDetectionARFoundation : MonoBehaviour
{
    ARCameraManager _cameraManager; // ARCameraManager for accessing the AR camera feed
    OpenCvSharp.Point detectedCircleCenter;
    int detectedCircleRadius;
    Texture2D _cameraTexture;
    public TextMeshProUGUI infoText;

    void Start()
    {
        // Get the ARCameraManager component to access the AR camera feed
        _cameraManager = GetComponent<ARCameraManager>();
    }

    void Update()
    {
        // Try to get the latest CPU image from the AR camera
        if (_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            // Process the camera image to detect circles
            ProcessCameraImage(image);

            // Always dispose of the image after use
            image.Dispose();
        }
    }

    void ProcessCameraImage(XRCpuImage image)
    {
        Debug.Log("Image processing");
        //infoText.text = "1";
        // Conversion parameters for the image to RGBA format
        XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.MirrorY // Adjust for AR camera mirroring
        };
        //infoText.text += "2";

        // Get the size of the image buffer
        int size = image.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);
        //infoText.text += "3";

        // Convert the image to RGBA format
        image.Convert(conversionParams, buffer);
        //infoText.text += "4";

        // Load the data into a Texture2D
        if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
        {
            //infoText.text += "5";

            _cameraTexture = new Texture2D(image.width, image.height, conversionParams.outputFormat, false);
        }
        infoText.text = $"texture: Width {image.width}, Height {image.height}";

        _cameraTexture.LoadRawTextureData(buffer);
        _cameraTexture.Apply();
        //infoText.text += "6";

        buffer.Dispose();
        //infoText.text += "7";

        // Convert the Texture2D to OpenCV Mat
        Mat frame = OpenCvSharp.Unity.TextureToMat(_cameraTexture);
        //infoText.text += "8";

        // Detect circles in the camera image
        DetectCircles(frame);
        //infoText.text += "9";

        // Optionally display the detected circles back on the AR camera feed
        Display(frame);
        //infoText.text += "0";

    }

    // Detect circles using HoughCircles
    void DetectCircles(Mat frame)
    {
        Debug.Log("Detect Circles");
        // Convert the image to grayscale
        Mat gray = new Mat();
        //Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

        // Apply a Gaussian blur to reduce noise
        //Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(9, 9), 2, 2);

        // Detect circles using HoughCircles
        CircleSegment[] circles = Cv2.HoughCircles(gray, HoughMethods.Gradient, 1, gray.Rows / 8, 100, 20, 0, 0);

        // Check if any circles were detected
        if (circles.Length > 0)
        {
            detectedCircleCenter = circles[0].Center;
            detectedCircleRadius = (int)circles[0].Radius;
        }
        else
        {
            detectedCircleCenter = new OpenCvSharp.Point(0, 0); // Reset if no circle is found
            detectedCircleRadius = 0;
        }
    }

    // Display the detected circles on the AR camera feed
    void Display(Mat frame)
    {
        Debug.Log("Display");
        // If a circle is detected, draw it on the frame
        if (detectedCircleRadius > 0)
        {
            Debug.Log("DETECTED");
            // Draw the detected circle
            frame.Circle(detectedCircleCenter, detectedCircleRadius, new Scalar(0, 0, 255), 3);

            // Draw a small green circle at the center of the detected circle
            frame.Circle(detectedCircleCenter, 5, new Scalar(0, 255, 0), -1);
        }

        // Convert the Mat back to a Texture2D (if needed for further display)
        Texture2D processedTexture = OpenCvSharp.Unity.MatToTexture(frame);

        // In ARFoundation, the camera feed is already displayed, so you can optionally process the texture further
        // For instance, you could display it on a UI element or use it for additional AR effects.
    }
}
