using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.TestGridEye
{
    public class GE_Image_Data
    {
        public short maxValue = 0;
        public short minValue = short.MaxValue;
        private float iirCoeff = 1.0f;
        public bool iirFilterActiv = false;
        public bool flipXaxisEnabled = false;
        public bool flipYaxisEnabled = false;
        public bool filterBufferIsFilled = false;
        //private short[,] indataArrayBuffer = new short[8, 8];


        public short g_shThsTemp;					/* thermistor temperature */
        public byte[] g_aucRaw = new byte[Constants.SNR_SZ * 2];
        private ulong g_ulFrameNum;
        private short[,] g_a2shRawTemp = new short[Constants.TEMP_FRAME_NUM, Constants.SNR_SZ];
        public short[] g_ashSnrAveTemp = new short[Constants.SNR_SZ];
        public short[] g_ashAveTemp = new short[Constants.IMG_SZ];// interpolated thermal matrix (15*15)

        public void reset()
        {
            g_shThsTemp = 0;					/* thermistor temperature */
            g_ulFrameNum = 0;
            filterBufferIsFilled = false;
            for (int idx = 0; idx < Constants.SNR_SZ * 2; idx++)
            {
                g_aucRaw[idx] = 0;
            }
            for (int idx = 0; idx < Constants.TEMP_FRAME_NUM; idx++)
            {
                for (int idx2 = 0; idx2 < Constants.SNR_SZ; idx2++)
                {
                    g_a2shRawTemp[idx, idx2] = 0;
                }
            }
            for (int idx = 0; idx < Constants.SNR_SZ; idx++)
            {
                g_ashSnrAveTemp[idx] = 0;
            }
            for (int idx = 0; idx < Constants.IMG_SZ; idx++)
            {
                g_ashAveTemp[idx] = 0;
            }
        }

        public void setIIRCoeff(float coeff)
        {
            if (coeff >= 1)
            {
                iirCoeff = 0.95f;
            }
            else if (coeff < 0)
            {
                iirCoeff = 0;
            }
            else
            {
                iirCoeff = coeff;
            }
        }

        private void convertTemperatures()
        {
            short tempMaxValue = 0;
            short tempMinValue = short.MaxValue;
            for (int idx = 0; idx < Constants.SNR_SZ; idx++)
            {
                short help = (short)(((short)(g_aucRaw[idx * 2 + 1] & 0x07) << 8));
                help |= (short)g_aucRaw[idx * 2 ];
                if (0 != (0x08 & g_aucRaw[idx * 2 + 1]))
                {
                    help -= 2048;
                }
                help *= 64;
                
                g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, idx] = help;
                
                if (help < tempMinValue)
                {
                    tempMinValue = help;
                }
                if (help > tempMaxValue)
                {
                    tempMaxValue = help;
                }
            }

            maxValue = tempMaxValue;
            minValue = tempMinValue;
            if (flipXaxisEnabled)
            {
                HFlipArray();
            }
            if (flipYaxisEnabled)
            {
                VFlipArray();
            }
        }

        private void convertTemperaturesOrNot()
        {
            short tempMaxValue = 0;
            short tempMinValue = short.MaxValue;
            for (int idx = 0; idx < Constants.SNR_SZ; idx++)
            {
                short help = (short)(g_aucRaw[idx]);

                g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, idx] = help;

                if (help < tempMinValue)
                {
                    tempMinValue = help;
                }
                if (help > tempMaxValue)
                {
                    tempMaxValue = help;
                }
            }

            maxValue = tempMaxValue;
            minValue = tempMinValue;
            if (flipXaxisEnabled)
            {
                HFlipArray();
            }
            if (flipYaxisEnabled)
            {
                VFlipArray();
            }
        }


        public void AddNewData(byte[] globalInData)
        {
            // Extract the thermistor value
            short thHelp = (short)(((short)(globalInData[4] & 0x07) << 8) | globalInData[3]);
            if (0 != (0x08 & globalInData[4]))
            {
                thHelp *= -1;
            }
            thHelp *= 16;
            g_shThsTemp = thHelp;

            // Extract the temperature values
            for (int idx = 0; idx < 128; idx++)
            {
                g_aucRaw[idx] = globalInData[idx + 5];      // idx 0 - 4 = "***th"
            }

            // Convert the temperatures into fixpoint number. The real temperature can be extracted with pixel/256.
            convertTemperatures();

            // If frame buffer is full start to create filtered images
            g_ulFrameNum++;
            if(g_ulFrameNum >= Constants.TEMP_FRAME_NUM)
            {
                for (int idx = 0; idx < Constants.SNR_SZ; idx++)
                {
                    // Use a median filter for each pixel through 8 images
                    short shAveTemp = shAMG_PUB_CMN_CalcAve( idx );
                    if(g_ulFrameNum == Constants.TEMP_FRAME_NUM)
                    {
                        // Initial filtered values
                        g_ashSnrAveTemp[idx] = shAveTemp;
                    }
                    else
                    {
                        // After the buffer is filled and the filtered value array is initialized, use an IIR filter for the next values
                        g_ashSnrAveTemp[idx] = shAMG_PUB_CMN_CalcIIR(g_ashSnrAveTemp[idx], shAveTemp, shAMG_PUB_CMN_ConvFtoS(iirCoeff));
                    }
                }
                filterBufferIsFilled = true;

                // Create the 15 x 15 bilinear interpolated image
                bAMG_PUB_IMG_LinearInterpolationSQ15();
            }
        }

        public void addNewData64(byte[] globalInData)
        {
            // Extract the thermistor value
            //short thHelp = (short)(((short)(globalInData[4] & 0x07) << 8) | globalInData[3]);
            //if (0 != (0x08 & globalInData[4]))
            //{
            //    thHelp *= -1;
            //}
            //thHelp *= 16;
            g_shThsTemp = 0;

            // Extract the temperature values
            for (int idx = 0; idx < 64; idx++)
            {
                g_aucRaw[idx] = globalInData[idx];      // idx 0 - 4 = "***th"
            }

            // Convert the temperatures into fixpoint number. The real temperature can be extracted with pixel/256.
            convertTemperaturesOrNot();

            // If frame buffer is full start to create filtered images
            g_ulFrameNum++;
            if (g_ulFrameNum >= Constants.TEMP_FRAME_NUM)
            {
                for (int idx = 0; idx < Constants.SNR_SZ; idx++)
                {
                    // Use a median filter for each pixel through 8 images
                    short shAveTemp = shAMG_PUB_CMN_CalcAve(idx);
                    if (g_ulFrameNum == Constants.TEMP_FRAME_NUM)
                    {
                        // Initial filtered values
                        g_ashSnrAveTemp[idx] = shAveTemp;
                    }
                    else
                    {
                        // After the buffer is filled and the filtered value array is initialized, use an IIR filter for the next values
                        g_ashSnrAveTemp[idx] = shAMG_PUB_CMN_CalcIIR(g_ashSnrAveTemp[idx], shAveTemp, shAMG_PUB_CMN_ConvFtoS(iirCoeff));
                    }
                }
                filterBufferIsFilled = true;

                // Create the 15 x 15 bilinear interpolated image
                bAMG_PUB_IMG_LinearInterpolationSQ15();
            }
        }

        private short shAMG_PUB_CMN_CalcIIR(short shVal1, short shVal2, short shTh)
        {
            const short c_shMinTh = 0;
            const short c_shMaxTh = 256;
            long loAddVal = 0;

            /* Adjust parameter. */
            if (c_shMinTh > shTh)
            {
                shTh = c_shMinTh;
            }
            if (c_shMaxTh < shTh)
            {
                shTh = c_shMaxTh;
            }

            /* Calculate average. */
            loAddVal = (long)shTh * (shVal2 - shVal1);
            return (short)(shVal1 + (short)(loAddVal / c_shMaxTh));
        }

        private short shAMG_PUB_CMN_CalcAve( int idx )
        {
            short shAve = 0;
            ushort usSize = Constants.TEMP_FRAME_NUM;
            byte ucSkip = Constants.SNR_SZ;
            byte ucMedian = Constants.TEMP_MEDIAN_FILTER;
            bool[] bMedianWork = new bool[Constants.TEMP_FRAME_NUM];

            if (1 >= usSize)
            {
                return (g_a2shRawTemp[0, idx]);
            }

            /* Adjust parameter. */
            if (1 > ucSkip)
            {
                ucSkip = 1;
            }
            if (ucMedian > ((usSize - 1) / 2))
            {
                ucMedian = (byte)((usSize - 1) / 2);
            }

            /* Calculate average. */
            if (0 == ucMedian)
            {
                ushort usCnt = 0;
                long loSum = 0;

                for (usCnt = 0; usCnt < usSize; usCnt++)
                {
                    short shCurData = g_a2shRawTemp[usCnt, idx];
                    loSum += shCurData;
                }
                shAve = (short)(loSum / usSize);
            }
            else
            {
                ushort usCnt = 0;
                long loSum = 0;
                byte ucMedianCnt = 0;

                for (usCnt = 0; usCnt < usSize; usCnt++)
                {
                    bMedianWork[usCnt] = true;
                }

                for (ucMedianCnt = 0; ucMedianCnt < ucMedian; ucMedianCnt++)
                {
                    short shMaxData = short.MinValue;
                    short shMinData = short.MaxValue;
                    byte ucIndex = 0;

                    for (usCnt = 0; usCnt < usSize; usCnt++)
                    {
                        if (false != bMedianWork[usCnt])
                        {
                            short shCurData = g_a2shRawTemp[usCnt, idx];
                            if (shMaxData < shCurData)
                            {
                                shMaxData = shCurData;
                                ucIndex = (byte)usCnt;
                            }
                        }
                    }
                    bMedianWork[ucIndex] = false;

                    for (usCnt = 0; usCnt < usSize; usCnt++)
                    {
                        if (false != bMedianWork[usCnt])
                        {
                            short shCurData = g_a2shRawTemp[usCnt, idx];
                            if (shMinData > shCurData)
                            {
                                shMinData = shCurData;
                                ucIndex = (byte)usCnt;
                            }
                        }
                    }
                    bMedianWork[ucIndex] = false;
                }

                for (usCnt = 0; usCnt < usSize; usCnt++)
                {
                    short shCurData = g_a2shRawTemp[usCnt, idx];
                    if (false != bMedianWork[usCnt])
                    {
                        loSum += shCurData;
                    }
                }
                shAve = (short)(loSum / (usSize - ucMedian * 2));
            }
            return (shAve);
        }

        private short shAMG_PUB_CMN_ConvFtoS(float fVal)
        {
            return ((fVal > 0) ? (short)(fVal * 256 + 0.5) : (short)(fVal * 256 - 0.5));
        }

        private bool bAMG_PUB_IMG_LinearInterpolationSQ15()
        {
            const byte c_ucImgWidth = 15;
            const byte c_ucImgHeight = 15;
            bool bRet = false;

            if (this.g_ashSnrAveTemp != g_ashAveTemp)
            {
                //Debug.Log("lol");
                byte ucX = 0;
                byte ucY = 0;
                for (ucY = 0; ucY < c_ucImgHeight; ucY += 2)
                {
                    for (ucX = 0; ucX < c_ucImgWidth; ucX += 2)
                    {
                        byte ucSnr = (byte)(ucX / 2 + ucY / 2 * Constants.SNR_SZ_X);
                        byte ucImg = (byte)(ucX + ucY * c_ucImgWidth);
                        g_ashAveTemp[ucImg] = this.g_ashSnrAveTemp[ucSnr];
                        //Debug.Log("ashmachin: " + this.g_ashSnrAveTemp[ucSnr]);
                    }
                    for (ucX = 1; ucX < c_ucImgWidth; ucX += 2)
                    {
                        byte ucImg = (byte)(ucX + ucY * c_ucImgWidth);
                        g_ashAveTemp[ucImg] = (short)( (g_ashAveTemp[ucImg - 1] + g_ashAveTemp[ucImg + 1]) / 2 );
                    }
                }
                for (ucY = 1; ucY < c_ucImgHeight; ucY += 2)
                {
                    for (ucX = 0; ucX < c_ucImgWidth; ucX++)
                    {
                        byte ucImg = (byte)(ucX + ucY * c_ucImgWidth);
                        g_ashAveTemp[ucImg] = (short)( (g_ashAveTemp[ucImg - c_ucImgWidth] + g_ashAveTemp[ucImg + c_ucImgWidth]) / 2 );
                        
                    }
                }

                bRet = true;
            }

            return (bRet);
        }

        private void VFlipArray()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int index = 0; index < 8 / 2; index++)
                {
                    short temp = g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, row * 8 + index];
                    g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, row * 8 + index] = g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, row * 8 + 8 - index - 1];
                    g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, row * 8 + 8 - index - 1] = temp;
                }
            }
        }

        private void HFlipArray()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int index = 0; index < 8 / 2; index++)
                {
                    short temp = g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, index * 8 + row];
                    g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, index * 8 + row] = g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, 64 - ((index+1) * 8) + row /*index + 8 - row * 8 - 1*/];
                    g_a2shRawTemp[(int) g_ulFrameNum % Constants.TEMP_FRAME_NUM, 64 - ((index + 1) * 8) + row/*index + 8 - row * 8 - 1*/] = temp;
                }
            }
        }
    }
}
