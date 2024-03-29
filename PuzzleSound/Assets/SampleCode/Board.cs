using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 盤面クラス
public class Board : MonoBehaviour {

    AudioSource audioSource;// SE関連
    public AudioClip sound1;// SE関連
    public AudioClip sound2;// SE関連
    public AudioClip sound3;// SE関連
    public AudioClip sound4;// SE関連
    public AudioClip sound5;// SE関連
    public AudioClip sound6;// SE関連

    //const
    private const float FillPieceDuration = 0.2f;
    private const float SwitchPieceCuration = 0.02f;

    // serialize field.
    [SerializeField]
    private GameObject piecePrefab;
    [SerializeField]
    private GameObject TimingBar;

    // private.
    private Piece[,] board; //これ何？
    private int width;
    private int height;
    private int pieceWidth;
    private int timingBarWidth;
    private int randomSeed;
    private int BarYPos;
    private Vector2 TimingBarPos;

    private Vector2[] directions = new Vector2[] { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
    private List<AnimData> fillPieceAnim = new List<AnimData>();
    private List<Vector2> pieceCreatePos = new List<Vector2>();

    //MusicTapの者達
    private Piece targetPiece;

    private Vector2 touchStartPos;
    private Vector2 touchUpPos;
    private Vector2 touchNowPos;
    private string direction;
    private PieceKind kind;
    private bool touchnow;
    private int longd;
    private Vector2 musicTpos;
    
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();  //SE関連
    }

    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    // 特定の幅と高さに盤面を初期化する/TimingBarの位置と大きさを初期化
    public void InitializeBoard(int boardWidth, int boardHeight)
    {
        
        width = boardWidth;
        height = boardHeight;

        pieceWidth = ( Screen.width / 3 ) / boardWidth;
        timingBarWidth = ( Screen.width / 3 ) / boardWidth;

        //バー生成
        Vector2 firstBarPos = new Vector2(Screen.width / 2, Screen.height / 2 + pieceWidth * 4.0f);
        TimingBar.GetComponent<RectTransform>().position = firstBarPos;
        TimingBarPos = TimingBar.GetComponent<RectTransform>().position;

        board = new Piece[width, height];

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                CreatePiece(new Vector2(i, j));
            }
        }

        //animManager.AddListAnimData(fillPieceAnim);
    }
    
    // 入力されたクリック(タップ)位置から最も近いピースの位置を返す
    public Piece GetNearestPiece(Vector3 input)
    {
        var minDist = float.MaxValue;
        Piece nearestPiece = null;

        // 入力値と盤面のピース位置との距離を計算し、一番距離が短いピースを探す
        foreach (var p in board)
        {
            var dist = Vector3.Distance(input, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestPiece = p;
            }
        }

        return nearestPiece;
    }

    // 盤面上のピースを交換する
    public void SwitchPiece(Piece p1, Piece p2)
    {
        // 位置を移動する
        var p1Position = p1.transform.position;
        p1.transform.position = p2.transform.position;
        p2.transform.position = p1Position;

        // 盤面データを更新する
        var p1BoardPos = GetPieceBoardPos(p1);
        var p2BoardPos = GetPieceBoardPos(p2);
        board[(int)p1BoardPos.x, (int)p1BoardPos.y] = p2;
        board[(int)p2BoardPos.x, (int)p2BoardPos.y] = p1;
    }
    //-------------------------------------------------------------------------------

    // 盤面上にマッチングしているピースがあるかどうかを判断する
    public bool HasMatch()
    {
        var note = PieceKind.TapNote;//ノーツは３つ揃っても消えない
        foreach (var piece in board)　//?????boardの中のpiece全てに対してチェックしてる？そもそもpieceがどこで宣言された何なのかがよくわからない。
        {
            var kind = piece.GetKind();
            if (IsMatchPiece(piece) && kind != note)
            {
                return true;
            }
        }
        return false;
    }

    // マッチしているピースの削除フラグを立てる
    public void DeleteMatchPiece()
    {
        var note = PieceKind.TapNote;
        
        // マッチしているピースの削除フラグを立てる
        foreach (var piece in board)
        {
            var kind = piece.GetKind();
            if( kind != note)
            {
                piece.deleteFlag = IsMatchPiece(piece);
            }
        }
    }

    //ピースのKindをNoteに変更しフラグを削除
    public void CreateNotes(Piece piece1, Piece piece2)
    {
        var flicd = (FlicDir)UnityEngine.Random.Range(0, Enum.GetNames(typeof(FlicDir)).Length-1);
        var tapnote = PieceKind.TapNote;
        var flicnote = PieceKind.FlicNote;
        var longnote = PieceKind.LongNote;
        var musicnote = PieceKind.MusicNote;

        if(piece1.deleteFlag && !piece2.deleteFlag)//selectedPieceにフラグが立っている場合
        {
            piece1.deleteFlag = false;
            switch(piece1.GetKind())
            {
                case PieceKind.Red:
                    piece1.SetKind(tapnote);
                    break;
                case PieceKind.Blue:
                    piece1.SetKind(flicnote);
                    piece1.SetFlicDir(flicd);
                    Debug.Log(piece1.GetDir());
                    break;
                case PieceKind.Green:
                    int longcount = LongNoteLength(piece1);
                    piece1.SetKind(longnote);
                    var enumlongcount = (CountLength)Enum.ToObject(typeof(CountLength),longcount);
                    piece1.SetCountLength(enumlongcount);
                    Debug.Log(piece1.GetLength());
                    break;
                case PieceKind.Yellow:
                    piece1.SetKind(musicnote);
                    break;
                default:
                
                    break;
            }
        }
        else if(piece2.deleteFlag && !piece1.deleteFlag)//NextPieceにフラグが立っている場合
        {
            piece2.deleteFlag = false;
            switch(piece2.GetKind())
            {
                case PieceKind.Red:
                    piece2.SetKind(tapnote);
                    break;
                case PieceKind.Blue:
                    piece2.SetKind(flicnote);
                    piece2.SetFlicDir(flicd);
                    Debug.Log(piece2.GetDir());
                    break;
                case PieceKind.Green:
                    int longcount = LongNoteLength(piece2);
                    piece2.SetKind(longnote);
                    var enumlongcount = (CountLength)Enum.ToObject(typeof(CountLength),longcount);
                    piece2.SetCountLength(enumlongcount);
                    Debug.Log(piece2.GetLength());
                    break;
                case PieceKind.Yellow:
                    piece2.SetKind(musicnote);
                    break;
                default:
                
                    break;
            }
        }
        else if(piece1.deleteFlag && piece2.deleteFlag)
        {
            piece1.deleteFlag = false;
            piece2.deleteFlag = false;
            switch(piece1.GetKind())
            {
                case PieceKind.Red:
                    piece1.SetKind(tapnote);
                    break;
                case PieceKind.Blue:
                    piece1.SetKind(flicnote);
                    piece1.SetFlicDir(flicd);
                    Debug.Log(piece1.GetDir());
                    break;
                case PieceKind.Green:
                    int longcount = LongNoteLength(piece1);
                    piece1.SetKind(longnote);
                    var enumlongcount = (CountLength)Enum.ToObject(typeof(CountLength),longcount);
                    piece1.SetCountLength(enumlongcount);
                    Debug.Log(piece1.GetLength());
                    break;
                case PieceKind.Yellow:
                    piece1.SetKind(musicnote);
                    break;
                default:
                
                    break;
            }
            switch(piece2.GetKind())
            {
                case PieceKind.Red:
                    piece2.SetKind(tapnote);
                    break;
                case PieceKind.Blue:
                    piece2.SetKind(flicnote);
                    piece2.SetFlicDir(flicd);
                    Debug.Log(piece2.GetDir());
                    break;
                case PieceKind.Green:
                    int longcount = LongNoteLength(piece2);
                    piece2.SetKind(longnote);
                    var enumlongcount = (CountLength)Enum.ToObject(typeof(CountLength),longcount);
                    piece2.SetCountLength(enumlongcount);
                    Debug.Log(piece2.GetLength());
                    break;
                case PieceKind.Yellow:
                    piece2.SetKind(musicnote);
                    break;
                default:
                
                    break;
            }
        }
    }

    public IEnumerator DeletePiece(Action endCallBadk)
    {
        
        // 削除フラグが立っているオブジェクトを削除する
        foreach (var piece in board)
        {
            if (piece != null && piece.deleteFlag)
            {
                GameManager.score++;
                Destroy(piece.gameObject);
            }
        }
        yield return new WaitForSeconds(0.2f);
        endCallBadk();
    }

    public void BarMovePos(float countd)
    {
        Vector2 BarPos = TimingBarPos;
        BarPos.y = (Screen.height / 2 + pieceWidth * 3.5f) - pieceWidth * (8 - countd);
        TimingBar.GetComponent<RectTransform>().position = BarPos;

        TimingBarPos = TimingBar.GetComponent<RectTransform>().position;
        if(7.5 < countd && countd <= 8.0)
        {
            BarYPos = 7;
        }
        else if(6.5 < countd && countd <= 7.5)
        {
            BarYPos = 6;
        }
        else if(5.5 < countd && countd <= 6.5)
        {
            BarYPos = 5;
        }
        else if(4.5 < countd && countd <= 5.5)
        {
            BarYPos = 4;
        }
        else if(3.5 < countd && countd <= 4.5)
        {
            BarYPos = 3;
        }
        else if(2.5 < countd && countd <= 3.5)
        {
            BarYPos = 2;
        }
        else if(1.5 < countd && countd <= 2.5)
        {
            BarYPos = 1;
        }
        else if(0 < countd && countd <= 1.5)
        {
            BarYPos = 0;
        }
        else
        {
            BarYPos = 10;
        }
    
    //Debug.Log("今のBarYPos" + BarYPos + "count" + countd);
    }

    public void MusicTap(float countd)
    {
        //var musicTpos = GetPieceBoardPos(targetPiece);
        var touchCount = Input.touchCount;
        Debug.Log(musicTpos.x);
        for(var i = 0; i < touchCount; i++)
        {
            string flicdStr;
            
            var touch = Input.GetTouch(i);
            float scorejadge = Math.Abs(countd - BarYPos);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                {
                    targetPiece = GetNearestPiece(touch.position);
                    musicTpos = GetPieceBoardPos(targetPiece);
                    touchStartPos = new Vector2(touch.position.x, touch.position.y);
                    kind = targetPiece.GetKind();
                    //if(musicTpos.y == BarYPos)
                    //{
                    if(0 <= scorejadge || scorejadge <= 0.2)
                    {
                        switch(kind)
                        {
                            case PieceKind.TapNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicFlag = true;
                                targetPiece.musicPerfect = true;
                                targetPiece.musicBad = false;
                                Debug.Log("赤色true");
                                break;
                            case PieceKind.MusicNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicFlag = true;
                                targetPiece.musicPerfect = true;
                                targetPiece.musicBad = false;
                                Debug.Log("黄色true");
                                break;
                            case PieceKind.FlicNote:
                                targetPiece.musicPerfect = true;
                                targetPiece.musicBad = false;
                                break;
                            case PieceKind.LongNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicPerfect = true;
                                targetPiece.musicBad = false;
                                break;
                            default:
                                break;
                        }
                    }
                    else if(0.2 < scorejadge || scorejadge <= 0.5)
                    {
                        switch(kind)
                        {
                            case PieceKind.TapNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicFlag = true;
                                targetPiece.musicGreat = true;
                                targetPiece.musicBad = false;
                                Debug.Log("赤色true");
                                break;
                            case PieceKind.MusicNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicFlag = true;
                                targetPiece.musicGreat = true;
                                targetPiece.musicBad = false;
                                Debug.Log("黄色true");
                                break;
                            case PieceKind.FlicNote:
                                targetPiece.musicGreat = true;
                                targetPiece.musicBad = false;
                                break;
                            case PieceKind.LongNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicGreat = true;
                                targetPiece.musicBad = false;
                                break;
                            default:
                                break;
                        }
                    }
                    else if(0.5 < scorejadge || scorejadge <= 0.8)
                    {
                        switch(kind)
                        {
                            case PieceKind.TapNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicFlag = true;
                                targetPiece.musicGood = true;
                                targetPiece.musicBad = false;
                                Debug.Log("赤色true");
                                break;
                            case PieceKind.MusicNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicFlag = true;
                                targetPiece.musicGood = true;
                                targetPiece.musicBad = false;
                                Debug.Log("黄色true");
                                break;
                            case PieceKind.FlicNote:
                                targetPiece.musicGood = true;
                                targetPiece.musicBad = false;
                                break;
                            case PieceKind.LongNote:
                                SoundColl((int)musicTpos.x);
                                targetPiece.musicGood = true;
                                targetPiece.musicBad = false;
                                break;
                            default:
                                break;
                        }
                    }
                    else if(0.8 < scorejadge)
                    {
                        switch(kind)
                        {
                            case PieceKind.TapNote:
                                targetPiece.musicBad = true;
                                break;
                            case PieceKind.MusicNote:
                                targetPiece.musicBad = true;
                                break;
                            case PieceKind.FlicNote:
                                targetPiece.musicBad = true;
                                break;
                            case PieceKind.LongNote:
                                targetPiece.musicBad = true;
                                break;
                            default:
                                break;
                        }
                    }
                    //}
                break;
                }
                case TouchPhase.Moved:
                {
                    touchNowPos = new Vector2(touch.position.x, touch.position.y);
                    GetDirection();//フリックの方向を取得
                    Debug.Log("マウスの方向" + direction);
                    switch(kind)
                    {
                        case PieceKind.FlicNote:
                            var flicd = targetPiece.GetDir();
                            flicdStr = flicd.ToString();
                            if(direction == flicdStr && targetPiece.musicFlag == false)
                            {
                                SoundColl((int)musicTpos.x);
                                if(targetPiece.musicPerfect || targetPiece.musicGreat || targetPiece.musicGood)
                                {
                                    targetPiece.musicFlag = true;
                                    Debug.Log("青色true");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                }
                case TouchPhase.Stationary:
                {
                    switch(kind)
                    {
                        case PieceKind.LongNote:
                            longd = (int)targetPiece.GetLength();
                            targetPiece.longdowncount += Time.deltaTime;
                            if(targetPiece.longdowncount >= longd)
                            {
                                Debug.Log("こえた");
                                if(targetPiece.musicPerfect || targetPiece.musicGreat || targetPiece.musicGood)
                                {
                                    targetPiece.musicFlag = true;
                                }
                            }
                            else if(BarYPos == 0)
                            {
                                Debug.Log("ついた");
                                if(targetPiece.musicPerfect || targetPiece.musicGreat || targetPiece.musicGood)
                                {
                                    targetPiece.musicFlag = true;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                }
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Canceled:
                    // システムがタッチの追跡をキャンセルした時に行いたい処理をここに書く
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void DeleteNotes()
    {
        var ntap = PieceKind.TapNote;
        var nflic = PieceKind.FlicNote;
        var nlong = PieceKind.LongNote;
        var nmusic = PieceKind.MusicNote;

        int truecount = 0;
        // 削除フラグが立っているノーツを削除する
        foreach (var piece in board)
        {
            var kind = piece.GetKind();
            if (piece != null && (kind == ntap || kind == nflic || kind == nlong || kind == nmusic))
            {
                if(piece.musicFlag && piece.musicPerfect)
                {
                    GameManager.score+=20;
                    truecount++;
                    Debug.Log(truecount);
                }
                else if(piece.musicFlag && piece.musicGreat)
                {
                    GameManager.score+=10;
                    truecount++;
                    Debug.Log(truecount);
                }
                else if(piece.musicFlag && piece.musicGood)
                {
                    GameManager.score+=5;
                    truecount++;
                    Debug.Log(truecount);
                }
                else if(piece.musicBad)
                {
                    GameManager.score-=10;
                }
                Destroy(piece.gameObject);
            }
        }
    }

    // ピースが消えている場所を詰めて、新しいピースを生成する
    public IEnumerator FillPiece(Action endCallBack)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                FillPiece(new Vector2(i, j));
            }
        }
        yield return new WaitForSeconds(0.5f);
        endCallBack();
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    // 特定の位置にピースを作成する
    private void CreatePiece(Vector2 position)
    {
        // ピースの生成位置を求める
        var createPos = GetPieceWorldPos(position);

        // 生成するピースの種類をランダムに決める
        var kind = (PieceKind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PieceKind)).Length-4); //ノーツ分マイナス

        // ピースを生成、ボードの子オブジェクトにする
        var piece = Instantiate(piecePrefab, createPos, Quaternion.identity).GetComponent<Piece>();
        piece.transform.SetParent(transform);
        piece.SetSize(pieceWidth);
        piece.SetKind(kind);

        // 盤面にピースの情報をセットする
        board[(int)position.x, (int)position.y] = piece;
    }

    // 盤面上の位置からピースオブジェクトのワールド座標での位置を返す
    private Vector3 GetPieceWorldPos(Vector2 boardPos)
    {
        return new Vector3(boardPos.x* pieceWidth + ( Screen.width / 2 ) - pieceWidth*(2.5f), boardPos.y* pieceWidth + ( Screen.height / 2 ) - pieceWidth*(3.5f), 0); //ここ数字でやってるの凄い嫌だ。
    }

    // ピースが盤面上のどの位置にあるのかを返す
    private Vector2 GetPieceBoardPos(Piece piece)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (board[i, j] == piece)
                {
                    return new Vector2(i, j);
                }
            }
        }

        return Vector2.zero;
    }

    // 対象のピースがマッチしているかの判定を行う
    private bool IsMatchPiece(Piece piece)
    {
        // ピースの情報を取得
        var pos = GetPieceBoardPos(piece);
        var kind = piece.GetKind();

        // 縦方向にマッチするかの判定 MEMO: 自分自身をカウントするため +1 する
        var verticalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.up) + GetSameKindPieceNum(kind, pos, Vector2.down) + 1;

        // 横方向にマッチするかの判定 MEMO: 自分自身をカウントするため +1 する
        var horizontalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.right) + GetSameKindPieceNum(kind, pos, Vector2.left) + 1;

        return verticalMatchCount >= GameManager.MachingCount || horizontalMatchCount >= GameManager.MachingCount;
    }

    // 対象の方向に引数で指定したの種類のピースがいくつあるかを返す
    private int GetSameKindPieceNum(PieceKind kind, Vector2 piecePos, Vector2 searchDir) 
    {
        var count = 0;
        while (true)
        {
            piecePos += searchDir; //piecePos = piecePos + searchDirと同義
            if (IsInBoard(piecePos) && board[(int)piecePos.x, (int)piecePos.y].GetKind() == kind)//対象のpieceがboardからはみ出していない且つ「たて又は横方向に隣のpiece」が同じ属性なら
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    // 対象の座標がボードに存在するか(ボードからはみ出していないか)を判定する
    private bool IsInBoard(Vector2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }

    // 特定のピースのが削除されているかを判断し、削除されているなら詰めるか、それができなければ新しく生成する
    private void FillPiece(Vector2 pos)
    {
        var piece = board[(int)pos.x, (int)pos.y];
        if (piece != null && !piece.deleteFlag)//pieceがある且つデリートフラグが立っていない
        {
            // ピースが削除されていなければ何もしない
            return;
        }

        // 対象のピースより上方向に有効なピースがあるかを確認、あるなら場所を移動させる
        var checkPos = pos + Vector2.up;
        while (IsInBoard(checkPos))
        {
            var checkPiece = board[(int)checkPos.x, (int)checkPos.y];
            if (checkPiece != null && !checkPiece.deleteFlag)//pieceがある且つデリートフラグが立っていない
            {
                checkPiece.transform.position = GetPieceWorldPos(pos);
                board[(int)pos.x, (int)pos.y] = checkPiece;//1個下のマスに代入
                board[(int)checkPos.x, (int)checkPos.y] = null;//元のpieceを削除
                return;
            }
            checkPos += Vector2.up;//さらに上のpieceで同じことする
        }

        // 有効なピースがなければ新しく作る
        CreatePiece(pos);
    }

    //フリック時の方向の取得
    private void GetDirection()
    {
        float directionX = touchNowPos.x - touchStartPos.x;
        float directionY = touchNowPos.y - touchStartPos.y;

        //差の大きさによる条件分岐
        if(Mathf.Abs(directionY) < Math.Abs(directionX))
        {
            if( 30 < directionX )
            {
                direction = "right";
            }
            else if( -30 > directionX )
            {
                direction = "left";
            }
        }
        else if(Mathf.Abs(directionX) < Mathf.Abs(directionY))
        {
            if (30 < directionY)
           {
               direction = "up";
           }
           else if (-30 > directionY)
           {
               direction = "down";
           }
        }

        else
       {
           //タッチを検出
           direction = "touch";
       }
    }

    //ロングノーツの長さの取得
    private int LongNoteLength(Piece piece)
    {
        // ピースの情報を取得
        var pos = GetPieceBoardPos(piece);
        var kind = piece.GetKind();

        // 縦方向にマッチするかの判定 MEMO: 自分自身をカウントするため +1 する
        var verticalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.up) + GetSameKindPieceNum(kind, pos, Vector2.down) + 1;

        // 横方向にマッチするかの判定 MEMO: 自分自身をカウントするため +1 する
        var horizontalMatchCount = GetSameKindPieceNum(kind, pos, Vector2.right) + GetSameKindPieceNum(kind, pos, Vector2.left) + 1;

        var clong = verticalMatchCount + horizontalMatchCount - 1;

        return clong;
    }

    private void SoundColl(int pos)
    {
        Debug.Log(pos);
        switch(pos)
        {
            case 0:
                audioSource.PlayOneShot(sound1);
                Debug.Log("do");
                break;
            case 1:
                audioSource.PlayOneShot(sound2);
                Debug.Log("re");
                break;
            case 2:
                audioSource.PlayOneShot(sound3);
                Debug.Log("mi");
                break;
            case 3:
                audioSource.PlayOneShot(sound4);
                Debug.Log("fa");
                break;
            case 4:
                audioSource.PlayOneShot(sound5);
                Debug.Log("so");
                break;
            case 5:
                audioSource.PlayOneShot(sound6);
                Debug.Log("ra");
                break;
            default:
                break;
        }
    }

    private void MusicScoreColl()
    {

    }
}