using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    float moveSpeed = 6f;
    void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * moveSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
}
