using Assets.Scripts.Audio;
using System.Collections.Generic;
using UnityEngine;

public class EngineRealtime : MonoBehaviour
{
    private const float RADIUS = 50f;
    private const int NUM_BARS = 256;
    private const float AMPLITUDE = 1.25f;

    public float RotationRate;
    public Transform VisScaler;
    public Transform Rotater;

    private RealtimeAudio audioSource;
    private List<IVisualizer> visualizers;
    private float[] currentSpectrum;

    private void Awake()
    {
        currentSpectrum = new float[NUM_BARS];
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

        audioSource = new RealtimeAudio(NUM_BARS, ScalingStrategy.Sqrt, (spectrum) =>
        {
            currentSpectrum = spectrum;
        });

        audioSource.StartListen();
    }

    public void Update()
    {
        Rotater.Rotate(new Vector3(0f, RotationRate * Time.deltaTime, 0f));
        for (int i = 0; i < NUM_BARS; ++i)
        {
            float scale = Mathf.Min(currentSpectrum[i] * AMPLITUDE);
            visualizers[i].VisualizeValue(scale);
        }
    }

    public void OnApplicationQuit()
    {
        audioSource.StopListen();
    }
}
