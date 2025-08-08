using MemoryMatchGame.Core.Configuration;
using MemoryMatchGame.Modules.Card;

namespace MemoryMatchGame.Modules.Board
{
    /// <summary>
    /// Interface defining the contract for board visual representation.
    /// Handles board-level UI operations and card view creation.
    /// Separates board presentation logic from business logic following Interface Segregation Principle.
    /// </summary>
    public interface IBoardView
    {
        /// <summary>
        /// Initializes the board visual layout based on configuration.
        /// Should create the grid structure and prepare for card placement.
        /// </summary>
        void SetupBoard(BoardConfiguration config);

        /// <summary>
        /// Clears the board of all card views and resets visual state.
        /// Used when resetting or changing levels.
        /// </summary>
        void ClearBoard();

        /// <summary>
        /// Creates a new card view instance for the specified card ID.
        /// Should position the card appropriately within the board grid.
        /// </summary>
        ICardView CreateCardView(int cardId);

        /// <summary>
        /// Updates the visual display of the current score.
        /// Should provide immediate feedback to the player about their performance.
        /// </summary>
        void UpdateScore(int score);

        /// <summary>
        /// Updates the visual display of the current error count.
        /// Provides feedback about mistakes made during gameplay.
        /// </summary>
        void UpdateErrorCount(int errorCount);

        /// <summary>
        /// Displays the game won screen with the final score.
        /// Should show celebration visuals and final score presentation.
        /// </summary>
        void ShowGameWon(int finalScore);

        /// <summary>
        /// Enables or disables board-level interactions.
        /// Used to prevent input during animations or game state transitions.
        /// </summary>
        void SetBoardInteractable(bool interactable);
    }
}