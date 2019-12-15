using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private const int NUM_BARS = 16;
    private const int NUM_SAMPLES = 1024;
    private const float AMPLITUDE = 100f;
    private const float POS_OFFSET = 1.5f;

    public AudioSource Audio;
    public Transform VisScaler;

    private AudioClip clip;
    private float clipDuration;
    private float currentTime;
    private int prevSampleIndex;
    private int frequency;
    private float[] clipData;

    private List<Transform> bars;

    public void Start()
    {
        clip = Audio.clip;
        clipData = new float[clip.samples * clip.channels];
        frequency = clip.frequency;
        clipDuration = (float)((float)(clipData.Length / frequency) / clip.channels);
        clip.GetData(clipData, 0);

        bars = new List<Transform>();
        for (int i = 0; i < NUM_BARS; ++i)
        {
            GameObject bar = GameObject.Instantiate(VisScaler.gameObject);
            bar.transform.Translate(new Vector3(POS_OFFSET * (i + 1), 0f, 0f));
            bars.Add(bar.transform);
        }
        VisScaler.gameObject.SetActive(false);
    }

    public void Update()
    {
        //int numSamplesThisFrame = Mathf.Min(clipData.Length - prevSampleIndex, Mathf.RoundToInt(Time.deltaTime * frequency) * 2);
        //float avgSample = 0f;
        //for (int i = prevSampleIndex, end = prevSampleIndex + numSamplesThisFrame; i < end; ++i)
        //{
        //    avgSample += (clipData[i] + 1f) / 2f;
        //}
        //avgSample /= Mathf.Max(numSamplesThisFrame, 1);
        //VisScaler.localScale = new Vector3(1f, avgSample * AMPLITUDE, 1f);

        //prevSampleIndex += numSamplesThisFrame;

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
            bars[i].localScale = new Vector3(1f, avgVal * AMPLITUDE, 1f);
        }

            //float targetFrequency = 234f;
            //float hertzPerBin = (float)AudioSettings.outputSampleRate / 2f / 1024;
            //int targetIndex = Mathf.RoundToInt(targetFrequency / hertzPerBin);

            //string outString = "";
            //for (int i = targetIndex - 3; i <= targetIndex + 3; i++)
            //{
            //    outString += string.Format("| Bin {0} : {1}Hz : {2} |   ", i, i * hertzPerBin, curSpectrum[i]);
            //}
        }
}
