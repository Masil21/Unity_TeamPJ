using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // [접근] Player.cs, Enemy.cs, GameManager.cs에서 사용
    public static UIManager Instance;

    public Image[] images;
    public Image[] boomImages;
    public GameObject gameOverPanel;
    public Button retryButton;
    public TextMeshProUGUI scoreText;

    private int score;
    private int boomCount = 0;
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
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryButtonClick);

        UpdateScoreText(force: true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
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

    public void HandlePlayerHit(GameObject playerGo, Vector3 respawnPosition, float respawnDelay)
    {
        if (playerGo == null)
            return;

        bool isGameOver = DecreaseLife();
        playerGo.SetActive(false);

        if (isGameOver)
            return;

        StartCoroutine(RespawnPlayer(playerGo, respawnPosition, respawnDelay));
    }

    IEnumerator RespawnPlayer(GameObject playerGo, Vector3 respawnPosition, float respawnDelay)
    {
        if (playerGo == null)
            yield break;

        yield return new WaitForSeconds(respawnDelay);
        if (playerGo == null)
            yield break;
        if (gameOverPanel != null && gameOverPanel.activeSelf)
            yield break;

        playerGo.transform.position = respawnPosition;
        playerGo.SetActive(true);
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

    // public void AddScoreByEnemyType(Enemy.EnemyType enemyType)
    // {
    //     int delta = enemyType switch
    //     {
    //         Enemy.EnemyType.A => 100,
    //         Enemy.EnemyType.B => 200,
    //         Enemy.EnemyType.C => 300,
    //         _ => 0
    //     };
    //
    //     if (delta == 0)
    //         return;
    //
    //     score += delta;
    //     UpdateScoreText(force: false);
    // }

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
