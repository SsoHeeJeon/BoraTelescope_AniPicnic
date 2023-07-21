using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCol : MonoBehaviour
{
    [SerializeField]
    Material Red;
    [SerializeField]
    Material Blue;
    [SerializeField]
    CaptureManager captruemanager;
    public int Count = 0;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Contains("HandClap"))
        {
            Count++;
            if(Count>=3)
            {
                Count = 0;
                captruemanager.capturestate = CaptureManager.CaptureState.Ready;
                transform.gameObject.SetActive(false);
            }
            else
            {
                //float a = GetComponent<MeshRenderer>().material.color.a;
                //a -= 0.1f;
                //GetComponent<MeshRenderer>().material.color = new Vector4(1, 0, 0, a);
                if (GetComponent<MeshRenderer>().material.name.Contains("red"))
                {
                    GetComponent<MeshRenderer>().material = Blue;
                }
                else
                {
                    GetComponent<MeshRenderer>().material = Red;
                }
            }
        }
    }
}
