using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject blockPrefab;
    public float moveSpeed = 2.5f;
    
    [Header("Block Properties")]
    public float blockHeight = 0.2f;
    public float initBlockWidth = 2f;
    public Vector3 startPosition = new Vector3(0, 0, 0);
    public GameObject perfectPlacementEffect;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    
    private GameObject currentBlock;
    private GameObject lastBlock;
    private Vector3 lastBlockPosition;
    private float currentBlockWidth;
    private int score;
    private bool isGameOver;
    private ColorManager colorManager;
    private int perfectStreak;
    private int imperfectStreak;
    private bool isGameActive;
    private int highScore;
    
    public int CurrentScore => score;
    public int HighScore => highScore;

    private void Awake()
    {
        Instance = this;
        currentBlockWidth = initBlockWidth;
        lastBlockPosition = startPosition;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUIText();
    }

    void Start()
    {
        ColorManager.Instance.GenerateNewPalette();
        SpawnNewBlock();
    }

    void Update()
    {
        if (!isGameActive) return;
        if (!isGameOver && Input.GetMouseButtonDown(0)) PlaceBlock();
    }

    void SpawnNewBlock()
    {
        float spawnX = score % 2 == 0 ? -5f : 5f;
        float spawnY = lastBlockPosition.y + blockHeight;
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        currentBlock.transform.localScale = new Vector3(currentBlockWidth, blockHeight, currentBlockWidth);

        Rigidbody rb = currentBlock.GetComponent<Rigidbody>() ?? currentBlock.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        BlockController blockController = currentBlock.GetComponent<BlockController>();
        blockController.SetDirection(score % 2 == 0 ? 1 : -1);

        MeshRenderer renderer = currentBlock.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material.color = ColorManager.Instance.GetNextBlockColor();
        }
    }

    void PlaceBlock()
    {
        if (currentBlock == null) return;

        currentBlock.GetComponent<BlockController>().Stop();
        Rigidbody rb = currentBlock.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        float currentX = currentBlock.transform.position.x;
        float targetX = lastBlockPosition.x;
        float distanceFromTarget = currentX - targetX;

        float tolerance = 0.1f;
        if (Mathf.Abs(distanceFromTarget) < tolerance)
        {
            HandlePerfectPlacement(currentX, targetX);
        }
        else
        {
            HandleImperfectPlacement(currentX, targetX, distanceFromTarget);
        }

        score++;
        UpdateScore();
        lastBlockPosition = currentBlock.transform.position;
        lastBlock = currentBlock;
        
        CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.MoveUp(blockHeight);
        }

        SpawnNewBlock();
    }

    void HandlePerfectPlacement(float currentX, float targetX)
    {
        currentBlock.transform.position = new Vector3(targetX, currentBlock.transform.position.y, 0);
        perfectStreak++;
        imperfectStreak = 0;
        
        score += 2;
        
        if (perfectStreak >= 2)
        {
            currentBlockWidth += 0.1f;
            currentBlockWidth = Mathf.Min(currentBlockWidth, 2f);
            perfectStreak = 0;
        }

        if (perfectPlacementEffect != null)
        {
            SpawnPerfectEffect();
        }
    }

    void HandleImperfectPlacement(float currentX, float targetX, float distanceFromTarget)
    {
        imperfectStreak++;
        perfectStreak = 0;
        
        if (imperfectStreak >= 5)
        {
            currentBlockWidth += 0.1f;
            currentBlockWidth = Mathf.Min(currentBlockWidth, 2f);
            imperfectStreak = 0;
        }
        
        float hangingAmount = Mathf.Abs(distanceFromTarget);
    
        if (hangingAmount >= currentBlockWidth)
        {
            score += 0;
            UpdateScore();
            GameOver();
            return;
        }
    
        float successfulWidth = currentBlockWidth - hangingAmount;
        
        if (distanceFromTarget > 0)
        {
            CreateFallingPiece(hangingAmount, currentX + (successfulWidth / 2));
        }
        else
        {
            CreateFallingPiece(hangingAmount, currentX - (successfulWidth / 2));
        }
    
        currentBlock.transform.position = new Vector3(targetX, currentBlock.transform.position.y, 0);
        currentBlock.transform.localScale = new Vector3(successfulWidth, blockHeight, currentBlockWidth);
        currentBlockWidth = successfulWidth;
    }

    void SpawnPerfectEffect()
    {
        float halfHeight = currentBlock.transform.localScale.y * 0.5f;
        Vector3 effectPos = currentBlock.transform.position - new Vector3(0, halfHeight, 0);
        GameObject effect = Instantiate(perfectPlacementEffect, effectPos, Quaternion.identity);

        Color blockColor = currentBlock.GetComponent<MeshRenderer>().material.color;
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = blockColor;

            float blockWidth = currentBlock.transform.localScale.x;
            var shape = ps.shape;
            shape.radius = blockWidth * 0.5f;
        }
    }

    private void UpdateScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (highScoreText != null) highScoreText.text = $"Best: {highScore}";
    }

    void CreateFallingPiece(float width, float xPosition)
    {
        GameObject fallingPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fallingPiece.transform.localScale = new Vector3(width, blockHeight, currentBlockWidth);
        fallingPiece.transform.position = new Vector3(xPosition, currentBlock.transform.position.y, 0);
        
        Rigidbody rb = fallingPiece.AddComponent<Rigidbody>();
        rb.useGravity = true;
        
        StartCoroutine(FadeOutAndDestroy(fallingPiece, 2f));
    }

    IEnumerator FadeOutAndDestroy(GameObject obj, float duration)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if(renderer != null && renderer.material.HasProperty("_Color"))
        {
            Color initialColor = renderer.material.color;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(initialColor.a, 0f, elapsed / duration);
                Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                renderer.material.color = newColor;
                yield return null;
            }
        }
        Destroy(obj);
    }

    void GameOver()
    {
        isGameOver = true;
        Invoke(nameof(RestartGame), 1f);
    }

    void RestartGame()
    {
        score = 0;
        currentBlockWidth = initBlockWidth;
        lastBlockPosition = startPosition;
        isGameOver = false;
        perfectStreak = 0;
        imperfectStreak = 0;
        UpdateUIText();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetGameActive(bool active) => isGameActive = active;
}