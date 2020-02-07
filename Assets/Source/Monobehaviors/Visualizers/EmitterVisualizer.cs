using System.Collections.Generic;
using UnityEngine;

public class EmitterVisualizer : MonoBehaviour, IFullSpectrumVisualizer
{
    private const float SCALE_DOWN_SPEED = 6f;
    private const float COLOR_KEY_ROTATION_TIME = 10f;
    private const float COLOR_VALUE_MAX = 1.3f;
    private const float DEAD_ZONE = 0.1f;
    private const float DEAD_ZONE_FADE_IN_TIME = 1f;
    private const float DEAD_ZONE_FADE_OUT_TIME = 3f;

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
    private float deadZonePct;

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
        float dt = Time.deltaTime;
        colorTimeLeft -= dt;
        if (colorTimeLeft <= 0f)
        {
            SetNextColorRotation();
        }
        float colorPct = Mathf.Pow(1f - (colorTimeLeft / COLOR_KEY_ROTATION_TIME), 10f);
        Color currentInnerColor = Color.Lerp(currentColors.InnerColor, nextColors.InnerColor, colorPct);
        Color currentOuterColor = Color.Lerp(currentColors.OuterColor, nextColors.OuterColor, colorPct);
        float colorValueModifier = Mathf.Min(value, COLOR_VALUE_MAX) / COLOR_VALUE_MAX;
        Color finalColor = Color.Lerp(currentOuterColor, currentInnerColor, colorValueModifier);

        //float deadZonePct = Mathf.Min(1f, value / DEAD_ZONE);
        if (value < DEAD_ZONE)
        {
            deadZonePct = Mathf.Clamp(deadZonePct - (dt / DEAD_ZONE_FADE_OUT_TIME), 0f, 1f);
        }
        else
        {
            deadZonePct = Mathf.Clamp(deadZonePct + (dt / DEAD_ZONE_FADE_IN_TIME), 0f, 1f);
        }

        finalColor.a *= deadZonePct;

        ParticleSystem.MainModule mainModule = Particles.main;
        setScalar = Mathf.Max(setScalar, value);
        float adjustedScalar = setScalar * 25;
            // setScalar * (25f * (Mathf.Cos((Mathf.PI * ((float)index + (float)EngineRealtime.NUM_BARS)) / (float)EngineRealtime.NUM_BARS)) + 15f);
        adjustedScalar = Mathf.Min(EngineRealtime.RADIUS - 20f, adjustedScalar);
        transform.localPosition = startPos - ((adjustedScalar) * startPosNormal);
        mainModule.startSpeedMultiplier = baseSpeed + (value * 100f);
        mainModule.startColor = new ParticleSystem.MinMaxGradient(finalColor);
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
