using System;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("he Chocado");
    }
}