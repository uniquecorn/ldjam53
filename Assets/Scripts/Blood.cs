using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Blood : MonoBehaviour
{
    public SpriteRenderer sr;
    public float aliveTime;
    private MaterialPropertyBlock _mpb;
    private static readonly int Animate = Shader.PropertyToID("_Animate");

    private void Reset()
    {
        TryGetComponent(out sr);
    }

    void Update()
    {
        if (aliveTime < 1)
        {
            aliveTime += Time.deltaTime;
            if (_mpb == null)
            {
                _mpb = new MaterialPropertyBlock();
            }
            sr.GetPropertyBlock(_mpb);
            _mpb.SetFloat(Animate,aliveTime);
            sr.SetPropertyBlock(_mpb);
        }
    }
}
