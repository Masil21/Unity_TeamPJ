using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image[] images;
    public Image[] boomImages;
    public GameObject gameOverPanel;
    public Button retryButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;

    private int score;
    private int boomCount;
    private int lastScoreShown = int.MinValue;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        AutoFindUIReferences();

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryButtonClick);

        UpdateScoreText(force: true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void AutoFindUIReferences()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null) return;
        Transform ct = canvas.transform;

        if (images == null || images.Length == 0 || images[0] == null)
        {
            images = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Transform t = ct.Find("Life_" + (i + 1));
                if (t != null) images[i] = t.GetComponent<Image>();
            }
        }

        if (boomImages == null || boomImages.Length == 0 || boomImages[0] == null)
        {
            boomImages = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                Transform t = ct.Find("Boom_" + (i + 1));
                if (t != null) boomImages[i] = t.GetComponent<Image>();
            }
        }

        if (gameOverPanel == null)
        {
            Transform t = ct.Find("GameOverPanel");
            if (t != null) gameOverPanel = t.gameObject;
        }

        if (retryButton == null && gameOverPanel != null)
            retryButton = gameOverPanel.GetComponentInChildren<Button>(true);

        if (scoreText == null)
        {
            Transform t = ct.Find("ScoreText");
            if (t != null) scoreText = t.GetComponent<TextMeshProUGUI>();
        }
    }

    void OnDestroy()
    {
        if (retryButton != null)
            retryButton.onClick.RemoveListener(OnRetryButtonClick);

        if (Instance == this)
            Instance = null;
    }

    bool ShowGameOver()
    {
        if (finalScoreText != null)
            finalScoreText.text = score.ToString("#,##0");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        return true;
    }

    public bool DecreaseLife()
    {
        if (images == null || images.Length == 0)
            return ShowGameOver();

        int len = images.Length;
        int visibleLifeCount = 0;
        for (int i = 0; i < len; i++)
        {
            Image img = images[i];
            if (img != null && img.color.a > 0f)
                visibleLifeCount++;
        }

        for (int i = len - 1; i >= 0; i--)
        {
            Image img = images[i];
            if (img == null)
                continue;

            Color color = img.color;
            if (color.a <= 0f)
                continue;

            color.a = 0f;
            img.color = color;

            if (visibleLifeCount == 1)
                return ShowGameOver();
            return false;
        }

        return ShowGameOver();
    }

    public void OnRetryButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void AddScore(int amount)
    {
        if (amount == 0)
            return;
        score += amount;
        UpdateScoreText(force: false);
    }

    void UpdateScoreText(bool force)
    {
        if (scoreText == null)
            return;
        if (!force && score == lastScoreShown)
            return;

        lastScoreShown = score;
        scoreText.text = score.ToString("#,##0");
    }

    public void AddBoom()
    {
        if (boomCount >= 3)
            return;
        boomCount++;
        UpdateBoomUI();
    }

    public bool UseBoom()
    {
        if (boomCount <= 0)
            return false;
        boomCount--;
        UpdateBoomUI();
        return true;
    }

    void UpdateBoomUI()
    {
        if (boomImages == null || boomImages.Length == 0)
            return;

        int len = boomImages.Length;
        for (int i = 0; i < len; i++)
        {
            Image img = boomImages[i];
            if (img == null)
                continue;

            Color color = img.color;
            color.a = i < boomCount ? 1f : 0f;
            img.color = color;
        }
    }
}
