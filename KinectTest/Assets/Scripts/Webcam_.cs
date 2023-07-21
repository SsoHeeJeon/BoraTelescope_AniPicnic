using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Webcam_ : MonoBehaviour
{
    public RawImage display;
    WebCamTexture camTexture;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("On", 1f);
    }

    void On()
    {
        if (camTexture != null)
        {
            display.texture = null;
            camTexture.Stop();
            camTexture = null;
        }
        WebCamDevice device = WebCamTexture.devices[0];
        camTexture = new WebCamTexture(device.name);
        display.texture = camTexture;
        camTexture.Play();
    }
}
