using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    Camera self;
    RenderTexture tex = null;
    Camera temp;

    Shader outline;
    Material outlineMat;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Camera>();
        temp = new GameObject().AddComponent<Camera>();
        //silhouette = Resources.Load<Shader>("Silhouette");
        outline = Resources.Load<Shader>("Outline");

        //if (silhouette == null)
        //{
        //    Debug.Log("was unable to load silhouette shader");
        //}

        if (outline == null)
        {
            Debug.Log("was unable to load outline shader");
        }

        outlineMat = new Material(outline);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        temp.CopyFrom(self);
        temp.clearFlags = CameraClearFlags.Color;
        temp.backgroundColor = Color.black;

        temp.cullingMask = 1 << LayerMask.NameToLayer("outline");
        
        
        if (tex == null)
        {
            tex = new RenderTexture(source.width, source.height, 0);
            tex.Create();
        } else if (tex.width != source.width || tex.height != source.height)
        {
            tex.Release();
            tex.width = source.width;
            tex.height = source.height;
            tex.Create();
        }

        temp.targetTexture = tex;
        temp.Render();

        Graphics.Blit(tex, destination, outlineMat);

    }
}
