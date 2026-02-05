using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public bool canMove = true;   // ⬅️ AJOUT IMPORTANT

    [Header("Visual")]
    public SpriteRenderer sr;
    public Transform visualTransform;

    [Header("Bobbing (even when idle)")]
    public float bobAmplitude = 0.05f;
    public float bobFrequency = 6f;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector3 visualStartLocalPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (visualTransform == null && sr != null) visualTransform = sr.transform;

        if (visualTransform != null)
            visualStartLocalPos = visualTransform.localPosition;
    }

    void Update()
    {
        // ⛔ Si la partie est finie, on ne lit plus les inputs
        if (!canMove)
        {
            input = Vector2.zero;
            HandleBobbingAlways(); // le bobbing continue (effet vivant)
            return;
        }

        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        HandleOrientation4Directions();
        HandleBobbingAlways();
    }

    void FixedUpdate()
    {
        // ⛔ Sécurité : on ne bouge plus si canMove = false
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = input * speed;
    }

    void HandleOrientation4Directions()
    {
        if (visualTransform == null || sr == null) return;

        // Si on ne bouge pas, on garde la dernière orientation
        if (input.sqrMagnitude < 0.001f) return;

        float ax = Mathf.Abs(input.x);
        float ay = Mathf.Abs(input.y);

        // Priorité à l'axe dominant (utile en diagonale)
        if (ay >= ax)
        {
            // Vertical
            sr.flipX = false;

            if (input.y > 0.1f)
                visualTransform.localRotation = Quaternion.Euler(0, 0, 90f);   // HAUT
            else
                visualTransform.localRotation = Quaternion.Euler(0, 0, -90f);  // BAS
        }
        else
        {
            // Horizontal
            visualTransform.localRotation = Quaternion.Euler(0, 0, 0f);
            sr.flipX = input.x < 0; // gauche = flip
        }
    }

    void HandleBobbingAlways()
    {
        if (visualTransform == null) return;

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        visualTransform.localPosition = visualStartLocalPos + new Vector3(0f, bob, 0f);
    }
}
