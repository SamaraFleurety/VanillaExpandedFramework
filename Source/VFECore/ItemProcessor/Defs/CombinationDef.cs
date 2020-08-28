﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ItemProcessor
{
    public class CombinationDef : Def
    {

        //CombinationDefs are used for single and multiple slot buildings. They are the "recipes" of this system.

        //They define a list of items as ingredients, and the resulting product, as well as yield

        //Product quality increasing attributes are assigned here.

        //This is to signal this is a single ingredient (slot) recipe. It's the default
        public bool singleIngredientRecipe = true;
        //defName of the building accepting this combination of ingredients
        public string building;
        //A list of combination's ingredient defNames
        public List<string> items;
        //defName of the resulting product
        public string result = "";
        //yield of the resulting product
        public int yield = 1;
        //Amount of the ingredients
        public List<int> amount;
        //Does the product's quality increase with time?
        public bool useQualityIncreasing = false;

        //This defines the recipe as a category instead of single item one
        public bool isCategoryRecipe = false;

        //If not, what is the single time period to process the product?
        public int singleTimeIfNotQualityIncreasing = 3;

        //If so, what are the periods to advance to the next quality level (these are not cumulative, they are all from 0 to each number)?
        public float awfulQualityAgeDaysThreshold = 1f;
        public float poorQualityAgeDaysThreshold = 3f;
        public float normalQualityAgeDaysThreshold = 8f;
        public float goodQualityAgeDaysThreshold = 14f;
        public float excellentQualityAgeDaysThreshold = 20f;
        public float masterworkQualityAgeDaysThreshold = 50f;
        public float legendaryQualityAgeDaysThreshold = 120f;

        //Custom message to show when the product finishes being processed
        public string finishedProductMessage = "IP_GenericProductFinished";

        //Does this combination produce different products in each quality level?
        public bool differentProductsByQuality = false;
        //A list of products if this combination produces different products by quality level (it must have 7 items)
        public List<string> productsByQuality = new List<string>();

    }
}