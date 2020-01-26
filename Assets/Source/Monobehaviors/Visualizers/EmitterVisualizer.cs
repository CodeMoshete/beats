using UnityEngine;

public class EmitterVisualizer : MonoBehaviour, IFullSpectrumVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;

    public ParticleSystem Particles;

    private float baseSpeed;
    private float setScalar;
    private Vector3 startPos;
    private Vector3 startPosNormal;
    private int index;

    public void Initialize(int index)
    {
        this.index = index;
    }
    
    private void Start()
    {
        startPos = transform.localPosition;
        startPosNormal = startPos.normalized;
        baseSpeed = Particles.main.startSpeedMultiplier;
    }

    private void Update()
    {
        if (setScalar > 0)
        {
            setScalar -= (SCALE_DOWN_SPEED * Time.deltaTime);
        }
    }

    public void VisualizeValue(float value)
    {
        ParticleSystem.MainModule mainModule = Particles.main;
        setScalar = Mathf.Max(setScalar, value);
        float adjustedScalar = 
            setScalar * (25f * (Mathf.Cos((Mathf.PI * ((float)index + (float)EngineRealtime.NUM_BARS)) / (float)EngineRealtime.NUM_BARS)) + 15f);
        adjustedScalar = Mathf.Min(EngineRealtime.RADIUS - 5f, adjustedScalar);
        transform.localPosition = startPos - ((adjustedScalar) * startPosNormal);
        mainModule.startSpeedMultiplier = baseSpeed + (value * 100f);
        float colorVal = Mathf.Pow(value, 10);
        Color particleColor = new Color(colorVal, colorVal, 1f - colorVal);
        mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColor);
    }
}
