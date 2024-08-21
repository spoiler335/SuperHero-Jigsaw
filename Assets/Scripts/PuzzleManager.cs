using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        yield return null;
        dimentions = GetDimentions(currTexture, difficulty);
        GneratePieces();
        yield return new WaitForSeconds(1);
        ScatterPieces();
        yield return new WaitForEndOfFrame();
        UpdateBorder();
    }

    private void UpdateBorder()
    {
        LineRenderer lr = puzzleParent.GetComponent<LineRenderer>();
        Assert.IsNotNull(lr, "Please provide a line renderer");

        float halfWidth = (width * dimentions.x) / 2f;
        float halfHeight = (height * dimentions.y) / 2f;

        lr.positionCount = 4;
        lr.SetPosition(0, new Vector3(-halfWidth, halfHeight, 0));
        lr.SetPosition(1, new Vector3(halfWidth, halfHeight, 0));
        lr.SetPosition(2, new Vector3(halfWidth, -halfHeight, 0));
        lr.SetPosition(3, new Vector3(-halfWidth, -halfHeight, 0));

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        lr.enabled = true;
    }

    private void ScatterPieces()
    {
        float orthoHeight = Camera.main.orthographicSize;
        float scrrenAspect = (float)Screen.width / Screen.height;
        float orthoWidth = orthoHeight * scrrenAspect;

        // To ensure pieces are not near edjes
        float pWidth = width * puzzleParent.localScale.x;
        float pHeight = height * puzzleParent.localScale.y;

        orthoHeight -= pHeight;
        orthoWidth -= pWidth;

        foreach (var piece in pieces)
        {
            float x = Random.Range(-orthoWidth, orthoWidth);
            float y = Random.Range(-orthoHeight, orthoHeight);
            piece.position = puzzleParent.position + new Vector3(x, y, -1);
        }
    }

    private void GneratePieces()
    {
        float aspect = (float)currTexture.width / currTexture.height;
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
    }

    private Vector2Int GetDimentions(Texture2D texture, int diff)
    {
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
