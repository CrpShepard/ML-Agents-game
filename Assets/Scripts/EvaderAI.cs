using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

public class EvaderAI : MonoBehaviour
{
    [SerializeField] private GameObject catcher;
    [SerializeField] private GameObject sceneParams;
    float moveSpeed;
    float displacementDist = 5f;
    bool touchingWall = false;
    Vector3 collisionPos;

    void Start()
    {
        moveSpeed = sceneParams.GetComponent<SceneParams>().evaderSpeed;
    }

    void Update()
    {
        if (catcher.GetComponent<Catcher>().lapCount < sceneParams.GetComponent<SceneParams>().maxLaps)
        {
            Evade(touchingWall);
        }
    }

    private void Evade(bool touchingWall)
    {
        if (!touchingWall)
        {
            Vector3 normDir = (catcher.transform.localPosition - transform.localPosition).normalized;
            normDir = Quaternion.AngleAxis(Random.Range(0, 179), Vector3.up) * normDir;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition - (normDir * displacementDist), moveSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 normDir = Quaternion.AngleAxis(Random.Range(0, 89), Vector3.up) * collisionPos;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition - (normDir * displacementDist), moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            collisionPos = other.contacts[0].point;
            touchingWall = true;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            collisionPos = other.contacts[0].point;
            touchingWall = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        touchingWall = false;
    }
}
