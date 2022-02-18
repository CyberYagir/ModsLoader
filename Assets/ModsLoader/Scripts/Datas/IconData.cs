using System;
using UnityEngine;

[System.Serializable]
public class IconData
{
    public byte[] textureRaw;
    public Vector2Int size;
    public int textureFormat;


    public Texture2D GetTexture()
    {
        try
        {
            if (textureRaw.Length != 0)
            {
                var tex = new Texture2D(size.x, size.y, (TextureFormat) textureFormat, false);
                tex.LoadRawTextureData(textureRaw);
                tex.Apply();
                return tex;
            }
        }
        catch (Exception e)
        {
        }
        return null;
    }
}