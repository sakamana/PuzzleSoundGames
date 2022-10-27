using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingBar : MonoBehaviour
{
    //public
    public bool deleteBarFlag;

    //private
    private SpriteRenderer thisSpriteRenderer;
    private RectTransform thisRectTransform;

    //初期化
    private void Awake()
    {
        //フラグ初期化
        deleteBarFlag = false;

        //アタッチされているコンポーネントの取得
        thisRectTransform = GetComponent<RectTransform>();
    }

    //Barの大きさをセットする
    public void SetBarSize(int size)
    {
        this.thisRectTransform.sizeDelta = Vector2.one * size;
    }
}
