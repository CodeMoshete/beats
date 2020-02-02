using Assets.Scripts.Audio;
using System.Collections.Generic;
using UnityEngine;

public class EngineRealtime : MonoBehaviour
{
    public const int NUM_BARS = 256;
    public const float RADIUS = 90f;
    private const float AMPLITUDE = 1.25f;

    public float RotationRate;
    public Transform VisScaler;
    public Transform VisEmitter;
    public Transform Rotater;
    public Transform VisRotater;
    public List<GameObject> BeatVisualizers;

    private RealtimeAudio audioSource;
    private List<List<IFullSpectrumVisualizer>> fsVisualizers;
    private List<IAverageSpectrumVisualizer> avgVisualizers;
    private float[] currentSpectrum;

    private void Awake()
    {
        currentSpectrum = new float[NUM_BARS];
        fsVisualizers = new List<List<IFullSpectrumVisualizer>>();
        avgVisualizers = new List<IAverageSpectrumVisualizer>();

        //GenerateVisCircle(VisScaler);
        GenerateVisCircle(VisEmitter, true);

        for (int i = 0, count = BeatVisualizers.Count; i < count; ++i)
        {
            IAverageSpectrumVisualizer vis = BeatVisualizers[i].GetComponent<IAverageSpectrumVisualizer>();
            avgVisualizers.Add(vis);
        }

        audioSource = new RealtimeAudio(NUM_BARS, ScalingStrategy.Sqrt, (spectrum) =>
        {
            currentSpectrum = spectrum;
        });

        audioSource.StartListen();
    }

    private void GenerateVisCircle(Transform original, bool rotating = false)
    {
        float stepAmt = (2f * Mathf.PI) / NUM_BARS;
        List<IFullSpectrumVisualizer> visualizerSet = new List<IFullSpectrumVisualizer>();
        for (int i = 0; i < NUM_BARS; ++i)
        {
            Transform parent = rotating ? VisRotater : null;
            GameObject bar = GameObject.Instantiate(original.gameObject, parent);
            float posX = Mathf.Sin(stepAmt * i) * RADIUS;
            float posZ = Mathf.Cos(stepAmt * i) * RADIUS;
            Vector3 pos = new Vector3(posX, 0f, posZ);
            bar.transform.position = pos;
            bar.transform.LookAt(Vector3.zero);
            IFullSpectrumVisualizer visualizer = bar.GetComponent<IFullSpectrumVisualizer>();
            visualizer.Initialize(i);
            visualizerSet.Add(visualizer);
        }
        fsVisualizers.Add(visualizerSet);
        original.gameObject.SetActive(false);
    }

    public void Update()
    {
        //Rotater.Rotate(new Vector3(0f, RotationRate * Time.deltaTime, 0f));
        VisRotater.Rotate(new Vector3(0f, RotationRate * Time.deltaTime, 0f));
        for (int j = 0, count = fsVisualizers.Count; j < count; ++j)
        {
            for (int i = 0; i < NUM_BARS; ++i)
            {
                float scale = Mathf.Min(currentSpectrum[i] * AMPLITUDE);
                fsVisualizers[j][i].VisualizeValue(scale);
            }
        }

        float spectrumAverage = 0f;
        for (int i = 0; i < NUM_BARS; ++i)
        {
            spectrumAverage += currentSpectrum[i];
        }
        spectrumAverage /= (float)NUM_BARS;

        for (int i = 0; i < avgVisualizers.Count; ++i)
        {
            avgVisualizers[i].VisualizeValue(currentSpectrum, spectrumAverage);
        }
    }

    public void OnApplicationQuit()
    {
        audioSource.StopListen();
    }
}
