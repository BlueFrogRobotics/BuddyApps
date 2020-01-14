using BlueQuark;
using System;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    /// <summary>
    /// Struct from JSON for the List of recipe
    /// </summary>
    [Serializable]
    public class Result
    {
        public int id;
        public string title;
        public int readyInMinutes;
        public int servings;
        public string image;
        public List<string> imageUrls;

        public Result()
        {
            imageUrls = new List<string>();
        }
    }
    [Serializable]
    public class RootObjectList
    {
        public List<Result> results;
        public string baseUri;
        public int offset;
        public int number;
        public int totalResults;
        public int processingTimeMs;
        public long expires;

        public RootObjectList()
        {
            results = new List<Result>();
        }
    }
    [Serializable]
    public class Us
    {
        public double amount;
        public string unitShort;
        public string unitLong;
    }
    [Serializable]
    public class Metric
    {
        public double amount;
        public string unitShort;
        public string unitLong;
    }
    [Serializable]
    public class Measures
    {
        public Us us;
        public Metric metric;
    }
    [Serializable]
    public class ExtendedIngredient
    {
        public int id;
        public string aisle;
        public string image;
        public string consitency;
        public string name;
        public string original;
        public string originalString;
        public string originalName;
        public double amount;
        public string unit;
        public List<object> meta;
        public List<object> metaInformation;
        public Measures measures;

        public ExtendedIngredient()
        {
            meta = new List<object>();
            metaInformation = new List<object>();
        }
    }
    [Serializable]
    public class ProductMatch
    {
        public int id;
        public string title;
        public string description;
        public string price;
        public string imageUrl;
        public double averageRating;
        public double ratingCount;
        public double score;
        public string link;
    }
    [Serializable]
    public class WinePairing
    {
        public List<string> pairedWines;
        public string pairingText;
        public List<ProductMatch> productMatches;

        public WinePairing()
        {
            pairedWines = new List<string>();
            productMatches = new List<ProductMatch>();
        }
    }
    [Serializable]
    public class Length
    {
        public int number;
        public string unit;
    }
    [Serializable]
    public class Ent
    {
        public int Id;
        public string name;
        public string image;
    }
    [Serializable]
    public class Step
    {
        public int number;
        public string step;
        public List<Ent> ingredients;
        public List<Ent> equipment;
        public Length length;

        public Step()
        {
            ingredients = new List<Ent>();
            equipment = new List<Ent>();
        }
    }


    [Serializable]
    public class AnalyzedInstruction
    {
        public string name;
        public List<Step> steps;

        public AnalyzedInstruction()
        {
            steps = new List<Step>();
        }
    }
    [Serializable]
    public class RootObject
    {
        public bool vegetarian;
        public bool vegan;
        public bool glutenFree;
        public bool dairyFree;
        public bool veryHealthy;
        public bool cheap;
        public bool veryPopular;
        public bool sustainable;
        public int weightWatcherSmartPoints;
        public string gaps;
        public bool lowFodmap;
        public bool ketogenic;
        public bool whole30;
        public int preparationMinutes;
        public int cookingMinutes;
        public string sourceUrl;
        public string spoonacularSourceUrl;
        public int aggregateLikes;
        public double spoonacularScore;
        public double healthScore;
        public string creditsText;
        public string sourceName;
        public double pricePerServing;
        public List<ExtendedIngredient> extendedIngredients;
        public int id;
        public string title;
        public int readyInMinutes;
        public int servings;
        public string image;
        public string imageType;
        public List<string> cuisines;
        public List<string> dishTypes;
        public List<object> diets;
        public List<object> occasions;
        public WinePairing winePairing;
        public string instructions;
        public List<AnalyzedInstruction> analyzedInstructions;

        public RootObject()
        {
            extendedIngredients = new List<ExtendedIngredient>();
            cuisines = new List<string>();
            dishTypes = new List<string>();
            diets = new List<object>();
            occasions = new List<object>();
            analyzedInstructions = new List<AnalyzedInstruction>();
        }
    }

    /* Data are stored in xml file for persistent data purpose */
    public class RecipeData : AAppData
    {
        public string mRecipeString { get; set; }
        public int mIdRecipeWanted { get; set; }
        public int mIndexRecipe { get; set; }
        public RootObjectList mRootObjectList { get; set; }
        public RootObject mRootObject { get; set; }
        public List<string> mListIngredient { get; set; }
        public string mGoogleCredentials { get; set; }
        public int mIndexStep { get; set; }
        private bool _mUserWantMovingBuddy = true;
        public bool mUserWantMovingBuddy { get { return _mUserWantMovingBuddy; } set { _mUserWantMovingBuddy = value; } }
        /*
         * Data singleton access
         */
        public static RecipeData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RecipeData>();
                return sInstance as RecipeData;
            }
        }
    }
}
