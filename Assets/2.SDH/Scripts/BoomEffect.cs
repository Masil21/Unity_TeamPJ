using System;
using System.Collections;
using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    public static event Action OnBoom;

    void Start()
    {
        OnBoom?.Invoke();
        StartCoroutine(BoomRoutine());
    }

    IEnumerator BoomRoutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
