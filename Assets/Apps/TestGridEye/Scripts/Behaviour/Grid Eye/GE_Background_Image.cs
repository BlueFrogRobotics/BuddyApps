using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuddyApp.TestGridEye
{
    class GE_Background_Image
    {
        public short[] g_ashBackTemp = new short[Constants.IMG_SZ];
        public bool backgroundInitialized = false;

        private byte[] g_imageDilationArray = new byte[Constants.IMG_SZ];
        private short shAve = 0;

        public void reset()
        {
            backgroundInitialized = false;
            for (int idx = 0; idx < Constants.IMG_SZ; idx++)
            {
                g_ashBackTemp[idx] = 0;
            }
        }

        public void initializeBackgroundImage( GE_Image_Data Image )
        {
            for (int usCnt = 0; usCnt < Constants.IMG_SZ; usCnt++)
            {
                g_ashBackTemp[usCnt] = Image.g_ashAveTemp[usCnt];
            }
            backgroundInitialized = true;
        }

        public void upgradeBackgroundTemperatures( GE_People_Detector peopleDetector )
        {
            bAMG_PUB_IMG_ImageDilation1(peopleDetector);
            bAMG_PUB_BGT_UpdateBackTemp(peopleDetector);
        }

        private bool bAMG_PUB_IMG_ImageDilation1( GE_People_Detector peopleDetector )
        {
            byte ucWidth = (byte)Constants.IMG_SZ_X;
            byte ucHeight = (byte)Constants.IMG_SZ_Y;

            ushort usImg = 0;
            ushort usSize = (ushort)(ucWidth * ucHeight);
            bool bRet = false;

            //if (peopleDetector.g_aucDetectImg != g_ashBackTemp)
            {
                for (usImg = 0; usImg < usSize; usImg++)
                {
                    g_imageDilationArray[usImg] = 0;
                }
                for (usImg = 0; usImg < usSize; usImg++)
                {
                    byte ucImgX = (byte)(usImg % ucWidth);
                    byte ucImgY = (byte)(usImg / ucWidth);

                    if (0 != peopleDetector.g_aucDetectImg[usImg])
                    {
                        g_imageDilationArray[usImg] = 1;
                        if (0 != ucImgX)
                        {
                            g_imageDilationArray[usImg - 1] = 1;
                        }
                        if ((ucWidth - 1) != ucImgX)
                        {
                            g_imageDilationArray[usImg + 1] = 1;
                        }
                        if (0 != ucImgY)
                        {
                            g_imageDilationArray[usImg - ucWidth] = 1;
                        }
                        if ((ucHeight - 1) != ucImgY)
                        {
                            g_imageDilationArray[usImg + ucWidth] = 1;
                        }
                    }
                }
                bRet = true;
            }
            return (bRet);
        }

        private bool bAMG_PUB_BGT_UpdateBackTemp( GE_People_Detector peopledetector)
        {
            ushort usSize = (ushort)Constants.IMG_SZ;

            short shTh = shAMG_PUB_CMN_ConvFtoS(Constants.BKUPDT_COEFF);

            const short c_shMinTh = 0;
            const short c_shMaxTh = 256;
            bool bRet = false;
            ushort usImg = 0;

            /* Adjust parameter. */
            if (c_shMinTh > shTh)
            {
                shTh = c_shMinTh;
            }
            if (c_shMaxTh < shTh)
            {
                shTh = c_shMaxTh;
            }

            if (false != bAMG_PUB_FEA_CalcAveTemp(usSize, 0, peopledetector))
            {
                bRet = true;
                for (usImg = 0; usImg < usSize; usImg++)
                {
                    short shTemp = 0;
                    if (0 == g_imageDilationArray[usImg])
                    {
                        shTemp = (short)((long)shTh * peopledetector.g_ashDiffTemp[usImg] / c_shMaxTh);
                    }
                    else
                    {
                        shTemp = (short)((long)shTh * shAve / c_shMaxTh);
                    }
                    g_ashBackTemp[usImg] += shTemp;
                }
            }

            return (bRet);
        }

        private bool bAMG_PUB_FEA_CalcAveTemp(ushort usSize, byte ucLabelNo, GE_People_Detector peopleDetector)
        {
            bool bRet = false;
            ushort usImg = 0;
            long loSum = 0;
            ushort usCnt = 0;

            for (usImg = 0; usImg < usSize; usImg++)
            {
                if (ucLabelNo == g_imageDilationArray[usImg])
                {
                    bRet = true;
                    loSum += peopleDetector.g_ashDiffTemp[usImg];
                    usCnt++;
                }
            }

            if (false != bRet)
            {
                shAve = (short)(loSum / usCnt);
            }

            return (bRet);
        }

        private short shAMG_PUB_CMN_ConvFtoS(float fVal)
        {
            return ((fVal > 0) ? (short)(fVal * 256 + 0.5) : (short)(fVal * 256 - 0.5));
        }
    }
}
