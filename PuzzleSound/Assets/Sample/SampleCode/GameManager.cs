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
        DeletePiece,
        FillPiece,
        MusicTap,
        FillPieceAfterMusic,
    }

    // serialize field.
    [SerializeField]
    private Board board;
    [SerializeField]
    private Text stateText;

    // private.
    private GameState currentState;
    private Piece selectedPiece;

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
        Debug.Log(countdown);
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
                case GameState.DeletePiece:
                    DeletePiece();
                    break;
                // case GameState.CreateNotes:
                //     CreateNotes();
                //     break;
                case GameState.FillPiece:
                    FillPiece();
                    break;
                default:
                    break;
            }
        }
        // else if(0 < countdown && countdown <= 8) 0~8でリズムphase
        // {
        //     switch (currentState)
        //     {
        //         case GameState.MusicTap:
        //             MusicTap();
        //             break;
        //         default:
        //             break;
        //     }
        // }
        else if(countdown <= 0)
        {
            countdown = 24.0f;
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
            var piece = board.GetNearestPiece(Input.mousePosition);//ピースに最短距離のピースを代入
            if (piece != selectedPiece)//最短距離のピースと、選択したピースが不一致
            {
                board.SwitchPiece(selectedPiece, piece);//選択したピースと、場所入れ替え
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
            currentState = GameState.DeletePiece;
        }
        else
        {
            currentState = GameState.Idle;
        }
    }

    // マッチングしているピースを削除する
    private void DeletePiece()
    {
        board.DeleteMatchPiece();
        currentState = GameState.FillPiece;
    }

    //削除後さっき動かした場所にノーツを生成
    //さっき動かした場所をどう指定すればよいのかが不明selectPeaceっぽい？

    // 盤面上のかけている部分にピースを補充する
    private void FillPiece()
    {
        board.FillPiece();
        currentState = GameState.MatchCheck;
    }

    private void MusicTap()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0 )
        {
            currentState = GameState.MatchCheck;
        }
    }
}