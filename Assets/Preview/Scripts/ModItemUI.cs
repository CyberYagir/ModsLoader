using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private TMP_Text modName, modVer, modData;
    [SerializeField] private Image image;
    [SerializeField] private RawImage icon;
    [SerializeField] private Mod mod;
    private Color startColor, overedColor;
    private bool isOver;
    public void Init(Mod mod)
    {
        this.mod = mod;
        SetData(this.mod);
    }
    public void SetData(Mod mod)
    {
        if (mod == null || mod.data == null){Destroy(gameObject); return;}
        startColor = image.color;
        overedColor = (startColor / 1.2f) + new Color(0, 0, 0, 1);
        modName.text = mod.data.modName;
        modVer.text = mod.data.modVersionData.ToString();
        modData.text = "Assets: " + mod.GetAssetsCount + "\nScenes: " + mod.GetScenesCount;

        if (mod.data.iconData.textureRaw.Length != 0)
        {
            icon.texture = mod.data.iconData.GetTexture();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = overedColor;
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = startColor ;
        isOver = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = (startColor / 1.2f) + new Color(1f,0,0,1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isOver)
        {
            image.color = overedColor;
        }
        else
        {
            image.color = startColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mod.LoadSceneFromAsset(0);
    }

    public void MoveUp()
    {
        ModsManager.Instance.modLoader.loadChain.MoveModUp(mod.data.modName);
    }
    public void MoveDown()
    {
        ModsManager.Instance.modLoader.loadChain.MoveModDown(mod.data.modName);
    }
}
