using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fixed_Jump_Button_Boss : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button jumpButton;
    public Player_Movement_Boss playerMovement;
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Truck_Thing");
        playerMovement = player.GetComponent<Player_Movement_Boss>();
        // Remove onClick since it only works for clicks
        jumpButton.onClick.RemoveAllListeners(); 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        playerMovement.jumping = true; // Boost when button is pressed
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        playerMovement.jumping = false; // Stop boost when button is released
    }
}
