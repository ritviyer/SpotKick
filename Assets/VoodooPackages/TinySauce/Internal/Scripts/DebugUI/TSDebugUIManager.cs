using UnityEngine;

namespace Voodoo.Sauce.Internal
{
    public class TSDebugUIManager : MonoBehaviour
    {
        [SerializeField] private TSDebugUIBehaviour tsDebugUIPrefab;

        private bool isDebugUIOpen = false;

        private int nbTapTop = 2;
        private int nbTapBottom = 2;

        private float maxDurationBetweenTap = 2.5f;
        private float countDown;

        private int countTapTL = 0;
        private int countTapTR = 0;
        private int countTapBL = 0;

        private Vector3 mousePos;
        private int smallerScreenSliceNb = 6;
        private int biggerScreenSliceNb = 8;
        private int screenWidthSliceNb;
        private int screenHeightSliceNb;

        private int _screenSliceWidth;
        public int ScreenSliceWidth { get => _screenSliceWidth; }

        private int _screenSliceHeight;
        public int ScreenSliceHeight { get => _screenSliceHeight; }


        private void Start()
        {
            if (Screen.width < Screen.height)
            {
                screenWidthSliceNb = smallerScreenSliceNb;
                screenHeightSliceNb = biggerScreenSliceNb;
            }
            else
            {
                screenWidthSliceNb = biggerScreenSliceNb;
                screenHeightSliceNb = smallerScreenSliceNb;
            }

            _screenSliceWidth = Screen.width / screenWidthSliceNb;
            _screenSliceHeight = Screen.height / screenHeightSliceNb;
        }

        private void Update()
        {
            isDebugUIOpen = TSDebugUIBehaviour.Instance != null;
            mousePos = Input.mousePosition;

            if (!isDebugUIOpen)
            {
                if (countDown > 0)
                {
                    countDown -= Time.deltaTime;

                    if (countDown <= 0) ResetCountsTap();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (countTapTL < nbTapTop) TapTopLeftElseReset();
                    else if (countTapTR < nbTapTop) TapTopRightElseReset();
                    else if (countTapBL < nbTapBottom) TapBottomLeftElseReset();
                    else if (countTapTL < nbTapTop * 2) TapTopLeftElseReset();
                    else if (countTapTR < nbTapTop * 2) TapTopRightElseReset();
                }

                if (countTapTL == (nbTapTop * 2) && countTapTR == (nbTapTop * 2) && countTapBL == nbTapBottom)
                {
                    Instantiate(tsDebugUIPrefab);
                    ResetCountsTap();
                }
            }
        }

        #region [TAP_FUNCTIONS]
        private void TapTopLeftElseReset()
        {
            if (mousePos.x <= ScreenSliceWidth && mousePos.y >= ScreenSliceHeight * (screenHeightSliceNb - 1))
                ValidTap(ref countTapTL);
            else
                ResetCountsTap();
        }

        private void TapTopRightElseReset()
        {
            if (mousePos.x >= ScreenSliceWidth * (screenWidthSliceNb - 1) && mousePos.y >= ScreenSliceHeight * (screenHeightSliceNb - 1))
                ValidTap(ref countTapTR);
            else
                ResetCountsTap();
        }

        private void TapBottomLeftElseReset()
        {
            if (mousePos.x <= ScreenSliceWidth && mousePos.y <= ScreenSliceHeight)
                ValidTap(ref countTapBL);
            else
                ResetCountsTap();
        }

        private void ValidTap(ref int countToIncrement)
        {
            countDown = maxDurationBetweenTap;
            countToIncrement++;
        }
        #endregion

        private void ResetCountsTap()
        {
            //Debug.Log("RESET : TL=" + countTapTL + " // TR=" + countTapTR + " // BL=" + countTapBL + " // countDown=" + countDown);
            countDown = 0;
            countTapTL = 0;
            countTapTR = 0;
            countTapBL = 0;
        }
    }
}