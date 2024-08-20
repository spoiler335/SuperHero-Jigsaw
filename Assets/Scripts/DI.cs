
using UnityEngine;

public class DI
{
    public static readonly DI di = new DI();

    private DI() { }

    public Texture2D cuurentPuzzleTexture { get; private set; }

    public void SetCurrentPuzzleTexture(Texture2D texture) => cuurentPuzzleTexture = texture;

    public void ClearPuzzleTexture() => cuurentPuzzleTexture = null;
}