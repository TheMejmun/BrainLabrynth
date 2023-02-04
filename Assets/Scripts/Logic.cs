using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Unity.Burst.Intrinsics.Arm;

public class Logic : MonoBehaviour
{

    public GameObject tunnelPrefab;
    public GameObject thoughtGameObject;
    public GameObject neuronPrefab;
    private WeirdWikiManager wikiManager = new WeirdWikiManager();
    private List<string> solvingTitles;
    private Vector3 lastPrincipalPosition;
    private List<GameObject> tunnels = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        lastPrincipalPosition = thoughtGameObject.transform.position;
        string currentTitle = "Albert Einstein";
        placeJoinedTunnels(thoughtGameObject, currentTitle);
        solvingTitles = computeSolvingTitles("United States", 3);
    }

    // Update is called once per frame
    void Update()
    {
       /* float distance = Vector3.Distance(thoughtGameObject.transform.position, lastPrincipalPosition);
        if(distance > 8.4)
        {
            float minDist = 10000;
            string closestTunnelText = "err";
            GameObject closestTunnel = tunnels[0];
            foreach (GameObject tunnel in tunnels)
            {
                float dist = Vector3.Distance(tunnel.transform.position, thoughtGameObject.transform.position);
                if (dist < minDist)
                {
                    closestTunnel = tunnel;
                    closestTunnelText = closestTunnel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
                    minDist = dist;
                }
            }

            print(closestTunnelText);
            placeJoinedTunnels(thoughtGameObject, closestTunnelText);
            lastPrincipalPosition = thoughtGameObject.transform.position;
        }  */
    }


    public List<string> computeSolvingTitles(string startTitle, int pathLength)
    {
        List<string> solvingPath = new List<string>();
        solvingPath.Add(startTitle);

        WeirdNodeData iterationNodeData = wikiManager.GetNode(startTitle);
        for (int i = 0; i < pathLength; i++)
        {
            string selectedPageTitle = iterationNodeData.LinksTo[Random.Range(0, iterationNodeData.LinksTo.Count)];
            solvingPath.Add(selectedPageTitle);
            iterationNodeData = wikiManager.GetNode(selectedPageTitle);
        } 

        return solvingPath;
    }

    public void onNeuronTriggered(Transform neuronPosition)
    {
        float minDist = 10000;
        string closestTunnelText = "err";
        GameObject closestTunnel = tunnels[0];
        foreach (GameObject tunnel in tunnels)
        {
            float dist = Vector3.Distance(tunnel.transform.position, thoughtGameObject.transform.position);
            if (dist < minDist)
            {
                closestTunnel = tunnel;
                closestTunnelText = closestTunnel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
                minDist = dist;
            }
        }

        print(closestTunnelText);
        lastPrincipalPosition = neuronPosition.position;
        placeJoinedTunnels(thoughtGameObject, closestTunnelText);
    }

    public void placeJoinedTunnels(GameObject thoughtGameObject, string currentTitle)
    {

        // Get current position of thought game object, we want to place the tunnels around the player
        Vector3 thoughtGameObjectPosition = thoughtGameObject.transform.position;
        // Calculate angle depending on how many tunnels to place

        WeirdNodeData nodeData = wikiManager.GetNode(currentTitle);
        int numberOfTunnels = System.Math.Min(nodeData.LinksTo.Count, 5);
        List<string> data = nodeData.LinksTo.GetRange(0, numberOfTunnels);
        //print(data);

        Vector3 initialDirection = new Vector3(1, 0, 0);
        int rotationAngle = 360 / numberOfTunnels;

        for (int i = 0; i < numberOfTunnels; i++)
        {
            // Calculate the rotation around z-axis
            Quaternion rotationQuaternion = Quaternion.AngleAxis(rotationAngle * i, transform.forward);
            Vector3 shiftDirection = rotationQuaternion * initialDirection * 0.7f;
            // Place the tunnel
            TMPro.TextMeshProUGUI textMesh = tunnelPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            textMesh.text = data[i];
            var tunnelInstance = Instantiate(tunnelPrefab, thoughtGameObjectPosition + shiftDirection, rotationQuaternion);
            var neuronInstance = Instantiate(neuronPrefab, getPositionFromRadiusAndAngleAroundCharacter(8.4f, rotationAngle * i, lastPrincipalPosition), Quaternion.identity);
            //lastPrincipalPosition = neuronInstance.transform.position;
            tunnels.Add(tunnelInstance);
        }
    }

    private Vector3 getPositionFromRadiusAndAngleAroundCharacter(float radius, float angle, Vector3 spawnPoint)
    {
        float randomX = radius * Mathf.Cos((Mathf.PI / 180) * angle);
        float randomY = radius * Mathf.Sin((Mathf.PI / 180) * angle);
        return spawnPoint + new Vector3(randomX, randomY, 0);

    }
}
