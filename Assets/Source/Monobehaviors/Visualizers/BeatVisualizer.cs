using System.Collections.Generic;
using UnityEngine;

public class BeatVisualizer : MonoBehaviour, IAverageSpectrumVisualizer
{
    private const int STACK_SIZE = 120;
    private const float PEAK_THRESHOLD = 1.25f;

    public bool DebugMode;
    public Transform Bar;
    public Transform Threshold;
    public GameObject BeatObject;

    private Queue<float> averages;
    private float upperSampleAverage;
    private int thresholdExceeded;

    public BeatVisualizer()
    {
        averages = new Queue<float>();
    }

    public void Start()
    {
        Bar.gameObject.SetActive(DebugMode);
        Threshold.gameObject.SetActive(DebugMode);
    }

    public void VisualizeValue(float[] values)
    {
        float average = 0f;
        int numValues = 12;// values.Length;
        for (int i = 0; i < numValues; ++i)
        {
            average += values[i];
        }
        average /= numValues;
        averages.Enqueue(average);

        if (averages.Count > STACK_SIZE)
        {
            averages.Dequeue();

            float avg = 0f;
            foreach (float val in averages)
            {
                avg += val;
            }
            avg /= STACK_SIZE;
            upperSampleAverage = avg * PEAK_THRESHOLD;

            Bar.localScale = new Vector3(1f, average, 1f);
            Threshold.localPosition = new Vector3(0f, upperSampleAverage, 0f);

            if (thresholdExceeded > 0)
            {
                thresholdExceeded += 1;
                if (average <= avg)
                {
                    thresholdExceeded = 0;
                    GameObject.Instantiate(BeatObject);
                }

                if (thresholdExceeded > STACK_SIZE)
                {
                    thresholdExceeded = 0;
                }
            }
            else if (average > upperSampleAverage)
            {
                thresholdExceeded = 1;
            }

            //Debug.Log("Average: " + average + ", Threshold: " + upperSampleAverage);
        }
    }
}
