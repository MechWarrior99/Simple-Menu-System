using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ProjectMenuZ
{
    [AddComponentMenu("UI/Menu Transitioner")]
    [RequireComponent(typeof(Button))]
    public class MenuTransitioner : MonoBehaviour
    {
        [Tooltip("The menu to transition from (close).")]
        [SerializeField] private CanvasMenu _fromMenu;
        [Tooltip("The menu to transition to (open).")]
        [SerializeField] private CanvasMenu _toMenu;

        /// <summary>
        /// The <see cref="CanvasMenu"/> to transition from (Close).
        /// </summary>
        public CanvasMenu FromMenu
        {
            get { return _fromMenu; }
            set { _fromMenu = value; }
        }

        /// <summary>
        /// The <see cref="CanvasMenu"/> to transition to (Open).
        /// </summary>
        public CanvasMenu ToMenu
        {
            get { return _toMenu; }
            set { _toMenu = value; }
        }

        private void Reset()
        {
            // Finds the fisrt parent object with a CanvasMenu component and sets the _fromMenu field to that menu.
            for (Transform parent = transform.parent; parent != null && parent.transform is RectTransform; parent = parent.parent)
            {
                if (parent.TryGetComponent<CanvasMenu>(out CanvasMenu menu))
                {
                    _fromMenu = menu;
                    break;
                }
            }

            // Adds the transition to the button's click event.
            var onClick = GetComponent<Button>().onClick;

            int removeAt = -1;
            for (int i = 0; i < onClick.GetPersistentEventCount(); i++)
            {
                if (onClick.GetPersistentMethodName(i) == nameof(Transition))
                {
                    removeAt = i;
                    break;
                }
            }

            if (removeAt < 0)
                UnityEventTools.AddPersistentListener(GetComponent<Button>().onClick, new UnityAction(Transition));
        }

        public void Transition()
        {
            _fromMenu.TransitionTo(_toMenu);
        }
    } 
}
