using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private int hiddenAreaCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HiddenArea"))
        {
            hiddenAreaCount++;
            other.GetComponent<HiddenAreaController>()?.SetHidden(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HiddenArea"))
        {
            hiddenAreaCount--;

            // Chỉ hiện lại nếu player KHÔNG còn trong hidden area nào
            if (hiddenAreaCount <= 0)
                other.GetComponent<HiddenAreaController>()?.SetHidden(false);
        }
    }
}
