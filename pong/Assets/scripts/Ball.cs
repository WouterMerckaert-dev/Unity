using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;

    public float baseSpeed = 5f;
    public float maxSpeed = 10f; // Maximale snelheid van de bal
    public float speedIncreaseFactor = 1.05f;  // Factor waarmee de snelheid toeneemt bij elke botsing

    public float currentSpeed { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetPosition()
    {
        transform.position = Vector2.zero;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Reset de snelheid
    }

    public void AddStartingForce()
    {
        // Flip a coin to determine if the ball starts left or right
        float x = Random.value < 0.5f ? -1f : 1f;

        // Flip a coin to determine if the ball goes up or down. Set the range
        // between 0.5 -> 1.0 to ensure it does not move completely horizontal.
        float y = Random.value < 0.5f ? Random.Range(-1f, -0.5f)
                                      : Random.Range(0.5f, 1f);

        // Apply the initial force and set the current speed
        Vector2 direction = new Vector2(x, y);
        rb.AddForce(direction * baseSpeed, ForceMode2D.Impulse);

        // Stel de huidige snelheid in op de basis snelheid
        currentSpeed = baseSpeed;
    }

    // Deze methode wordt aangeroepen wanneer de bal een paddle raakt
    public void OnPaddleHit()
    {
        // Verhoog de snelheid van de bal
        rb.velocity *= speedIncreaseFactor;
        Debug.Log("Ball speed increased to: " + rb.velocity.magnitude);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Computer"))
        {
            OnPaddleHit(); // Verhoog de snelheid wanneer de bal een paddle raakt
        }
    }
}
