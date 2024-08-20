using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectors : MonoBehaviour
{

    [SerializeField] private List<Texture2D> levelSprites = new List<Texture2D>();
    [SerializeField] private Button levelImagePrefab;

    private void Start()
    {
        foreach (var levelTexture in levelSprites)
        {
            Button levelButton = Instantiate(levelImagePrefab, transform);
            levelButton.image.sprite = Sprite.Create(levelTexture, new Rect(0, 0, levelTexture.width, levelTexture.height), Vector2.zero);

            levelButton.onClick.AddListener(() => StartCoroutine(StartPuzzle(levelTexture)));
        }
    }

    private IEnumerator StartPuzzle(Texture2D currentTexture)
    {
        DI.di.SetCurrentPuzzleTexture(currentTexture);
        Debug.Log($"Puzzle started {DI.di.cuurentPuzzleTexture.name}");
        yield return new WaitForSeconds(0.25f); // Wait for 2 seconds
        SceneManager.LoadSceneAsync("GamePlay");
    }
}
