using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    Vector3 startPos;
    Quaternion startRotation;
    bool isLevelComplete = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        startRotation = transform.rotation;
    }
    private void OnEnable()
    {
        EventManager.onRefreshGame += RefreshLevel;
    }
    private void OnDisable()
    {
        EventManager.onRefreshGame -= RefreshLevel;
    }
    void RefreshLevel() 
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPos;
        transform.rotation = startRotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Net"))
        {
            if (!isLevelComplete)
            {
                isLevelComplete = true;
                EventManager.LevelComplete();
            }
        }
    }
}
