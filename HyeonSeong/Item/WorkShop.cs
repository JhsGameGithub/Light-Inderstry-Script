using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkShop : MonoBehaviour
{
    public RecipeManager recipeManager;

    public PropSpawner propSpawner;

    GameObject material;


    //일어나서 마저완성
    private void Crafting(GameObject mat1, GameObject mat2)
    {
        int craftlevel = 0;

        Product prop1 = mat1.GetComponent<Product>();
        Product prop2 = mat2.GetComponent<Product>();

        if (prop1.productCode == prop2.productCode)
            return;

        if (prop1.LevelCheck() == prop2.LevelCheck())
        {
            craftlevel = prop1.LevelCheck();
            craftlevel++;
        }

        if (craftlevel!=0)
        {
            int propCode1 = prop1.productCode;
            int propCode2 = prop2.productCode;

            Destroy(mat1.gameObject.GetComponent<Product>());
            Destroy(mat2.gameObject);
            propSpawner.currentMaterialNum--;

            switch (craftlevel)
            {
                case 1:
                    mat1.gameObject.AddComponent<FinishedProduct>();
                    mat1.gameObject.GetComponent<FinishedProduct>().Combine(propCode1, propCode2, ref recipeManager);
                    break;
                default:
                    break;
            }
            Debug.Log(mat1.gameObject.GetComponent<Product>().productCode);
        }
    }

    private void Start()
    {
        material = null;
    }

    private void OnTriggerStay(Collider other)
    {
        //태그비교
        if(other.CompareTag("Prop"))
        {
            if (material == null)
            {
                if(!other.GetComponent<Product>().OwnCheck())
                {
                    material = other.gameObject;
                    material.GetComponent<Product>().SetBinder(gameObject);
                }
            }
            else
            {
                if(material != other.gameObject)
                {
                    if (!other.GetComponent<Product>().OwnCheck())
                    {
                        Crafting(material, other.gameObject);
                        material.transform.position = gameObject.transform.position;
                        material.transform.rotation = gameObject.transform.rotation;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject==material)
        {
            material = null;
        }
    }
}
