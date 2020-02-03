using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager
{
    private const float MAX_SCALE = 2f;
    private const float FADE_THRESHOLD = 0.3f;
    private const float MAX_FADE_PCT = 0.7f;

    private float transitionTimeTotal;
    private float transitionTimeCurrent;
    private Transform background;
    private Material bgMaterial;
    private List<Texture2D> backgroundTextures;
    private int bgIndex;

    public BackgroundManager(List<Texture2D> bgTextures, Transform bg, Material bgMat, float transitionTime)
    {
        transitionTimeTotal = transitionTime;
        transitionTimeCurrent = transitionTime;
        background = bg;
        backgroundTextures = bgTextures;
        bgMaterial = bgMat;
    }

    public void Update(float dt)
    {
        transitionTimeCurrent -= dt;
        float pct = 1f - (transitionTimeCurrent / transitionTimeTotal);
        float scale = (MAX_SCALE - 1f) * pct + 1f;
        background.localScale = new Vector3(scale, scale, scale);

        if (transitionTimeCurrent <= 0f)
        {
            bgIndex = (bgIndex == backgroundTextures.Count - 1) ? 0 : bgIndex + 1;
            bgMaterial.SetTexture("_MainTex", backgroundTextures[bgIndex]);
            transitionTimeCurrent = transitionTimeTotal;
        }

        float fadePct = 1f;
        if (pct < FADE_THRESHOLD)
        {
            float subPct = pct / FADE_THRESHOLD;
            fadePct = (Mathf.Sin((subPct - 0.5f) * Mathf.PI)/2) + 0.5f;
        }
        else if (pct > (1f - FADE_THRESHOLD))
        {
            float subPct = 1f - ((pct - (1f - FADE_THRESHOLD)) / FADE_THRESHOLD);
            fadePct = (Mathf.Sin((subPct - 0.5f) * Mathf.PI) / 2) + 0.5f;
        }
        fadePct *= MAX_FADE_PCT;

        bgMaterial.color = new Color(fadePct, fadePct, fadePct);
    }
}
