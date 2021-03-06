﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawColor : MonoBehaviour
{
    public Shader drawShader;
    public Shader clearShader;
    public Material drawMaterial;
    public Material clearMaterial;
    public Material myMaterial;
    [SerializeField]
    private List<RenderTexture> _splatMap = new List<RenderTexture>();
    [SerializeField]
    private List<GameObject> _terrain;
    [Range(0f, 500)]
    public float _brushSize;
    [Range(0.01f, 1)]
    public float _texSize;
    [Range(0, 1)]
    public float _brushStrength;
    [SerializeField]
    Texture2D[] splatTexture = new Texture2D[12];

    

    public List<GameObject> _Terrain { get => _terrain; set => _terrain = value; }

    private void Start()
    {

        //_terrain = GameObject.Find("Ground");
        drawMaterial = new Material(drawShader);
        clearMaterial = new Material(clearShader);
        drawMaterial.SetVector("_Color", Color.red);
        drawMaterial.SetTexture("_SplatTex", splatTexture[0]);
        clearMaterial.SetTexture("_SplatTex", splatTexture[0]);
        for (int i = 0; i < _terrain.Count; i++)
        {
            if(_terrain[i].GetComponent<Renderer>().materials.Length > 1)
            {
                Material[] _tempMaterial = _terrain[i].GetComponent<MeshRenderer>().materials;
                myMaterial = _tempMaterial[1];
            }
            else
            {
                myMaterial = _terrain[i].GetComponent<MeshRenderer>().material;
            }

            RenderTexture rend = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
            rend.name = "RenderTex " + i;
            _splatMap.Add(rend);

          //  Debug.Log(myMaterial);
            myMaterial.SetTexture("_SplatMap", _splatMap[i]);
        }
        
        
    }

    public void DrawOnSplatmap(RaycastHit hit, int id, Player player, float _sizeMultiplier = 1f)
    {

        int terrainNum = _terrain.IndexOf(hit.collider.gameObject);

        int _currentSplat = UnityEngine.Random.Range(0, 11);

        //coreCalc.Instance.CircleLogic(hit, (UInt16)id, terrainNum);

        drawMaterial.SetFloat("_Size", 0.1f * _sizeMultiplier);
        drawMaterial.SetTexture("_SplatTex", splatTexture[_currentSplat]);

        clearMaterial.SetFloat("_Size", 0.1f * _sizeMultiplier);
        clearMaterial.SetTexture("_SplatTex", splatTexture[_currentSplat]);


        //Debug.Log(terrainNum);

        drawMaterial.SetColor("_Color", SetColour(id));
        drawMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
        clearMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));

        RenderTexture temp = RenderTexture.GetTemporary(_splatMap[terrainNum].width, _splatMap[terrainNum].height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(_splatMap[terrainNum], temp);
        Graphics.Blit(temp, _splatMap[terrainNum], clearMaterial);
        Graphics.Blit(temp, _splatMap[terrainNum], drawMaterial);
        RenderTexture.ReleaseTemporary(temp);

       // Debug.Log("SplatMapHit:" + _splatMap[terrainNum].name);

        player.playerScore += 1;
    }

    private Color SetColour(int id)
    {

        Color color = new Color(0,0,0,0);

        switch (id)
        {
            case (0):
                {
                    color = new Color(1, 0, 0, 0);
                    break;
                }
            case (1):
                {
                    color = new Color(0, 1, 0, 0);
                    break;
                }
            case (2):
                {
                    color = new Color(0, 0, 1, 0);
                    break;
                }
            case (3):
                {
                    color = new Color(0, 0, 0, 1);
                    break;
                }
        }

        return color;

    }

    private void OnGUI()
    {
        ////USE TO VIEW A SPLATMAP
        //GUI.DrawTexture(new Rect(0, 0, 256, 128), _splatMap[0], ScaleMode.ScaleToFit, false, 1);
    }
}
