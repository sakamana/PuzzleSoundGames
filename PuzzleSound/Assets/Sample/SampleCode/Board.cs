using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 盤面クラス
public class Board : MonoBehaviour {

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

    //-------------------------------------------------------
    // Public Function
    //-------------------------------------------------------
    // 特定の幅と高さに盤面を初期化する/TimingBarの位置と大きさを初期化
    public void InitializeBoard(int boardWidth, int boardHeight)
    {
        width = boardWidth;
        height = boardHeight;

        pieceWidth = ( Screen.width / 3 ) / boardWidth;
        timingBarWidth = Screen.width / 2;

        CreateTimingBar(new Vector2(0,0)); //timingBarを配置

        board = new Piece[width, height];

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                CreatePiece(new Vector2(i, j));
            }
        }
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
        var note = PieceKind.Note;
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
        var note = PieceKind.Note;
        
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
        if(piece1.deleteFlag)//selectedPieceにフラグが立っている場合
        {
            piece1.deleteFlag = false;
            var kind = PieceKind.Note;
            piece1.SetKind(kind);
        }
        else if(piece2.deleteFlag)//NextPieceにフラグが立っている場合
        {
            piece2.deleteFlag = false;
            var kind = PieceKind.Note;
            piece2.SetKind(kind);
        }
    }

    public void DeletePiece()
    {
        
        // 削除フラグが立っているオブジェクトを削除する
        foreach (var piece in board)
        {
            if (piece != null && piece.deleteFlag)
            {
                Destroy(piece.gameObject);
            }
        }
    }

    // ピースが消えている場所を詰めて、新しいピースを生成する
    public void FillPiece()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                FillPiece(new Vector2(i, j));
            }
        }
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
        var kind = (PieceKind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PieceKind)).Length-1);

        // ピースを生成、ボードの子オブジェクトにする
        var piece = Instantiate(piecePrefab, createPos, Quaternion.identity).GetComponent<Piece>();
        piece.transform.SetParent(transform);
        piece.SetSize(pieceWidth);
        piece.SetKind(kind);

        // 盤面にピースの情報をセットする
        board[(int)position.x, (int)position.y] = piece;
    }

    private void CreateTimingBar(Vector2 position)
    {
        //バーの生成位置
        var createPos = GetPieceWorldPos(position);
        //バーの生成
        var timingBar = Instantiate(TimingBar, createPos, Quaternion.identity).GetComponent<TimingBar>(); 
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
    private bool IsMatchPiece(Piece piece) //?????Piece pieceって何の真偽値？
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
}