using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenEffect : MonoBehaviour
{
    public float intensity;
    public Texture DisplacementTex;
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Hidden/NewImageEffectShader"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_Strength", intensity);
        material.SetTexture("_DisplacementTex", DisplacementTex);
        Graphics.Blit(source, destination, material);
    }
}
