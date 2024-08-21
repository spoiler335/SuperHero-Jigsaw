using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayUIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI headertext;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        headertext.gameObject.SetActive(false);
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        EventsModel.ON_PUZZLE_BEGIN += OnPuzzleBegin;
        EventsModel.ON_PUZZLE_COMPLETE += OnPuzzleComplete;

        retryButton.onClick.AddListener(OnRetyClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void UnsubscribeEvents()
    {
        EventsModel.ON_PUZZLE_BEGIN -= OnPuzzleBegin;
        EventsModel.ON_PUZZLE_COMPLETE -= OnPuzzleComplete;
    }

    private void OnPuzzleBegin()
    {
        headertext.text = "START";
        headertext.gameObject.SetActive(true);
        StartCoroutine(HideHeaderAfterDelay(2f));
    }

    private IEnumerator HideHeaderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        headertext.gameObject.SetActive(false);
    }

    private void OnPuzzleComplete()
    {
        headertext.text = "PUZZLE COMPLETED";
        headertext.gameObject.SetActive(true);
    }

    private void OnRetyClicked()
    {
        EventsModel.ON_PUZZLE_RETRY?.Invoke();
    }

    private void OnQuitClicked()
    {
        SceneManager.LoadSceneAsync("Init");
    }

    private void OnDestroy() => UnsubscribeEvents();
}
