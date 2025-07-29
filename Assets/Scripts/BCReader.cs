using UnityEngine;
using ZXing;
using UnityEngine.XR.ARFoundation;
using ZXing.Common;
using UnityEngine.XR.ARSubsystems;
using System;
using Unity.Collections.LowLevel.Unsafe;
using System.IO;
using System.Drawing;

public class BCReader : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private float scanInterval = 0.5f;
    [SerializeField] private Texture2D debugTexture;
    [SerializeField] private Material debugMaterial;
    

    private IBarcodeReader barcodeReader;
    private float lastScanTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (arCameraManager == null)
        {
            Debug.LogError("SOMETHING BROKE WITH THE CAMERA MANAGER");
        }
        barcodeReader = new BarcodeReader();
        barcodeReader.Options = new ZXing.Common.DecodingOptions
        {
            TryHarder = true,
            PossibleFormats = new[] { BarcodeFormat.QR_CODE }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastScanTime < scanInterval) return;
        XRCpuImage image;
        bool success = arCameraManager.TryAcquireLatestCpuImage(out image);
        var conversionParams = new XRCpuImage.ConversionParams
        {
            // Convert the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Output at full resolution
            outputDimensions = new Vector2Int(image.width, image.height),

            // Convert to RGBA format
            outputFormat = TextureFormat.RGBA32
        };



        // Create a Texture2D to store the converted image
        var texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

        // Texture2D allows us write directly to the raw texture data as an optimization
        var rawTextureData = texture.GetRawTextureData<byte>();
        try
        {
            unsafe
            {
                // Synchronously convert to the desired TextureFormat
                image.Convert(
                    conversionParams,
                    new IntPtr(rawTextureData.GetUnsafePtr()),
                    rawTextureData.Length);
            }
        }
        finally
        {
            // Dispose the XRCpuImage after we're finished to prevent any memory leaks
            image.Dispose();
        }

        // Apply the converted pixel data to our texture
        texture.Apply();

        debugTexture = texture;

        var pixels = texture.GetPixels32();

        var result = barcodeReader.Decode(pixels, texture.width, texture.height);

        if (result != null) Debug.Log(result.Text);

        lastScanTime = Time.time;

        // If debugPlane is set, update its texture and position

        if (debugMaterial != null && debugTexture != null)
        {
            debugMaterial.mainTexture = debugTexture;
        }
    }

    

}
