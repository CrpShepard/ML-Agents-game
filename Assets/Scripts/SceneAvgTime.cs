using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAvgTime : MonoBehaviour
{
    [SerializeField] private GameObject avg1;
    [SerializeField] private GameObject avg2;
    [SerializeField] private GameObject avg3;
    [SerializeField] private GameObject avg4;
    [SerializeField] private GameObject avg5;
    float[] time = new float[5];

    void Update()
    {
        int count = 0;
        float sum = 0;
        time[0] = float.Parse(avg1.GetComponent<TextMesh>().text.Split(' ')[1].Replace('s', Char.MinValue));
        time[1] = float.Parse(avg2.GetComponent<TextMesh>().text.Split(' ')[1].Replace('s', Char.MinValue));
        time[2] = float.Parse(avg3.GetComponent<TextMesh>().text.Split(' ')[1].Replace('s', Char.MinValue));
        time[3] = float.Parse(avg4.GetComponent<TextMesh>().text.Split(' ')[1].Replace('s', Char.MinValue));
        time[4] = float.Parse(avg5.GetComponent<TextMesh>().text.Split(' ')[1].Replace('s', Char.MinValue));

        foreach (var t in time)
        {
            if (t > 0f)
            {
                count++;
                sum += t;
            }
        }

        if (count > 0)
        {
            this.gameObject.GetComponent<TextMesh>().text = "Avg: " + (sum / count) + "s";
        }
        else
        {
            this.gameObject.GetComponent<TextMesh>().text = "Avg: 0s";
        }
    }
}
