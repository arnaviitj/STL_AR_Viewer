using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text statusText;
    public Text iterationText;
    public Slider progressSlider;
    public Slider speedSlider;
    public Text speedLabel;
    public Button playPauseButton;
    public Button prevButton;
    public Button nextButton;
    public Button resetButton;

    private bool scrubbing = false;
    private bool ignoreSliderCallback = false;

    void Start()
    {
        // FIX: Removed the AddListener lines here. 
        // Let the Unity Inspector handle the button clicks so they don't fire twice!

        if (speedSlider != null)
        {
            speedSlider.minValue = 0.2f;
            speedSlider.maxValue = 3.0f;
            speedSlider.value = 1.5f;
            speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        }

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            progressSlider.wholeNumbers = false;
            progressSlider.onValueChanged.AddListener(OnProgressScrub);
        }

        ShowAllUI();
        SetStatus("App started - Loading models...");
    }

    void Update()
    {
        if (STLSpawner.Instance == null)
        {
            SetStatus("Waiting for spawner...");
            return;
        }

        if (!STLSpawner.Instance.IsAllLoaded())
        {
            SetStatus("Loading...");
            return;
        }

        if (!STLSpawner.Instance.IsPlaced())
        {
            SetStatus("Tap on a detected plane to place object");
            return;
        }

        int cur = STLSpawner.Instance.GetCurrent();
        int total = STLSpawner.Instance.GetTotal();

        if (iterationText != null)
            iterationText.text = "Iteration " + (cur + 1) + " / " + total;

        if (progressSlider != null && total > 1 && !scrubbing)
        {
            ignoreSliderCallback = true;
            float val = (float)cur / (float)(total - 1);
            progressSlider.value = val;
            ignoreSliderCallback = false;
        }

        if (playPauseButton != null)
        {
            Text btnText = playPauseButton.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = STLSpawner.Instance.IsPlaying() ? "Pause" : "Play";
        }
    }

    void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }

    public void OnPlayPause()
    {
        if (STLSpawner.Instance != null)
            STLSpawner.Instance.TogglePlay();
    }

    public void OnPrev()
    {
        if (STLSpawner.Instance == null) return;

        STLSpawner.Instance.SetPlaying(false);
        STLSpawner.Instance.StepBack();
    }

    public void OnNext()
    {
        if (STLSpawner.Instance == null) return;

        STLSpawner.Instance.SetPlaying(false);
        STLSpawner.Instance.StepForward();
    }

    public void OnReset()
    {
        if (STLSpawner.Instance == null) return;

        STLSpawner.Instance.ResetAll();

        if (progressSlider != null)
        {
            ignoreSliderCallback = true;
            progressSlider.value = 0f;
            ignoreSliderCallback = false;
        }
    }

    public void OnSpeedChanged(float val)
    {
        if (STLSpawner.Instance != null)
            STLSpawner.Instance.SetPlaySpeed(val);

        if (speedLabel != null)
            speedLabel.text = val.ToString("F1") + "x";
    }

    public void OnProgressScrub(float val)
    {
        if (STLSpawner.Instance == null) return;
        if (ignoreSliderCallback) return;

        scrubbing = true;

        int total = STLSpawner.Instance.GetTotal();
        int idx = Mathf.RoundToInt(val * (total - 1));

        STLSpawner.Instance.SetPlaying(false);
        STLSpawner.Instance.SetModel(idx);

        scrubbing = false;
    }

    void ShowAllUI()
    {
        if (statusText != null) statusText.gameObject.SetActive(true);
        if (iterationText != null) iterationText.gameObject.SetActive(true);
        if (progressSlider != null) progressSlider.gameObject.SetActive(true);
        if (speedSlider != null) speedSlider.gameObject.SetActive(true);
        if (speedLabel != null) speedLabel.gameObject.SetActive(true);
        if (playPauseButton != null) playPauseButton.gameObject.SetActive(true);
        if (prevButton != null) prevButton.gameObject.SetActive(true);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        if (resetButton != null) resetButton.gameObject.SetActive(true);
    }
}