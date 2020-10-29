using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectMenuZ
{
    [RequireComponent(typeof(Button))]
    public class MenuHistoryButton : MonoBehaviour
    {
        private Button _button;

        [SerializeField] private CanvasMenu _menu;
        [SerializeField] private bool _isForward = false;

        private void Start()
        {
            _button = GetComponent<Button>();
        }

        private void Update()
        {
            _button.interactable = _isForward ? _menu.ValidateForward() : _menu.ValidateBack();
        }

        private void Reset()
        {
            // Finds the fisrt parent object with a CanvasMenu component and sets the _menu field to that menu.
            for (Transform parent = transform.parent; parent != null && parent.transform is RectTransform; parent = parent.parent)
            {
                if (parent.TryGetComponent<CanvasMenu>(out CanvasMenu menu))
                {
                    _menu = menu;
                    break;
                }
            }
        }
    } 
}
