using OpenCvSharp;
using UnityEngine;

public static class TextureConverter
{
    // Converts Texture2D to OpenCvSharp Mat
    public static Mat Texture2DToMat(Texture2D texture)
    {
        // Create a new Mat with the same dimensions as the Texture2D
        Mat mat = new Mat(texture.height, texture.width, MatType.CV_8UC4);

        // Copy the pixel data from the Texture2D into a Color32 array
        Color32[] pixels = texture.GetPixels32();

        // Lock the Mat data so we can directly manipulate it
        unsafe
        {
            fixed (Color32* pixelPtr = pixels)
            {
                using (Mat matPixels = new Mat(texture.height, texture.width, MatType.CV_8UC4, (System.IntPtr)pixelPtr))
                {
                    matPixels.CopyTo(mat);
                }
            }
        }

        return mat;
    }
}
