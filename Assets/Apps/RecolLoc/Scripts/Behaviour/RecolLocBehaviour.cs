using Buddy;

using UnityEngine;
using UnityEngine.UI;

using System;
using Buddy.UI;

using System.IO;
using System.Collections.Generic;

namespace BuddyApp.RecolLoc
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class RecolLocBehaviour : MonoBehaviour
	{
		/*
         * Data of the application. Save on disc when app is quitted
         */
		private RecolLocData mAppData;
		private AndroidJavaObject mRecoLocModule;
		private AndroidJavaObject mCurrentActivity;
		//private AndroidJavaObject mParameters;
		private AndroidJavaObject mVerificationResult;
		//private AndroidJavaObject mVerificationResult2;
		//private AndroidJavaObject mVerificationResult3;

		public InputField mModelName;
		public UnityEngine.UI.Button mTrain;
		public UnityEngine.UI.Button mRecordModel;
		public UnityEngine.UI.Button mTestReco;
		private bool mTrainRecording;
		private AudioClip mClipRecord;
		public Text mResult;

		private

		void Start()
		{
			/*
			* You can setup your App activity here.
			*/
			RecolLocActivity.Init(null);

			mTrainRecording = false;

			mTrain.onClick.AddListener(TrainClicked);
			mRecordModel.onClick.AddListener(RecordModelClicked);
			mTestReco.onClick.AddListener(TestRecoClicked);



			/*
			* Init your app data
			*/
			mAppData = RecolLocData.Instance;


			if (Application.platform == RuntimePlatform.Android) {
				using (AndroidJavaClass lMainClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
					mCurrentActivity = lMainClass.GetStatic<AndroidJavaObject>("currentActivity");
					//using (AndroidJavaClass lParametersClass = new AndroidJavaClass("com.bfr.parameters.Parameters")) {
					//	Debug.Log("PARAMETERS");
					//	mParameters = lParametersClass.CallStatic<AndroidJavaObject>("instance");
					//	mParameters.Call("setContext", mCurrentActivity);
					//}
					//using (AndroidJavaClass lAlizeSpkDetSystemClass = new AndroidJavaClass("AlizeSpkRec.SimpleSpkDetSystem")) {
					Debug.Log("RECO LOC MODULE6");
					//mRecoLocModule = lAlizeSpkDetSystemClass.CallStatic<AndroidJavaObject>("getInstance", "AlizeDefault.txt", BYOS.Instance.Resources.GetPathToRaw(""));
					Debug.Log("RECO LOC path to config file: " + BYOS.Instance.Resources.GetPathToRaw("AlizeDefault.txt").Replace("AlizeDefault.txt", ""));
					mRecoLocModule = new AndroidJavaObject("AlizeSpkRec.SimpleSpkDetSystem", "AlizeDefault.txt", BYOS.Instance.Resources.GetPathToRaw("AlizeDefault.txt").Replace("AlizeDefault.txt", ""));
					//mRecoLocModule.Call("SetContext", mCurrentActivity);

					mRecoLocModule.Call("loadBackgroundModel", "world");

					Debug.Log("Status after initialization:");
					Debug.Log("  # of features: " + mRecoLocModule.Call<long>("featureCount"));
					Debug.Log("  # of models: " + mRecoLocModule.Call<long>("speakerCount"));
					Debug.Log("  UBM loaded: " + mRecoLocModule.Call<bool>("isUBMLoaded"));
					Debug.Log("***********************************************");


					/*

					// Record audio
					// (we load it from an asset, because it's easier than recording from the microphone for a quick demo)
					//byte[] tmpAudio = new byte[100000];
					//InputStream audioInput = BYOS.Instance.Resources.GetPathToRaw("data/PB2M-2_UV.s16");
					//int audioSize = audioInput.read(tmpAudio);
					//audioInput.close();
					//byte[] audio = new byte[audioSize];
					//System.arraycopy(tmpAudio, 0, audio, 0, audioSize);

					byte[] lAudio = File.ReadAllBytes(BYOS.Instance.Resources.GetPathToRaw("data/PB2M-2_UV.bin"));


					// Send audio to the system
					mRecoLocModule.Call("addAudio", lAudio);

					// Train a model with the audio
					mRecoLocModule.Call("createSpeakerModel", "UV");

					Debug.Log("Status after training first speaker model (UV):");
					Debug.Log("  # of features: " + mRecoLocModule.Call<long>("featureCount"));
					Debug.Log("  # of models: " + mRecoLocModule.Call<long>("speakerCount"));
					Debug.Log("  UBM loaded: " + mRecoLocModule.Call<bool>("isUBMLoaded"));
					Debug.Log("***********************************************");

					mRecoLocModule.Call("resetAudio");
					mRecoLocModule.Call("resetFeatures");

					// Record some more audio
					//audioInput = getApplicationContext().getAssets().open("data/PB2M-6_UV.s16");
					//audioSize = audioInput.read(tmpAudio);
					//audioInput.close();
					//audio = new byte[audioSize];
					//System.arraycopy(tmpAudio, 0, audio, 0, audioSize);


					byte[] lAudio2 = File.ReadAllBytes(BYOS.Instance.Resources.GetPathToRaw("data/PB2M-6_UV.bin"));

					// Send the new audio to the system
					mRecoLocModule.Call("addAudio", lAudio2);

					using (AndroidJavaClass lAlizeSpkRecResultClass = new AndroidJavaClass("AlizeSpkRec.SimpleSpkDetSystem$SpkRecResult")) {

						// Perform speaker verification against the model we created earlier
						//SpkRecResult verificationResult = mRecoLocModule.Call<SpkRecResult>("verifySpeaker", "UV");
						//mVerificationResult = new AndroidJavaObject("AlizeSpkRec.SimpleSpkDetSystem$SpkRecResult");
						Debug.Log("kikoo2testttttttt");
						mVerificationResult = mRecoLocModule.Call<AndroidJavaObject>("verifySpeaker", "UV");

						Debug.Log("Speaker verification against speaker UV:");
						Debug.Log("  match: " + mVerificationResult.Get<bool>("match"));
						Debug.Log("  score: " + mVerificationResult.Get<float>("score"));
						Debug.Log("***********************************************");


						// Extract a pre-built model that was packed with the application
						//copyAssetToLocalStorage("gmm/AG.gmm");

						// Load it into the system
						mRecoLocModule.Call("loadSpeakerModel", "AG", "AG");

						Debug.Log("Status after sending second audio and loading second speaker model:");
						Debug.Log("  # of features: " + mRecoLocModule.Call<long>("featureCount"));
						Debug.Log("  # of models: " + mRecoLocModule.Call<long>("speakerCount"));
						Debug.Log("  UBM loaded: " + mRecoLocModule.Call<bool>("isUBMLoaded"));
						Debug.Log("***********************************************");

						// Now that we have 2 speakers in the system, let's try identification
						// (we don't need to resend the audio signal, since we haven't called resetAudio and resetFeatures)
						//mVerificationResult2 = new AndroidJavaObject("AlizeSpkRec.SimpleSpkDetSystem$SpkRecResult");

						byte[] lAudio3 = File.ReadAllBytes(BYOS.Instance.Resources.GetPathToRaw("data/PB2M-2_WA.bin"));

						mRecoLocModule.Call("resetAudio");
						mRecoLocModule.Call("resetFeatures");

						// Send the new audio to the system
						mRecoLocModule.Call("addAudio", lAudio3);

						mVerificationResult2 = mRecoLocModule.Call<AndroidJavaObject>("identifySpeaker");

						Debug.Log("Result of speaker identification: data / PB2M - 2_WA.bin");
						Debug.Log("  closest speaker: " + mVerificationResult2.Get<string>("speakerId"));
						Debug.Log("  match: " + mVerificationResult2.Get<bool>("match"));
						Debug.Log("  score: " + mVerificationResult2.Get<float>("score"));
						Debug.Log("***********************************************");

						byte[] lAudio4 = File.ReadAllBytes(BYOS.Instance.Resources.GetPathToRaw("data/PB2M-2_AG.bin"));


						mRecoLocModule.Call("resetAudio");
						mRecoLocModule.Call("resetFeatures");

						// Send the new audio to the system
						mRecoLocModule.Call("addAudio", lAudio4);

						mVerificationResult3 = mRecoLocModule.Call<AndroidJavaObject>("identifySpeaker");

						Debug.Log("Result of speaker identification data/PB2M-2_AG.bin:");
						Debug.Log("  closest speaker: " + mVerificationResult3.Get<string>("speakerId"));
						Debug.Log("  match: " + mVerificationResult3.Get<bool>("match"));
						Debug.Log("  score: " + mVerificationResult3.Get<float>("score"));
						Debug.Log("***********************************************");



						// Now that we're done playing with this audio signal, let's not forget to unload it from the system
						// (This will remove some temporary files)
						mRecoLocModule.Call("resetAudio");
						mRecoLocModule.Call("resetFeatures");



					}
					*/
				}
				//}
				Screen.sleepTimeout = SleepTimeout.NeverSleep;


			}
		}

		private void TestRecoClicked()
		{
			AndroidJavaObject lVerificationResult = mRecoLocModule.Call<AndroidJavaObject>("identifySpeaker");
			mResult.text = lVerificationResult.Get<string>("speakerId");

			Debug.Log("kikoo4");
			Debug.Log("Result of speaker identification:");
			Debug.Log("  closest speaker: " + lVerificationResult.Get<string>("speakerId"));
			Debug.Log("  match: " + lVerificationResult.Get<bool>("match"));
			Debug.Log("  score: " + lVerificationResult.Get<float>("score"));
			Debug.Log("***********************************************");

			mRecoLocModule.Call("resetAudio");
			mRecoLocModule.Call("resetFeatures");


			Debug.Log("reset audio and feature");
		}


		private void RecordModelClicked()
		{

			mRecoLocModule.Call("createSpeakerModel", mModelName.text);

			Debug.Log("Status after training speaker model " + mModelName.text);
			Debug.Log("  # of features: " + mRecoLocModule.Call<long>("featureCount"));
			Debug.Log("  # of models: " + mRecoLocModule.Call<long>("speakerCount"));
			Debug.Log("  UBM loaded: " + mRecoLocModule.Call<bool>("isUBMLoaded"));
			Debug.Log("***********************************************");

			mRecoLocModule.Call("resetAudio");
			mRecoLocModule.Call("resetFeatures");

			Debug.Log("reset audio and feature");

		}

		private void TrainClicked()
		{

			if (!mTrainRecording) {
				// Start record for 10 secondes

				string lDevice = Microphone.devices[0];
				if (!string.IsNullOrEmpty(lDevice)) {
					mClipRecord = Microphone.Start(lDevice, false, 9, 44100);

					BYOS.Instance.Toaster.Display<CountdownToast>().With(
					10, EndRecordVocal);
				} else
					Debug.Log("Couldn't find microphone :/");


				mTrainRecording = true;
			}


		}

		private void EndRecordVocal()
		{
			//Micro Stopped
			float[] lSamples = new float[mClipRecord.samples * mClipRecord.channels];
			mClipRecord.GetData(lSamples, 0);
			var lByteArray = new byte[lSamples.Length * 4];

			// create a byte array and copy the floats into it.
			Buffer.BlockCopy(lSamples, 0, lByteArray, 0, lByteArray.Length);


			// create a second float array and copy the bytes into it...
			//var floatArray2 = new float[lByteArray.Length / 4];
			//Buffer.BlockCopy(lByteArray, 0, floatArray2, 0, lByteArray.Length);

			//for (int i = 0; i < lSamples.Length; ++i) {
			//	if (i < 5) {
			//		Debug.Log("first float preprocessing:" + lSamples[i]);
			//	}
			//}

			//// Save the audio bytes for testing:
			//for (int i = 0; i < floatArray2.Length; i++) {
			//	if (i < 5) {
			//		Debug.Log("first float postprocessing:" + floatArray2[i]);
			//	}
			//}

			//AudioClip audioClip = AudioClip.Create("testSound", floatArray2.Length, 1, 44100, false, false);
			//audioClip.SetData(floatArray2, 0);
			////And then play it


			//AudioClip audioClip2 = AudioClip.Create("testSound", lSamples.Length, 1, 44100, false, false);
			//audioClip2.SetData(lSamples, 0);

			//AudioSource.PlayClipAtPoint(audioClip, new Vector3(100, 100, 0), 1.0f);
			//Save(BYOS.Instance.Resources.GetPathToRaw("test.wav"), audioClip);
			//Debug.Log("saved file float to byte to float ");
			//Save(BYOS.Instance.Resources.GetPathToRaw("test2.wav"), audioClip2);
			//Debug.Log("saved file float");

			// Send the new audio to the system
			mRecoLocModule.Call("addAudio", lByteArray);
			mTrainRecording = false;
		}


		//	Copyright (c) 2012 Calvin Rien
		//        http://the.darktable.com
		//
		//	This software is provided 'as-is', without any express or implied warranty. In
		//	no event will the authors be held liable for any damages arising from the use
		//	of this software.
		//
		//	Permission is granted to anyone to use this software for any purpose,
		//	including commercial applications, and to alter it and redistribute it freely,
		//	subject to the following restrictions:
		//
		//	1. The origin of this software must not be misrepresented; you must not claim
		//	that you wrote the original software. If you use this software in a product,
		//	an acknowledgment in the product documentation would be appreciated but is not
		//	required.
		//
		//	2. Altered source versions must be plainly marked as such, and must not be
		//	misrepresented as being the original software.
		//
		//	3. This notice may not be removed or altered from any source distribution.
		//
		//  =============================================================================
		//
		//  derived from Gregorio Zanon's script
		//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734




		const int HEADER_SIZE = 44;

		public static bool Save(string filename, AudioClip clip)
		{
			if (!filename.ToLower().EndsWith(".wav")) {
				filename += ".wav";
			}

			var filepath = filename;

			Debug.Log(filepath);

			// Make sure directory exists if user is saving to sub dir.
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));

			using (var fileStream = CreateEmpty(filepath)) {

				ConvertAndWrite(fileStream, clip);

				WriteHeader(fileStream, clip);
			}

			return true; // TODO: return false if there's a failure saving the file
		}

		public static AudioClip TrimSilence(AudioClip clip, float min)
		{
			var samples = new float[clip.samples];

			clip.GetData(samples, 0);

			return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
		}

		public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
		{
			return TrimSilence(samples, min, channels, hz, false, false);
		}

		public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
		{
			int i;

			for (i = 0; i < samples.Count; i++) {
				if (Mathf.Abs(samples[i]) > min) {
					break;
				}
			}

			samples.RemoveRange(0, i);

			for (i = samples.Count - 1; i > 0; i--) {
				if (Mathf.Abs(samples[i]) > min) {
					break;
				}
			}

			samples.RemoveRange(i, samples.Count - i);

			var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);

			clip.SetData(samples.ToArray(), 0);

			return clip;
		}

		static FileStream CreateEmpty(string filepath)
		{
			var fileStream = new FileStream(filepath, FileMode.Create);
			byte emptyByte = new byte();

			for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
			{
				fileStream.WriteByte(emptyByte);
			}

			return fileStream;
		}

		static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
		{

			var samples = new float[clip.samples];

			clip.GetData(samples, 0);

			Int16[] intData = new Int16[samples.Length];
			//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

			Byte[] bytesData = new Byte[samples.Length * 2];
			//bytesData array is twice the size of
			//dataSource array because a float converted in Int16 is 2 bytes.

			int rescaleFactor = 32767; //to convert float to Int16

			for (int i = 0; i < samples.Length; i++) {
				intData[i] = (short)(samples[i] * rescaleFactor);
				Byte[] byteArr = new Byte[2];
				byteArr = BitConverter.GetBytes(intData[i]);
				byteArr.CopyTo(bytesData, i * 2);
			}

			fileStream.Write(bytesData, 0, bytesData.Length);
		}

		static void WriteHeader(FileStream fileStream, AudioClip clip)
		{

			var hz = clip.frequency;
			var channels = clip.channels;
			var samples = clip.samples;

			fileStream.Seek(0, SeekOrigin.Begin);

			Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
			fileStream.Write(riff, 0, 4);

			Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
			fileStream.Write(chunkSize, 0, 4);

			Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
			fileStream.Write(wave, 0, 4);

			Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
			fileStream.Write(fmt, 0, 4);

			Byte[] subChunk1 = BitConverter.GetBytes(16);
			fileStream.Write(subChunk1, 0, 4);

			UInt16 two = 2;
			UInt16 one = 1;

			Byte[] audioFormat = BitConverter.GetBytes(one);
			fileStream.Write(audioFormat, 0, 2);

			Byte[] numChannels = BitConverter.GetBytes(channels);
			fileStream.Write(numChannels, 0, 2);

			Byte[] sampleRate = BitConverter.GetBytes(hz);
			fileStream.Write(sampleRate, 0, 4);

			Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
			fileStream.Write(byteRate, 0, 4);

			UInt16 blockAlign = (ushort)(channels * 2);
			fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

			UInt16 bps = 16;
			Byte[] bitsPerSample = BitConverter.GetBytes(bps);
			fileStream.Write(bitsPerSample, 0, 2);

			Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
			fileStream.Write(datastring, 0, 4);

			Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
			fileStream.Write(subChunk2, 0, 4);

			//		fileStream.Close();
		}
	}

}