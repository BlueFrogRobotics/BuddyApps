using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Recipe
{
    public class IngredientScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject text;
        [SerializeField]
        private GameObject icon;

        public void FillIngredient(Ingredient iIngredient)
        {
            text.GetComponent<Text>().text = iIngredient.name + ": " + iIngredient.quantity + " " + iIngredient.unit;
            icon.GetComponent<Image>().sprite = Resources.Load(iIngredient.icon) as Sprite;
        }
    }
}