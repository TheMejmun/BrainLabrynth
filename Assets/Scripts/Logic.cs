using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Unity.Burst.Intrinsics.Arm;

public class Logic : MonoBehaviour
{

    public GameObject tunnelPrefab;
    public GameObject neuronPrefab;
    public GameObject characterGameObject;

    private WikiManager wikiManager = new WikiManager();

    private List<string> solvingTitles;
    private UnityEngine.Vector3 lastVisitedNeuronPosition;
    private List<GameObject> tunnels = new List<GameObject>();
    private List<GameObject> neurons = new List<GameObject>();
    private List<GameObject> unusedNeurons = new List<GameObject>();
    private string currentTitle;

    // Called before first frame
    void Start()
    {
        // Compute solution titles (f.e. USA -> Donald Trump -> Miami)
        solvingTitles = computeSolutionTitles(3);

        // Draw first neuron and tunnels at characters initial position using first solution title
        string neuronTitle = solvingTitles[0];
        lastVisitedNeuronPosition = characterGameObject.transform.position;
        drawNeuronAndTunnels(neuronTitle);
    }

    public List<string> computeSolutionTitles(int solutionSteps)
    {
        List<string> solvingPath = new List<string>();
        NodeData currentNode = wikiManager.GetNode("United States");
        //NodeData currentNode = wikiManager.GetRandomNode();
        solvingPath.Add(currentNode.Title);

        for (int i = 0; i < solutionSteps; i++)
        {
            string nextTitle = currentNode.LinksTo[Random.Range(0, currentNode.LinksTo.Count)];
            solvingPath.Add(nextTitle);

            currentNode = wikiManager.GetNode(nextTitle);
        } 

        return solvingPath;
    }

    // TODO: Generally: How to handle <5 adjacent neurons
    public void drawNeuronAndTunnels(string neuronTitle)
    {
        // TODO: Async this somehow??
        NodeData nodeData = wikiManager.GetNode(neuronTitle);
        int numTunnels = System.Math.Min(nodeData.LinksTo.Count, 5); // Maximally 5 adjacent neurons
        List<string> adjacentNeuronTitles = nodeData.LinksTo.GetRange(0, numTunnels);
        
        // Calculate rotation angle for tunnels
        int rotationAngle = 360 / numTunnels;

        // unitVector used for shifting stuff into place later
        Vector3 unitRight = new Vector3(1, 0, 0);

        for (int i = 0; i < numTunnels; i++)
        {
            // Rotation around z-axis as quaternion
            // Rotate unitRight into unitTunnelDirection
            Quaternion rotationQuaternion = Quaternion.AngleAxis(rotationAngle * i, transform.forward);
            Vector3 unitTunnelDirection = rotationQuaternion * unitRight;

            // TODO: Title should be in neuron not in tunnel
            tunnelPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = adjacentNeuronTitles[i];
            var tunnelInstance = Instantiate(tunnelPrefab, lastVisitedNeuronPosition + unitTunnelDirection * 0.9f, rotationQuaternion);
            var neuronInstance = Instantiate(neuronPrefab, lastVisitedNeuronPosition + (unitTunnelDirection * 7) + (unitTunnelDirection * 1.8f), Quaternion.identity);

            tunnels.Add(tunnelInstance);
            neurons.Add(neuronInstance);
            unusedNeurons.Add(neuronInstance);
        }
    }


    public void onNeuronTriggered(GameObject neuron)
    {

        lastVisitedNeuronPosition = neuron.transform.position;

        if (unusedNeurons.Contains(neuron)) {

            print(unusedNeurons.Count);
            unusedNeurons.RemoveAll(r => r.transform.position == neuron.transform.position);
            print(unusedNeurons.Count);

            float minDist = 10000;
            string closestTunnelText = "err";
            GameObject closestTunnel = tunnels[0];
            foreach (GameObject tunnel in tunnels)
            {
                float dist = Vector3.Distance(tunnel.transform.position, characterGameObject.transform.position);
                if (dist < minDist)
                {
                    closestTunnel = tunnel;
                    closestTunnelText = closestTunnel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;
                    minDist = dist;
                }
            }

            print(closestTunnelText);
            drawNeuronAndTunnels(closestTunnelText);
        }
    }

}
