using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class PlayerShoot : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePos;

    [Header("References")]
    public EnemyManager enemyManager;

    [Header("Magazine")]
    public int storedBullets = 0;          // số viên tích trữ
    public float fireRate = 0.25f;         // bắn mỗi 0.25s khi đang xả băng đạn
    private bool isFiringStored = false;

    /// <summary>
    /// Hàm được gọi khi puzzle giải đúng 1 mũi tên.
    /// Nếu có enemy → bắn ngay.
    /// Nếu không → tích băng đạn.
    /// </summary>
    public void AddBulletFromPuzzle()
    {
        // Có enemy → bắn ngay
        if (enemyManager != null && enemyManager.GetNearestEnemy(transform.position) != null)
        {
            ShootOneImmediate();
        }
        else
        {
            // không có enemy → tích trữ
            storedBullets++;
        }
    }

    /// <summary>
    /// Gọi ở EnemyManager mỗi khi có thêm enemy spawn vào scene.
    /// </summary>
    public void TryFireStoredBullets()
    {
        if (storedBullets > 0 && !isFiringStored)
        {
            StartCoroutine(FireStoredBulletsRoutine());
        }
    }

    IEnumerator FireStoredBulletsRoutine()
    {
        isFiringStored = true;

        while (storedBullets > 0)
        {
            ShootOneImmediate();
            storedBullets--;
            yield return new WaitForSeconds(fireRate);
        }

        isFiringStored = false;
    }

    public void ShootOneImmediate()
    {
        if (!enemyManager || !bulletPrefab || !firePos) return;

        Transform nearest = enemyManager.GetNearestEnemy(transform.position);
        if (nearest == null)
        {
            // nếu ngẫu nhiên enemy vừa chết → lưu lại viên đạn
            storedBullets++;
            return;
        }

        Vector2 dir = (nearest.position - firePos.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(Vector2.right, dir);

        Instantiate(bulletPrefab, firePos.position, rot);
    }

    /// <summary>
    /// Reset băng đạn khi next level hoặc replay.
    /// </summary>
    public void ResetMagazine()
    {
        storedBullets = 0;
        isFiringStored = false;
        StopAllCoroutines();
    }
}
}
