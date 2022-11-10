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
        var sizeBar = new Vector2(10,0.2f); //1,1の時はピースと同じ大きさ
        this.thisRectTransform.sizeDelta = sizeBar * size;
    }
}
