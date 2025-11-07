using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_R_Fire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform R_FirePoint;         // điểm bắn ra lốc xoáy
    [SerializeField] private List<GameObject> tornadoPool; // pool chứa prefab lốc xoáy (inactive lúc đầu)
    [SerializeField] private PlayerAttack playerAttack;

    [Header("Skill Settings")]
    [SerializeField] private float cooldown = 8f;           // hồi chiêu
    [SerializeField] private float spawnDelay = 0.05f;      // delay nhỏ trước khi bật tornado để debug thấy thứ tự (tùy chọn)

    private bool isOnCooldown = false;

    private void Awake()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();

        // safety: ensure pool objects are inactive at start
        for (int i = 0; i < tornadoPool.Count; i++)
        {
            if (tornadoPool[i] != null && tornadoPool[i].activeSelf)
            {
                tornadoPool[i].SetActive(false);
                Debug.Log($"[Skill_R_Fire] Pool element {i} was active at Awake — forcing inactive.");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isOnCooldown)
        {
            if (playerAttack != null && playerAttack.CurrentElement == PlayerAttack.Element.Fire)
            {
                StartCoroutine(CastSingleTornado());
            }
        }
    }

    private IEnumerator CastSingleTornado()
    {
        isOnCooldown = true;

        if (R_FirePoint == null)
        {
            Debug.LogError("[Skill_R_Fire] R_FirePoint is NOT assigned!");
            isOnCooldown = false;
            yield break;
        }

        if (tornadoPool == null || tornadoPool.Count == 0)
        {
            Debug.LogError("[Skill_R_Fire] tornadoPool is empty. Assign at least one prefab in Inspector.");
            isOnCooldown = false;
            yield break;
        }

        Debug.Log("🔥 Kích hoạt R Fire - Hellfire Tornado!");

        // hướng nhìn player: 1 = phải, -1 = trái
        float dir = Mathf.Sign(playerAttack.transform.localScale.x);
        Vector3 spawnPos = R_FirePoint.position;

        // lấy 1 object sẵn sàng từ pool (tìm first inactive)
        GameObject tornado = null;
        int foundIndex = -1;
        for (int i = 0; i < tornadoPool.Count; i++)
        {
            if (tornadoPool[i] != null && !tornadoPool[i].activeSelf)
            {
                tornado = tornadoPool[i];
                foundIndex = i;
                break;
            }
        }

        // nếu không tìm được inactive object thì fallback là dùng index 0 (vẫn bật lên, có thể reuse)
        if (tornado == null)
        {
            tornado = tornadoPool[0];
            foundIndex = 0;
            Debug.LogWarning("[Skill_R_Fire] No inactive tornado found in pool — reusing element 0.");
        }

        Debug.Log($"[Skill_R_Fire] Spawning tornado (poolIndex={foundIndex}) at {spawnPos}, dir={dir}");

        // small optional delay so logs appear nicely
        if (spawnDelay > 0f)
            yield return new WaitForSeconds(spawnDelay);

        // bật tornado và initialize
        tornado.transform.position = spawnPos;
        tornado.transform.rotation = Quaternion.identity;
        tornado.SetActive(true);

        HellfireTornado ht = tornado.GetComponent<HellfireTornado>();
        if (ht == null)
        {
            Debug.LogError("[Skill_R_Fire] The tornado prefab does not have HellfireTornado script!");
            // still wait cooldown but return
            yield return new WaitForSeconds(cooldown);
            isOnCooldown = false;
            yield break;
        }

        // khởi tạo hướng (moveDir) từ hướng nhìn của player
        Vector2 moveDir = Vector2.right * dir;
        ht.Initialize(moveDir);

        Debug.Log("[Skill_R_Fire] Tornado initialized and launched.");

        // cooldown
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
