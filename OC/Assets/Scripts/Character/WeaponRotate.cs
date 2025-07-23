using UnityEngine;

/// <summary>
/// �����������ϣ����������������ת������������ת�ǶȺ�������ת����
/// </summary>
public class WeaponRotate : MonoBehaviour
{
    [Header("��ת����")]
    public Transform rotationCenter; // ��ת���ģ�����ק��ɫTransform���Զ���㣩
    public float minAngle = -60f;    // ��С��ת�Ƕȣ�����ڳ�ʼ����
    public float maxAngle = 60f;     // �����ת�Ƕȣ�����ڳ�ʼ����
    
    [Header("��ɫ��ת���")]
    public Transform characterTransform; // ���ڼ���ɫ��ת��Transform��ͨ���ǽ�ɫ�����壩
    
    private Camera cam;
    private bool isFacingLeft = false;

    void Awake()
    {
        cam = Camera.main;
        if (rotationCenter == null)
            rotationCenter = transform.parent; // Ĭ���Ը�����Ϊ��ת����
        
        if (characterTransform == null)
            characterTransform = transform.root; // Ĭ��ʹ�ø�������Ϊ��ɫTransform
    }

    void Update()
    {
        if (cam == null || rotationCenter == null) return;

        // ����ɫ�Ƿ���
        isFacingLeft = characterTransform != null && characterTransform.localScale.x < 0;
        
        // ��ȡ��������������λ��
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = rotationCenter.position.z;

        // �����������ĵ����ķ���
        Vector2 dir = mouseWorldPos - rotationCenter.position;

        // ����Ŀ��Ƕȣ���rotationCenterΪԭ�㣬����Ϊ0�ȣ�
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        // ��׼���Ƕȵ�0-360��Χ
        float normalizedAngle = targetAngle;
        if (normalizedAngle < 0) normalizedAngle += 360f;
        
        // �Ƕ�ӳ��
        float mappedAngle;
        
        if (isFacingLeft)
        {
            // ����ɫ����ʱ��ǰ��Բ��90-270��
            if (normalizedAngle >= 90f && normalizedAngle <= 270f)
            {
                // ӳ��90->270��minAngle->maxAngle��������Ϊ����ʱ�Ƕ�������˳ʱ�룩
                mappedAngle = Mathf.Lerp(-minAngle, -maxAngle, (normalizedAngle - 90f) / 180f);
            }
            else
            {
                // ����ǰ��Բ��Χ���̶��ڱ߽�
                if (normalizedAngle > 270f && normalizedAngle < 360f || normalizedAngle >= 0f && normalizedAngle < 90f)
                {
                    // ��270-90�ȷ�Χ������0�ȣ�
                    mappedAngle = (normalizedAngle > 270f) ? -minAngle : -maxAngle;
                }
                else
                {
                    // ��ȫĬ��ֵ
                    mappedAngle = (normalizedAngle - 180f > 0) ? -minAngle : -maxAngle;
                }
            }
        }
        else
        {
            // ����ɫ����ʱ��ǰ��Բ��-90��90��
            if (normalizedAngle >= 270f || normalizedAngle <= 90f)
            {
                // ��Ҫ��270-360��0-90������ӳ�䵽minAngle->maxAngle
                float mappingAngle = normalizedAngle;
                if (mappingAngle >= 270f) mappingAngle -= 360f; // ת����-90��90��Χ
                
                // ��-90->90ӳ�䵽minAngle->maxAngle
                mappedAngle = Mathf.Lerp(minAngle, maxAngle, (mappingAngle + 90f) / 180f);
            }
            else
            {
                // ����ǰ��Բ��Χ���̶��ڱ߽�
                mappedAngle = (normalizedAngle < 180f) ? maxAngle : minAngle;
            }
        }

        // Ӧ����ת
        transform.position = rotationCenter.position; // ��֤����ʼ��Χ����ת����
        if (isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, -1*mappedAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, mappedAngle);
        }
    }
}