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

    private int numberOfLinks = 5;
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
            obj.GetComponent<Collider2D>().enabled = false;
        });
    }

    public void spawnGameObject(int numberOfLinks, Vector3 spawnPoint)
    {
        int randomRadius = (Random.Range(0, 3) + 5);
        List<GameObject> layer = new List<GameObject>();
        removeColliderFromLayer();
        for (int i = 0; i < numberOfLinks; i++)
        {
            int factor = 360 / numberOfLinks;
            int angle = i * factor;
            var randomSpawnPosition = getPositionFromRadiusAndAngleAroundCharacter(randomRadius, angle, spawnPoint);
            var gameObjectNeuron = Instantiate(neuron, randomSpawnPosition, Quaternion.identity);
            var gameObjectPathway = Instantiate(pathway, spawnPoint, Quaternion.identity);
            var size = gameObjectPathway.GetComponent<Renderer>().bounds.size.x;
            var scale = randomRadius / size;
            gameObjectPathway.transform.localScale = new Vector3(gameObjectPathway.transform.localScale.x * scale, gameObjectPathway.transform.localScale.y, gameObjectPathway.transform.localScale.z);
            size = gameObjectPathway.GetComponent<Renderer>().bounds.size.x;
            if (randomRadius >= 0)
            {
                gameObjectPathway.transform.position += new Vector3(size / 2, 0, 0);

            }
            else
            {
                gameObjectPathway.transform.position -= new Vector3(size / 2, 0, 0);
            }
            gameObjectPathway.transform.RotateAround(spawnPoint, Vector3.forward, angle);

            layer.Add(gameObjectNeuron);
            layer.Add(gameObjectPathway);
        }
        gameObjectBuffer.Add(layer);
        checkBufferDepthAndDeleteObjects();
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
