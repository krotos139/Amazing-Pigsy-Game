using UnityEngine;

namespace Demonixis.Effects
{
    [ImageEffectAllowedInSceneView]
    //[ExecuteInEditMode]
    public class CRTEffect : MonoBehaviour
    {
        private bool m_Supported = true;
        private Material m_Material;

        [SerializeField]
        private Shader m_Shader = null;

        public float Distortion = 0.1f;
        public float Lines = 1.0f;
        public float OutputGamma = 2.2f;
        public float TextureSize = 768f;

        private float time = 0.0f;
        private float time2 = 0.0f;

        private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
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
    }
}