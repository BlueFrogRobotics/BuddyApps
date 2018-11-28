using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public sealed class Photo
    {
        private PictureToast mPictureToast;
        private readonly string mApplicationName;
        private readonly string mStrFileLocation;
        private readonly string mStrFileName;
        private readonly string mStrFileExtension;
        private readonly DateTime mCreationDate;
        
        public Photo(Photograph iPhotograph)
        {
            if (null == iPhotograph)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photograph not initialized!");
                mStrFileLocation = null;
                mStrFileName = null;
                mStrFileExtension = null;
                return;
            }
            else
            {
                mStrFileLocation = iPhotograph.Location;
                mStrFileName = iPhotograph.Name;
                mStrFileExtension = iPhotograph.Extension;
                mCreationDate = iPhotograph.TimeStamp;
            }
        }

        public string GetPhotoName()
        {
            if (null == mStrFileName)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photo not initialized!");
                return null;
            }

            return mStrFileName;
        }

        public string GetPhotoFullpath()
        {
            if (null == mStrFileLocation || null == mStrFileName || null == mStrFileExtension) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photo not initialized!");
                return null;
            }

            return mStrFileLocation + mStrFileName + mStrFileExtension;
        }

        public string GetKeyValue()
        {
            if (null == mStrFileName)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photo not initialized!");
                return null;
            }

            return ""
                + mCreationDate.Year.ToString("0000")
                + mCreationDate.Month.ToString("00")
                + mCreationDate.Day.ToString("00")
                + mCreationDate.Hour.ToString("00")
                + mCreationDate.Minute.ToString("00")
                + mCreationDate.Second.ToString("00")
                + mCreationDate.Millisecond.ToString("000")
                + "_" + mStrFileName;
        }
        
        public string GetDate()
        {
            return mCreationDate.ToShortDateString();
        }

        public PictureToast GetSlide()
        {
            return mPictureToast;
        }

        public void SetSlide(ref PictureToast iPictureToast)
        {
            mPictureToast = iPictureToast;
        }

        public string GetApplicationName()
        {
            return mApplicationName;
        }

        public bool Delete ()
        {
            string lStrFullPath = GetPhotoFullpath();

            if (null == lStrFullPath)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photo not initialized!");
                return false;
            }

            if (File.Exists(lStrFullPath))
            {
                File.Delete(lStrFullPath);

                if (File.Exists(lStrFullPath)) // If not deleted
                {
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Can't delete photo : " + lStrFullPath);
                    return false;
                }
            }

            return true;
        }
    }
}
