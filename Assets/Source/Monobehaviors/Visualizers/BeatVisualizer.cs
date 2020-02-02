using System.Collections.Generic;
using UnityEngine;

public class BeatVisualizer : MonoBehaviour, IAverageSpectrumVisualizer
{
    private const int STACK_SIZE = 80;
    private const float PEAK_THRESHOLD = 1.25f;
    private const float PEAK_THRESHULD_SPECTRUM = 1.3f;

    public bool DebugMode;
    public Transform Bar;
    public Transform Threshold;
    public GameObject BeatObject;
    public int SpectrumStartIndex;
    public int SpectrumEndIndex;
    public Color ParticleColor;

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

    public void VisualizeValue(float[] values, float spectrumAverage)
    {
        float average = 0f;
        int numValues = SpectrumEndIndex - SpectrumStartIndex;
        for (int i = SpectrumStartIndex; i < SpectrumEndIndex; ++i)
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
            float spectrumUpperSampleAverage = spectrumAverage * PEAK_THRESHULD_SPECTRUM;

            Bar.localScale = new Vector3(1f, average, 1f);
            Threshold.localPosition = new Vector3(0f, upperSampleAverage, 0f);

            if (thresholdExceeded > 0)
            {
                thresholdExceeded += 1;
                if (average <= avg)
                {
                    thresholdExceeded = 0;
                    GameObject particleObj = GameObject.Instantiate(BeatObject);
                    ParticleSystem.MainModule mainModule = particleObj.GetComponent<ParticleSystem>().main;
                    Color particleColor = ParticleColor;
                    mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColor);
                }

                if (thresholdExceeded > STACK_SIZE)
                {
                    thresholdExceeded = 0;
                }
            }
            else if (average > upperSampleAverage && average > spectrumUpperSampleAverage)
            {
                thresholdExceeded = 1;
                FillAveragesWithValue(average);
            }

            //Debug.Log("Average: " + average + ", Threshold: " + upperSampleAverage);
        }
    }

    private void FillAveragesWithValue(float value)
    {
        averages.Clear();
        for (int i = 0; i < STACK_SIZE; ++i)
        {
            averages.Enqueue(value);
        }
    }
}
