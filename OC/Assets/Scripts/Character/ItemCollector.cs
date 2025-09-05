using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private float attractionRange = 10f; // 吸引范围
    [SerializeField] private float attractionSpeed = 20f; // 吸引速度
    [SerializeField] private LayerMask itemLayer; // 物品所在的层

    private float lastAttractionTime = 0f; // 上次吸引物品的时间

    private void Update()
    {
        if(Time.time - lastAttractionTime > 0.5f) // 每0.5秒一次
        {
            lastAttractionTime = Time.time;
            AttractItems();
        }
    }

    private void AttractItems()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, attractionRange, itemLayer);
        foreach (var item in items)
        {
            item.GetComponent<Item>()?.Attract(transform, attractionSpeed);
        }
    }
}
