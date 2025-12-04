// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class BossSkill1 : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private Transform firePoint;
//     [SerializeField] private List<GameObject> fireballs;

//     [Header("Settings")]
//     [SerializeField] private float spreadRadius = 1.2f;

//     [Header("Circle Settings")]
//     [SerializeField] private int fireballPerCircle = 10;

//     public bool IsCasting { get; private set; } = false;

//     private int nextFireballIndex = 0;

//     public IEnumerator CastSkill1()
//     {
//         IsCasting = true;
//         Debug.Log("Skill1 START");

//         // ⭐ Chỉ bắn 1 vòng duy nhất
//         CastFireCircle();

//         // ⭐ Boss đứng yên 0.6s mô phỏng thời gian ra chiêu
//         yield return new WaitForSeconds(0.6f);

//         IsCasting = false;
//         Debug.Log("Skill1 END");
//     }

//     private void CastFireCircle()
//     {
//         float angleStep = 360f / fireballPerCircle;

//         for (int i = 0; i < fireballPerCircle; i++)
//         {
//             GameObject fb = fireballs[nextFireballIndex];
//             nextFireballIndex = (nextFireballIndex + 1) % fireballs.Count;

//             float angle = angleStep * i * Mathf.Deg2Rad;
//             Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

//             Vector3 pos = firePoint.position + (Vector3)(dir * spreadRadius);

//             fb.transform.position = pos;
//             fb.transform.right = dir;
//             fb.SetActive(true);

//             BossProjectile proj = fb.GetComponent<BossProjectile>();
//             if (proj != null)
//             {
//                 proj.SetDirection(dir);
//             }
//         }
//     }
// }
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSkill1 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;   // ⭐ chỉ cần 1 prefab
    [SerializeField] private Transform projectileParent;    // ⭐ object chứa đạn

    [Header("Settings")]
    [SerializeField] private float spreadRadius = 1.2f;

    [Header("Circle Settings")]
    [SerializeField] private int fireballPerCircle = 10;

    private List<GameObject> pool = new List<GameObject>(); // ⭐ pool tạm
    private bool poolCreated = false;

    public bool IsCasting { get; private set; } = false;

    private void CreatePool()
    {
        if (poolCreated) return;

        for (int i = 0; i < fireballPerCircle; i++)
        {
            GameObject obj = Instantiate(projectilePrefab, projectileParent);
            obj.SetActive(false);
            pool.Add(obj);
        }

        poolCreated = true;
    }

    public IEnumerator CastSkill1()
    {
        IsCasting = true;

        // ⭐ tạo pool nếu chưa có
        CreatePool();

        Debug.Log("Skill1 START");

        CastFireCircle();

        // thời gian đứng yên sau skill
        yield return new WaitForSeconds(0.6f);

        Debug.Log("Skill1 END");
        IsCasting = false;
    }

    private void CastFireCircle()
    {
        float angleStep = 360f / fireballPerCircle;

        for (int i = 0; i < fireballPerCircle; i++)
        {
            GameObject fb = pool[i];

            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector3 pos = firePoint.position + (Vector3)(dir * spreadRadius);

            fb.transform.position = pos;
            fb.transform.right = dir;
            fb.SetActive(true);

            BossProjectile proj = fb.GetComponent<BossProjectile>();
            if (proj != null)
            {
                proj.SetDirection(dir);
            }
        }
    }
}
