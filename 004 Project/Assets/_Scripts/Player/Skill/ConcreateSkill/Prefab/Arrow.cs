using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Arrow : MonoBehaviour
{
    private CollisionHandler collisionHandler;
    private Rigidbody2D rb;

    private float currentDistance; // �̵��� �Ÿ��� �����ϴ� ���� �߰�
    private float maxDistance; // ȭ���� ���ư� �ִ� �Ÿ�

    private void FixedUpdate()
    {
        currentDistance += rb.velocity.magnitude * Time.fixedDeltaTime; // ȭ���� �̵��� �Ÿ� ���
        if (currentDistance >= maxDistance) // ���� �Ÿ� �̻� �̵��ϸ� ����
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        collisionHandler = GetComponent<CollisionHandler>();
        rb = GetComponent<Rigidbody2D>();
        currentDistance = 0f; // �ʱ�ȭ

        collisionHandler.OnColliderDetected += Detected;

    }

    private void Detected(Collider2D collision)
    {
        //Destroy(gameObject);
    }

    private void OnDestroy()
    {
        collisionHandler.OnColliderDetected -= Detected;
    }

    public void SetThrowDistance(float distance)
    {
        maxDistance = distance;
    }
}
