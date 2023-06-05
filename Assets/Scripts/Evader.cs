using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Evader : Agent
{
    Vector3 initialPos;
    Vector3 catcherInitialPos;
    public bool randomSpawn = true;

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform ground;
    public enum TypeOfGround { Rectangle, Circural, Other };
    public TypeOfGround typeOfGround;
    [SerializeField] private Transform spawnZone;

    public bool catcherIsAgent;
    public float moveSpeed = 6f;
    public bool fixedSpeed = true;
    [SerializeField] private GameObject sceneParams;

    [SerializeField] private GameObject currentLapTime;
    [SerializeField] private GameObject averageLapTime;
    [SerializeField] private GameObject lap;

    float startTime;
    float elapsedTime;
    bool hasStartedLap = false;
    float[] lapsTime;
    public int lapCount = 0;

    public int maxLaps;

    float currentSpeed = 0.0f;
    Vector3 lastPos;
    float catcherSpeed = 0.0f;
    Vector3 catcherLastPos;

    void Start()
    {
        catcherIsAgent = sceneParams.GetComponent<SceneParams>().catcherIsAgent;
        maxLaps = sceneParams.GetComponent<SceneParams>().maxLaps;
        lapsTime = new float[maxLaps];
        initialPos = transform.localPosition;
        catcherInitialPos = targetTransform.localPosition;

        if (fixedSpeed)
        {
            moveSpeed = sceneParams.GetComponent<SceneParams>().evaderSpeed;
        }
    }

    private void Update()
    {
        if (lapCount < maxLaps)
        {
            if (hasStartedLap && !catcherIsAgent)
            {
                elapsedTime = Mathf.Round((Time.time - startTime) * 100f) / 100f;
                currentLapTime.GetComponent<TextMesh>().text = "Current: " + elapsedTime.ToString() + "s";
            }
        }
        else
        {
            currentLapTime.GetComponent<TextMesh>().text = "Current: 0s";
        }
    }

    public override void OnEpisodeBegin()
    {
        if (lapCount < maxLaps)
        {
            if (hasStartedLap)
            {
                if (!catcherIsAgent)
                {
                    lapsTime[lapCount] = elapsedTime;
                    float averageTime = 0f;
                    for (int i = 0; i < lapCount; i++)
                    {
                        averageTime += lapsTime[i];
                    }
                    averageTime = averageTime / (lapCount + 1);
                    averageLapTime.GetComponent<TextMesh>().text = "Avg: " + averageTime.ToString() + "s";

                    lapCount++;
                    lap.GetComponent<TextMesh>().text += lapCount.ToString() + ") " + elapsedTime.ToString() + "s" + '\n';
                }
                else
                {
                    lapCount++;
                }
            }

            hasStartedLap = true;
            startTime = Time.time;
            if (randomSpawn)
            {
                if (typeOfGround == TypeOfGround.Rectangle)
                {
                    transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-(ground.localScale.x / 2f - 2f), ground.localScale.x / 2f - 2f), 0, Random.Range(-(ground.localScale.z / 2f - 2f), ground.localScale.z / 2f - 2f)), Quaternion.identity);
                    if (!catcherIsAgent)
                    {
                        targetTransform.SetLocalPositionAndRotation(new Vector3(Random.Range(-(ground.localScale.x / 2f - 2f), ground.localScale.x / 2f - 2f), 0, Random.Range(-(ground.localScale.z / 2f - 2f), ground.localScale.z / 2f - 2f)), Quaternion.identity);
                    }
                }
                if (typeOfGround == TypeOfGround.Circural)
                {
                    Vector2 circle = Random.insideUnitCircle * ground.localScale.x / 2;
                    transform.SetLocalPositionAndRotation(new Vector3(circle.x, 0, circle.y), Quaternion.identity);
                    if (!catcherIsAgent)
                    {
                        circle = Random.insideUnitCircle * ground.localScale.x / 2;
                        targetTransform.SetLocalPositionAndRotation(new Vector3(circle.x, 0, circle.y), Quaternion.identity);
                    }
                }
                if (typeOfGround == TypeOfGround.Other)
                {
                    int count = spawnZone.childCount;
                    int index = Random.Range(0, count);

                    for (int i = 0; i < count; i++)
                    {
                        if (i == index)
                        {
                            Transform child = spawnZone.GetChild(i);
                            transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-(child.localScale.x / 2f - 2f), child.localScale.x / 2f - 2f), 0, Random.Range(-(child.localScale.z / 2f - 2f), child.localScale.z / 2f - 2f)), Quaternion.identity);
                        }
                    }

                    if (!catcherIsAgent)
                    {
                        index = Random.Range(0, count);

                        for (int i = 0; i < count; i++)
                        {
                            if (i == index)
                            {
                                Transform child = spawnZone.GetChild(i);
                                targetTransform.SetLocalPositionAndRotation(new Vector3(Random.Range(-(child.localScale.x / 2f - 2f), child.localScale.x / 2f - 2f), 0, Random.Range(-(child.localScale.z / 2f - 2f), child.localScale.z / 2f - 2f)), Quaternion.identity);
                            }
                        }
                    }
                }
            }
            else
            {
                transform.SetLocalPositionAndRotation(initialPos, Quaternion.identity);
                targetTransform.SetLocalPositionAndRotation(catcherInitialPos, Quaternion.identity);
            }

            catcherLastPos = targetTransform.localPosition;
        }
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
        if (lapCount < maxLaps)
        {
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];

            transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

            currentSpeed = (transform.localPosition - lastPos).magnitude / Time.deltaTime;
            lastPos = transform.localPosition;

            catcherSpeed = (targetTransform.localPosition - catcherLastPos).magnitude / Time.deltaTime;
            catcherLastPos = targetTransform.localPosition;

            AddReward(0.0005f);
        }
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
