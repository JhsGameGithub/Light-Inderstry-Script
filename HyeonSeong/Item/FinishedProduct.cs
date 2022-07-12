using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedProduct : Product
{
    protected override void Awake()
    {
        base.Awake();
        productScore = 1;
        productLevel = 1;
    }

    public void Combine(int mat1, int mat2, ref RecipeManager recipe)
    {
        string combineCode;

        int temp1 = mat1;
        int temp2 = mat2;

        if(mat1>mat2)
        {
            temp1 = mat2;
            temp2 = mat1;
        }

        combineCode = temp1.ToString();
        combineCode += temp2.ToString();

        Debug.Log(combineCode);

        for (int i = 0; i < recipe.GetRecipeSize(productLevel); i++)
        {
            if (combineCode == recipe.GetRecipe(productLevel, i)) 
            {
                base.productCode = i;
                renderer.material = recipe.GetLevelMaterial(productLevel, productCode);
                Debug.Log(productCode);
                break;
            }
        }
    }
}
