using UnityEngine;
using ArrowsPuzzle;   // để dùng GamePlay và Arrow

public class ArrowToTDConnector : MonoBehaviour
{
    [Header("References")]
    public PlayerShoot playerShoot;   // Player trên tháp

    private GamePlay gamePlay;        // GamePlay của ArrowPuzzle

    private void Awake()
    {
        // Tìm GamePlay trong scene (nó nằm cùng object với GameManager)
        gamePlay = FindObjectOfType<GamePlay>();

        // Nếu quên gán PlayerShoot trên Inspector thì thử auto tìm
        if (!playerShoot)
            playerShoot = FindObjectOfType<PlayerShoot>();
    }

    private void OnEnable()
    {
        if (gamePlay != null)
            gamePlay.OnArrowCleared += HandleArrowCleared;
    }

    private void OnDisable()
    {
        if (gamePlay != null)
            gamePlay.OnArrowCleared -= HandleArrowCleared;
    }

    /// <summary>
    /// Được gọi mỗi khi 1 mũi tên puzzle đi thẳng thành công (GoForward xong).
    /// </summary>
    private void HandleArrowCleared(Arrow arrow)
    {
        if (playerShoot == null) return;

        playerShoot.AddBulletFromPuzzle();
    }

}
