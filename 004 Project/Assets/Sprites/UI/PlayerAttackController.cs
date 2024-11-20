using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttackController : MonoBehaviour
{
    public LayerMask attackLayer; // 공격 가능한 레이어만 선택

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI를 클릭한 경우 무시
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // 화면 클릭 위치에서 공격 가능한 오브젝트만 타겟팅
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, attackLayer))
            {
                PerformAttack(hit.collider.gameObject);
            }
        }
    }

    private void PerformAttack(GameObject target)
    {
        Debug.Log($"공격 대상: {target.name}");
        // 기본 공격 로직
    }
}