using LiteBindDI;
using UnityEngine;
using UnityEngine.UI;

namespace LiteBindDI.Services.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedTextLegacy : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField] private bool _isUpper;

        private ILocalizationService _localization;
        private Text _text;
        
        private void Awake()
        {
            _localization = LiteProjectContext.Container.Resolve<ILocalizationService>(); 
            _text = GetComponent<Text>();
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

        private void HandleLocalizationUpdate() => 
            _text.text = _isUpper ? _key.ToLocalization().ToUpper() : _key.ToLocalization();
    }
}