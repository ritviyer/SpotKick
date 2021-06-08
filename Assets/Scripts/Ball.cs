using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    Vector3 startPos;
    Quaternion startRotation;
    bool isLevelComplete = false;
    float initialDrag;
    static int totalBalls = 1;

    void Start()
    {
        totalBalls = FindObjectsOfType<Ball>().Length;
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        startRotation = transform.rotation;
        initialDrag = rb.drag;
    }
    private void OnEnable()
    {
        EventManager.onRefreshGame += RefreshLevel;
    }
    private void OnDisable()
    {
        EventManager.onRefreshGame -= RefreshLevel;
    }
    private void Update()
    {
        if(Time.frameCount%5 == 0)
        {
            if (rb.velocity.magnitude < 1 && rb.velocity.magnitude >= 0.05)
            {
                rb.drag += 0.01f;
                rb.angularVelocity *= 0.95f;
            }
            else
                rb.drag = initialDrag;

            if (rb.velocity.magnitude < 0.05)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
    void RefreshLevel() 
    {
        totalBalls = FindObjectsOfType<Ball>().Length;
        isLevelComplete = false;
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
                totalBalls--;
                isLevelComplete = true;
                if (totalBalls == 0)
                    EventManager.LevelComplete();
            }
        }
    }
}
