using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

public class CatcherAI : MonoBehaviour
{
    [SerializeField] private GameObject evader;
    [SerializeField] private GameObject sceneParams;
    float moveSpeed;

    void Start()
    {
        moveSpeed = sceneParams.GetComponent<SceneParams>().catcherSpeed;
    }

    void Update()
    {
        if (evader.GetComponent<Evader>().lapCount < sceneParams.GetComponent<SceneParams>().maxLaps)
        {
            Catch();
        }
    }

    private void Catch()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, evader.transform.localPosition, moveSpeed * Time.deltaTime);
    }
}
