using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NeuronSpawner : MonoBehaviour
{
    public Transform character;
    public GameObject neuron;
    public GameObject pathway;
    private int numberOfLinks = 5;
    // Start is called before the first frame update
    void Start()
    {

        int randomRadius = (Random.Range(0, 3) + 5);



        for (int i = 0; i < numberOfLinks; i++)
        {
            int factor = 360 / numberOfLinks; 
            int angle = i * factor;
            var randomSpawnPosition = getPositionFromRadiusAndAngleAroundCharacter(randomRadius, angle);
            Debug.Log(randomSpawnPosition);
            Instantiate(neuron, randomSpawnPosition, Quaternion.identity);
            var gameObjectPathway = Instantiate(pathway, character.position, Quaternion.identity);
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
            gameObjectPathway.transform.RotateAround(character.position, Vector3.forward, angle);

        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 getPositionFromRadiusAndAngleAroundCharacter(float radius, float angle)
    {
        float randomX = radius * Mathf.Cos((Mathf.PI / 180) * angle);
        float randomY = radius * Mathf.Sin((Mathf.PI / 180) * angle);
        return character.position + new Vector3(randomX, randomY, 0);
        
    }
}
