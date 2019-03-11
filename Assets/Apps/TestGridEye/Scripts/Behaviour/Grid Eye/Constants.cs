using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuddyApp.TestGridEye
{
    static class Constants
    {
        /* Grid-EYE's number of pixels */
        public const byte SNR_SZ_X      = 8;
        public const byte SNR_SZ_Y      = 8;
        public const byte SNR_SZ        = (SNR_SZ_X * SNR_SZ_Y);
        /* Setting size of interpolated image */
        public const uint IMG_SZ_X      = (SNR_SZ_X * 2 - 1);
        public const uint IMG_SZ_Y      = (SNR_SZ_Y * 2 - 1);
        public const uint IMG_SZ        = (IMG_SZ_X * IMG_SZ_Y);
        public const uint OUT_SZ_X      = (2);
        public const uint OUT_SZ_Y      = (2);
        public const uint OUT_SZ        = (OUT_SZ_X * OUT_SZ_Y);
        /* Parameters of human detection */
        public const ushort TEMP_FRAME_NUM      = (8);
        public const byte TEMP_MEDIAN_FILTER    = (2);
        public const float TEMP_SMOOTH_COEFF    = (0.60f);
        public const float DIFFTEMP_THRESH		= (0.3f);
        public const byte DETECT_MARK			= (0xFF);
        public const uint LABELING_THRESH		= (3);
        public const uint OUTPUT_THRESH		    = (6);
        public const float BKUPDT_COEFF	        = (0.3f);

        public const float TAN_30_DEGR          = (0.57735f);
        public const float DIAMETER_HUMAN       = (0.45f);		/* in m */
        public const float HUMAN_SPEED          = (0.24f);		/* in m/frame for a frame rate of 10/s */
        public const ushort MAX_PEOPLE_IN_IMG   = (4);
    }
}
