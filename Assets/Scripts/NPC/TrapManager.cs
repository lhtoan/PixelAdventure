using UnityEngine;
using System.Collections;

public class TrapManager : MonoBehaviour
{
    [Header("Trap List")]
    public GameObject[] traps;   // Chứa ArrowTrap, SpikedBall, SpikeHead, ...

    [Header("Timing Settings")]
    public float activeTime = 3f;   // thời gian trap bật
    public float inactiveTime = 4f; // thời gian trap tắt

    private bool running = false;
    private Coroutine trapRoutine;

    public void StartTrapCycle()
    {
        if (!running)
        {
            trapRoutine = StartCoroutine(TrapCycle());
        }
    }

    IEnumerator TrapCycle()
    {
        running = true;

        while (true)
        {
            // Bật trap
            SetTraps(true);
            yield return new WaitForSeconds(activeTime);

            // Tắt trap
            SetTraps(false);
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    // ⭐ Hàm dừng trap hoàn toàn
    public void StopTrapCycle()
    {
        if (running && trapRoutine != null)
        {
            StopCoroutine(trapRoutine);
        }

        running = false;

        // Tắt hết trap
        SetTraps(false);

        Debug.Log("Trap Cycle STOPPED!");
    }

    void SetTraps(bool state)
    {
        foreach (GameObject trap in traps)
        {
            if (trap != null)
                trap.SetActive(state);
        }
    }
}
