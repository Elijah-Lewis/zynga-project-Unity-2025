using System;
using UnityEngine;

public class CollisionBehavior : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Walls"){
            print("Collision");
        }
    }
}