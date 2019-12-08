using UnityEngine;

public class Engine : MonoBehaviour
{
    private const float AMPLITUDE = 1f;

    public AudioSource Audio;
    public Transform VisScaler;

    private AudioClip clip;
    private float clipDuration;
    private float currentTime;
    private int prevSampleIndex;
    private int frequency;
    private float[] clipData;

    public void Start()
    {
        clip = Audio.clip;
        clipData = new float[clip.samples * clip.channels];
        frequency = clip.frequency;
        clipDuration = (float)((float)(clipData.Length / frequency) / clip.channels);
        clip.GetData(clipData, 0);
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

        float[] curSpectrum = new float[1024];
        Audio.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);

        float targetFrequency = 234f;
        float hertzPerBin = (float)AudioSettings.outputSampleRate / 2f / 1024;
        int targetIndex = Mathf.RoundToInt(targetFrequency / hertzPerBin);

        string outString = "";
        for (int i = targetIndex - 3; i <= targetIndex + 3; i++)
        {
            outString += string.Format("| Bin {0} : {1}Hz : {2} |   ", i, i * hertzPerBin, curSpectrum[i]);
        }
    }
}
