using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Evader : Agent
{
    [SerializeField] private Transform targetTransform;
    public bool catcherIsAgent = false;
    float targetMoveSpeed = 0.0f;

    public float currentSpeed = 0.0f;
    public Vector3 lastPos;
    public float catcherSpeed = 0.0f;
    public Vector3 catcherLastPos;

    public override void OnEpisodeBegin()
    {
        transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-18.0f, 18.0f), 0, Random.Range(-18.0f, 18.0f)), Quaternion.identity);

        if (!catcherIsAgent)
        {
            targetMoveSpeed = Random.Range(1f, 3f);
        }

        catcherLastPos = targetTransform.localPosition;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(currentSpeed);
        sensor.AddObservation(catcherSpeed);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        currentSpeed = (transform.localPosition - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.localPosition;

        if (!catcherIsAgent)
        {
            targetTransform.localPosition = Vector3.MoveTowards(targetTransform.localPosition, transform.localPosition, targetMoveSpeed * Time.deltaTime);
        }

        catcherSpeed = (targetTransform.localPosition - catcherLastPos).magnitude / Time.deltaTime;
        catcherLastPos = targetTransform.localPosition;

        AddReward(0.0005f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.0005f);
        }
        if (other.gameObject.tag == "Catcher")
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.0005f);
        }
    }
}
