
using UnityEngine;

public class DI
{
    public static readonly DI di = new DI();

    private DI() { }

    public Texture2D cuurentPuzzleTexture { get; private set; }
    public int diffculty { get; private set; } = 2;

    public void SetCurrentPuzzleTexture(Texture2D texture) => cuurentPuzzleTexture = texture;

    public void ClearPuzzleTexture() => cuurentPuzzleTexture = null;

    public void SetDifficulty(int diff) => diffculty = diff;
}