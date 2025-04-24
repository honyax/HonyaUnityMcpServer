using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ゲームボードの大きさ
    public int boardWidth = 10;
    public int boardHeight = 20;

    // テトリミノのプレハブ
    public GameObject iPrefab;
    public GameObject tPrefab;
    public GameObject oPrefab;

    // 現在落下中のテトリミノ
    private GameObject currentTetrimino;
    
    // ゲームボード（テトリミノのあるマス: true, 空マス: false）
    private bool[,] board;
    
    // Start is called before the first frame update
    void Start()
    {
        // ゲームボードの初期化
        board = new bool[boardWidth, boardHeight];
        
        // 最初のテトリミノを生成
        SpawnTetrimino();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTetrimino != null)
        {
            // テトリミノの操作
            MoveTetrimino();
        }
    }
    
    // ランダムなテトリミノを生成
    void SpawnTetrimino()
    {
        // ランダムに選ぶ
        int randomIndex = Random.Range(0, 3);
        
        Vector3 spawnPosition = new Vector3(boardWidth / 2, boardHeight - 2, 0);
        
        switch (randomIndex)
        {
            case 0:
                currentTetrimino = Instantiate(iPrefab, spawnPosition, Quaternion.identity);
                break;
            case 1:
                currentTetrimino = Instantiate(tPrefab, spawnPosition, Quaternion.identity);
                break;
            case 2:
                currentTetrimino = Instantiate(oPrefab, spawnPosition, Quaternion.identity);
                break;
        }
        
        // テトリミノに名前を設定
        currentTetrimino.name = "CurrentTetrimino";
        
        // テトリミノをゲームボード上に配置できない場合はゲームオーバー
        if (!IsValidPosition())
        {
            Destroy(currentTetrimino);
            Debug.Log("Game Over!");
            return;
        }
    }
    
    // テトリミノの移動と回転
    void MoveTetrimino()
    {
        // 左への移動
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveTetriminoHorizontal(-1);
        }
        
        // 右への移動
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveTetriminoHorizontal(1);
        }
        
        // 回転
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateTetrimino();
        }
        
        // 早く落とす
        if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= fallTime)
        {
            MoveTetriminoDown();
        }
    }
    
    private float lastFall = 0;
    private float fallTime = 1.0f;
    
    // テトリミノを水平方向に移動
    void MoveTetriminoHorizontal(int direction)
    {
        // 仮に移動させる
        currentTetrimino.transform.position += new Vector3(direction, 0, 0);
        
        // 移動が有効かチェック
        if (IsValidPosition())
        {
            // 有効なら移動確定
        }
        else
        {
            // 無効なら元の位置に戻す
            currentTetrimino.transform.position -= new Vector3(direction, 0, 0);
        }
    }
    
    // テトリミノを下に移動
    void MoveTetriminoDown()
    {
        lastFall = Time.time;
        
        // 仮に下に移動
        currentTetrimino.transform.position += new Vector3(0, -1, 0);
        
        // 移動が有効かチェック
        if (IsValidPosition())
        {
            // 有効なら移動確定
        }
        else
        {
            // 無効なら元の位置に戻し、テトリミノを固定
            currentTetrimino.transform.position -= new Vector3(0, -1, 0);
            LandTetrimino();
        }
    }
    
    // テトリミノを回転
    void RotateTetrimino()
    {
        // O型は回転しない
        if (currentTetrimino.name.Contains("O"))
        {
            return;
        }
        
        // 回転する
        currentTetrimino.transform.Rotate(0, 0, 90);
        
        // 回転が有効かチェック
        if (IsValidPosition())
        {
            // 有効なら回転確定
        }
        else
        {
            // 無効なら元に戻す
            currentTetrimino.transform.Rotate(0, 0, -90);
        }
    }
    
    // テトリミノの現在の位置が有効か
    bool IsValidPosition()
    {
        if (currentTetrimino == null) return false;
        
        foreach (Transform child in currentTetrimino.transform)
        {
            // ブロックの位置をゲームボードの座標に変換
            Vector2 pos = RoundToGrid(child.position);
            
            // ゲームボードの外ならfalse
            if (pos.x < 0 || pos.x >= boardWidth || pos.y < 0)
            {
                return false;
            }
            
            // 天井より上ならtrue（スポーン時など）
            if (pos.y >= boardHeight)
            {
                continue;
            }
            
            // 既にブロックがあるマスならfalse
            if (board[(int)pos.x, (int)pos.y])
            {
                return false;
            }
        }
        
        return true;
    }
    
    // 落下しきったテトリミノを固定
    void LandTetrimino()
    {
        foreach (Transform child in currentTetrimino.transform)
        {
            Vector2 pos = RoundToGrid(child.position);
            
            // ゲームボード内に収まっていることを確認
            if (pos.y < boardHeight && pos.y >= 0 && pos.x >= 0 && pos.x < boardWidth)
            {
                board[(int)pos.x, (int)pos.y] = true;
            }
        }
        
        // ラインクリア判定
        CheckForLines();
        
        // 次のテトリミノを生成
        Destroy(currentTetrimino);
        SpawnTetrimino();
    }
    
    // ライン消去判定
    void CheckForLines()
    {
        for (int y = 0; y < boardHeight; y++)
        {
            bool lineIsFull = true;
            
            for (int x = 0; x < boardWidth; x++)
            {
                if (!board[x, y])
                {
                    lineIsFull = false;
                    break;
                }
            }
            
            if (lineIsFull)
            {
                // ラインを消去
                ClearLine(y);
                
                // 上のラインを下に移動
                MoveDownAllLinesAbove(y);
                
                // 同じラインを再チェック（消去後に新しいラインができる可能性）
                y--;
            }
        }
    }
    
    // ラインを消去
    void ClearLine(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            board[x, y] = false;
            
            // 表示されているオブジェクトも削除
            Vector3 pos = new Vector3(x, y, 0);
            GameObject block = GetBlockAtPosition(pos);
            if (block != null)
            {
                Destroy(block);
            }
        }
    }
    
    // 指定位置にあるブロックを取得
    GameObject GetBlockAtPosition(Vector3 position)
    {
        // シーン内のすべてのテトリミノブロック（子オブジェクト）を検索
        GameObject[] allBlocks = GameObject.FindGameObjectsWithTag("TetriminoBlock");
        
        foreach (GameObject block in allBlocks)
        {
            // 位置が一致するブロックを返す
            if (RoundToGrid(block.transform.position) == RoundToGrid(position))
            {
                return block;
            }
        }
        
        return null;
    }
    
    // 指定したライン以上のラインを下に移動
    void MoveDownAllLinesAbove(int clearedLine)
    {
        for (int y = clearedLine + 1; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                if (board[x, y])
                {
                    board[x, y] = false;
                    board[x, y - 1] = true;
                    
                    // 表示されているオブジェクトも移動
                    Vector3 oldPos = new Vector3(x, y, 0);
                    GameObject block = GetBlockAtPosition(oldPos);
                    if (block != null)
                    {
                        block.transform.position += new Vector3(0, -1, 0);
                    }
                }
            }
        }
    }
    
    // 座標をグリッドに合わせて丸める
    Vector2 RoundToGrid(Vector3 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
}
