
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

    //フリック方向用スプライト 
    public Sprite rightImg;
    public Sprite leftImg;
    public Sprite upImg;
    public Sprite downImg;

    //長押し拍数用スプライト
    public Sprite oneImg;
    public Sprite twoImg;
    public Sprite threeImg;
    public Sprite fourImg;
    public Sprite fiveImg;

    //音ゲー判定用bool
    public bool musicGood;
    public bool musicGreat;
    public bool musicPerfect;

    public float longdowncount;

    // private.
    private Image thisImage;
    private RectTransform thisRectTransform;
    private PieceKind kind;
    private FlicDir fDir;
    private CountLength notelength;

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

        musicGood = false;
        musicGreat = false;
        musicPerfect = false;

        longdowncount = 0;
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

    public void SetMusicScore(MusicScore musicScore)
    {
        Mscore = musicScore;
    }

    public void GetMusicScore()
    {
        return Mscore;
    }

    //フリックノーツに方向をセットする
    public void SetFlicDir(FlicDir flicDir)
    {
        fDir = flicDir;
        switch(fDir)
        {
            case FlicDir.right:
                this.gameObject.GetComponent<Image> ().sprite = rightImg;
                break;
            case FlicDir.left:
                this.gameObject.GetComponent<Image> ().sprite = leftImg;
                break;
            case FlicDir.up:
                this.gameObject.GetComponent<Image> ().sprite = upImg;
                break;
            case FlicDir.dowm:
                this.gameObject.GetComponent<Image> ().sprite = downImg;
                break;
            default:
                break;
        }
    }

    //方向を返す
    public FlicDir GetDir()
    {
        return fDir;
    }

    //longノーツに秒数をセットする
    public void SetCountLength(CountLength clength)
    {
        notelength = clength;
        switch(clength)
        {
            case CountLength.one:
                this.gameObject.GetComponent<Image> ().sprite = oneImg;
                break;
            case CountLength.two:
                this.gameObject.GetComponent<Image> ().sprite = twoImg;
                break;
            case CountLength.three:
                this.gameObject.GetComponent<Image> ().sprite = threeImg;
                break;
            case CountLength.four:
                this.gameObject.GetComponent<Image> ().sprite = fourImg;
                break;
            case CountLength.five:
                this.gameObject.GetComponent<Image> ().sprite = fiveImg;
                break;
            default:
                break;
        }
    }

    //longノーツの秒数を返す
    public CountLength GetLength()
    {
        return notelength;
    }

    // ピースのサイズをセットする
    public void SetSize(int size)
    {
        this.thisRectTransform.sizeDelta = Vector2.one * size;
    }

    //バーに当たったらフラグを立てる、ここが動きません！！！！！！！
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Bar"))
        {
            Debug.Log ("Triggeredpiece");
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
            case PieceKind.TapNote:
                thisImage.color = new Color(1, 0.6f, 0.6f, 1.0f);
                break;
            case PieceKind.FlicNote:
                thisImage.color = new Color(0.6f, 0.8f, 1, 1.0f);
                break;
            case PieceKind.LongNote:
                thisImage.color = new Color(0.6f, 1, 0.6f, 1.0f);
                break;
            case PieceKind.MusicNote:
                thisImage.color = new Color(1, 1, 0.6f, 1.0f);
                break;
            default:
                break;
        }
        
    }
}