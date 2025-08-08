namespace MemoryMatchGame.Modules.Card
{
    /// <summary>
    /// Interface defining the contract for card visual representation.
    /// Separates the visual concerns from the business logic following the Interface Segregation Principle.
    /// Implementations should handle Unity-specific UI components and animations.
    /// </summary>
    public interface ICardView
    {
        /// <summary>
        /// Sets the unique identifier for this card view.
        /// Used to link the view with its corresponding model and presenter.
        /// </summary>
        void SetCardId(int cardId);

        /// <summary>
        /// Displays the card with the specified sprite.
        /// Should show the card face with the given sprite identifier.
        /// </summary>
        void ShowCard(string spriteId);

        /// <summary>
        /// Hides the card by showing its back face.
        /// Should animate or immediately switch to the card back visual.
        /// </summary>
        void HideCard();

        /// <summary>
        /// Sets the card to its matched visual state.
        /// Should provide visual feedback that the card is permanently matched.
        /// </summary>
        void SetMatched();

        /// <summary>
        /// Plays the visual animation for a successful match.
        /// Should provide positive feedback to the player.
        /// </summary>
        void PlayMatchAnimation();

        /// <summary>
        /// Plays the visual animation for an unsuccessful match.
        /// Should provide feedback that the match attempt failed.
        /// </summary>
        void PlayMismatchAnimation();

        /// <summary>
        /// Controls whether the card can be interacted with by the player.
        /// Used to prevent clicks during animations or when cards are already revealed.
        /// </summary>
        void SetInteractable(bool interactable);
    }
}