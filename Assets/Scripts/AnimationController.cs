using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] GameObject cueBall;
    public void KickTheBall()
    {
        cueBall.GetComponent<KickBall>().AddForceToBall();
    }
}
