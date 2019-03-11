using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyApp.TestGridEye
{
    class GE_People_Detector
    {
        

        public short[] g_ashDiffTemp = new short[Constants.IMG_SZ];
        public byte[] g_aucDetectImg = new byte[Constants.IMG_SZ];
        private short[] g_aucObjectImg = new short[Constants.IMG_SZ];
        private bool[] abWork = new bool[Constants.IMG_SZ];

        
        private float g_usHumanRadiusQuad = 4;
        private float g_usHumanArea = 10;
        private short g_shMin = short.MaxValue;

        public float g_usHumanRadius = 2;
        public float g_usHumanSpeedPxl = 2;
        public byte ucDetectNum = 0;
        public coh_t[] astrCenterOfHeat = new coh_t[Constants.MAX_PEOPLE_IN_IMG] { new coh_t(), new coh_t(), new coh_t(), new coh_t() };

        public float diffTempTreshold = 0.3f;

        public void reset()
        {
            g_usHumanRadiusQuad = 4;
            g_usHumanArea = 10;
            g_shMin = short.MaxValue;

            g_usHumanRadius = 2;
            g_usHumanSpeedPxl = 2;
            ucDetectNum = 0;
        }

        public byte detectPeople( GE_Image_Data image, GE_Background_Image backgroundImage)
        {
            vAMG_PUB_ODT_CalcDiffImage(image.g_ashAveTemp, backgroundImage.g_ashBackTemp);
            vAMG_PUB_ODT_CalcDetectImage1();
            vAMG_PUB_ODT_CalcObjectImage(image);
            ucDetectNum = ucAMG_PUB_ODT_CalcPeopleLabeling();

            // Resets the center of heats array
            for (int usCnt = 0; usCnt < Constants.MAX_PEOPLE_IN_IMG; usCnt++)
            {
                astrCenterOfHeat[usCnt].ashCOH[0] = 0;
                astrCenterOfHeat[usCnt].ashCOH[1] = 0;
            }

            // Calculate center of heats
            for (int usCnt = 0; usCnt < ucDetectNum; usCnt++)
            {
                bAMG_PUB_FEA_CalcCenterTemp( (byte)(usCnt + 1), astrCenterOfHeat[usCnt].ashCOH);
            }

            return ucDetectNum;
        }

        private void vAMG_PUB_ODT_CalcDiffImage(short[] image1, short[] image2)
        {
            ushort usSize = (ushort)Constants.IMG_SZ;

            for (ushort usImg = 0; usImg < usSize; usImg++)
            {
                g_ashDiffTemp[usImg] = (short)(image1[usImg] - image2[usImg]);
            }
        }

        private void vAMG_PUB_ODT_CalcDetectImage1()
        {
            ushort usSize = (ushort)Constants.IMG_SZ;
            short shTh = shAMG_PUB_CMN_ConvFtoS(diffTempTreshold);
            byte ucMark = Constants.DETECT_MARK;

            for (ushort usImg = 0; usImg < usSize; usImg++)
            {
                g_aucDetectImg[usImg] = (byte)((shTh <= g_ashDiffTemp[usImg]) ? ucMark : 0);
            }
        }

        private void vAMG_PUB_ODT_CalcObjectImage( GE_Image_Data origImage)
        {
            ushort usSize = (ushort)Constants.IMG_SZ;
            byte ucMark = Constants.DETECT_MARK;

            for (ushort usImg = 0; usImg < usSize; usImg++)
            {
                g_aucObjectImg[usImg] = (short)((g_aucDetectImg[usImg] == ucMark) ? origImage.g_ashAveTemp[usImg] : 0);
            }
        }

        private short shAMG_PUB_CMN_ConvFtoS(float fVal)
        {
            return ((fVal > 0) ? (short)(fVal * 256 + 0.5) : (short)(fVal * 256 - 0.5));
        }

        private byte ucAMG_PUB_ODT_CalcPeopleLabeling()
        {
            byte ucImgZsX = (byte)Constants.IMG_SZ_X;
            byte ucImgZsY = (byte)Constants.IMG_SZ_Y;
            byte ucMark = Constants.DETECT_MARK;
            byte ucMaxAmountOfHuman = (byte)Constants.MAX_PEOPLE_IN_IMG;

            ushort usIdx = 0;
            ushort usIdx_X = 0;
            ushort usIdx_Y = 0;
            short shOffset = 0;
            ushort usHiTemp = 0;
            ushort usLoTemp = 65535;        // Max value for USHORT
            ushort usHiTempIdx = 0;
            ushort usPointXCoord = 0;
            ushort usPointYCoord = 0;
            short shActXCoord = 0;
            short shActYCoord = 0;
            short shXDist = 0;
            short shYDist = 0;
            ushort usThreshold = 0;
            bool bDetectInLine = false;

            ushort usHmnPxlRadius = 0;
            ushort usRadiusQuad = 0;

            ushort usImgSize = (ushort)(ucImgZsX * ucImgZsY);
            //USHORT usHmnMaskSizeX		= (usHmnPxlRadius * 2 + 1);
            //USHORT usHmnMaskSizeY		= (usHmnPxlRadius * 2 + 1);
            //USHORT usHmnMaskSize		= usHmnMaskSizeX * usHmnMaskSizeY;
            ushort usHmnMaskSizeX = 0;
            ushort usHmnMaskSizeY = 0;
            ushort usHmnMaskSize = 0;
            ushort usAreaCounter = 0;
            ushort usOverlappCounter = 0;
            byte ucLabelNumber = 1;

            ushort usOverlappThreshold = (ushort)((g_usHumanArea + 0.5) * 0.0f);      // Amount of pixels which are over lapping between two persons
            ushort usAreaThreshold = (ushort)((g_usHumanArea + 0.5) * 0.3f);      // Amount of pixels which are needed to recognize a person
            float usTempThreshold = 0.4f;                       // 

            // Automatic people size estimation
            ushort[] ausSizeArray = { 0, 0, 0, 0 };
            ushort usSizeArrayDevisor = 0;
            ushort usLastTempValue = 0;
            short shSizeOffset = 0;
            //ushort usMaxRadius = 0;
            float flHmnRadius = 0.0f;



            //// Debugging Array
            //ashInputImg[197] = 12000;
            //
            //ashInputImg[212] = 11000;
            //
            //ashInputImg[196] = 11000;
            //ashInputImg[195] = 10000;
            //
            //ashInputImg[182] = 11000;
            //ashInputImg[167] = 10000;
            //ashInputImg[152] = 9000;
            //
            //ashInputImg[198] = 11000;
            //ashInputImg[199] = 10000;
            //ashInputImg[200] = 9000;
            //ashInputImg[201] = 8000;



            // Initialize work array
            for (usIdx = 0; usIdx < usImgSize; usIdx++)
            {
                abWork[usIdx] = false;
            }

            // Find highest and lowest Temperature value
            for (usIdx = 0; usIdx < usImgSize; usIdx++)
            {
                // Highest
                if (g_aucObjectImg[usIdx] > usHiTemp)
                {
                    usHiTemp = (ushort)g_aucObjectImg[usIdx];
                    usHiTempIdx = usIdx;
                }
                // Lowest
                if ((g_aucObjectImg[usIdx] < usLoTemp) && (g_aucObjectImg[usIdx] != 0))
                {
                    usLoTemp = (ushort)g_aucObjectImg[usIdx];
                }
            }

            // Set Threshold
            //usThreshold = (usHiTemp + usLoTemp) / 2;
            usThreshold = (ushort)(usLoTemp + (usHiTemp - usLoTemp) * usTempThreshold);


            // People labeling algorithm
            if (usHiTemp != 0)
            {
                while (usHiTemp > usThreshold)
                {
                    usAreaCounter = 0;
                    usOverlappCounter = 0;
                    shOffset = 0;
                    usHiTemp = 0;


                    // Find highest temperature value
                    for (usIdx = 0; usIdx < usImgSize; usIdx++)
                    {
                        if ((g_aucObjectImg[usIdx] > usHiTemp) && (abWork[usIdx] != true))
                        {
                            usHiTemp = (ushort)g_aucObjectImg[usIdx];
                            usHiTempIdx = usIdx;
                        }
                    }

                    /*********************************************/
                    /********* Object size estimation ************/
                    /*********************************************/
                    ausSizeArray[0] = 0;
                    ausSizeArray[1] = 0;
                    ausSizeArray[2] = 0;
                    ausSizeArray[3] = 0;
                    usSizeArrayDevisor = 0;
                    // Calculate coordinates of the actually point of interest and the start index
                    usPointYCoord = (ushort)(usHiTempIdx / ucImgZsY);     // Y coordinate
                    usPointXCoord = (ushort)(usHiTempIdx % ucImgZsX);     // X coordinate
                                                                // Estimate distance to the object boundary in -y direction
                    usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx];
                    shSizeOffset = (short)-ucImgZsX;
                    shOffset = (short)shSizeOffset;
                    while ( (usHiTempIdx + shOffset >= 0)
                            && (g_aucObjectImg[usHiTempIdx + shOffset] != 0)
                            && (g_aucObjectImg[usHiTempIdx + shOffset] <= usLastTempValue) )
                    {
                        ausSizeArray[0]++;
                        usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx + shOffset];
                        shOffset += (short)shSizeOffset;
                        if (usHiTempIdx + shOffset < 0)
                        {
                            ausSizeArray[0] = 0;
                        }
                    }
                    if (ausSizeArray[0] != 0)
                    {
                        usSizeArrayDevisor++;
                    }
                    // Estimate distance to the object boundary in +y direction
                    usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx];
                    shSizeOffset = ucImgZsX;
                    shOffset = (short)shSizeOffset;
                    while ((usHiTempIdx + shOffset < usImgSize)
                        && (g_aucObjectImg[usHiTempIdx + shOffset] != 0)
                        && (g_aucObjectImg[usHiTempIdx + shOffset] <= usLastTempValue))
                    {
                        ausSizeArray[1]++;
                        usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx + shOffset];
                        shOffset += (short)shSizeOffset;
                        if (usHiTempIdx + shOffset >= usImgSize)
                        {
                            ausSizeArray[1] = 0;
                        }
                    }
                    if (ausSizeArray[1] != 0)
                    {
                        usSizeArrayDevisor++;
                    }
                    // Estimate distance to the object boundary in -x direction
                    usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx];
                    shSizeOffset = -1;
                    shOffset = shSizeOffset;
                    while ((usPointXCoord - (ausSizeArray[2] + 1) >= 0)
                        && (g_aucObjectImg[usHiTempIdx + shOffset] != 0)
                        && (g_aucObjectImg[usHiTempIdx + shOffset] <= usLastTempValue))
                    {
                        ausSizeArray[2]++;
                        usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx + shOffset];
                        shOffset += shSizeOffset;
                    }
                    if (usPointXCoord - (ausSizeArray[2]) < 0)
                    {
                        ausSizeArray[2] = 0;
                    }
                    if (ausSizeArray[2] != 0)
                    {
                        usSizeArrayDevisor++;
                    }
                    // Estimate distance to the object boundary in +x direction
                    usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx];
                    shSizeOffset = 1;
                    shOffset = shSizeOffset;
                    while ((usPointXCoord + (ausSizeArray[3] + 1) < ucImgZsX)
                        && (g_aucObjectImg[usHiTempIdx + shOffset] != 0)
                        && (g_aucObjectImg[usHiTempIdx + shOffset] <= usLastTempValue))
                    {
                        ausSizeArray[3]++;
                        usLastTempValue = (ushort)g_aucObjectImg[usHiTempIdx + shOffset];
                        shOffset += shSizeOffset;
                    }
                    if (usPointXCoord + (ausSizeArray[3]) >= ucImgZsX)
                    {
                        ausSizeArray[3] = 0;
                    }
                    if (ausSizeArray[3] != 0)
                    {
                        usSizeArrayDevisor++;
                    }
                    if (usSizeArrayDevisor != 0)
                    {
                        flHmnRadius = (float)(ausSizeArray[0] + ausSizeArray[1] + ausSizeArray[2] + ausSizeArray[3]) / usSizeArrayDevisor;
                    }
                    else
                    {
                        flHmnRadius = 0;
                    }

                    /*********************************************/
                    /********* Object area estimation ************/
                    /*********************************************/
                    // Convert radius in short for the following calculations
                    usHmnPxlRadius = (ushort)(flHmnRadius + 0.5f);
                    shOffset = (short)(usHiTempIdx - usHmnPxlRadius * ucImgZsX - usHmnPxlRadius);    // Start index
                                                                                            //usOffset = usHiTempIdx - ausSizeArray[0] * ucImgZsX - ausSizeArray[2];	// Start index

                    shActYCoord = (short)(usPointYCoord - usHmnPxlRadius);   // Y coordinate of first test point in human area
                    shActXCoord = (short)(usPointXCoord - usHmnPxlRadius);   // X coordinate of first test point in human area
                                                                    //shActYCoord = shPointYCoord - ausSizeArray[0];	// Y coordinate of first test point in human area
                                                                    //shActXCoord = shPointXCoord - ausSizeArray[2];	// X coordinate of first test point in human area
                                                                    // Count valid pixels within the radius
                    usHmnMaskSizeX = (ushort)(usHmnPxlRadius * 2 + 1);
                    usHmnMaskSizeY = (ushort)(usHmnPxlRadius * 2 + 1);
                    usHmnMaskSize = (ushort)(usHmnMaskSizeX * usHmnMaskSizeY);
                    //usHmnMaskSizeX = ausSizeArray[2] + ausSizeArray[3] + 1;
                    //usHmnMaskSizeY = ausSizeArray[0] + ausSizeArray[1] + 1;
                    //usHmnMaskSize = usHmnMaskSizeX * usHmnMaskSizeY;
                    usRadiusQuad = (ushort)(flHmnRadius * flHmnRadius);
                    for (usIdx_Y = 0; usIdx_Y < usHmnMaskSizeY; usIdx_Y++)
                    {
                        bDetectInLine = false;

                        for (usIdx_X = 0; usIdx_X < usHmnMaskSizeX; usIdx_X++)
                        {
                            // Check if (x1-x2)^2 + (y1-y2)^2 <= r^2
                            shXDist = (short)(usPointXCoord - shActXCoord);
                            shYDist = (short)(usPointYCoord - shActYCoord);
                            if ((shXDist * shXDist) + (shYDist * shYDist) <= usRadiusQuad)
                            {
                                // Pixel is within the human radius
                                if (shActXCoord < 0 || shActXCoord >= ucImgZsX || shActYCoord < 0 || shActYCoord >= ucImgZsY)
                                {
                                    // Pixel is outside the image boundaries
                                    //usAreaCounter++;
                                }
                                else
                                {
                                    // Pixel is inside the image boundaries
                                    if ((g_aucObjectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] != 0) && (g_aucDetectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] == ucMark))
                                    {
                                        usAreaCounter++;
                                    }
                                    // 
                                    if (g_aucDetectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] != ucMark && g_aucDetectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] != 0)
                                    {
                                        usOverlappCounter++;
                                    }
                                }
                                bDetectInLine = true;
                            }
                            else if (bDetectInLine == true)
                            {
                                // In this line none of the next pixel will be within the radius
                                break;
                            }
                            shActXCoord++;
                        }
                        if ((usAreaCounter + (usHmnMaskSize - usIdx_X - (usIdx_Y * usHmnMaskSizeY))) < usAreaThreshold)
                        {
                            // The needed size of AreaCounter cant be reached
                            break;
                        }
                        if (usOverlappCounter > usOverlappThreshold)
                        {
                            // The Area overlaps a labeled area too much
                            usAreaCounter = 0;
                            break;
                        }
                        shActYCoord++;
                        shActXCoord = (short)(usPointXCoord - usHmnPxlRadius);
                        //shActXCoord = shPointXCoord - ausSizeArray[2];
                    }


                    // Label detected person
                    if (usAreaCounter > usAreaThreshold)
                    {
                        // Actualize the human radius and area variable
                        vAMG_PUB_ODT_ActualizeHumanRadiusAreaAndSpeed(flHmnRadius);

                        // Calculate coordinates of the actually point of interest
                        shActYCoord = (short)(usPointYCoord - usHmnPxlRadius);   // Y coordinate of first test point in human area
                        shActXCoord = (short)(usPointXCoord - usHmnPxlRadius);   // X coordinate of first test point in human area
                                                                        //shActYCoord = shPointYCoord - ausSizeArray[0];	// Y coordinate of first test point in human area
                                                                        //shActXCoord = shPointXCoord - ausSizeArray[2];	// X coordinate of first test point in human area

                        // Count valid pixels within the radius
                        for (usIdx_Y = 0; usIdx_Y < usHmnMaskSizeY; usIdx_Y++)
                        {
                            if ((shActYCoord >= 0) && (shActYCoord < ucImgZsY))
                            {
                                // Line is inside the boundaries
                                bDetectInLine = false;

                                for (usIdx_X = 0; usIdx_X < usHmnMaskSizeX; usIdx_X++)
                                {
                                    // Check if (x1-x2)^2 + (y1-y2)^2 <= r^2
                                    shXDist = (short)(usPointXCoord - shActXCoord);
                                    shYDist = (short)(usPointYCoord - shActYCoord);
                                    if ((shXDist * shXDist) + (shYDist * shYDist) <= (ushort)(g_usHumanRadiusQuad + 0.5f))
                                    {
                                        // Pixel is within the human radius
                                        if (shActXCoord >= 0 && shActXCoord < ucImgZsX && shActYCoord >= 0 && shActYCoord < ucImgZsY)
                                        {
                                            // It is within the image boundaries
                                            if (g_aucObjectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] != 0)
                                            {
                                                g_aucDetectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] = ucLabelNumber;
                                                g_aucObjectImg[shOffset + usIdx_X + (usIdx_Y * ucImgZsX)] = 0;
                                            }
                                        }
                                        bDetectInLine = true;
                                    }
                                    else if (bDetectInLine == true)
                                    {
                                        // In this line none of the next pixel will be within the radius
                                        break;
                                    }
                                    shActXCoord++;
                                }
                            }
                            shActYCoord++;
                            shActXCoord = (short)(usPointXCoord - usHmnPxlRadius);
                            //shActXCoord = usPointXCoord - ausSizeArray[2];
                        }
                        ucLabelNumber++;
                        if ((ucLabelNumber - 1) > ucMaxAmountOfHuman)
                        {
                            return ((byte)(ucLabelNumber - 1));
                        }
                    }
                    else
                    {
                        abWork[usHiTempIdx] = true;
                    }
                }
            }
            return ((byte)(ucLabelNumber - 1));     // No detectable people left
        }
        
        private void vAMG_PUB_ODT_ActualizeHumanRadiusAreaAndSpeed(float flRadius)
        {
            float usHelper = 0;
            if (g_usHumanRadiusQuad == 0)
            {
                // Initial value
                g_usHumanRadius = flRadius;
                g_usHumanArea = flRadius * flRadius * 3.14159265359f;
                g_usHumanRadiusQuad = flRadius * flRadius;
                g_usHumanSpeedPxl = flRadius * 0.24f / 0.225f;              // 0.24 = speed of a person in m/s. 0.225 = radius of a person in m.
            }
            else
            {
                g_usHumanRadius = (float)(flRadius + (g_usHumanRadius - flRadius) * 0.95);   // * 19 / 20 = 0.95

                usHelper = flRadius * flRadius * 3.14159265359f;
                g_usHumanArea = (float)(usHelper + (g_usHumanArea - usHelper) * 0.95);   // * 19 / 20 = 0.95

                usHelper = flRadius * flRadius + 0.5f;
                g_usHumanRadiusQuad = (float)(usHelper + (g_usHumanRadiusQuad - usHelper) * 0.95);   // * 19 / 20 = 0.95

                usHelper = flRadius * 0.24f / 0.225f;
                g_usHumanSpeedPxl = (float)(usHelper + (g_usHumanSpeedPxl - usHelper) * 0.95);
            }
        }

        private bool bAMG_PUB_FEA_CalcCenterTemp( byte ucLabelNo, short[] pshRet)
        {
            byte ucWidth = (byte)Constants.IMG_SZ_X;
            byte ucHeight = (byte)Constants.IMG_SZ_Y;

            bool bRet = false;
            ushort usImg = 0;
            ushort usSize = (ushort)(ucWidth * ucHeight);
            //short shMin = short.MaxValue;
            ulong ulGx = 0;
            ulong ulGy = 0;
            ulong ulGw = 0;
            
            if (false != bAMG_PUB_FEA_CalcMinTemp((byte)usSize, ucLabelNo))
            {
                g_shMin -= 1;
                bRet = true;
            }
            if (false != bRet)
            {
                for (usImg = 0; usImg < usSize; usImg++)
                {
                    if (ucLabelNo == g_aucDetectImg[usImg])
                    {
                        byte ucImgX = (byte)(usImg % ucWidth);
                        byte ucImgY = (byte)(usImg / ucWidth);
                        ulong ulWeight = (ulong)(g_ashDiffTemp[usImg] - g_shMin);
                        ulGx += ulWeight * (ulong)(ucImgX + 1);
                        ulGy += ulWeight * (ulong)(ucImgY + 1);
                        ulGw += ulWeight;
                    }
                }

                if (0 < ulGw)
                {
                    pshRet[0] = (short)(ulGx * 256 / ulGw - 256);
                    pshRet[1] = (short)(ulGy * 256 / ulGw - 256);
                }
                else
                {
                    pshRet[0] = 0;
                    pshRet[1] = 0;
                }
            }

            return (bRet);
        }

        private bool bAMG_PUB_FEA_CalcMinTemp(byte usSize, byte ucLabelNo)
        {
            bool bRet = false;
            ushort usImg = 0;
            short shMin = short.MaxValue;
            g_shMin = short.MaxValue;

            for (usImg = 0; usImg < usSize; usImg++)
            {
                if (ucLabelNo == g_aucDetectImg[usImg])
                {
                    bRet = true;
                    if (shMin > g_ashDiffTemp[usImg])
                    {
                        shMin = g_ashDiffTemp[usImg];
                    }
                }
            }

            if (false != bRet)
            {
                g_shMin = shMin;
            }

            return (bRet);
        }
    }

    class coh_t
    {
        public short[] ashCOH = new short[2] { 0, 0 };
    };
}
