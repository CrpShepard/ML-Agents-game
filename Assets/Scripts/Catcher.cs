using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Catcher : Agent
{
    [SerializeField] private Transform targetTransform;
    public bool evaderIsAgent = false;
    float targetMoveSpeed = 0.0f;
    Vector3 targetMovePosition = Vector3.zero;

    public float currentSpeed = 0.0f;
    public Vector3 lastPos;
    public float evaderSpeed = 0.0f;
    public Vector3 evaderLastPos;

    public override void OnEpisodeBegin()
    {
        transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-18.0f, 18.0f), 0, Random.Range(-18.0f, 18.0f)), Quaternion.identity);

        if (!evaderIsAgent) 
        {
            targetMoveSpeed = Random.Range(0f, 3f);
            targetMovePosition = new Vector3(Random.Range(-19.0f, 19.0f), 0, Random.Range(-19.0f, 19.0f));
        }

        evaderLastPos = targetTransform.localPosition;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(currentSpeed);
        sensor.AddObservation(evaderSpeed);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        currentSpeed = (transform.localPosition - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.localPosition;

        if (!evaderIsAgent)
        {
            if (targetTransform.localPosition != targetMovePosition)
                targetTransform.localPosition = Vector3.MoveTowards(targetTransform.localPosition, targetMovePosition, targetMoveSpeed * Time.deltaTime);
            else
            {
                targetMovePosition = new Vector3(Random.Range(-19.0f, 19.0f), 0, Random.Range(-19.0f, 19.0f));
            }
        }

        evaderSpeed = (targetTransform.localPosition - evaderLastPos).magnitude / Time.deltaTime;
        evaderLastPos = targetTransform.localPosition;

        AddReward(-0.0005f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Evader")
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }
}
