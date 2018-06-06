using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Buddy;

namespace BuddyApp.Radioplayer
{
    /// <summary>
    /// This class manage to show the radio informations and commands
    /// </summary>
    public class RadioUIManager : MonoBehaviour
    {
        [SerializeField]
        private Text radioName;

        [SerializeField]
        private Text showDescription;

        [SerializeField]
        private InputField radioSearch;

        [SerializeField]
        private RawImage image;

        [SerializeField]
        private Slider sliderVolume;

        public string RadioNameSearched { get { return radioSearch.text; } }

        // Use this for initialization
        void Start()
        {
            sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetShowDescription(string iDescription)
        {
            showDescription.text = iDescription;
        }

        public void SetRadioName(string iName)
        {
            radioName.text = iName;
        }

        public void SetPictureFromUrl(string iUrl)
        {
            StartCoroutine(LoadPicture(iUrl));
        }

        public void SwitchSLiderVolume()
        {
            sliderVolume.gameObject.SetActive(!sliderVolume.gameObject.activeInHierarchy);
        }

        private IEnumerator LoadPicture(string iUrl)
        {
            Texture2D tex;
            tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
            using (WWW www = new WWW(iUrl))
            {
                yield return www;
                www.LoadImageIntoTexture(tex);
                image.texture = tex;
            }
        }

        private void OnVolumeChanged(float iVolume)
        {
            int lVolume = (int)(iVolume * 100.0F);
            Debug.Log("volume: " + lVolume);
            BYOS.Instance.Primitive.Speaker.ChangeVolume(lVolume);
        }
    }
}