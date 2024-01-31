using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaBlock : MonoBehaviour
{
    public float spreadRadius = 3f;
    string ownerID = null;
    MeshRenderer colorRenderer;
    // Start is called before the first frame update
    void Start()
    {
        colorRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void HitBlock(string hittingPlayerID, Material color, Boolean canSpread = true)
    {
        colorRenderer.material = color;
        if (hittingPlayerID == ownerID && canSpread)
        {
            SpreadColor();
        }
        else 
        {
            ownerID = hittingPlayerID;
        }
    }

    public string GetOwnerID()
    {
        return ownerID;
    }

    private void SpreadColor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spreadRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<ChromaBlock>(out var chromaBlock))
            {
                chromaBlock.HitBlock(ownerID, colorRenderer.material, false);
            }
        }
    }
}