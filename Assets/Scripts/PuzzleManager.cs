using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4;
    [SerializeField] private Transform puzzleParent;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces = new List<Transform>();
    private Vector2Int dimentions;
    private Texture2D currTexture;
    private float width;
    private float height;


    private void Awake() => currTexture = DI.di.cuurentPuzzleTexture;

    private void Start() => StartCoroutine(GenratePuzzle());
    private IEnumerator GenratePuzzle()
    {
        Debug.Log($"Test-1");
        yield return null;
        dimentions = GetDimentions(currTexture, difficulty);
        // yield return new WaitUntil(() => (dimentions.x > 0 && dimentions.y > 0));

        Debug.Log($"Test-3");

        float aspect = (float)currTexture.width / currTexture.height;

        Debug.Log($"Test-4");
        height = 1f / dimentions.y;
        width = aspect / dimentions.x;

        for (int i = 0; i < dimentions.y; i++)
        {
            for (int j = 0; j < dimentions.x; j++)
            {
                Transform piece = Instantiate(piecePrefab, puzzleParent);
                piece.localPosition = new Vector3(
          (-width * dimentions.x / 2) + (width * j) + (width / 2),
          (-height * dimentions.y / 2) + (height * i) + (height / 2),
          -1);
                piece.localScale = new Vector3(width, height, 1f);
                piece.name = $"Piece {(i * dimentions.x) + j}";
                pieces.Add(piece);

                // Assign the correct part of the texture for this jigsaw piece
                // // We need our width and height both to be normalised between 0 and 1 for the UV.
                float width1 = 1f / dimentions.x;
                float height1 = 1f / dimentions.y;
                // // UV coord order is anti-clockwise: (0, 0), (1, 0), (0, 1), (1, 1)
                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * j, height1 * i);
                uv[1] = new Vector2(width1 * (j + 1), height1 * i);
                uv[2] = new Vector2(width1 * j, height1 * (i + 1));
                uv[3] = new Vector2(width1 * (j + 1), height1 * (i + 1));
                // // Assign our new UVs to the mesh.
                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;
                // // Update the texture on the piece
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", currTexture);
            }
        }
        // CreateJigsawPieces(currTexture);

        Debug.Log($"Test-5");
    }

    void CreateJigsawPieces(Texture2D jigsawTexture)
    {
        // Calculate piece sizes based on the dimentions.
        height = 1f / dimentions.y;
        float aspect = (float)jigsawTexture.width / jigsawTexture.height;
        width = aspect / dimentions.x;

        for (int row = 0; row < dimentions.y; row++)
        {
            for (int col = 0; col < dimentions.x; col++)
            {
                // Create the piece in the right location of the right size.
                Transform piece = Instantiate(piecePrefab, piecePrefab);
                piece.localPosition = new Vector3(
                  (-width * dimentions.x / 2) + (width * col) + (width / 2),
                  (-height * dimentions.y / 2) + (height * row) + (height / 2),
                  -1);
                piece.localScale = new Vector3(width, height, 1f);

                // // We don't have to name them, but always useful for debugging.
                piece.name = $"Piece {(row * dimentions.x) + col}";
                pieces.Add(piece);

                // // Assign the correct part of the texture for this jigsaw piece
                // // We need our width and height both to be normalised between 0 and 1 for the UV.
                // float width1 = 1f / dimentions.x;
                // float height1 = 1f / dimentions.y;
                // // UV coord order is anti-clockwise: (0, 0), (1, 0), (0, 1), (1, 1)
                // Vector2[] uv = new Vector2[4];
                // uv[0] = new Vector2(width1 * col, height1 * row);
                // uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                // uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                // uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));
                // // Assign our new UVs to the mesh.
                // Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                // mesh.uv = uv;
                // // Update the texture on the piece
                // piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);
            }
        }
    }

    private Vector2Int GetDimentions(Texture2D texture, int diff)
    {
        Debug.Log($"Test-2");
        Vector2Int dimentions = Vector2Int.zero;

        if (texture.width > texture.height)
        {
            dimentions.x = (diff * texture.width) / texture.height;
            dimentions.y = diff;
        }
        else
        {
            dimentions.x = diff;
            dimentions.y = (diff * texture.height) / texture.width;
        }

        return dimentions;
    }
}
