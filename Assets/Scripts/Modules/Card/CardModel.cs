using MemoryMatchGame.Core.Enums;

namespace MemoryMatchGame.Modules.Card
{
    /// <summary>
    /// Model representing the data and business logic of a single card.
    /// Encapsulates card identity, visual reference, and state management.
    /// Follows Single Responsibility Principle by handling only card data logic.
    /// </summary>
    public class CardModel
    {
        public int Id { get; }
        public string SpriteId { get; }
        public CardState State { get; private set; }

        public CardModel(int id, string spriteId)
        {
            Id = id;
            SpriteId = spriteId;
            State = CardState.Hidden;
        }

        /// <summary>
        /// Determines if the card can be flipped based on its current state.
        /// Only hidden cards can be flipped.
        /// </summary>
        public bool CanFlip() => State == CardState.Hidden;

        /// <summary>
        /// Reveals the card by changing its state to Revealed.
        /// Should only be called after validating CanFlip().
        /// </summary>
        public void Reveal()
        {
            if (CanFlip())
            {
                State = CardState.Revealed;
            }
        }

        /// <summary>
        /// Hides the card by changing its state back to Hidden.
        /// Used when cards don't match and need to be hidden again.
        /// </summary>
        public void Hide()
        {
            if (State == CardState.Revealed)
            {
                State = CardState.Hidden;
            }
        }

        /// <summary>
        /// Permanently matches the card by setting its state to Matched.
        /// Matched cards cannot be hidden or revealed again.
        /// </summary>
        public void SetMatched()
        {
            if (State == CardState.Revealed)
            {
                State = CardState.Matched;
            }
        }

        /// <summary>
        /// Checks if this card matches another card based on sprite identity.
        /// </summary>
        public bool Matches(CardModel other)
        {
            return other != null && SpriteId == other.SpriteId;
        }
    }
}