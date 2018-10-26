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
        public static readonly string[] ALLOWED_EXTENSION_LIST = { "*.png", "*.jpg" };
        
        // Singleton design pattern
        [SerializeField]
        private static readonly PhotoManager mPhotoManagerInstance = new PhotoManager();
       
        [SerializeField]
        private static SortedList mPhotoSortedList = null;


        [SerializeField]
        private static SlideSet mSlideSet = null;

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

        public void Initialize()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "Initializing application with image directory...");

            mPhotoSortedList = new SortedList();
            // TODO: Scan repository to initialize the photo list

            ScanDirectory(Buddy.Resources.GetRawFullPath(STR_GALLERY_DIRECTORY));
        }

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

                    Photo p = new Photo(strCurrentFile);

                    if (null == p)
                    {
                        return;
                    }
                
                    string key = "";
                    p.GetKeyValue(ref key);
                    AddPhoto(ref key, ref p);
                }
            }
        }

        //public SlideSet SlideSet { get { return mSlideSet; } set { mSlideSet = value; } }

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

        public void AddPhoto(ref string key, ref Photo ioPhoto)
        {
            if (null == mPhotoSortedList) {
                // Error, not initialized.
                ioPhoto = null;
                return;
            }

            mPhotoSortedList.Add(key, ioPhoto);
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