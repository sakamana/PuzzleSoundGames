using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ピースクラス
public class Piece : MonoBehaviour
{
    // public.
    public bool deleteFlag;
    public bool musicFlag;

    // private.
    private Image thisImage;
    private RectTransform thisRectTransform;
    private PieceKind kind;

    //-------------------------------------------------------
    // MonoBehaviour Function
    //-------------------------------------------------------
    // 初期化処理
    private void Awake()
    {
        // アタッチされている各コンポーネントを取得
        thisImage = GetComponent<Image>();
        thisRectTransform = GetComponent<RectTransform>();

        // フラグを初期化
        deleteFlag = false;
        musicFlag = false;
    }

    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    // ピースの種類とそれに応じた色をセットする
    public void SetKind(PieceKind pieceKind)
    {
        kind = pieceKind;
        SetColor();
    }

    // ピースの種類を返す
    public PieceKind GetKind()
    {
        return kind;
    }

    // ピースのサイズをセットする
    public void SetSize(int size)
    {
        this.thisRectTransform.sizeDelta = Vector2.one * size;
    }

    //バーに当たったらフラグを立てる、ここが動きません！！！！！！！
    void OnTriggerStay(Collider t)
    {
        if (t.CompareTag("Bar"))
        {
            musicFlag = true;
            Debug.Log("Hit");
        }
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    // ピースの色を自身の種類の物に変える
    private void SetColor()
    {
        switch (kind)
        {
            case PieceKind.Red:
                thisImage.color = Color.red;
                break;
            case PieceKind.Blue:
                thisImage.color = Color.blue;
                break;
            case PieceKind.Green:
                thisImage.color = Color.green;
                break;
            case PieceKind.Yellow:
                thisImage.color = Color.yellow;
                break;
            // case PieceKind.Black:
            //     thisImage.color = Color.black;
            //     break;
            // case PieceKind.Magenta:
            //     thisImage.color = Color.magenta;
            //     break;
            case PieceKind.Note:
                thisImage.color = Color.white;
                break;
            default:
                break;
        }
        
    }
}