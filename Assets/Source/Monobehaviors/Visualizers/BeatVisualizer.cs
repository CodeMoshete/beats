using System.Collections.Generic;
using UnityEngine;

public class BeatVisualizer : MonoBehaviour, IAverageSpectrumVisualizer
{
    private const float STACK_TIME_BEAT = 1f;
    private const float VOLUME_MAX = 1.3f;
    private const float STACK_TIME_VOLUME = 3f;
    private const float PEAK_THRESHULD_SPECTRUM = 1.3f;
    private const float MAX_ALPHA = 0.5f;

    public bool DebugMode;
    public Transform Bar;
    public Transform Threshold;
    public GameObject BeatObject;
    public int SpectrumStartIndex;
    public int SpectrumEndIndex;
    public Color ParticleColor;
    public float PeakThreshold = 1.3f;

    private Queue<float> beatAverages;
    private Queue<float> volumeAverages;
    private float stackTestTimeBeatLeft;
    private float stackTestTimeVolumeLeft;
    private float upperSampleAverage;
    private int thresholdExceeded;

    public BeatVisualizer()
    {
        beatAverages = new Queue<float>();
        volumeAverages = new Queue<float>();
        stackTestTimeBeatLeft = STACK_TIME_BEAT;
        stackTestTimeVolumeLeft = STACK_TIME_VOLUME;
    }

    public void Start()
    {
        Bar.gameObject.SetActive(DebugMode);
        Threshold.gameObject.SetActive(DebugMode);
    }

    public void VisualizeValue(float[] values, float spectrumAverage)
    {
        float selectedSpectrumAverage = 0f;
        int numValues = SpectrumEndIndex - SpectrumStartIndex;
        for (int i = SpectrumStartIndex; i < SpectrumEndIndex; ++i)
        {
            selectedSpectrumAverage += values[i];
        }
        selectedSpectrumAverage /= numValues;
        beatAverages.Enqueue(selectedSpectrumAverage);

        volumeAverages.Enqueue(spectrumAverage);
        stackTestTimeVolumeLeft -= Time.deltaTime;
        float volumeAverage = 0f;
        if (stackTestTimeVolumeLeft <= 0f)
        {
            volumeAverages.Dequeue();

            int stackSize = volumeAverages.Count;

            foreach (float val in volumeAverages)
            {
                volumeAverage += val;
            }
            volumeAverage /= stackSize;
            volumeAverage = Mathf.Min(volumeAverage, VOLUME_MAX);
            volumeAverage = volumeAverage / VOLUME_MAX;
        }

        stackTestTimeBeatLeft -= Time.deltaTime;
        if (stackTestTimeBeatLeft <= 0f)
        {
            beatAverages.Dequeue();

            int stackSize = beatAverages.Count;

            float beatAverage = 0f;
            foreach (float val in beatAverages)
            {
                beatAverage += val;
            }
            beatAverage /= stackSize;
            upperSampleAverage = beatAverage * PeakThreshold;
            float spectrumUpperSampleAverage = spectrumAverage * PEAK_THRESHULD_SPECTRUM;

            Bar.localScale = new Vector3(1f, selectedSpectrumAverage, 1f);
            Threshold.localPosition = new Vector3(0f, upperSampleAverage, 0f);

            if (thresholdExceeded > 0)
            {
                thresholdExceeded += 1;
                if (selectedSpectrumAverage <= beatAverage)
                {
                    thresholdExceeded = 0;
                    GameObject particleObj = GameObject.Instantiate(BeatObject);
                    ParticleSystem.MainModule mainModule = particleObj.GetComponent<ParticleSystem>().main;
                    Color particleColor = ParticleColor;
                    particleColor.a = Mathf.Lerp(0f, MAX_ALPHA, volumeAverage);
                    mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColor);
                }

                if (thresholdExceeded > stackSize)
                {
                    thresholdExceeded = 0;
                }
            }
            else if (selectedSpectrumAverage > upperSampleAverage && selectedSpectrumAverage > spectrumUpperSampleAverage)
            {
                thresholdExceeded = 1;
                FillAveragesWithValue(selectedSpectrumAverage);
            }
        }
    }

    private void FillAveragesWithValue(float value)
    {
        int averagesSize = beatAverages.Count;
        beatAverages.Clear();
        for (int i = 0; i < averagesSize; ++i)
        {
            beatAverages.Enqueue(value);
        }
    }
}
