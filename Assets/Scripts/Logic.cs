
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{

    public GameObject tunnelPrefab;
    public GameObject thoughtGameObject;
    public GameObject neuronPrefab;
    private List<string> solvingTitles;
    private Vector3 lastPrincipalPosition;
    private float rotationOfLastChosenTunnel;
    private List<List<GameObject>> gameObjectBuffer;
    public int maxNetworkDepth;

    private List<TunnelDataHolder> tunnelDataList;


    // Start is called before the first frame update
    void Start()
    {
        gameObjectBuffer = new List<List<GameObject>>();
        tunnelDataList = new List<TunnelDataHolder>();
        lastPrincipalPosition = thoughtGameObject.transform.position;

        //print("TEST");
        //return;
        WikiManager.Instance.RequestRandomNode((node) => {
            placeJoinedTunnels(node, thoughtGameObject, node.Title);
           // solvingTitles = computeSolvingTitles(node, 3);
        });
        
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


    public List<string> computeSolvingTitles(NodeData startTitle, int pathLength)
    {
        List<string> solvingPath = new List<string>();
        solvingPath.Add(startTitle.Title);

        /*NodeData iterationNodeData = WikiManager.Instance.GetRandomNode();
        for (int i = 0; i < pathLength; i++)
        {
            string selectedPageTitle = iterationNodeData.LinksTo[Random.Range(0, iterationNodeData.LinksTo.Count)];
            solvingPath.Add(selectedPageTitle);
            iterationNodeData = WikiManager.Instance.GetNode(selectedPageTitle);
        } */

        return solvingPath;
    }

    public void onNeuronTriggered(Transform neuronPosition)
    {
        float minDist = 10000;
        string closestTunnelText = "err";
        
        tunnelDataList.ForEach(tunnel =>
        {
            float dist = Vector3.Distance(tunnel.position, thoughtGameObject.transform.position);
            if (dist < minDist)
            {
                closestTunnelText = tunnel.text;
                minDist = dist;
                rotationOfLastChosenTunnel = tunnel.rotation.eulerAngles.z;
            }
        });

        // delete all old entries from tunnels
        tunnelDataList.Clear();

        print(closestTunnelText);
        lastPrincipalPosition = neuronPosition.position;
        removeColliderFromLayer();
        WikiManager.Instance.RequestNode(closestTunnelText , (nodeData) =>
        {
            placeJoinedTunnels(nodeData, thoughtGameObject, closestTunnelText);

        });
        checkBufferDepthAndDeleteObjects();
    }

    public void placeJoinedTunnels(NodeData nodeData, GameObject thoughtGameObject, string currentTitle)
    {

        // Get current position of thought game object, we want to place the tunnels around the player
        Vector3 thoughtGameObjectPosition = thoughtGameObject.transform.position;
        // Calculate angle depending on how many tunnels to place

        int numberOfTunnels = System.Math.Min(nodeData.LinksTo.Count, 5);
       
        List<string> data = nodeData.LinksTo.GetRange(0, numberOfTunnels);
        //print(data);


        Vector3 initialDirection = new Vector3(1, 0, 0);
        int rotationAngle = 360 / numberOfTunnels;
        List<GameObject> layer = new List<GameObject>();
        for (int i = 0; i < numberOfTunnels; i++)
        {
            // Calculate the rotation around z-axis
            var angle = rotationAngle * i;
            Debug.Log(angle + "this is angle number " + i);
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angle, transform.forward);
            Vector3 shiftDirection = rotationQuaternion * initialDirection * 0.7f;
            // Place the tunnel
            TMPro.TextMeshProUGUI textMesh = tunnelPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            textMesh.text = data[i];
            var tunnelInstance = Instantiate(tunnelPrefab, thoughtGameObjectPosition + shiftDirection, rotationQuaternion);
            var neuronInstance = Instantiate(neuronPrefab, getPositionFromRadiusAndAngleAroundCharacter(8.4f, angle, thoughtGameObjectPosition), Quaternion.identity);
            //lastPrincipalPosition = neuronInstance.transform.position;
            layer.Add(tunnelInstance);
            layer.Add(neuronInstance);

            TunnelDataHolder tunnelData = new TunnelDataHolder();
            tunnelData.text = data[i];
            tunnelData.position = tunnelInstance.transform.position;
            tunnelData.rotation = rotationQuaternion;
            tunnelDataList.Add(tunnelData);
        }
        gameObjectBuffer.Add(layer);
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

    private Vector3 getPositionFromRadiusAndAngleAroundCharacter(float radius, float angle, Vector3 spawnPoint)
    {
        float randomX = radius * Mathf.Cos((Mathf.PI / 180) * angle);
        float randomY = radius * Mathf.Sin((Mathf.PI / 180) * angle);
        return spawnPoint + new Vector3(randomX, randomY, 0);

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

    struct TunnelDataHolder
    {
        public string text;
        public Vector3 position;
        public Quaternion rotation;
    }
}
