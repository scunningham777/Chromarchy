using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class ChromaBall : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        Vector2 velocity2d = Random.insideUnitCircle.normalized * speed;
        rb = GetComponent<Rigidbody>();
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
            ChromaTransfer chromaTransfer = GetComponent<ChromaTransfer>();
            chromaBlock.HitBlock(chromaTransfer.ownerID, chromaTransfer.colorToTransfer);
            MMGameEvent.Trigger("ChromaBlockColorChange");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ChromaSword"))
        {
            Reflect(rb, other.transform.forward);
            ChromaTransfer otherChromaTransfer = other.GetComponentInParent<ChromaTransfer>();
            if (otherChromaTransfer != null && otherChromaTransfer.colorToTransfer != null)
            {
                GetComponent<Renderer>().material = otherChromaTransfer.colorToTransfer;
                GetComponent<ChromaTransfer>().colorToTransfer = otherChromaTransfer.colorToTransfer;
                GetComponent<ChromaTransfer>().ownerID = otherChromaTransfer.ownerID;
                if (otherChromaTransfer.colorToTransfer.HasProperty("_Color"))
                {
                    GetComponent<Light>().color = otherChromaTransfer.colorToTransfer.color;
                }
            }
        }
    }

    private void Reflect(Rigidbody rb, Vector3 reflectVector)
    {
        rb.velocity = Vector3.Reflect(rb.velocity, reflectVector);
    }
}
