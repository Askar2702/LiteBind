using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LiteBindDI.Services.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string _key;

        private ILocalizationService _localization;
        private TMP_Text _text;

        private void Awake()
        {
            _localization = LiteProjectContext.Container.Resolve<ILocalizationService>();
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (_localization != null)
            {
                HandleLocalizationUpdate();
                _localization.OnLocalizationUpdate += HandleLocalizationUpdate;
            }
        }

        private void OnDisable()
        {
            if (_localization != null)
                _localization.OnLocalizationUpdate -= HandleLocalizationUpdate;
        }

        private void HandleLocalizationUpdate()
        {
            _text.text = _key.ToLocalization();
        }
    }
}