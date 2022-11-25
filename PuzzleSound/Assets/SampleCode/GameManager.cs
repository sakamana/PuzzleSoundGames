using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲーム管理クラス
public class GameManager : MonoBehaviour {

    // const.
    public const int MachingCount = 3;

    public float countdown;
    public int countdownTriger = 0;

    // enum.
    private enum GameState
    {
        Idle,
        PieceMove,
        MatchCheck,
        DeleteCheck,
        CreateNotes,
        DeletePiece,
        FillPiece,
        MusicTap,
        DeleteNotes,
        AfterMusicFillPiece,
    }

    // serialize field.
    [SerializeField]
    private Board board;
    [SerializeField]
    private Text stateText;

    // private.
    private GameState currentState;
    private Piece selectedPiece;
    private Piece NextPiece;
    private Piece targetPiece;

    

    //-------------------------------------------------------
    // MonoBehaviour Function
    //-------------------------------------------------------
    // ゲームの初期化処理
    private void Start()
    {
        board.InitializeBoard(6, 8);
        currentState = GameState.Idle;
    }

    // ゲームのメインループ
    private void Update()
    {
        countdown -=Time.deltaTime;
        //Debug.Log(currentState);
        if(8 < countdown) //16秒間パズルphase(24~8)
        {
            switch (currentState)
            {
                case GameState.Idle:
                    Idle();
                    break;
                case GameState.PieceMove:
                    PieceMove();
                    break;
                case GameState.MatchCheck:
                    MatchCheck();
                    break;
                case GameState.DeleteCheck:
                    DeleteCheck();
                    break;
                case GameState.CreateNotes:
                    CreateNotes();
                    break;
                case GameState.DeletePiece:
                    DeletePiece();
                    break;
                case GameState.FillPiece:
                    FillPiece();
                    break;
                case GameState.DeleteNotes:
                    DeleteNotes();
                    break;
                default:
                    break;
            }
        }
        else if(0 < countdown && countdown <= 8) //0~8でリズムphase
        {
            currentState = GameState.MusicTap;
            switch (currentState)
            {
                case GameState.MusicTap:
                    MusicTap();
                    break;
                case GameState.DeleteNotes:
                    DeleteNotes();
                    break;
                default:
                    break;
            }
        }
        else if(-1 < countdown && countdown <= 0)
        {
            currentState = GameState.DeleteNotes;
            switch (currentState)
            {
                case GameState.DeleteNotes:
                    DeleteNotes();
                    break;
                default:
                    break;
            }
        }
        else if(countdown <= -1)
        {
            countdown = 23.0f;
            currentState = GameState.Idle;
        }
        stateText.text = currentState.ToString();
    }

    //-------------------------------------------------------
    // Private Function
    //-------------------------------------------------------
    // プレイヤーの入力を検知し、ピースを選択状態にする
    private void Idle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedPiece = board.GetNearestPiece(Input.mousePosition);
            currentState = GameState.PieceMove;
        }
    }

    // プレイヤーがピースを選択しているときの処理
    private void PieceMove()
    {
        if (Input.GetMouseButton(0))
        {
            NextPiece = board.GetNearestPiece(Input.mousePosition);//ピースに最短距離のピースを代入
            if (NextPiece != selectedPiece)//最短距離のピースと、選択したピースが不一致
            {
                board.SwitchPiece(selectedPiece, NextPiece);//選択したピースと、場所入れ替え
                currentState = GameState.MatchCheck;
            }
        }
        else
        {
            currentState = GameState.MatchCheck;
        }
    }

    // 盤面上にマッチングしているピースがあるかどうかを判断する
    private void MatchCheck()
    {
        if (board.HasMatch())
        {
            currentState = GameState.DeleteCheck;
        }
        else
        {
            currentState = GameState.Idle;
        }
    }

    // マッチングしているピースをにフラグをたてる
    private void DeleteCheck()
    {
        board.DeleteMatchPiece();
        currentState = GameState.CreateNotes;
    }

    //ノーツの生成
    private void CreateNotes()
    {
        // Vector2 position = selectedPiece.transform.position;
        board.CreateNotes(selectedPiece,NextPiece);
        currentState = GameState.DeletePiece;
    }

    //フラグの立ったピース削除
    private void DeletePiece()
    {
        board.DeletePiece();
        currentState = GameState.FillPiece;
    }


    // 盤面上のかけている部分にピースを補充する
    private void FillPiece()
    {
        board.FillPiece();
        currentState = GameState.MatchCheck;
    }

    private void MusicTap()
    {   
        board.BarMovePos(countdown);
        
        if(Input.GetMouseButtonDown(0))//ここDowmだとまずいかも。
        {
            targetPiece = board.GetNearestPiece(Input.mousePosition);
            board.MusicTap(targetPiece);//ノーツの行数の取得
        }

        if(countdown <= 0 )
        {
            currentState = GameState.DeleteNotes;
        }
    }

    private void DeleteNotes()
    {
        board.DeleteNotes();
        board.FillPiece();
    }
}