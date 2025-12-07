using UnityEngine;

public class BController : MonoBehaviour
{
    public float speed = 2f;
    public float halfSpaceSize = 5f;

    [Header("初期方向ベクトル")]
    public Vector3 initialDirection = new Vector3(1, 0, 0);

    private Vector3 velocity;
    private Vector3 startPos;
    private Quaternion startRot;    // ← 追加：初期回転も保存する

    void Awake()
    {
        // Start より Awake の方が GA から確実にリセットできる
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Start()
    {
        ResetB();   // 初期実行も安全に
    }

    void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;

        Vector3 pos = transform.position;

        if (pos.x < -halfSpaceSize || pos.x > halfSpaceSize) velocity.x *= -1;
        if (pos.y < -halfSpaceSize || pos.y > halfSpaceSize) velocity.y *= -1;
        if (pos.z < -halfSpaceSize || pos.z > halfSpaceSize) velocity.z *= -1;

        pos.x = Mathf.Clamp(pos.x, -halfSpaceSize, halfSpaceSize);
        pos.y = Mathf.Clamp(pos.y, -halfSpaceSize, halfSpaceSize);
        pos.z = Mathf.Clamp(pos.z, -halfSpaceSize, halfSpaceSize);
        transform.position = pos;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("A")) return;

        GameManager.instance.AddDamage(10f * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("A")) return;

        Vector3 normal = collision.contacts[0].normal;
        velocity = Vector3.Reflect(velocity, normal);
    }

    public void ResetB()
    {
        // 初期状態に完全リセット
        transform.position = startPos;
        transform.rotation = startRot;
        velocity = initialDirection.normalized * speed;

        // 念のため外部から書き換えられた可能性をつぶす
        velocity.Normalize();
        velocity *= speed;
    }
}
