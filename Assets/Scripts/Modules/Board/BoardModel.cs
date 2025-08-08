using System;
using System.Collections.Generic;
using System.Linq;
using MemoryMatchGame.Core.Configuration;
using MemoryMatchGame.Modules.Card;

namespace MemoryMatchGame.Modules.Board
{
    /// <summary>
    /// Model representing the game board state and business logic.
    /// Manages card collection, match validation, scoring, and game state.
    /// Follows Single Responsibility Principle by handling only board-level data logic.
    /// </summary>
    public class BoardModel
    {
        private Dictionary<int, CardModel> _cards;
        private List<int> _revealedCardIds;
        private BoardConfiguration _config;

        public int MatchedPairs { get; private set; }
        public int ErrorCount { get; private set; }
        public int Score { get; private set; }

        public BoardModel()
        {
            _cards = new Dictionary<int, CardModel>();
            _revealedCardIds = new List<int>();
        }

        /// <summary>
        /// Initializes the board with the given configuration and theme.
        /// Creates and shuffles cards based on the provided parameters.
        /// </summary>
        public void Initialize(BoardConfiguration config, ThemeConfiguration theme)
        {
            if (!config.IsValidConfiguration())
            {
                throw new ArgumentException("Invalid board configuration", nameof(config));
            }

            if (!theme.IsValid() || !theme.HasSufficientSprites(config))
            {
                throw new ArgumentException("Invalid or insufficient theme configuration", nameof(theme));
            }

            _config = config;
            CreateCards(theme);
            ResetGameState();
        }

        /// <summary>
        /// Gets all card models in the board.
        /// Returns a copy to maintain encapsulation.
        /// </summary>
        public CardModel[] GetAllCards()
        {
            return _cards.Values.ToArray();
        }

        /// <summary>
        /// Retrieves a specific card model by its ID.
        /// Returns null if the card doesn't exist.
        /// </summary>
        public CardModel GetCard(int cardId)
        {
            return _cards.TryGetValue(cardId, out var card) ? card : null;
        }

        /// <summary>
        /// Adds a card to the revealed cards collection.
        /// Used for tracking cards that are currently face-up.
        /// </summary>
        public void AddRevealedCard(int cardId)
        {
            if (_cards.ContainsKey(cardId) && !_revealedCardIds.Contains(cardId))
            {
                _revealedCardIds.Add(cardId);
            }
        }

        /// <summary>
        /// Checks if enough cards are revealed to process a match attempt.
        /// Based on the match requirement configuration.
        /// </summary>
        public bool CanProcessMatch()
        {
            return _revealedCardIds.Count == _config.MatchRequirement;
        }

        /// <summary>
        /// Validates if the currently revealed cards form a match.
        /// All revealed cards must have the same sprite ID.
        /// </summary>
        public bool CheckMatch()
        {
            if (_revealedCardIds.Count != _config.MatchRequirement)
                return false;

            if (_revealedCardIds.Count == 0)
                return false;

            var firstCardSprite = _cards[_revealedCardIds[0]].SpriteId;
            return _revealedCardIds.All(id => _cards[id].SpriteId == firstCardSprite);
        }

        /// <summary>
        /// Gets the IDs of currently revealed cards.
        /// Returns a copy to maintain encapsulation.
        /// </summary>
        public int[] GetRevealedCardIds()
        {
            return _revealedCardIds.ToArray();
        }

        /// <summary>
        /// Marks the currently revealed cards as matched.
        /// Updates score and clears the revealed cards collection.
        /// </summary>
        public void SetCardsMatched()
        {
            foreach (var cardId in _revealedCardIds)
            {
                _cards[cardId].SetMatched();
            }

            MatchedPairs++;
            Score += CalculateMatchScore();
            _revealedCardIds.Clear();
        }

        /// <summary>
        /// Hides the currently revealed cards after a failed match.
        /// Updates error count and clears the revealed cards collection.
        /// </summary>
        public void HideRevealedCards()
        {
            foreach (var cardId in _revealedCardIds)
            {
                _cards[cardId].Hide();
            }

            ErrorCount++;
            _revealedCardIds.Clear();
        }

        /// <summary>
        /// Checks if the game has been won.
        /// Game is won when all possible pairs have been matched.
        /// </summary>
        public bool IsGameWon()
        {
            var totalPairs = _config.TotalCards / 2;
            return MatchedPairs >= totalPairs;
        }

        /// <summary>
        /// Resets the board to its initial state.
        /// Clears all cards and game state variables.
        /// </summary>
        public void Reset()
        {
            _cards.Clear();
            _revealedCardIds.Clear();
            ResetGameState();
        }

        private void ResetGameState()
        {
            MatchedPairs = 0;
            ErrorCount = 0;
            Score = 0;
        }

        private void CreateCards(ThemeConfiguration theme)
        {
            _cards.Clear();
            var pairs = _config.TotalCards / 2;
            var cardData = new List<string>();

            // Create pairs
            for (int i = 0; i < pairs; i++)
            {
                var spriteId = theme.CardSprites[i % theme.CardSprites.Count];
                cardData.Add(spriteId);
                cardData.Add(spriteId);
            }

            // Shuffle using Fisher-Yates algorithm
            ShuffleCards(cardData);

            // Create card models
            for (int i = 0; i < cardData.Count; i++)
            {
                _cards[i] = new CardModel(i, cardData[i]);
            }
        }

        private void ShuffleCards(List<string> cardData)
        {
            for (int i = cardData.Count - 1; i > 0; i--)
            {
                var randomIndex = UnityEngine.Random.Range(0, i + 1);
                (cardData[i], cardData[randomIndex]) = (cardData[randomIndex], cardData[i]);
            }
        }

        private int CalculateMatchScore()
        {
            const int baseScore = 100;
            const int errorPenalty = 10;
            const int minimumScore = 10;
            
            return Math.Max(baseScore - (ErrorCount * errorPenalty), minimumScore);
        }
    }
}