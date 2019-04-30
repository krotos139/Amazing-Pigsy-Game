using UnityEngine;


[ImageEffectAllowedInSceneView]
//[ExecuteInEditMode]
public class CRTEffect : MonoBehaviour
{
    private bool m_Supported = true;
    private Material m_Material;

    [SerializeField]
    private Shader m_Shader = null;

    [Range(0, 1)]
    public float Effect = 0.0f;

    [Range(0, 1)]
    public float Die = 0.0f;
    public float Distortion = 0.1f;
    public float Lines = 1.0f;
    public float OutputGamma = 2.2f;
    public float TextureSize = 768f;

    private float time = 0.0f;
    private float time2 = 0.0f;

    private float EffectTarget = 0.0f;
    private float DieTarget = 0.0f;

    private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        //Die += (DieTarget - Die) * 0.1f;
        Effect += (EffectTarget - Effect) * 0.1f;
        if (EffectTarget>0.0f)
        {
            EffectTarget -= 0.001f;
        } else
        {
            EffectTarget = 0.0f;
        }
        if (m_Material == null)
        {
            m_Material = new Material(m_Shader);

            if (!m_Shader.isSupported)
            {
                Debug.Log("This shader is not supported.");
                enabled = false;
                return;
            }
        }

        if (m_Material != null)
        {
            time += Time.deltaTime;
            if (time > Mathf.PI*2.0f)
            {
                time -= Mathf.PI * 2.0f;
            }
            time2 += Time.deltaTime*1.3f;
            if (time2 > Mathf.PI * 2.0f)
            {
                time2 -= Mathf.PI * 2.0f;
            } 
            m_Material.SetFloat("_Effect", 1.0f-Effect);
            m_Material.SetFloat("_Die", 0.0f);
            m_Material.SetFloat("_Distortion", Mathf.Sin(time)*Distortion);
            m_Material.SetFloat("_lines", (1.0f+Mathf.Sin(time2)) * Lines);
            m_Material.SetFloat("_OutputGamma", OutputGamma);
            m_Material.SetFloat("_HueShift", Mathf.Sin(time) * 0.4f);
                
            m_Material.SetVector("_TextureSize", new Vector2((1.2f + Mathf.Sin(time2)*0.5f) * TextureSize, (1.5f + Mathf.Sin(time2)*0.5f) * TextureSize));
            Graphics.Blit(sourceTexture, destTexture, m_Material);
        }
        else
            Graphics.Blit(sourceTexture, destTexture);
    }

    public void onGlitches()
    {
        EffectTarget = 1.0f;
    }
    public void onDie()
    {
        DieTarget = 1.0f;
    }
    public void onLife()
    {
        DieTarget = 0.0f;
    }
}
