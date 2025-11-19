using System;
using Unity.VisualScripting;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Collision");
    }
}
