using System.Collections.Generic;
using UnityEngine;

public class VolumeVisualizer : MonoBehaviour, IAverageSpectrumVisualizer
{
    private const float STACK_TIME = 3f;
    private const float VOLUME_MAX = 1.3f;
    private const float SPEED_MIN = 300f;
    private const float SPEED_MAX = 800f;
    private const float SIZE_MIN = 1f;
    private const float SIZE_MAX = 2f;
    private const float RATE_MIN = 0f;
    private const float RATE_MAX = 300f;

    private Queue<float> averages;
    private float stackTestTimeLeft;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.MainModule mainModule;

    void Start()
    {
        averages = new Queue<float>();
        stackTestTimeLeft = STACK_TIME;
        ParticleSystem particleComp = gameObject.GetComponent<ParticleSystem>();
        mainModule = particleComp.main;
        emissionModule = particleComp.emission;
    }

    public void VisualizeValue(float[] values, float spectrumAverage)
    {
        averages.Enqueue(spectrumAverage);
        if (stackTestTimeLeft <= 0f)
        {
            averages.Dequeue();
        }
        stackTestTimeLeft -= Time.deltaTime;

        float avg = 0f;
        foreach (float val in averages)
        {
            avg += val;
        }
        avg /= averages.Count;
        float pct = Mathf.Pow(Mathf.Min(avg / VOLUME_MAX, 1f), 6f);
        float speed = Mathf.Lerp(SPEED_MIN, SPEED_MAX, pct);
        float size = Mathf.Lerp(SIZE_MIN, SIZE_MAX, pct);
        float rate = Mathf.Lerp(RATE_MIN, RATE_MAX, pct);
        mainModule.startSpeed = speed;
        mainModule.startSize = size;
        emissionModule.rateOverTime = rate;
    }
}
