using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private const float RADIUS = 15f;
    private const int NUM_BARS = 64;
    private const int NUM_SAMPLES = 1024;
    private const float AMPLITUDE = 100f;
    private const float MAX_AMPLITUDE = 3.5f;
    private const float POS_OFFSET = 1.5f;

    public float RotationRate;
    public AudioSource Audio;
    public Transform VisScaler;
    public Transform VisEmitter;
    public Transform Rotater;

    private AudioClip clip;
    private float clipDuration;
    private float currentTime;
    private int prevSampleIndex;
    private int frequency;
    private float[] clipData;

    private List<IVisualizer> visualizers;

    public void Start()
    {
        clip = Audio.clip;
        clipData = new float[clip.samples * clip.channels];
        frequency = clip.frequency;
        clipDuration = (float)((float)(clipData.Length / frequency) / clip.channels);
        clip.GetData(clipData, 0);

        visualizers = new List<IVisualizer>();
        float stepAmt = (2f * Mathf.PI) / NUM_BARS;
        for (int i = 0; i < NUM_BARS; ++i)
        {
            GameObject bar = GameObject.Instantiate(VisScaler.gameObject);
            float posX = Mathf.Sin(stepAmt * i) * RADIUS;
            float posZ = Mathf.Cos(stepAmt * i) * RADIUS;
            Vector3 pos = new Vector3(posX, 0f, posZ);
            bar.transform.position = pos;
            bar.transform.LookAt(Vector3.zero);
            visualizers.Add(bar.GetComponent<IVisualizer>());
        }
        VisScaler.gameObject.SetActive(false);
    }

    public void Update()
    {
        float[] curSpectrum = new float[NUM_SAMPLES];
        Audio.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);

        int samplesPerBar = (NUM_SAMPLES / NUM_BARS) / 16;
        for (int i = 0; i < NUM_BARS; ++i)
        {
            float avgVal = 0f;
            for (int j = 0; j < samplesPerBar; ++j)
            {
                int currentIndex = (i * samplesPerBar) + j;
                avgVal += curSpectrum[currentIndex];
            }
            avgVal /= samplesPerBar;
            float scale = Mathf.Min(MAX_AMPLITUDE, avgVal * AMPLITUDE);
            visualizers[i].VisualizeValue(scale);
        }

        Rotater.Rotate(new Vector3(0f, RotationRate * Time.deltaTime, 0f));
    }
}
