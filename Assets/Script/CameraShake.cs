using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private void Start()
    {
        //訂閱GameManager的搖晃事件
        GameManager.GameMainManager.ScreenShake += StartShake;
    }

    /// <summary>
    /// 開始晃動
    /// </summary>
    void StartShake()
    {
        StartCoroutine(Shake());
    }

    public IEnumerator Shake()
    {
        //取得GameManager的搖晃時間與幅度
        float duration = GameManager.GameMainManager.ShakeDuration;
        float magnitude = GameManager.GameMainManager.ShakeMagnitude;
        Vector3 OriginPos = transform.localPosition;

        float elasped = 0.0f;
        //如果搖晃時間還沒過，就繼續搖
        while (elasped < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            transform.localPosition = new Vector3(x, y, OriginPos.z);
            elasped += Time.deltaTime;

            yield return null;
        }


        //回復搖晃前狀態
        transform.localPosition = OriginPos;
    }
}
