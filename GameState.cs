using System.Drawing;

public class GameState
{
    public enum WinState
    {
        No_Winner,
        Player1_Wins,
        Player2_Wins,
        Tie
    }

    public int PlayerTurn => CurrentTurn % 2 == 0 ? 1 : 2;
    public int CurrentTurn { get; private set; }
    public bool GameOver { get; private set; }

    private readonly int[,] _board = new int[6, 7];
    private readonly int[] _columnHeights = new int[7];
    private readonly List<string> _moveHistory = new();

    public IReadOnlyList<string> MoveHistory => _moveHistory.AsReadOnly();

    public void ResetBoard()
    {
        for (int row = 0; row < 6; row++)
            for (int col = 0; col < 7; col++)
                _board[row, col] = 0;

        Array.Clear(_columnHeights, 0, _columnHeights.Length);
        _moveHistory.Clear();
        CurrentTurn = 0;
        GameOver = false;
    }

    public int PlayPiece(int col)
    {
        if (col < 0 || col > 6)
            throw new ArgumentException("Invalid column. Choose a column between 1 and 7.");

        if (_columnHeights[col] >= 6)
            throw new ArgumentException($"Column {col + 1} is full. Choose another column.");

        if (GameOver)
            throw new ArgumentException("The game is over. Reset to play again.");

        int row = 5 - _columnHeights[col];
        _board[row, col] = PlayerTurn;
        _columnHeights[col]++;

        // Record the move in history (move history feature)
        _moveHistory.Add($"Turn {CurrentTurn + 1}: Player {PlayerTurn} → Column {col + 1}");

        CurrentTurn++;

        var winState = CheckForWin();
        if (winState != WinState.No_Winner)
            GameOver = true;

        return row;
    }

    public WinState CheckForWin()
    {
        // Check horizontal
        for (int row = 0; row < 6; row++)
            for (int col = 0; col < 4; col++)
                if (CheckLine(row, col, 0, 1))
                    return _board[row, col] == 1 ? WinState.Player1_Wins : WinState.Player2_Wins;

        // Check vertical
        for (int row = 0; row < 3; row++)
            for (int col = 0; col < 7; col++)
                if (CheckLine(row, col, 1, 0))
                    return _board[row, col] == 1 ? WinState.Player1_Wins : WinState.Player2_Wins;

        // Check diagonal (down-right)
        for (int row = 0; row < 3; row++)
            for (int col = 0; col < 4; col++)
                if (CheckLine(row, col, 1, 1))
                    return _board[row, col] == 1 ? WinState.Player1_Wins : WinState.Player2_Wins;

        // Check diagonal (down-left)
        for (int row = 0; row < 3; row++)
            for (int col = 3; col < 7; col++)
                if (CheckLine(row, col, 1, -1))
                    return _board[row, col] == 1 ? WinState.Player1_Wins : WinState.Player2_Wins;

        // Check for tie
        if (_columnHeights.All(h => h >= 6))
            return WinState.Tie;

        return WinState.No_Winner;
    }

    private bool CheckLine(int row, int col, int rowDelta, int colDelta)
    {
        int player = _board[row, col];
        if (player == 0) return false;

        for (int i = 1; i < 4; i++)
        {
            if (_board[row + i * rowDelta, col + i * colDelta] != player)
                return false;
        }
        return true;
    }
}