using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiffSelector : MonoBehaviour
{
    [SerializeField] private Slider diffslider;
    [SerializeField] private TextMeshProUGUI diffText;


    private void Awake()
    {
        diffslider.value = diffslider.minValue;
        diffslider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateDiffText();
    }

    public void OnSliderValueChanged(float value)
    {
        UpdateDiffText();
        DI.di.SetDifficulty((int)value);
    }

    private void UpdateDiffText()
    {
        switch ((int)diffslider.value)
        {
            case 2:
                diffText.text = "Very Easy";
                break;
            case 3:
                diffText.text = "Easy";
                break;
            case 4:
                diffText.text = "Medium";
                break;
            case 5:
                diffText.text = "Hard";
                break;
            case 6:
                diffText.text = "Expert";
                break;
        }
    }
}