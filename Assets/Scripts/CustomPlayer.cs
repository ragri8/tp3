using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CustomPlayer : MonoBehaviour
{
    public ColorPickerUnityUI color1;
    public ColorPickerUnityUI color2;
    public Slider skincolor;
    public Slider weight;
    public Slider size;
    public GameObject player;
    public GameObject[] apparences;
    public Texture2D[] textures;
    public Transform hip;
    public Transform head;
    public Transform armL;
    public Transform armR;
    public Transform legL;
    public Transform legR;
    public Material material;
    private int currentapparence = 0;
    private float weightValue ;
    private float sizeValue ;
    private float skinValue ;
    private Color valueColor1;
    private Color valueColor2;
    private bool colorhaschanged=false;
    // Start is called before the first frame update
    void Start()
    {
        updateTransform();//initialisation tranform + texture
        updateTexture(0,false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!weightValue.Equals(weight.value) || !sizeValue.Equals(size.value))//quand il y a des changement pour le transform
        {
            updateTransform();
        }

        if (!valueColor1.Equals(color1.value) || !valueColor2.Equals(color2.value))//quand il ya des changement pour la texture
        {
            colorhaschanged = true;
            updateTexture(currentapparence,true);
        }

        if (!skinValue.Equals(skincolor.value))
        {
            updateTexture(currentapparence,colorhaschanged);
        }
    }

    public void updateTransform()
    {
        weightValue = weight.value;
        sizeValue = size.value;
        player.transform.localScale=Vector3.one*size.value;
        hip.localScale= new Vector3(1, weightValue, 1);
        head.localScale= new Vector3(1, 1/weightValue, 1);
        armL.localScale=new Vector3(weightValue,1,weightValue);
        armR.localScale=new Vector3(weightValue,1,weightValue);
        legL.localScale=new Vector3(1,weightValue,weightValue);
        legR.localScale=new Vector3(1,weightValue,weightValue);
    }

    public void updateTexture(int nbapparence,bool changecolor)
    {
        skinValue = skincolor.value;
        valueColor1 = color1.value;
        valueColor2 = color2.value;
        Texture2D whiteskin = textures[nbapparence * 3];
        Texture2D blackskin = textures[nbapparence * 3 +1];
        Texture2D clothes = textures[nbapparence * 3 +2];
        Texture2D customtexture=new Texture2D(whiteskin.height,whiteskin.width);
        Color[] colormix = whiteskin.GetPixels();
        Color[] colorblack = blackskin.GetPixels();
        Color[] colorclothes = clothes.GetPixels();
        for (int i = 0; i < colormix.Length; i++)
        {
            if (!colorclothes[i].Equals(Color.white) && changecolor) //priorité au vetements
            {
                if (colorclothes[i].Equals(new Color(0.2f,0.2f,0.2f)))
                {
                    colormix[i] = valueColor1*colormix[i].maxColorComponent;
                }
                else if (colorclothes[i].Equals(Color.black) )
                {
                    colormix[i] = valueColor2*colormix[i].maxColorComponent;
                }
                else
                {
                    colormix[i] = colorclothes[i];
                }
                 
            }
            else
            {
                colormix[i]= colormix[i]*(1-skinValue) + colorblack[i]*skinValue;
            }
        }
        customtexture.SetPixels(colormix);
        customtexture.Apply();
        material.mainTexture = customtexture;
        apparences[nbapparence].GetComponent<SkinnedMeshRenderer>().material = material;

    }
    public void next()
    { 
        apparences[currentapparence].SetActive(false);
        currentapparence=(currentapparence+1)%12;
        colorhaschanged = false;
        updateTexture(currentapparence,false);
        apparences[currentapparence].SetActive(true);
    }

    public void previous()
    {
        apparences[currentapparence].SetActive(false);
        currentapparence=(currentapparence-1>0)?currentapparence-1:11;
        colorhaschanged = false;
        updateTexture(currentapparence,false);
        apparences[currentapparence].SetActive(true);
    }
}
