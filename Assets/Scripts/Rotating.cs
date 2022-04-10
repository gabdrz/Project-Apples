using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    [SerializeField]
    float speed = 50.0f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
