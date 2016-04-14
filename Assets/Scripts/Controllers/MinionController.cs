﻿using UnityEngine;

public class MinionController : MonoBehaviour
{
    private SpriteRenderer greenGlowRenderer;
    private SpriteRenderer whiteGlowRenderer;
    private SpriteRenderer redGlowRenderer;

    private bool CanTarget = true;
    private bool IsLegendary = false;
    private bool IsTaunt = false;

    public static void AddTo(GameObject gameObject, bool legendary, bool taunt)
    {
        MinionController minionController = gameObject.AddComponent<MinionController>();
        minionController.IsLegendary = legendary;
        minionController.IsTaunt = taunt;

        minionController.Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        whiteGlowRenderer = CreateChildSprite("WhiteGlow", 2);
        greenGlowRenderer = CreateChildSprite("GreenGlow", 1);
        redGlowRenderer = CreateChildSprite("RedGlow", 0);

        UpdateSprites();
    }

    public void Remove()
    {
        whiteGlowRenderer.DisposeSprite();
        Destroy(whiteGlowRenderer);

        greenGlowRenderer.DisposeSprite();
        Destroy(greenGlowRenderer.gameObject);

        redGlowRenderer.DisposeSprite();
        Destroy(redGlowRenderer);
    }

    public void UpdateSprites()
    {
        // Getting the string path to the glows
        string glowString = GetGlowString();

        // Cleaning up the old sprites and textures to avoid memory leaks
        whiteGlowRenderer.DisposeSprite();
        greenGlowRenderer.DisposeSprite();
        redGlowRenderer.DisposeSprite();

        whiteGlowRenderer.sprite = Resources.Load<Sprite>(glowString + "WhiteGlow");
        greenGlowRenderer.sprite = Resources.Load<Sprite>(glowString + "GreenGlow");
        redGlowRenderer.sprite = Resources.Load<Sprite>(glowString + "RedGlow");
    }

    private string GetGlowString()
    {
        string glowString = "Sprites/Glows/Minion_";

        if (IsLegendary)
        {
            glowString += "Legendary_";
        }
        else
        {
            glowString += "Normal_";
        }

        if (IsTaunt)
        {
            glowString += "Taunt_";
        }

        return glowString;
    }
    
    private SpriteRenderer CreateChildSprite(string name, int order)
    {
        // Creating a GameObject to hold the SpriteRenderer
        GameObject glowObject = new GameObject(name);
        glowObject.transform.parent = this.transform;
        glowObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        glowObject.transform.localEulerAngles = Vector3.zero;
        glowObject.transform.localScale = Vector3.one * 2f;

        // Creating the SpriteRenderer and adding it to the GameObject
        SpriteRenderer glowRenderer = glowObject.AddComponent<SpriteRenderer>();
        glowRenderer.sortingLayerName = "Minion";
        glowRenderer.sortingOrder = order;
        glowRenderer.enabled = false;

        return glowRenderer;
    }

    #region Unity Messages

    private void OnMouseEnter()
    {
        whiteGlowRenderer.enabled = true;

        if (InterfaceManager.Instance.IsDragging)
        {
            InterfaceManager.Instance.EnableArrowCircle();
        }
    }

    private void OnMouseDown()
    {
        if (this.CanTarget)
        {
            InterfaceManager.Instance.EnableArrow();
        }
    }

    private void OnMouseUp()
    {
        InterfaceManager.Instance.DisableArrow();
    }

    private void OnMouseExit()
    {
        whiteGlowRenderer.enabled = false;

        if (InterfaceManager.Instance.IsDragging)
        {
            InterfaceManager.Instance.DisableArrowCircle();
        }
    }

    #endregion
}