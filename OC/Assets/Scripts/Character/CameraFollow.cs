using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [Header("�������")]
    public Transform target; // ����Ŀ�꣨��ɫ��
    public float followSpeed = 5f;
    public float maxOffsetDistance = 3f; // ��ɫ����Ļ���������루���絥λ��

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (target == null)
        {
            var moveObj = FindObjectOfType<Move>();
            if (moveObj != null)
                target = moveObj.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ��ȡ��������������λ��
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // �����ɫ�������е�
        Vector3 midPoint = (target.position + mouseWorldPos) / 2f;

        // ��������ͷƫ�ƣ���֤��ɫ������ͷ���ľ��벻����������
        Vector3 camToTarget = target.position - midPoint;
        if (camToTarget.magnitude > maxOffsetDistance)
        {
            midPoint = target.position - camToTarget.normalized * maxOffsetDistance;
        }

        // ��������ͷz�᲻��
        midPoint.z = transform.position.z;

        // ʹ�ò�ֵƽ���ƶ�����ͷ������DOTweenÿ֡����Tween���¿���
        transform.position = Vector3.Lerp(transform.position, midPoint, followSpeed * Time.deltaTime);
    }
}