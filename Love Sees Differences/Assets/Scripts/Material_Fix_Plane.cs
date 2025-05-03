using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Fix_Plane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Vector3 scale = transform.lossyScale;

        renderer.material = new Material(renderer.material);

        float tileSize = 0.2f; // world units per tile
        //float xRescale = (scale.x > scale.z) ? scale.x / tileSize : scale.z / tileSize;
        renderer.material.mainTextureScale = new Vector2(scale.x / tileSize, scale.z / tileSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
