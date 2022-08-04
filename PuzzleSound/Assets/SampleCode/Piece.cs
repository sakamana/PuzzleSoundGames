using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    //public
    public bool deleteFlag;

    private Image thisImage;
    private RectTransform thisRectTransform;
    private PieceKind kind;

    //初期化処理
    private void Awake()
    {
        //コンポーネントの取得
        thisImage = GetComponent<Image>();
        thisRectTransform = GetComponent<RectTransform>();

        //フラグの初期化
        deleteFlag = false;
    }

    //-----------------------------------------------------------------------------
    //ピースの種類とそれに応じた色をセット
    public void SetKind(PieceKind pieceKind)
    {
        kind = pieceKind;
        SetColor();
    }

    //ピースの種類を返す
    public PieceKind GetKind()
    {
        return kind;
    }

    //ピースのサイズをセット
    public void Setsize(int size)
    {
        this.thisRectTransform.sizeDelta = Vector2.one * size;
    }

    //-----------------------------------------------------------------------------
    
    //ピースの色を自信の属性に変更
    private void SetColor()
    {
        switch(kind)
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
            case PieceKind.Black:
                thisImage.color = Color.black;
                break;
            case PieceKind.Magenta:
                thisImage.color = Color.magenta;
                break;
            default:
                break;
        }
    }
}
