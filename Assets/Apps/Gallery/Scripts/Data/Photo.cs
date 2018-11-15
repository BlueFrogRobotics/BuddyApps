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
        [SerializeField]
        private PictureToast mPictureToast;

        [SerializeField]
        private readonly string mApplicationName;
        
        [SerializeField]
        private readonly string mStrFileLocation;

        [SerializeField]
        private readonly string mStrFileName;

        [SerializeField]
        private readonly string mStrFileExtension;

        [SerializeField]
        private readonly DateTime mCreationDate;
        
        public Photo(Photograph photograph)
        {
            if (null == photograph)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photograph not initialized!");
                mStrFileLocation = null;
                mStrFileName = null;
                mStrFileExtension = null;
                return;
            }
            else
            {
                mStrFileLocation = photograph.Location;
                mStrFileName = photograph.Name;
                mStrFileExtension = photograph.Extension;
                mCreationDate = photograph.TimeStamp;
            }
        }

        public string GetPhotoName()
        {
            if (null == mStrFileName) {
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
            if (null == mStrFileName) {
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

        //private string GetMetaDataValue(string strKey)
        //{
        //    /*
        //    // https://forum.unity.com/threads/assetimporter-userdata-not-writing-to-the-meta-file-in-4-6.267493/
        //    AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mPhotograph.Image.texture));
        //    importer.userData += "Edited!";
        //    string strAuthor = importer.userData.Substring(importer.userData.IndexOf("author:"))
        //    EditorUtility.SetDirty(mPhotograph.Image.texture);
        //    AssetDatabase.SaveAssets();
        //    */

        //    string strUserData = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mPhotograph.Image.texture)).userData;

        //    int iStart = strUserData.IndexOf(strKey + ": ") + strKey.Length + 2; // Find key in user metadata
        //    int iLength = strUserData.IndexOf('\n', iStart) - iStart; // Find seperator in order to calculate length

        //    if (-1 == iStart || 0 == iLength)
        //        return null;

        //    return strUserData.Substring(iStart, iLength);
        //}

        //private void SetMetaDataValue(string strKey, string strValue)
        //{
        //    /*
        //    // https://forum.unity.com/threads/assetimporter-userdata-not-writing-to-the-meta-file-in-4-6.267493/
        //    AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mPhotograph.Image.texture));
        //    importer.userData += "Edited!";
        //    string strAuthor = importer.userData.Substring(importer.userData.IndexOf("author:"))
        //    EditorUtility.SetDirty(mPhotograph.Image.texture);
        //    AssetDatabase.SaveAssets();
        //    */
        //    string strUserData = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mPhotograph.Image.texture)).userData;
        //    string strToAdd = strKey + ": " + strValue + '\n';

        //    int iFinishFirstPart = strUserData.IndexOf(strKey);
        //    if (-1 == iFinishFirstPart) // Not already present => add
        //    {
        //        strUserData += strToAdd;
        //    }
        //    else // Already present => Update
        //    {
        //        int iStartSecondPart = strUserData.IndexOf('\n', iFinishFirstPart) + 1; // Find seperator
        //        strUserData = strUserData.Substring(0, iFinishFirstPart) + strToAdd + strUserData.Substring(iStartSecondPart);
        //    }
            
        //    EditorUtility.SetDirty(mPhotograph.Image.texture);
        //    AssetDatabase.SaveAssets();
        //}

        //public string GetAuthor()
        //{
        //    return GetMetaDataValue("author");
        //}

        //public void SetAuthor(string strAuthorId)
        //{
        //    SetMetaDataValue("author", strAuthorId);
        //}

        //public string GetApplication()
        //{
        //    return GetMetaDataValue("appname");
        //}

        //public void SetApplication(string strAppId)
        //{
        //    SetMetaDataValue("appname", strAppId);
        //}

        public string GetDate()
        {
            return mCreationDate.ToShortDateString();
        }

        public PictureToast GetSlide()
        {
            return mPictureToast;
        }

        public void SetSlide(ref PictureToast pictureToast)
        {
            mPictureToast = pictureToast;
        }

        public string GetApplicationName()
        {
            return mApplicationName;
        }

        public bool Delete ()
        {
            string strFullPath = GetPhotoFullpath();

            if (null == strFullPath)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Photo not initialized!");
                return false;
            }

            if (File.Exists(strFullPath))
            {
                File.Delete(strFullPath);
                if (File.Exists(strFullPath)) // If not deleted
                {
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Can't delete photo : " + strFullPath);
                    return false;
                }
            }

            return true;
        }
    }
}
