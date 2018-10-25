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
        public const string STR_RECYCLE_BIN_PATH = "C:\\";

        [SerializeField]
        private FileInfo mFileInfo = null;

        [SerializeField]
        private PictureToast mPictureToast;

        public Photo()
        {
            mFileInfo = null;
        }

        public Photo(string strFilePath)
        {
            mFileInfo = new FileInfo(strFilePath);

            if (!mFileInfo.Exists)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: File does not exist!");
                return;
            }
        }
        
        public string GetPhotoName()
        {
            if (null == mFileInfo)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: File does not exist!");
                return null;
            }

            return mFileInfo.Name;
        }
        
        public void GetDirectoryPath(ref string oStrDirPath)
        {
            if (null == mFileInfo) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: File does not exist!");
                return;
            }

            oStrDirPath = mFileInfo.DirectoryName;
        }

        public void GetKeyValue(ref string oStrKey)
        {
            if (null == mFileInfo) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: File does not exist!");
                return;
            }
            

            System.DateTime mCreationDate = mFileInfo.CreationTime;
            if (null == mCreationDate)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Cannot read creation time!");
            }

            oStrKey = ""
                + mCreationDate.Year
                + mCreationDate.Month
                + mCreationDate.Day
                + mCreationDate.Hour
                + mCreationDate.Minute
                + mCreationDate.Second
                + mCreationDate.Millisecond
                + "_" + mFileInfo.Name;
        }

        public void GetAuthor(ref string oStrAuthorName)
        {
            // TODO: Learn how to manage metadata for definite extensions
        }

        public void GetApplication(ref string oStrApplicationName)
        {
            // TODO: Learn how to manage metadata for definite extensions 
        }

        public void GetDate(ref string oStrDate)
        {
            if (null == mFileInfo) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: File does not exist!");
                return;
            }

            oStrDate = mFileInfo.CreationTime.ToShortDateString();
        }

        public PictureToast GetSlide()
        {
            return mPictureToast;
        }

        public void SetSlide(ref PictureToast pictureToast)
        {
            mPictureToast = pictureToast;
        }

        public bool Delete ()
        {
            if (null == mFileInfo)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "File does not exist!");
                return false;
            }

            DirectoryInfo recycleBinDir = new DirectoryInfo(STR_RECYCLE_BIN_PATH);
            if (!recycleBinDir.Exists)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Recycle bin could not be found!");
                return false;
            }

            mFileInfo.Delete();
            return !(new FileInfo(mFileInfo.FullName).Exists);
        }

        public Sprite ToSprite()
        {
            if (null == mFileInfo) {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "File does not exist!");
                return null;
            }
            
            Texture2D spriteTexture = new Texture2D(1, 1);
            spriteTexture.hideFlags = HideFlags.HideAndDontSave;
            spriteTexture.LoadImage(File.ReadAllBytes(mFileInfo.FullName));
            spriteTexture.Apply();

            return Sprite.Create(spriteTexture, new UnityEngine.Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5F, 0.5F));
        }
    }
}
