using System;

namespace MemoryMatchGame.Modules.Card
{
    /// <summary>
    /// Presenter coordinating between CardModel and ICardView.
    /// Implements MVP pattern to separate business logic from UI concerns.
    /// Handles user interactions and coordinates model-view synchronization.
    /// </summary>
    public class CardPresenter
    {
        private readonly CardModel _model;
        private readonly ICardView _view;

        public int CardId => _model.Id;
        
        /// <summary>
        /// Event triggered when the card is flipped by user interaction.
        /// Allows the board controller to react to card selection.
        /// </summary>
        public event Action<int> OnCardFlipped;

        public CardPresenter(CardModel model, ICardView view)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            
            InitializeView();
        }

        /// <summary>
        /// Handles user click/tap interaction on the card.
        /// Validates the action and triggers appropriate events.
        /// </summary>
        public void OnCardClicked()
        {
            if (_model.CanFlip())
            {
                _model.Reveal();
                OnCardFlipped?.Invoke(_model.Id);
            }
        }

        /// <summary>
        /// Instructs the view to show the card with its sprite.
        /// Should be called when the card is revealed.
        /// </summary>
        public void ShowCard()
        {
            _view.ShowCard(_model.SpriteId);
        }

        /// <summary>
        /// Instructs the view to hide the card.
        /// Updates the model state and synchronizes with the view.
        /// </summary>
        public void HideCard()
        {
            _model.Hide();
            _view.HideCard();
        }

        /// <summary>
        /// Marks the card as matched and updates the view accordingly.
        /// Permanently sets the card to matched state.
        /// </summary>
        public void SetMatched()
        {
            _model.SetMatched();
            _view.SetMatched();
        }

        /// <summary>
        /// Triggers the match animation on the view.
        /// Provides positive visual feedback for successful matches.
        /// </summary>
        public void PlayMatchAnimation()
        {
            _view.PlayMatchAnimation();
        }

        /// <summary>
        /// Triggers the mismatch animation on the view.
        /// Provides feedback for unsuccessful match attempts.
        /// </summary>
        public void PlayMismatchAnimation()
        {
            _view.PlayMismatchAnimation();
        }

        /// <summary>
        /// Controls card interactability through the view.
        /// Used to prevent interactions during animations or invalid states.
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            _view.SetInteractable(interactable);
        }

        /// <summary>
        /// Gets the sprite identifier for this card.
        /// Used for match comparison logic.
        /// </summary>
        public string GetSpriteId() => _model.SpriteId;

        /// <summary>
        /// Checks if this card matches another card presenter.
        /// Delegates to the model for business logic.
        /// </summary>
        public bool Matches(CardPresenter other)
        {
            return other != null && _model.Matches(other._model);
        }

        private void InitializeView()
        {
            _view.SetCardId(_model.Id);
            _view.HideCard(); // Start with card hidden
        }
    }
}