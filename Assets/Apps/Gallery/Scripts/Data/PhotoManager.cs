using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    public sealed class PhotoManager
    {
        public static readonly string STR_GALLERY_DIRECTORY = "Images"; //"C:\\Users\\YavinIV\\GalleryTest";
        public static readonly string[] ALLOWED_EXTENSION_LIST = { "*.psd", "*.tiff", "*.jpg", "*.tga", "*.png", "*.gif", "*.bmp", "*.iff", "*.pict"};
        
        // Singleton design pattern
        private static readonly PhotoManager mPhotoManagerInstance = new PhotoManager();
        private static string mFirstPhotoPath = null;
        private static SortedList mPhotoSortedList = new SortedList();
        private static SlideSet mSlideSet = null;
        private static int mFirstImageIndex;

        // Singleton design pattern
        static PhotoManager()
        {
        }

        // Singleton design pattern
        private PhotoManager()
        {
        }

        // Singleton design pattern
        public static PhotoManager GetInstance()
        {
            return mPhotoManagerInstance;
        }

        public void Initialize(string strAppName, string strPhotoName)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Initializing application with image directory...");

            if (null != mPhotoSortedList)
            {
                mPhotoSortedList.Clear();
            }

            // Scan repository to initialize the photo list
            ScanPhotographs();
            //ScanDirectory(Buddy.Resources.GetRawFullPath(STR_GALLERY_DIRECTORY));

            // Determine first index
            int firstIndex;

            // First : check if input file exists
            // Second : check if app has photo
            firstIndex = GetInitialIndex(strAppName, strPhotoName);
            if (-1 != firstIndex)
            {
                mFirstImageIndex = firstIndex;
            }
            else
            {
                mFirstImageIndex = mPhotoSortedList.Count - 1; // Last : Set to last if nothing if found
            }
        }

        public void Free()
        {
            mFirstPhotoPath = null;
            mPhotoSortedList = new SortedList();
            mSlideSet = null;
            mFirstImageIndex = -1;
    }
        
        /*
        private void ScanDirectory(string strDirectoryPath)
        {
            
            // Check if directory path exists
            if (!Directory.Exists(strDirectoryPath))
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: Impossible to find image directory!");
                // TODO: Error when file does not exist
                return;
            }

            // For each extension
            foreach(string strExtension in ALLOWED_EXTENSION_LIST)
            {
                IEnumerable<string> lDirectory = Directory.EnumerateFiles(strDirectoryPath, strExtension, SearchOption.AllDirectories);
            
                foreach (string strCurrentFile in lDirectory)
                {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.LOADING, "File: " + strCurrentFile);

                    Photo photo = new Photo(strCurrentFile);

                    if (null == photo)
                    {
                        return;
                    }
                
                    AddPhoto(photo);
                }
            }
        }
        */

        private void ScanPhotographs()
        {
            Photograph[] lPhotographs = Buddy.Platform.Users.GetUserPhotographs(false);

            if (null == lPhotographs)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.ACCESSING, "Error: Can't access user photographs!");
                return;
            }
            
            for (int i = 0; i < lPhotographs.Length; ++i)
            {
                ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.LOADING, "File: " + lPhotographs[i].Name);

                Photo photo = new Photo(lPhotographs[i]);

                if (null == photo) {
                    return;
                }

                AddPhoto(photo);
            }
        }
        
        private int GetInitialIndex(string strAppName, string strPhotoName)
        {
            if (null == strPhotoName || null == mPhotoSortedList)
            {
                return -1;
            }

            int iApplicationIndex = -1;
            for (int i = mPhotoSortedList.Count - 1; i < 0; --i)
            {
                Photo p = GetPhotoByIndex(i);
                if (string.Equals(strPhotoName, p.GetPhotoName()))
                {
                    return i;
                }

                if (-1 == iApplicationIndex && string.Equals(strAppName, p.GetApplicationName()))
                {
                    iApplicationIndex = i;
                }

            }

            return iApplicationIndex;
        }

        public int GetFirstSlideIndex()
        {
            return mFirstImageIndex;
        }

        public SlideSet GetSlideSet ()
        {
            return mSlideSet;
        }

        public void SetSlideSet (SlideSet slider)
        {
            mSlideSet = slider;
        }

        public bool IsEmpty()
        {
            return (0 == mPhotoSortedList.Count);
        }

        public int GetCount()
        {
            return mPhotoSortedList.Count;
        }

        public int GetCurrentIndex()
        {
            return mSlideSet.CurrentIndex;
        }

        public void SetCurrentIndex(int index)
        {
            if (null == mPhotoSortedList) {
                // Error, not initialized.
                return;
            }
            
            if (index < 0) {
                // Error, index inferior to allowed range of index.
                return;
            }

            if (mPhotoSortedList.Count <= index) {
                // Error, index superior to allowed range of index.
                return;
            }
            
            mSlideSet.GoTo(index);
        }

        public void AddPhoto(Photo photo)
        {
            if (null == mPhotoSortedList) {
                // Error, not initialized.
                return;
            }

            if (null == photo) {
                // Error, not initialized.
                return;
            }

            mPhotoSortedList.Add(photo.GetKeyValue(), photo);
        }
        
        public Photo GetPhotoByIndex(int index)
        {
            if (null == mPhotoSortedList) {
                // Error, not initialized.
                return null;
            }

            if (index < 0) {
                // Error, index inferior to allowed range of index.
                return null;
            }

            if (mPhotoSortedList.Count <= index) {
                // Error, index superior to allowed range of index.
                return null;
            }

            if (mPhotoSortedList.Count <= index) {
                // Error, index superior to allowed range of index.
                return null;
            }

            return (Photo)mPhotoSortedList.GetByIndex(index);
        }

        public Photo GetCurrentPhoto()
        {
            if (null == mPhotoSortedList) {
                // Error, not initialized.
                return null;
            }

            if (mSlideSet.CurrentIndex < 0) {
                // Error, index inferior to allowed range of index.
                return null;
            }

            if (mPhotoSortedList.Count <= mSlideSet.CurrentIndex) {
                // Error, index superior to allowed range of index.
                return null;
            }

            return (Photo)mPhotoSortedList.GetByIndex(mSlideSet.CurrentIndex);
        }

        public Photo GetNextPhoto()
        {
            if (null == mPhotoSortedList)
            {
                // Error, not initialized.
                return null;
            }

            if (mPhotoSortedList.Count - 1 == mSlideSet.CurrentIndex)
            {
                // Error, cannot get next photo, already last one.
                return null;
            }

            mSlideSet.GoNext();
            return (Photo)mPhotoSortedList.GetByIndex(mSlideSet.CurrentIndex);
        }

        public Photo GetPreviousPhoto()
        {
            if (null == mPhotoSortedList)
            {
                // Error, not initialized.
                return null;
            }

            if (0 == mSlideSet.CurrentIndex)
            {
                return null;
            }

            mSlideSet.GoPrevious();
            return (Photo)mPhotoSortedList.GetByIndex(mSlideSet.CurrentIndex);
        }
        
        public bool DeleteCurrentPhoto()
        {
            if (null == mPhotoSortedList || null == mSlideSet)
            {
                // Error, not initialized.
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Photo Manager not initialized.");
                return false;
            }

            if (mSlideSet.CurrentIndex < 0) {
                // Error, index inferior to allowed range of index.
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Slide index out of range.");
                return false;
            }

            if (mSlideSet.Count <= mSlideSet.CurrentIndex) {
                // Error, index superior to allowed range of index.
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Slide index out of range.");
                return false;
            }

            // Remove current slide
            int iCurrent = mSlideSet.CurrentIndex;
            
            try
            {
                if (!mSlideSet.RemoveSlide<PictureToast>(GetCurrentPhoto().GetSlide()))
                {
                    ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Could not delete current slide.");
                    return false;
                }
            }
            catch (Exception e)
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Exception caught : " + e);
            }
            
            if (!((Photo)mPhotoSortedList.GetByIndex(iCurrent)).Delete())
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Could not delete photo from disk.");
                return false;
            }

            // Remove photo from list
            mPhotoSortedList.RemoveAt(iCurrent);
            return true;
        }
    }
}