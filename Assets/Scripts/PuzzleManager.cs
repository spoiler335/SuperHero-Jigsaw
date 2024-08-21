using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private Transform puzzleParent;
    [SerializeField] private Transform piecePrefab;

    private int difficulty => DI.di.diffculty;
    private List<Transform> pieces = new List<Transform>();
    private Vector2Int dimentions;
    private Texture2D currTexture;
    private float width;
    private float height;
    private Transform draggingPiece;
    private Vector3 offSet;

    private int correctPieceCount = 0;


    private void Awake()
    {
        EventsModel.ON_PUZZLE_RETRY += OnPuzzleRetry;
        currTexture = DI.di.cuurentPuzzleTexture;
    }

    private void Start() => StartCoroutine(GenratePuzzle());
    private IEnumerator GenratePuzzle()
    {
        yield return null;
        dimentions = GetDimentions(currTexture, difficulty);
        GneratePieces();
        yield return new WaitForEndOfFrame();
        UpdateBorder();
        yield return new WaitForSeconds(1f);
        ScatterPieces();
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
            piece.GetComponent<BoxCollider2D>().enabled = true;
        }

        EventsModel.ON_PUZZLE_BEGIN?.Invoke();
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
                piece.name = $"Piece ({i},{j})";
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                draggingPiece = hit.transform;
                offSet = draggingPiece.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offSet += Vector3.back;
            }
        }

        // When finger is realease stop dragging the piece
        if (draggingPiece && Input.GetMouseButtonUp(0))
        {
            SnapAndDisableIfCorretPosition();
            draggingPiece.position += Vector3.forward;
            draggingPiece = null;
        }
        // set position object to touch position
        if (draggingPiece)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos += offSet;
            draggingPiece.position = newPos;
        }
    }

    private void SnapAndDisableIfCorretPosition()
    {
        int idx = pieces.IndexOf(draggingPiece);

        // The coordinates of the piece in the puzzle.
        int col = idx % dimentions.x;
        int row = idx / dimentions.x;

        // The target position in the non-scaled coordinates.
        Vector2 targetPosition = new((-width * dimentions.x / 2) + (width * col) + (width / 2),
                                     (-height * dimentions.y / 2) + (height * row) + (height / 2));

        // Check if we're in the correct location.
        if (Vector2.Distance(draggingPiece.localPosition, targetPosition) < (width / 2))
        {
            //snap to target position
            draggingPiece.localPosition = targetPosition;

            // Disable collidert to disable Input
            draggingPiece.GetComponent<BoxCollider2D>().enabled = false;

            ++correctPieceCount;
            if (correctPieceCount == pieces.Count)
            {
                Debug.Log("Puzzle solved!");
                EventsModel.ON_PUZZLE_COMPLETE?.Invoke();
            }
        }
    }

    private void OnPuzzleRetry()
    {
        SceneManager.LoadScene("GamePlay");
    }

    private void OnDestroy()
    {
        EventsModel.ON_PUZZLE_RETRY -= OnPuzzleRetry;
    }
}