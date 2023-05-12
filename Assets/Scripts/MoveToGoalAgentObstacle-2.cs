using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgentObstacle2 : Agent
{
    [SerializeField] private Transform targetTransform;
    //[SerializeField] private Material winMaterial;
    //[SerializeField] private Material loseMaterial;
    //[SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private GameObject spawnZone;

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = Vector3.zero;
        //transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-8.0f, 8.0f), 0, Random.Range(-8.0f, 8.0f)), Quaternion.identity);
        //targetTransform.localPosition = new Vector3(Random.Range(-8.0f, 8.0f), 0, Random.Range(-8.0f, 8.0f));
        int range = spawnZone.transform.childCount;
        int agentZone = Random.Range(0, range - 1);
        int goalZone = Random.Range(0, range - 1);

        for (int i = 0; i < range; i++)
        {
            if (agentZone == i)
            {
                var bounds = spawnZone.transform.GetChild(i).GetComponent<Collider>().bounds;
                var dx = transform.parent.position.x;
                var dz = transform.parent.position.z;
                var px = Random.Range(bounds.min.x, bounds.max.x) - dx;
                var pz = Random.Range(bounds.min.z, bounds.max.z) - dz;
                transform.SetLocalPositionAndRotation(new Vector3(px, 0, pz), Quaternion.identity);
            }
            if (goalZone == i)
            {
                var bounds = spawnZone.transform.GetChild(i).GetComponent<Collider>().bounds;
                var dx = transform.parent.position.x;
                var dz = transform.parent.position.z;
                var px = Random.Range(bounds.min.x, bounds.max.x) - dx;
                var pz = Random.Range(bounds.min.z, bounds.max.z) - dz;
                targetTransform.localPosition = new Vector3(px, 0, pz);
            }
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions[0]);
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        AddReward(-0.0005f); // for each step per time step
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(1.0f);
            //floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        //if (other.TryGetComponent<Wall>(out Wall wall))
        //{
        //    SetReward(-0.1f);
        //    //floorMeshRenderer.material = loseMaterial;
        //    //EndEpisode();
        //}
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.05f);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.005f);
        }
        
    }
}
