using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Processor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("_Unprocessed"))
        {
            return;
        }
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.spritePixelsPerUnit = 150;
    }
}