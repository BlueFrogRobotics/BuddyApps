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
        private static string mStrFirstPhotoPath = null;
        private static SortedList mLPhotoSortedList = new SortedList();
        private static SlideSet mSlideSet = null;
        private static int mIFirstImageIndex;

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

        public void Initialize(string iStrAppName, string iStrPhotoName)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Initializing application with image directory...");

            if (null != mLPhotoSortedList)
            {
                mLPhotoSortedList.Clear();
            }

            // Scan repository to initialize the photo list
            ScanPhotographs();

            // Determine first index
            int lIFirstIndex;

            // First : check if input file exists
            // Second : check if app has photo
            lIFirstIndex = GetInitialIndex(iStrAppName, iStrPhotoName);
            if (-1 != lIFirstIndex)
            {
                mIFirstImageIndex = lIFirstIndex;
            }
            else
            {
                mIFirstImageIndex = mLPhotoSortedList.Count - 1; // Last : Set to last if nothing if found
            }
        }

        public void Free()
        {
            mStrFirstPhotoPath = null;
            mLPhotoSortedList = new SortedList();
            mSlideSet = null;
            mIFirstImageIndex = -1;
        }
        
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
        
        private int GetInitialIndex(string iStrAppName, string iStrPhotoName)
        {
            if (null == iStrPhotoName || null == mLPhotoSortedList)
            {
                return -1;
            }

            int iApplicationIndex = -1;
            for (int i = mLPhotoSortedList.Count - 1; i < 0; --i)
            {
                Photo lPhoto = GetPhotoByIndex(i);
                if (string.Equals(iStrPhotoName, lPhoto.GetPhotoName()))
                {
                    return i;
                }

                if (-1 == iApplicationIndex && string.Equals(iStrAppName, lPhoto.GetApplicationName()))
                {
                    iApplicationIndex = i;
                }

            }

            return iApplicationIndex;
        }

        public int GetFirstSlideIndex()
        {
            return mIFirstImageIndex;
        }

        public SlideSet GetSlideSet ()
        {
            return mSlideSet;
        }

        public void SetSlideSet (SlideSet iSlider)
        {
            mSlideSet = iSlider;
        }

        public bool IsEmpty()
        {
            return (0 == mLPhotoSortedList.Count);
        }

        public int GetCount()
        {
            return mLPhotoSortedList.Count;
        }

        public int GetCurrentIndex()
        {
            return mSlideSet.CurrentIndex;
        }

        public void SetCurrentIndex(int iIndex)
        {
            if (null == mLPhotoSortedList) {
                // Error, not initialized.
                return;
            }
            
            if (iIndex < 0) {
                // Error, index inferior to allowed range of index.
                return;
            }

            if (mLPhotoSortedList.Count <= iIndex) {
                // Error, index superior to allowed range of index.
                return;
            }
            
            mSlideSet.GoTo(iIndex);
        }

        public void AddPhoto(Photo iPhoto)
        {
            if (null == mLPhotoSortedList) {
                // Error, not initialized.
                return;
            }

            if (null == iPhoto) {
                // Error, not initialized.
                return;
            }

            mLPhotoSortedList.Add(iPhoto.GetKeyValue(), iPhoto);
        }
        
        public Photo GetPhotoByIndex(int iIndex)
        {
            if (null == mLPhotoSortedList) {
                // Error, not initialized.
                return null;
            }

            if (iIndex < 0) {
                // Error, index inferior to allowed range of index.
                return null;
            }

            if (mLPhotoSortedList.Count <= iIndex) {
                // Error, index superior to allowed range of index.
                return null;
            }

            return (Photo)mLPhotoSortedList.GetByIndex(iIndex);
        }

        public Photo GetCurrentPhoto()
        {
            if (null == mLPhotoSortedList) {
                // Error, not initialized.
                return null;
            }

            if (mSlideSet.CurrentIndex < 0) {
                // Error, index inferior to allowed range of index.
                return null;
            }

            if (mLPhotoSortedList.Count <= mSlideSet.CurrentIndex) {
                // Error, index superior to allowed range of index.
                return null;
            }

            return (Photo)mLPhotoSortedList.GetByIndex(mSlideSet.CurrentIndex);
        }

        public Photo GetNextPhoto()
        {
            if (null == mLPhotoSortedList)
            {
                // Error, not initialized.
                return null;
            }

            if (mLPhotoSortedList.Count - 1 == mSlideSet.CurrentIndex)
            {
                // Error, cannot get next photo, already last one.
                return null;
            }

            mSlideSet.GoNext();
            return (Photo)mLPhotoSortedList.GetByIndex(mSlideSet.CurrentIndex);
        }

        public Photo GetPreviousPhoto()
        {
            if (null == mLPhotoSortedList)
            {
                // Error, not initialized.
                return null;
            }

            if (0 == mSlideSet.CurrentIndex)
            {
                return null;
            }

            mSlideSet.GoPrevious();
            return (Photo)mLPhotoSortedList.GetByIndex(mSlideSet.CurrentIndex);
        }
        
        public bool DeleteCurrentPhoto()
        {
            if (null == mLPhotoSortedList || null == mSlideSet)
            {
                // Error, not initialized.
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Photo Manager not initialized.");
                return false;
            }

            if (mSlideSet.CurrentIndex < 0)
            {
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
            int lICurrent = mSlideSet.CurrentIndex;
            
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
            
            if (!((Photo)mLPhotoSortedList.GetByIndex(lICurrent)).Delete())
            {
                ExtLog.E(ExtLogModule.APP, GetType(), LogStatus.FAILURE, LogInfo.DELETING, "Could not delete photo from disk.");
                return false;
            }

            // Remove photo from list
            mLPhotoSortedList.RemoveAt(lICurrent);
            return true;
        }
    }
}