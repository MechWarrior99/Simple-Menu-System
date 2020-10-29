using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectMenuZ
{
    [AddComponentMenu("UI/Canvas Menu")]
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasMenu : MonoBehaviour
    {
        public enum TransitionType { None, Animation, Custom }

        private CanvasGroup _canvasGroup;
        private Animator _animator;
        private readonly Stack<CanvasMenu> _nextMenus = new Stack<CanvasMenu>();
        private readonly Stack<CanvasMenu> _previousMenus = new Stack<CanvasMenu>();

        [HideInInspector]
        [SerializeField] private bool _isOpen = false;
        [HideInInspector]
        [SerializeField] private bool _ignoreHistoryChange = false;
        [SerializeField] private TransitionType _openTransitionType;
        [SerializeField] private TransitionType _closeTransitionType;
        [Tooltip("The name of the Trigger animation parameter responsible for triggering the open state.")]
        [SerializeField] private string _openTriggerName = "OpenTrigger";
        [Tooltip("The name of the Trigger animation parameter responsible for triggering the close state.")]
        [SerializeField] private string _closeTriggerName = "CloseTrigger";
        [Tooltip("The name of the Animation State that is responsible for closing the menu.")]
        [SerializeField] private string _closeStateName = "Close";
        [SerializeField] private UnityEvent _onOpened = new UnityEvent();
        [SerializeField] private UnityEvent _onClosed = new UnityEvent();

        /// <summary>
        /// The open transition <see cref="Coroutine"/> to play when <see cref="OpenTransitionType"/> is set to <see cref="TransitionType.Custom"/>
        /// </summary>
        public Func<IEnumerator> OpenTransition
        {
            get;
            set;
        }

        /// <summary>
        /// The close transition <see cref="Coroutine"/> to play when <see cref="CloseTransitionType"/> is set to <see cref="TransitionType.Custom"/>
        /// </summary>
        public Func<IEnumerator> CloseTransition
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if the <see cref="CanvasMenu"/> is open.
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// The type of transition to use when opening the <see cref="CanvasMenu"/>.
        /// </summary>
        public TransitionType OpenTransitionType
        {
            get { return _openTransitionType; }
            set { _openTransitionType = value; }
        }

        /// <summary>
        /// The type of transition to use when closing the <see cref="CanvasMenu"/>.
        /// </summary>
        public TransitionType CloseTransitionType
        {
            get { return _closeTransitionType; }
            set { _closeTransitionType = value; }
        }

        /// <summary>
        /// Callback for when the <see cref="CanvasMenu"/> is opened.
        /// </summary>
        public UnityEvent OnOpened
        {
            get { return _onOpened; }
            set { _onOpened = value; }
        }

        /// <summary>
        /// Callback for when the <see cref="CanvasMenu"/> is closed.
        /// </summary>
        public UnityEvent OnClosed
        {
            get { return _onClosed; }
            set { _onClosed = value; }
        }

        /// <summary>
        /// Callback for when a transition between two <see cref="CanvasMenu"/>s occurs via <see cref="TransitionTo(CanvasMenu)"/>.
        /// </summary>
        public static event Action<CanvasMenu, CanvasMenu> Transitioned;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _animator = GetComponent<Animator>();
            MenuManager.CanvasMenus.Add(this);
        }

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {
            MenuManager.CanvasMenus.Remove(this);
        }

        /// <summary>
        /// Opens the <see cref="CanvasMenu"/>, playing the open transition. Does not record history.
        /// </summary>
        public virtual void Open()
        {
            OpenImmediate();

            if (_openTransitionType == TransitionType.Animation)
            {
                _animator.SetTrigger(_openTriggerName);
            }
            else if (_openTransitionType == TransitionType.Custom && OpenTransition != null)
            {
                StartCoroutine(OpenTransition());
            }
        }

        /// <summary>
        /// Opens the <see cref="CanvasMenu"/> without playing the open transition. Does not record history.
        /// </summary>
        public void OpenImmediate()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            _isOpen = true;
            if (!MenuManager.OpenCanvasMenus.Contains(this))
                MenuManager.OpenCanvasMenus.Add(this);

            OnOpened.Invoke();
        }

        /// <summary>
        /// Closes the <see cref="CanvasMenu"/>, plays closing transition if any before closing. Does not record history.
        /// </summary>
        public virtual void Close()
        {
            if (_closeTransitionType == TransitionType.Animation)
            {
                StartCoroutine(AnimationTransitionClose());
            }
            else if (_closeTransitionType == TransitionType.Custom && CloseTransition != null)
            {
                StartCoroutine(CustomTransitionClose());
            }
            else
            {
                CloseImmediate();
            }
        }

        /// <summary>
        /// Closes the <see cref="CanvasMenu"/> without playing the close transition. Does not record history.
        /// </summary>
        public void CloseImmediate()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _isOpen = false;
            MenuManager.OpenCanvasMenus.Remove(this);

            OnClosed.Invoke();
        }

        /// <summary>
        /// Transitions to the previous <see cref="CanvasMenu"/> in history if any.
        /// </summary>
        public void Back()
        {
            if (ValidateBack())
            {
                _previousMenus.Peek()._nextMenus.Push(this);

                _ignoreHistoryChange = true;
                TransitionTo(_previousMenus.Pop());
            }
        }

        /// <summary>
        /// Transitions to the next <see cref="CanvasMenu"/> in history if any.
        /// </summary>
        public void Forward()
        {
            if (ValidateForward())
            {
                _nextMenus.Peek()._previousMenus.Push(this);

                _ignoreHistoryChange = true;
                TransitionTo(_nextMenus.Pop());
            }
        }

        /// <summary>
        /// Returns true if there are any <see cref="CanvasMenu"/>s to go back to in history.
        /// </summary>
        /// <returns></returns>
        public bool ValidateBack()
        {
            return _previousMenus.Count > 0;
        }

        /// <summary>
        /// Returns true if there are any <see cref="CanvasMenu"/>s to go forward to in history.
        /// </summary>
        /// <returns></returns>
        public bool ValidateForward()
        {
            return _nextMenus.Count > 0;
        }

        /// <summary>
        /// Transitions from the <see cref="CanvasMenu"/> to <paramref name="to"/>, records history.
        /// </summary>
        /// <param name="to"></param>
        public void TransitionTo(CanvasMenu to)
        {
            to.RegisterHistoryChange(this);
            to.Open();
            Close();

            Transitioned?.Invoke(this, to);
        }



        private IEnumerator AnimationTransitionClose()
        {
            _animator.SetTrigger(_closeTriggerName);

            // Wait for the close animation to finish playing.
            bool closedStateReached = false;
            while (!closedStateReached)
            {
                if (!_animator.IsInTransition(0))
                    closedStateReached = _animator.GetCurrentAnimatorStateInfo(0).IsName(_closeStateName);

                yield return new WaitForEndOfFrame();
            }

            CloseImmediate();
        }

        private IEnumerator CustomTransitionClose()
        {
            yield return StartCoroutine(CloseTransition());

            CloseImmediate();
        }

        private void RegisterHistoryChange(CanvasMenu from)
        {
            if (_ignoreHistoryChange)
            {
                _ignoreHistoryChange = false;
                return;
            }

            _previousMenus.Push(from);

            _nextMenus.Clear();
        }
    }
}