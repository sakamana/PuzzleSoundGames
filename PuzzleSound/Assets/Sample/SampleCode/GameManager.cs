using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲーム管理クラス
public class GameManager : MonoBehaviour {

    // const.
    public const int MachingCount = 3;

    public float countdown = 5.0f;
    public int countdownTriger = 0;

    // enum.
    private enum GameState
    {
        Idle,
        PieceMove,
        MatchCheck,
        DeletePiece,
        FillPiece,
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
        board.InitializeBoard(6, 6);//ここ数字でやってるの凄い嫌なのはここいじればええんちゃう？

        currentState = GameState.Idle;
    }

    // ゲームのメインループ
    private void Update()
    {
        switch (currentState)
        {
            case GameState.Idle:
                countdown = 5.0f;
                countdownTriger = 0;
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
            case GameState.FillPiece:
                FillPiece();
                break;
            default:
                break;
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
            selectedPiece = board.GetNearestPiece(Input.mousePosition); //ピースがアロンアルファ問題ここかも
            currentState = GameState.PieceMove;
        }
    }

    // プレイヤーがピースを選択しているときの処理、入力終了を検知したら盤面のチェックの状態に移行する

    // ここを制限時間制にしたいけど、updateの外なのでここではカウントダウン出来ない？
    private void PieceMove()
    {
            countdownTriger = 1;
            if(countdownTriger == 1)
            {
                countdown -= Time.deltaTime;
                Debug.Log(countdown);
                if (Input.GetMouseButton(0))
                {
                    var piece = board.GetNearestPiece(Input.mousePosition);//ピースに最短距離のピースを代入
                    if (piece != selectedPiece)//最短距離のピースと、選択したピースが不一致
                    {
                        board.SwitchPiece(selectedPiece, piece);//選択したピースと、場所入れ替え
                    }
                }
                else if (countdown <= 0)
                {
                    currentState = GameState.MatchCheck;
                }
            }
        //この中で、nearestPieceをマウスクリックの度に初期化したい。
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

    // 盤面上のかけている部分にピースを補充する
    private void FillPiece()
    {
        board.FillPiece();
        currentState = GameState.MatchCheck;
    }
}