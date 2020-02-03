using System.Collections.Generic;
using UnityEngine;

public class EmitterVisualizer : MonoBehaviour, IFullSpectrumVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;
    private const float COLOR_KEY_ROTATION_TIME = 10f;

    public ParticleSystem Particles;
    public List<ColorRotationKey> ColorKeys;

    private float baseSpeed;
    private float setScalar;
    private Vector3 startPos;
    private Vector3 startPosNormal;
    private int index;

    private float colorTimeLeft;
    private int colorKeyIndex;
    private ColorRotationKey currentColors;
    private ColorRotationKey nextColors;

    public void Initialize(int index)
    {
        this.index = index;
    }
    
    private void Start()
    {
        startPos = transform.localPosition;
        startPosNormal = startPos.normalized;
        baseSpeed = Particles.main.startSpeedMultiplier;
        SetNextColorRotation();
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
        colorTimeLeft -= Time.deltaTime;

        ParticleSystem.MainModule mainModule = Particles.main;
        setScalar = Mathf.Max(setScalar, value);
        float adjustedScalar = setScalar * 25;
            // setScalar * (25f * (Mathf.Cos((Mathf.PI * ((float)index + (float)EngineRealtime.NUM_BARS)) / (float)EngineRealtime.NUM_BARS)) + 15f);
        adjustedScalar = Mathf.Min(EngineRealtime.RADIUS - 20f, adjustedScalar);
        transform.localPosition = startPos - ((adjustedScalar) * startPosNormal);
        mainModule.startSpeedMultiplier = baseSpeed + (value * 100f);
        float colorVal = Mathf.Pow(value, 10);
        Color particleColor = new Color(colorVal, colorVal, 1f - colorVal);
        mainModule.startColor = new ParticleSystem.MinMaxGradient(particleColor);
    }

    private void SetNextColorRotation()
    {
        colorTimeLeft = COLOR_KEY_ROTATION_TIME;
        colorKeyIndex = colorKeyIndex >= ColorKeys.Count - 1 ? 0 : colorKeyIndex + 1;
        int nextKeyIndex = colorKeyIndex >= ColorKeys.Count - 1 ? 0 : colorKeyIndex + 1;
        currentColors = ColorKeys[colorKeyIndex];
        nextColors = ColorKeys[nextKeyIndex];
    }
}
