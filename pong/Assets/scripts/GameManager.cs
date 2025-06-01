using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private Paddle playerPaddle;
    [SerializeField] private Paddle computerPaddle;
    [SerializeField] private Text playerScoreText;
    [SerializeField] private Text computerScoreText;
    [SerializeField] private GameObject endScreen;  // Verwijzing naar het eindscherm
    [SerializeField] private Text winnerText;       // Tekst op het eindscherm
    [SerializeField] private Button restartButton;  // Herstartknop

    private int playerScore;
    private int computerScore;
    private int winningScore = 5;  // De score waarbij het spel stopt

    private void Start()
    {
        endScreen.SetActive(false);  // Zorg ervoor dat het eindscherm verborgen is aan het begin
        NewGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        SetPlayerScore(0);
        SetComputerScore(0);
        NewRound();

        // Verberg het eindscherm bij een nieuwe game
        endScreen.SetActive(false);
        ball.gameObject.SetActive(true);  // Zorg ervoor dat de bal actief is

        // Maak de paddles weer zichtbaar
        playerPaddle.gameObject.SetActive(true);
        computerPaddle.gameObject.SetActive(true);
    }

    public void NewRound()
    {
        playerPaddle.ResetPosition();
        computerPaddle.ResetPosition();
        ball.ResetPosition();  // Reset de bal naar het midden

        CancelInvoke();
        Invoke(nameof(StartRound), 1f);
    }

    private void StartRound()
    {
        ball.AddStartingForce();
    }

    public void OnPlayerScored()
    {
        SetPlayerScore(playerScore + 1);

        if (playerScore >= winningScore)
        {
            EndGame("Player");
        }
        else
        {
            NewRound();
        }
    }

    public void OnComputerScored()
    {
        SetComputerScore(computerScore + 1);

        if (computerScore >= winningScore)
        {
            EndGame("Computer");
        }
        else
        {
            NewRound();
        }
    }

    private void SetPlayerScore(int score)
    {
        playerScore = score;
        playerScoreText.text = score.ToString();
    }

    private void SetComputerScore(int score)
    {
        computerScore = score;
        computerScoreText.text = score.ToString();
    }

    // Functie om het spel te beëindigen en het eindscherm te tonen
    private void EndGame(string winner)
    {
        Debug.Log(winner + " heeft gewonnen!");

        // Stop de bal en verdere interacties
        ball.gameObject.SetActive(false);
        CancelInvoke();  // Stop het starten van nieuwe rondes

        // Verberg de paddles
        playerPaddle.gameObject.SetActive(false);
        computerPaddle.gameObject.SetActive(false);

        // Toon het eindscherm met de winnaar
        endScreen.SetActive(true);
        winnerText.text = winner + " heeft gewonnen!";
    }

    // Functie om de game opnieuw te starten vanuit het eindscherm
    public void OnRestartButtonPressed()
    {
        NewGame();
    }
}
