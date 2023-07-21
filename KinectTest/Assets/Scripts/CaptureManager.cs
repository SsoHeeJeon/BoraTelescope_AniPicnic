using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class CaptureManager : MonoBehaviour
{
    private static Color32[] EncodeURL(string UrlText, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = width,
                Height = height
            }
        };
        return writer.Write(UrlText);
    }

    [SerializeField]
    Text Timetxt;
    float Checktime;
    [SerializeField]
    RawImage QRCodeImage;
    string boranum;
    public enum CaptureState
    {
        Idle,
        Ready,
        Capture,
        QR,
    }
    public CaptureState capturestate = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetBoranum();
    }

    // Update is called once per frame
    void Update()
    {
        if (capturestate == CaptureState.Ready)
        {
            Timetxt.gameObject.SetActive(true);
            Checktime += Time.deltaTime;
            Timetxt.text = Math.Truncate((4 - Checktime)).ToString();
            if ((4 - Checktime) < 1)
            {
                Checktime = 0;
                Timetxt.gameObject.SetActive(false);
                capturestate = CaptureState.Capture;
            }
        }
        else if (capturestate == CaptureState.Capture)
        {
            StartCoroutine("CaptureandSave");
            capturestate = CaptureState.Idle;
        }
    }

    private IEnumerator CaptureandSave()
    {
        yield return new WaitForEndOfFrame();
        Camera Objcamera = Camera.main.GetComponent<Camera>();
        DirectoryInfo dir = new DirectoryInfo("C:/ScreenShot");

        if (!dir.Exists)
        {
            Directory.CreateDirectory("C:/ScreenShot");
        }

        string name;
        string filename;
        name = "C:/ScreenShot/"+ boranum+"-" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        filename = boranum + "-"+System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        Objcamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);
        Objcamera.Render();
        RenderTexture.active = rt;

        screenShot.ReadPixels(rec, 0, 0);
        //screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        //startflasheffect = false;
        Debug.Log(bytes);
        File.WriteAllBytes(name, bytes);



        PutImageObject(name, filename);

        Objcamera.targetTexture = null;

        Destroy(screenShot);
    }

    public void PutImageObject(string filepath, string filename)
    {
        HttpClient httpClient = new HttpClient();
        MultipartFormDataContent form = new MultipartFormDataContent();

        byte[] imagebytearraystring = ImageFileToByteArray(filepath); // 파일 경로 넣기
        form.Add(new ByteArrayContent(imagebytearraystring, 0, imagebytearraystring.Length), "boraphotofile", filename); // key 이름, 업로드 될 때 이름

        try
        {
            HttpResponseMessage response = httpClient.PostAsync("https://bora.web.awesomeserver.kr/info/BoraUploadForPhotoToS3", form).Result; // 요청할 페이지 주소 (반드시 http 나 https 로 시작해야함)
            httpClient.Dispose();
            string url = "";
            url = response.Content.ReadAsStringAsync().Result; // 성공적으로 완료 될 시 서버 측에서의 답변 값
            print(url);
            Console.WriteLine(url);

            if (url == "Fail Upload")
            {
                return;
            }
            else if (url.Contains("error") || url.Contains("Error"))
            {
                Debug.Log("no");
                return;
            }
            MakeQRCode(url);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            print("error");
        }
    }

    byte[] ImageFileToByteArray(string fullFilePath)
    {
        FileStream fs = File.OpenRead(fullFilePath);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
        fs.Close();
        return bytes;
    }

    public void MakeQRCode(string url)
    {
        Texture2D QRImage = CreateQR(url);
        QRCodeImage.gameObject.SetActive(true);
        QRCodeImage.texture = QRImage;
        Invoke("waitQRcode", 1f);

    }

    public Texture2D CreateQR(string URL)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = EncodeURL(URL, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();

        return encoded;
    }

    public void waitQRcode()
    {

    }

    public void PutImageObject2(string filepath, string filename)
    {
        HttpClient httpClient = new HttpClient();
        MultipartFormDataContent form = new MultipartFormDataContent();

        byte[] imagebytearraystring = ImageFileToByteArray(filepath); // 파일 경로 넣기
        form.Add(new ByteArrayContent(imagebytearraystring, 0, imagebytearraystring.Length), "boraphotofile", filename); // key 이름, 업로드 될 때 이름

        try
        {
            HttpResponseMessage response = httpClient.PostAsync("https://bora.web.awesomeserver.kr/info/BoraUploadForPhotoToS3", form).Result; // 요청할 페이지 주소 (반드시 http 나 https 로 시작해야함)
            httpClient.Dispose();
            string url = "";
            //string sd = response.Content.ReadAsStringAsync().Result; // 성공적으로 완료 될 시 서버 측에서의 답변 값
            url = response.Content.ReadAsStringAsync().Result; // 성공적으로 완료 될 시 서버 측에서의 답변 값
            Console.WriteLine(url);

            if (url == "Fail Upload")
            {
                return;
            }
            else if (url.Contains("error") || url.Contains("Error"))
            {
                Debug.Log("no");
                return;
            }
            MakeQRCode(url);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void GetBoranum()
    {
        string borainfo = File.ReadAllText("C:/XRTeleSpinCam/bora_info.txt");
        borainfo.Replace("\r\n", string.Empty);
        boranum = borainfo.Substring(13, borainfo.IndexOf("\r\n") - 13);
    }
}
