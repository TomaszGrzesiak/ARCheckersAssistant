using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using OpenCvSharp;
using System;
using Unity.Collections;
using TMPro;

[RequireComponent(typeof(ARCameraManager))]
public class BlackCircleDetectionAR : MonoBehaviour
{
    ARCameraManager _cameraManager;
    Texture2D _cameraTexture;
    public TextMeshProUGUI infoText;

    void Start()
    {
        _cameraManager = GetComponent<ARCameraManager>();
    }

    void Update()
    {
        if (_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            ProcessCameraImage(image);
            image.Dispose();
        }
    }

    void ProcessCameraImage(XRCpuImage image)
    {
        // Conversion to RGBA format for OpenCV processing
        XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        int size = image.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);
        image.Convert(conversionParams, buffer);

        // Create Texture2D from AR Camera Feed
        if (_cameraTexture == null || _cameraTexture.width != image.width || _cameraTexture.height != image.height)
        {
            _cameraTexture = new Texture2D(image.width, image.height, conversionParams.outputFormat, false);
        }
        _cameraTexture.LoadRawTextureData(buffer);
        _cameraTexture.Apply();
        buffer.Dispose();

        // Convert Texture2D to OpenCV Mat for processing
        Mat frame = OpenCvSharp.Unity.TextureToMat(_cameraTexture);

        // Detect circles
        DetectBlackCircles(frame);
    }

    void DetectBlackCircles(Mat frame)
    {
        Mat gray = new Mat();
        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(9, 9), 2, 2);

        CircleSegment[] circles = Cv2.HoughCircles(gray, HoughMethods.Gradient, 1, gray.Rows / 8, 100, 20, 0, 0);

        foreach (CircleSegment circle in circles)
        {
            // Calculate region of interest around the circle
            OpenCvSharp.Point center = circle.Center;
            int radius = (int)circle.Radius;

            // Define bounding box
            int x = Mathf.Max(0, center.X - radius);
            int y = Mathf.Max(0, center.Y - radius);
            int width = Mathf.Min(frame.Cols - x, 2 * radius);
            int height = Mathf.Min(frame.Rows - y, 2 * radius);
            Mat roi = new Mat(frame, new OpenCvSharp.Rect(x, y, width, height));

            // Calculate the average color within the circle’s bounding box
            Scalar meanColor = Cv2.Mean(roi);

            if (IsBlack(meanColor))
            {
                frame.Circle(center, radius, new Scalar(0, 0, 0), 3); // Draw black outline
                Cv2.PutText(frame, "Black Circle", center, HersheyFonts.HersheySimplex, 0.6, new Scalar(255, 255, 255), 2);
                infoText.text = "Black Circle Detected!";
            }
        }
    }

    bool IsBlack(Scalar meanColor)
    {
        double threshold = 50;
        return meanColor.Val0 < threshold && meanColor.Val1 < threshold && meanColor.Val2 < threshold;
    }
}
