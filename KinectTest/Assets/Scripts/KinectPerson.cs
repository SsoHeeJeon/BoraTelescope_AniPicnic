using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectPerson : MonoBehaviour
{
    [SerializeField]
    GameObject Head;

    private void Update()
    {
        transform.position = new Vector3(Head.transform.position.x, Head.transform.position.y + 0.5f, Head.transform.position.z+0.5f);
    }
}
