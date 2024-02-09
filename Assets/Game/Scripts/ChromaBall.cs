using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ChromaBall : MonoBehaviour
{
    public float speed = 5f;
    public ParticleSystem hitParticles;
    public ParticleSystem colorChangeParticles;
    Rigidbody rb;
    Renderer ballRenderer;
    ChromaTransfer chromaTransfer;
    Light ballLight;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballRenderer = GetComponent<Renderer>();
        chromaTransfer = GetComponent<ChromaTransfer>();
        ballLight = GetComponent<Light>();

        Vector2 velocity2d = UnityEngine.Random.insideUnitCircle.normalized * speed;
        rb.velocity = new Vector3(velocity2d.x, 0, velocity2d.y);
    }

    void LateUpdate()
    {
        rb.velocity = rb.velocity.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        ChromaBlock chromaBlock = collision.gameObject.GetComponent<ChromaBlock>();
        if (chromaBlock != null)
        {
            chromaBlock.HitBlock(chromaTransfer.ownerID, chromaTransfer.colorToTransfer);
            MMGameEvent.Trigger("ChromaBlockColorChange");
            hitParticles.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ChromaSword"))
        {
            Reflect(rb, other.transform.forward);
            colorChangeParticles.transform.forward = other.transform.forward;
            ChromaTransfer otherChromaTransfer = other.GetComponentInParent<ChromaTransfer>();
            if (otherChromaTransfer != null && otherChromaTransfer.colorToTransfer != null && otherChromaTransfer.ownerID != chromaTransfer.ownerID)
            {
                transferColor(otherChromaTransfer);
            }
        }
    }

    private void transferColor(ChromaTransfer newData)
    {
        ballRenderer.material = newData.colorToTransfer;
        chromaTransfer = newData;
        if (newData.colorToTransfer.HasProperty("_Color"))
        {
            ballLight.color = newData.colorToTransfer.color;
            ballLight.intensity = 2f;
            
            var particlesRenderer = colorChangeParticles.GetComponent<ParticleSystemRenderer>();
            particlesRenderer.trailMaterial = newData.colorToTransfer;
            colorChangeParticles.Play();
        }

    }

    private void Reflect(Rigidbody rb, Vector3 reflectVector)
    {
        // Get the current velocity
        Vector3 currentVelocity = rb.velocity;

        // Generate a small random vector
        Vector3 randomVector = new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            0,
            UnityEngine.Random.Range(-1f, 1f)
        );

        // Normalize and scale the random vector
        randomVector = randomVector.normalized * .05f;

        // Add the random vector to the current velocity
        rb.velocity = Vector3.Reflect(currentVelocity + randomVector, reflectVector);
    }
}
