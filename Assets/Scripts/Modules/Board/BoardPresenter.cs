using System;
using System.Collections.Generic;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using MemoryMatchGame.Core.Configuration;
using MemoryMatchGame.Events;
using MemoryMatchGame.Modules.Card;

namespace MemoryMatchGame.Modules.Board
{
    /// <summary>
    /// Presenter coordinating the board's model and view with the messaging system.
    /// Implements MVP pattern and integrates with VContainer for dependency injection.
    /// Handles card interactions, match processing, and event publishing.
    /// </summary>
    public class BoardPresenter : IInitializable, IDisposable
    {
        private readonly BoardModel _model;
        private readonly IBoardView _view;
        private readonly IPublisher<BoardInitializedEvent> _boardInitializedPublisher;
        private readonly IPublisher<CardRevealedEvent> _cardRevealedPublisher;
        private readonly IPublisher<CardsMatchedEvent> _cardsMatchedPublisher;
        private readonly IPublisher<CardsMismatchEvent> _cardsMismatchPublisher;
        private readonly IPublisher<GameWonEvent> _gameWonPublisher;

        private Dictionary<int, CardPresenter> _cardPresenters;
        private bool _processingMatch;

        [Inject]
        public BoardPresenter(
            BoardModel model,
            IBoardView view,
            IPublisher<BoardInitializedEvent> boardInitializedPublisher,
            IPublisher<CardRevealedEvent> cardRevealedPublisher,
            IPublisher<CardsMatchedEvent> cardsMatchedPublisher,
            IPublisher<CardsMismatchEvent> cardsMismatchPublisher,
            IPublisher<GameWonEvent> gameWonPublisher)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _boardInitializedPublisher = boardInitializedPublisher ?? throw new ArgumentNullException(nameof(boardInitializedPublisher));
            _cardRevealedPublisher = cardRevealedPublisher ?? throw new ArgumentNullException(nameof(cardRevealedPublisher));
            _cardsMatchedPublisher = cardsMatchedPublisher ?? throw new ArgumentNullException(nameof(cardsMatchedPublisher));
            _cardsMismatchPublisher = cardsMismatchPublisher ?? throw new ArgumentNullException(nameof(cardsMismatchPublisher));
            _gameWonPublisher = gameWonPublisher ?? throw new ArgumentNullException(nameof(gameWonPublisher));

            _cardPresenters = new Dictionary<int, CardPresenter>();
            _processingMatch = false;
        }

        public void Initialize()
        {
            // Initialize any required subscriptions or setup
        }

        /// <summary>
        /// Initializes the board with the given configuration and theme.
        /// Creates card presenters and sets up the board view.
        /// </summary>
        public void InitializeBoard(BoardConfiguration config, ThemeConfiguration theme)
        {
            try
            {
                _model.Initialize(config, theme);
                CreateCardPresenters();
                _view.SetupBoard(config);
                
                UpdateUI();
                _boardInitializedPublisher.Publish(new BoardInitializedEvent(config));
            }
            catch (ArgumentException ex)
            {
                // Log error and handle gracefully
                UnityEngine.Debug.LogError($"Board initialization failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Handles card flip events from card presenters.
        /// Manages the flow of card reveal, match checking, and event publishing.
        /// </summary>
        public void OnCardFlipped(int cardId)
        {
            if (_processingMatch || !_cardPresenters.ContainsKey(cardId))
                return;

            var card = _model.GetCard(cardId);
            if (card == null) return;

            _model.AddRevealedCard(cardId);
            
            var cardPresenter = _cardPresenters[cardId];
            cardPresenter.ShowCard();
            
            _cardRevealedPublisher.Publish(new CardRevealedEvent(cardId, card.SpriteId));

            if (_model.CanProcessMatch())
            {
                _processingMatch = true;
                ProcessMatchAsync();
            }
        }

        /// <summary>
        /// Resets the board to its initial state.
        /// Clears all presenters and resets the view.
        /// </summary>
        public void ResetBoard()
        {
            _model.Reset();
            
            foreach (var presenter in _cardPresenters.Values)
            {
                presenter.OnCardFlipped -= OnCardFlipped;
            }
            
            _cardPresenters.Clear();
            _view.ClearBoard();
            _processingMatch = false;
        }

        public void Dispose()
        {
            ResetBoard();
        }

        private void CreateCardPresenters()
        {
            _cardPresenters.Clear();
            var cards = _model.GetAllCards();

            foreach (var cardModel in cards)
            {
                var cardView = _view.CreateCardView(cardModel.Id);
                var cardPresenter = new CardPresenter(cardModel, cardView);
                cardPresenter.OnCardFlipped += OnCardFlipped;
                _cardPresenters[cardModel.Id] = cardPresenter;
            }
        }

        private async void ProcessMatchAsync()
        {
            // Disable board interactions during processing
            _view.SetBoardInteractable(false);
            
            // Small delay to let card reveal animations complete
            await Cysharp.Threading.Tasks.UniTask.Delay(300);
            
            var revealedIds = _model.GetRevealedCardIds();
            
            if (_model.CheckMatch())
            {
                await HandleSuccessfulMatch(revealedIds);
            }
            else
            {
                await HandleFailedMatch(revealedIds);
            }

            UpdateUI();
            
            // Re-enable interactions
            _view.SetBoardInteractable(true);
            _processingMatch = false;
        }

        private async Cysharp.Threading.Tasks.UniTask HandleSuccessfulMatch(int[] revealedIds)
        {
            // Set cards as matched
            _model.SetCardsMatched();
            
            // Play match animations
            foreach (var cardId in revealedIds)
            {
                _cardPresenters[cardId].SetMatched();
                _cardPresenters[cardId].PlayMatchAnimation();
            }

            // Publish match event
            _cardsMatchedPublisher.Publish(new CardsMatchedEvent(revealedIds, _model.Score));

            // Wait for animations
            await Cysharp.Threading.Tasks.UniTask.Delay(500);

            // Check for game won
            if (_model.IsGameWon())
            {
                _gameWonPublisher.Publish(new GameWonEvent(_model.Score));
                _view.ShowGameWon(_model.Score);
            }
        }

        private async Cysharp.Threading.Tasks.UniTask HandleFailedMatch(int[] revealedIds)
        {
            // Play mismatch animations
            foreach (var cardId in revealedIds)
            {
                _cardPresenters[cardId].PlayMismatchAnimation();
            }

            // Wait for mismatch animation
            await Cysharp.Threading.Tasks.UniTask.Delay(300);

            // Hide cards
            _model.HideRevealedCards();
            foreach (var cardId in revealedIds)
            {
                _cardPresenters[cardId].HideCard();
            }

            // Publish mismatch event
            _cardsMismatchPublisher.Publish(new CardsMismatchEvent(revealedIds, _model.ErrorCount));
        }

        private void UpdateUI()
        {
            _view.UpdateScore(_model.Score);
            _view.UpdateErrorCount(_model.ErrorCount);
        }
    }
}