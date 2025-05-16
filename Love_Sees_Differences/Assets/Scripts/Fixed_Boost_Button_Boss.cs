using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fixed_Boost_Button_Boss : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button boostButton;
    public Player_Movement_Boss playerMovement;
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Truck_Thing_Boss");
        playerMovement = player.GetComponent<Player_Movement_Boss>();
        // Remove onClick since it only works for clicks
        boostButton.onClick.RemoveAllListeners(); 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        playerMovement.boosted = true; // Boost when button is pressed
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        playerMovement.boosted = false; // Stop boost when button is released
    }
}
