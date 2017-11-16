//using System;
//using NUnit.Framework;
//using System.Collections;

namespace BuddyApp.Weather
{
}

/*
namespace BuddyApp.Weather
{
	/// <summary>
	/// Phrase engine test.
	/// </summary>
	[TestFixture]
	public class PhraseEngineTest
	{
		private PhraseEngine phraseEngine;
		[SetUp]
		public void SetUp() {
			Representation [] representations = {
				new Representation("ajoute", new float[]{0.002f, 0.003f}),
				new Representation("moi", new float[]{0.002f, 0.003f}),
				new Representation("fleur", new float[]{0.002f, 0.003f}),
				new Representation("et", new float[]{0.002f, 0.003f}),
				new Representation("achete", new float[]{0.002f, 0.003f}),
				new Representation("achetes", new float[]{0.002f, 0.003f}),
				new Representation("toi", new float[]{0.002f, 0.003f}),
				new Representation("beaucoup", new float[]{0.002f, 0.003f}),
				new Representation("des", new float[]{0.002f, 0.003f}),
				new Representation("fleurs", new float[]{0.002f, 0.003f}),
				new Representation("roses", new float[]{0.002f, 0.003f})
			};
			Vocabulary vocabulary = new Vocabulary (representations, 2);
			this.phraseEngine = new PhraseEngine (vocabulary);
			this.phraseEngine.AddPhraseRef ("achetes moi des fleurs", "buy(fleur)");
			this.phraseEngine.AddPhraseRef ("achetes moi des roses", "buy(roses)");
			this.phraseEngine.AddPhraseRef ("ajoute des fleurs et des roses", "add(fleurs, roses)");
		}

		[TearDown]
		public void TearDown() {

		}

		[Test]
		public void TestAddAndGetAction() {

			ArrayList search_1 = this.phraseEngine.GetSimPhrases ("achete moi des fleurs et des roses", 3);

			foreach (DictionaryEntry phrase in search_1) {
				Assert.IsNotNull(this.phraseEngine.GetActypeOfAPhrase ((string)phrase.Key));
			}
			ArrayList search_2 = this.phraseEngine.GetSimPhrases ("achete moi des fleurs et des roses");

			foreach (DictionaryEntry phrase in search_2) {
				Assert.IsNotNull(this.phraseEngine.GetActypeOfAPhrase ((string)phrase.Key));
			}
		}


		[Test]
		public void TestExceptionPhraseNotRepresentable() {
			Assert.Throws<PhraseHasNoRepresentation> (() => this.phraseEngine.GetSimPhrases ("Du pamplemousse et de l'ail", 3));
			Assert.DoesNotThrow (() => this.phraseEngine.GetSimPhrases ("achete moi des fleurs et des roses"));
		}

	}
}
*/
