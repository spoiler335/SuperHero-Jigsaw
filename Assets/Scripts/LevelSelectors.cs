using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectors : MonoBehaviour
{

    [SerializeField] private List<Texture2D> levelSprites = new List<Texture2D>();
    [SerializeField] private Button levelImagePrefab;

    private int textureSize = 512;

    private void Start()
    {
        foreach (var levelTexture in levelSprites)
        {
            Button levelButton = Instantiate(levelImagePrefab, transform);
            levelButton.image.sprite = Sprite.Create(levelTexture, new Rect(0, 0, levelTexture.width, levelTexture.height), Vector2.zero);
        }
    }

}
