using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TMP_Text text;
    public float floatUpDistance = 1.0f;
    public float duration = 0.7f;

    private Vector3 startPos;
    private float timer;

    public void Show(int damage, Vector3 position)
    {
        text.text = damage.ToString();
        startPos = position;
        transform.position = startPos;
        timer = 0f; 
        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        // ÉÏÉýºÍ½¥Òþ
        transform.position = startPos + Vector3.up * floatUpDistance * t;
        text.alpha = 1f - t;

        if (timer >= duration)
        {
            gameObject.SetActive(false);
        }
    }
}
