using System;
using Runtime.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Fabricator
{
    public class ModuleItemUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image moduleIcon;
        [SerializeField] private Image moduleFill;
        [SerializeField] private CanvasGroup moduleUIGroup;
        
        [Header("Configuration")]
        [SerializeField] private Color starvedColor = Color.yellow;
        [SerializeField] private Color processColor = Color.green;
        [SerializeField] private Color blockedColor = Color.red;
        
        private void Start()
        {
            Disable();
        }

        public void Disable()
        {
            moduleUIGroup.alpha = 0;
            moduleUIGroup.blocksRaycasts = false;
        }
        
        public void Enable()
        {
            moduleUIGroup.alpha = 1;
            moduleUIGroup.blocksRaycasts = true;
        }

        public void SetModuleIcon(Sprite icon) => moduleIcon.sprite = icon;

        public void SetState(ModuleState state, float elapsedTime, float totalTime)
        {
            switch (state)
            {
                case ModuleState.Starved:
                    Starve();
                    break;
                case ModuleState.Processing:
                    Process(elapsedTime, totalTime);
                    break;
                case ModuleState.Blocked:
                    Block();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void Starve()
        {
            moduleFill.fillAmount = 1;
            moduleFill.color = starvedColor;
        }

        private void Process(float elapsedTime, float totalTime)
        {
            var progress = elapsedTime / totalTime;
            moduleFill.fillAmount = progress;
            moduleFill.color = processColor;
        }

        private void Block()
        {
            moduleFill.fillAmount = 1;
            moduleFill.color = blockedColor;
        }
    }
}