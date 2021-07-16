using UnityEngine;

namespace Voodoo.Sauce.Internal
{
    public class TSDebugUIScreen : MonoBehaviour
    {
        protected TinySauceSettings _tsSettings;

        public TinySauceSettings TSSettings
        {
            get => _tsSettings;
            set
            {
                _tsSettings = value;
                UpdateInfo();
            }
        }

        protected virtual void UpdateInfo() { }
    }
}
