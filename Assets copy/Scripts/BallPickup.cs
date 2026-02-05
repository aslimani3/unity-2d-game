using UnityEngine;

public class BallPickup : MonoBehaviour
{
    public enum BallType { GoodSoccer, Bad }
    public BallType ballType = BallType.GoodSoccer;

    public int goodPoints = 10;
    public int badPoints = -5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance est null. Ajoute le GameManager dans la scène.");
            return;
        }

        if (ballType == BallType.GoodSoccer)
            GameManager.Instance.CollectGood(goodPoints);
        else
            GameManager.Instance.CollectBad(badPoints);

        Destroy(gameObject);
    }
}
