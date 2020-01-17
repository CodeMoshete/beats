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
    public Transform VisEmitter;
    public Transform Rotater;
    public Transform VisRotater;

    private RealtimeAudio audioSource;
    private List<IVisualizer> visualizers;
    private float[] currentSpectrum;

    private void Awake()
    {
        currentSpectrum = new float[NUM_BARS];
        visualizers = new List<IVisualizer>();

        //GenerateVisCircle(VisScaler);
        GenerateVisCircle(VisEmitter);

        audioSource = new RealtimeAudio(NUM_BARS, ScalingStrategy.Sqrt, (spectrum) =>
        {
            currentSpectrum = spectrum;
        });

        audioSource.StartListen();
    }

    private void GenerateVisCircle(Transform original)
    {
        float stepAmt = (2f * Mathf.PI) / NUM_BARS;
        for (int i = 0; i < NUM_BARS; ++i)
        {
            GameObject bar = GameObject.Instantiate(original.gameObject, VisRotater);
            float posX = Mathf.Sin(stepAmt * i) * RADIUS;
            float posZ = Mathf.Cos(stepAmt * i) * RADIUS;
            Vector3 pos = new Vector3(posX, 0f, posZ);
            bar.transform.position = pos;
            bar.transform.LookAt(Vector3.zero);
            visualizers.Add(bar.GetComponent<IVisualizer>());
        }
        original.gameObject.SetActive(false);
    }

    public void Update()
    {
        //Rotater.Rotate(new Vector3(0f, RotationRate * Time.deltaTime, 0f));
        VisRotater.Rotate(new Vector3(0f, RotationRate * Time.deltaTime, 0f));
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
