using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleGameManager : MonoBehaviour
{
    //const（定数）
    public const int MachingCount = 3;

    //enum
    private enum GameState
    {
        Idle,
        PieceMove,
        MatchCheck,
        DeletePiece,
        FillPiece,
    }

    //serialize(privateだけどコンポーネントで変更できる)
    [SerializeField]private Board board;
    [SerializeField]private Text stateText;

    //---------------------------
    private GameState currentState;
    private Piece selectedPiece;
    //----------------------------

    //ゲームの初期化
    private void Start()
    {
        board.InitializeBoard( 6, 5 );
        currentState = GameState.Idle;
    }

    //パズルゲームのターンループ
    private void Update()
    {
        switch(currentState)
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
            case GameState.FillPiece:
                FillPiece();
                break;
            default:
                break;
        }
        stateText.text = currentState.ToString();
    }

    //---------------------------------------------

    //何もしない状態
    private void Idle()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectedPiece = board.GetNearestPiece(Input.mousePosition);
            currentState = GameState.PieceMove;
        }
    }

    //
    private void PieceMove()
    {
        if(Input.GetMouseButton(0))
        {
            var piece = board.GetNearestPiece(Input.mousePosition);
            if(piece != selectedPiece)
            {
                board.SwitchPiece(selectedPiece, piece);
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            currentState = GameState.MatchCheck;
        }
    }

    //マッチングしてるピースがあるか判断
    private void MatchCheck()
    {
        if(board.HasMatch())
        {
            currentState = GameState.DeletePiece;
        }
        else
        {
            currentState = GameState.Idle;
        }
    }

    //マッチングしているピースを削除
    private void DeletePiece()
    {
        board.DeleteMatchPiece();
        currentState = GameState.FillPiece;
    }

    //欠けているところにピース補充
    private void FillPiece()
    {
        board.FillPiece();
        currentState = GameState.MatchCheck;
    }

}
