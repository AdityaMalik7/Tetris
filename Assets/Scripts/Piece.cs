using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private bool isInitialized = false;
    private bool isLocked = false;

    public float fallTime = 1.0f; // Time between automatic drops
    private float fallTimer;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.isLocked = false;

        if (this.cells == null || this.cells.Length != data.cells.Length)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }

        this.fallTimer = this.fallTime;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || isLocked) return;

        fallTimer -= Time.deltaTime;

        this.board.Clear(this);

        HandleInput();

        // Natural gravity fall
        if (fallTimer <= 0f)
        {
            if (!Move(Vector2Int.down))
            {
                Lock();
                return;
            }

            fallTimer = fallTime; // Reset timer after successful fall
        }

        this.board.Set(this);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!Move(Vector2Int.down))
            {
                Lock();
            }
            else
            {
                fallTimer = fallTime; // Reset timer if moved manually
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);
        if (valid)
        {
            this.position = newPosition;
        }

        return valid;
    }

    private void Lock()
    {
        isLocked = true;
        this.board.Set(this);      // Fix the piece on the board
        this.board.SpawnPiece();   // Spawn a new one
    }
}
