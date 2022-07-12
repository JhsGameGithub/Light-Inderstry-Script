using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public List<string> lv1Recipes;
    public List<Material> lv1Materials;
    public List<Material> lv0Materials;

    public Material GetLevelMaterial(int level, int productCode)
    {
        Material material = null;
        switch(level)
        {
            case 0:
                material = lv0Materials[productCode];
                break;

            case 1:
                material = lv1Materials[productCode];
                break;
            default:
                break;
        }
        return material;
    }

    public int GetRecipeSize(int level)
    {
        int size = 0;
        switch(level)
        {
            case 1:
                size = lv1Recipes.Count;
                break;
            default:
                break;
        }

        return size;
    }

    public string GetRecipe(int level, int index)
    {
        string recipe = null;
        switch(level)
        {
            case 1:
                recipe= lv1Recipes[index];
                break;
            default:
                break;
        }

        return recipe;
    }
}
