using UnityEngine;
using OpenCvSharp;
using TMPro;

public class CheckerDetectPlane : MonoBehaviour
{
    WebCamTexture _webCamTexture;
    public TextMeshProUGUI InfoText;

    void Start()
    {
        // Access the device's camera
        WebCamDevice[] devices = WebCamTexture.devices;

        _webCamTexture = new WebCamTexture(devices[0].name);
        _webCamTexture.Play();
    }

    void Update()
    {
        // Update the texture in the scene
        GetComponent<Renderer>().material.mainTexture = _webCamTexture;
        //InfoText.text = "A";
        // XANA cia
        // Convert the webcam frame to OpenCV Mat
        Mat frame = OpenCvSharp.Unity.TextureToMat(_webCamTexture);
        Debug.Log("frame: " + frame.ToString());
        InfoText.text = "B";

        // Detect and display circles
        DetectCircles(frame);


    }

    // Detect circles using HoughCircles and annotate them
    void DetectCircles(Mat frame)
    {
        // Convert to grayscale
        Mat gray = new Mat();
        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

        // Blur the image slightly to reduce noise
        Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(9, 9), 2, 2);

        // Detect circles using HoughCircles
        CircleSegment[] circles = Cv2.HoughCircles(gray, HoughMethods.Gradient, 1, gray.Rows / 8, 100, 20, 0, 0);
        InfoText.text = "C";
        foreach (CircleSegment circle in circles)
        {
            // Get the region of interest (ROI) around the detected circle
            int radius = (int)circle.Radius;
            OpenCvSharp.Point center = circle.Center;
            InfoText.text = "D";
            // Define bounding box for the circle (with some padding to avoid out-of-bounds errors)
            int x = Mathf.Max(0, center.X - radius);
            int y = Mathf.Max(0, center.Y - radius);
            int width = Mathf.Min(frame.Cols - x, 2 * radius);
            int height = Mathf.Min(frame.Rows - y, 2 * radius);
            Mat roi = new Mat(frame, new OpenCvSharp.Rect(x, y, width, height));

            // Calculate average color within the circle's bounding box
            Scalar meanColor = Cv2.Mean(roi);
            InfoText.text = "E";
            // Determine if it's black or red based on color thresholds
            if (IsBlack(meanColor))
            {
                frame.Circle(center, radius, new Scalar(0, 0, 0), 3); // Outline the black circle
                Cv2.PutText(frame, "black", new OpenCvSharp.Point(center.X - radius / 2, center.Y), HersheyFonts.HersheySimplex, 0.6, new Scalar(255, 255, 255), 2);
            }
            else if (IsRed(meanColor))
            {
                frame.Circle(center, radius, new Scalar(0, 0, 255), 3); // Outline the red circle
                Cv2.PutText(frame, "red", new OpenCvSharp.Point(center.X - radius / 2, center.Y), HersheyFonts.HersheySimplex, 0.6, new Scalar(255, 255, 255), 2);
            }
            InfoText.text = "F";
        }

        // Convert the Mat back to a Texture2D and display it on the object
        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        InfoText.text = "G";
        GetComponent<Renderer>().material.mainTexture = newTexture;
        InfoText.text = "H";
    }

    // Function to check if the color is black based on its BGR mean values
    bool IsBlack(Scalar meanColor)
    {
        InfoText.text = "found black circle";
        // Define thresholds for detecting black color
        double threshold = 50;  // You can adjust this threshold
        return meanColor.Val0 < threshold && meanColor.Val1 < threshold && meanColor.Val2 < threshold;
    }

    // Function to check if the color is red based on its BGR mean values
    bool IsRed(Scalar meanColor)
    {
        InfoText.text = "found red circle";
        // Define thresholds for detecting red color (red has high B, and low G, R)
        return meanColor.Val2 > 150 && meanColor.Val1 < 70 && meanColor.Val0 < 70;
    }
}
