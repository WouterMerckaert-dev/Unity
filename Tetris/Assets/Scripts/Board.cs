using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    [SerializeField] private Text playerScoreText;
    [SerializeField] private Text endPlayerScoreText;
    [SerializeField] private Text winOrLos;
    [SerializeField] private GameObject startScreen; // Canvas for the start screen
    [SerializeField] private GameObject endScreen; // Canvas for the end screen
    [SerializeField] private GameObject gameScreen; // Canvas for the Game
    [SerializeField] private GameObject border; // Border
    [SerializeField] private GameObject grid; // Grid
    [SerializeField] private Text highScoreText; // Voor de highscore in de UI
    [SerializeField] private GameObject ghostPiece; // het ghost-object
    [SerializeField] private RickRollManager rickRollManager;
    [SerializeField] public AudioSource backgroundMusic;



    private int playerScore = 0;
    private int highScore = 0; // Houdt de highscore bij


    private bool doublePointsActive = false;
    private float doublePointsDuration = 10f; // 10 seconds of double points
    private Coroutine doublePointsCoroutine;

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    private float normalStepDelay = 1f;
    private float hardStepDelay = 0.5f;

    public bool isGameOver { get; private set; } // Houdt bij of het spel voorbij is

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        isGameOver = false; // Zet isGameOver op false bij het starten van het spel
        highScore = PlayerPrefs.GetInt("HighScore", 0); // Haal de huidige highscore op
        highScoreText.text = "High Score: " + highScore.ToString(); // Update highscore tekst in de UI
        ShowStartScreen(); // Toon het startscherm
    }

    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowStartScreen()
    {
        startScreen.SetActive(true);
        gameScreen.SetActive(false);
        endScreen.SetActive(false);
        border.SetActive(false);
        grid.SetActive(false);
    }

    public void StartNormalMode()
    {
        activePiece.stepDelay = normalStepDelay; // Set step delay to normal mode
        StartGame();
    }

    public void StartHardMode()
    {
        activePiece.stepDelay = hardStepDelay; // Set step delay to hard mode
        StartGame();
    }

    private void StartGame()
    {
        isGameOver = false; // Zet isGameOver op false bij het starten van een nieuw spel
        ShowGameScreen(); // Toon het spel scherm
        int random = Random.Range(0, tetrominoes.Length);  // Kies een willekeurige Tetromino
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);  
        SpawnPiece(); // Spawn het eerste stuk
        
    }

    private void ShowGameScreen()
    {
        startScreen.SetActive(false); // Hide start screen
        gameScreen.SetActive(true);
        border.SetActive(true);
        grid.SetActive(true);
        rickRollManager.rickRollVideoObject.SetActive(false); // Zorg ervoor dat de RickRoll video hier uitgeschakeld is

    }

    public void SpawnPiece()
    {
        Debug.Log("Spawn");

        int random = Random.Range(0, tetrominoes.Length);  // Kies een willekeurige Tetromino
        TetrominoData data = tetrominoes[random];

        // Initialiseer het nieuwe stuk
        activePiece.Initialize(this, spawnPosition, data);

        // Check of de nieuwe positie van het stuk geldig is
        if (!IsValidPosition(activePiece, spawnPosition))
        {
            Debug.Log("Game over 1");
            GameOver();
            activePiece.gameObject.SetActive(false);
            ghostPiece.SetActive(false);// Eindig het spel als het nieuwe stuk niet kan worden geplaatst
        }
        else
        {
            Set(activePiece);  // Plaats het nieuwe stuk op het bord
        }
    }

    public void GameOver()
    {
        Debug.Log("Game over");

        isGameOver = true; // Zet de isGameOver flag op true
        tilemap.ClearAllTiles();
        Debug.Log(isGameOver);

        // Update de eindscore tekst
        endPlayerScoreText.text = "Your Score: " + playerScore.ToString();


        // Bepaal of de speler gewonnen of verloren heeft
        if (playerScore >= 20000)
        {
            winOrLos.text = "You Won!";
        }
        else
        {
            winOrLos.text = "You Lost!";
        }

        // Highscore logica
        highScore = PlayerPrefs.GetInt("HighScore", 0); // Haal de huidige highscore op
        if (playerScore > highScore)
        {
            highScore = playerScore; // Update highscore
            PlayerPrefs.SetInt("HighScore", highScore); // Sla de nieuwe highscore op
        }

        // Update highscore tekst in de UI
        highScoreText.text = "High Score: " + highScore.ToString();

        endScreen.SetActive(true);
        startScreen.SetActive(false);
        gameScreen.SetActive(false);
        border.SetActive(false);
        grid.SetActive(false);
    }
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesCleared = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++;
            }
            else
            {
                row++;
            }
        }

        // Update score after clearing lines
        if (linesCleared > 0)
        {
            int baseScore = 100 * linesCleared;

            // Activate double points if 2 or more lines are cleared
            if (linesCleared >= 2)
            {
                ActivateDoublePoints();
            }

            // Halve step delay if 3 or more lines are cleared
            if (linesCleared >= 3)
            {
                StartCoroutine(HalveStepDelayFor10Seconds());
            }

            // Add score (double points if active)
            if (doublePointsActive)
            {
                SetPlayerScore(playerScore + (baseScore * 2));
            }
            else
            {
                SetPlayerScore(playerScore + baseScore);
            }
        }
    }

    private void SetPlayerScore(int score)
    {
        playerScore = score;
        playerScoreText.text = score.ToString();

        // zet op 100 om te testen
        if (playerScore == 2430)
        {
            TriggerRickRoll();
        }
    }

    private void TriggerRickRoll()
    {
        Debug.Log("Rickroll triggered!");

        if (rickRollManager != null)
        {
            rickRollManager.PlayRickRoll();
            Debug.Log("Playing Rickroll video");
        }
        else
        {
            Debug.LogWarning("RickRollManager is not set");
        }
    }
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    private void ActivateDoublePoints()
    {
        if (doublePointsCoroutine != null)
        {
            StopCoroutine(doublePointsCoroutine);
        }

        doublePointsActive = true;
        doublePointsCoroutine = StartCoroutine(DoublePointsTimer());
    }

    private IEnumerator DoublePointsTimer()
    {
        yield return new WaitForSeconds(doublePointsDuration);
        doublePointsActive = false;
    }

    private IEnumerator HalveStepDelayFor10Seconds()
    {
        float originalStepDelay = activePiece.stepDelay;
        activePiece.stepDelay = originalStepDelay * 2;

        yield return new WaitForSeconds(10f);

        activePiece.stepDelay = originalStepDelay;
    }
}
