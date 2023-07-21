using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandClap : MonoBehaviour
{
    [SerializeField]
    GameObject head;
    [SerializeField]
    Text txt;
    [SerializeField]
    TrackerHandler handler;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("HandClap"))
        {
            if (head.transform.position.y - transform.position.y < -0.15f)
            {
                txt.text = "�μ��� ������ �Ӹ�����";
            }
            else
            {
                txt.text = "";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        txt.text = "";
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("HandClap"))
        {
            print("�ڼ�");
            if((int)handler.main.m_lastFrameData.NumOfBodies<=handler.PersonCount)
            {
                handler.PersonCount = 0;
                handler.IndexCount = (int)handler.main.m_lastFrameData.Bodies[handler.PersonCount].Id;
            }
            else
            {
                handler.PersonCount += 1;
                handler.IndexCount = (int)handler.main.m_lastFrameData.Bodies[handler.PersonCount].Id;
            }
        }
    }
}
