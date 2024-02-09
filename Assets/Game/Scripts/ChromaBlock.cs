using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ChromaBlock : MonoBehaviour
{
    public float spreadRadius = 3f;
    public ParticleSystem spreadParticles;
    public MMFeedbacks hitFeedback;
    string ownerID = null;
    MeshRenderer colorRenderer;
    Light chromaLight;
    Color defaultLightColor = new(0.009951297f, 0.3137255f, 0f);
    Color defaultSpreadParticleColor = new(.6f, .6f, .6f, .3f);
    // Start is called before the first frame update
    void Start()
    {
        colorRenderer = GetComponentInChildren<MeshRenderer>();
        chromaLight = GetComponentInChildren<Light>();
    }

    public void HitBlock(string hittingPlayerID, Material color, bool isSpreadHit = false)
    {
        colorRenderer.material = color;
        var particlesMain = spreadParticles.main;

        if (color.HasProperty("_Color"))
        {
            chromaLight.color = color.color;
            particlesMain.startColor = colorRenderer.material.color;
        } 
        else 
        {
            chromaLight.color = defaultLightColor;
            particlesMain.startColor = defaultSpreadParticleColor;
        }

        if (hittingPlayerID == ownerID && !isSpreadHit)
        {
            SpreadColor();
        }
        else 
        {
            ownerID = hittingPlayerID;
        }

        if (!isSpreadHit)
        {
            hitFeedback.PlayFeedbacks();
        }
    }

    public string GetOwnerID()
    {
        return ownerID;
    }

    private void SpreadColor()
    {
        spreadParticles.Play();

        Collider[] colliders = Physics.OverlapSphere(transform.position, spreadRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<ChromaBlock>(out var chromaBlock))
            {
                chromaBlock.HitBlock(ownerID, colorRenderer.material, true);
            }
        }
    }
}
