using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NeuronSpawner : MonoBehaviour
{
    public Transform character;
    public GameObject neuron;
    public GameObject pathway;
    private List<List<GameObject>> gameObjectBuffer;
    public int maxNetworkDepth = 5;
    // Start is called before the first frame update
    void Start()
    {
        gameObjectBuffer = new List<List<GameObject>>();
        spawnGameObject(5, character.transform.position);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void removeColliderFromLayer()
    {
        int size = gameObjectBuffer.Count;
        if (size == 0)
        {
            return;
        }
        var layer = gameObjectBuffer[size - 1];
        layer.ForEach(obj =>
        {
            if (obj.tag == "neuron")
            {
               obj.GetComponent<Collider2D>().enabled = false;

            }
            else if (obj.tag == "plate")
            {
                var components = obj.GetComponentsInChildren<EdgeCollider2D>();
                foreach (var component in components)
                {
                    component.enabled = false;
                }
            }
        });
    }

    public void spawnGameObject(int numberOfLinks, Vector3 spawnPoint)
    {
        int radius = 5;
        List<GameObject> layer = new List<GameObject>();
        removeColliderFromLayer();
        for (int i = 0; i < numberOfLinks; i++)
        {
            int factor = 360 / numberOfLinks;
            int angle = i * factor;
            var randomSpawnPosition = getPositionFromRadiusAndAngleAroundCharacter(radius, angle, spawnPoint);
            var gameObjectPathway = Instantiate(pathway, spawnPoint, Quaternion.identity);
            var gameObjectNeuron = Instantiate(neuron, randomSpawnPosition, Quaternion.identity);
            var size = gameObjectPathway.GetComponent<Renderer>().bounds.size.x;
            var scale = radius / (size + 1.2f);
            gameObjectPathway.transform.localScale = new Vector3(gameObjectPathway.transform.localScale.x * scale, gameObjectPathway.transform.localScale.y, gameObjectPathway.transform.localScale.z);
            size = gameObjectPathway.GetComponent<Renderer>().bounds.size.x;
         
            
            gameObjectPathway.transform.position += new Vector3((size / 2)+0.5f, 0, 0);

            gameObjectPathway.transform.RotateAround(spawnPoint, Vector3.forward, angle);

            layer.Add(gameObjectNeuron);
            layer.Add(gameObjectPathway);
        }
        gameObjectBuffer.Add(layer);
        checkBufferDepthAndDeleteObjects();
        assignLayersToEachObject();
    }

    private void assignLayersToEachObject()
    {
        for (int i = 0; i < gameObjectBuffer.Count; i++)
        {
            int layerNumber = i;

            gameObjectBuffer[i].ForEach(gameObj =>
            {
                gameObj.layer = LayerMask.NameToLayer(layerNumber.ToString());
            });
        }
    }

    private void checkBufferDepthAndDeleteObjects()
    {
        Debug.Log(gameObjectBuffer.Count);
        if (gameObjectBuffer.Count > maxNetworkDepth)
        {
            gameObjectBuffer[0].ForEach(obj => Destroy(obj));
            gameObjectBuffer.RemoveAt(0);
        }
    }

    private Vector3 getPositionFromRadiusAndAngleAroundCharacter(float radius, float angle, Vector3 spawnPoint)
    {
        float randomX = radius * Mathf.Cos((Mathf.PI / 180) * angle);
        float randomY = radius * Mathf.Sin((Mathf.PI / 180) * angle);
        return spawnPoint + new Vector3(randomX, randomY, 0);
        
    }
}
