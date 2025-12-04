using UnityEngine;

public class RoomCameraTrigger : MonoBehaviour
{
    public enum TriggerType { Enter, Exit }
    [SerializeField] private TriggerType triggerType;

    [Header("Controller")]
    public RoomCameraController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (triggerType == TriggerType.Enter)
        {
            controller.ActivateRoomCamera();
        }
        else if (triggerType == TriggerType.Exit)
        {
            controller.DeactivateRoomCamera();
        }
    }
}
