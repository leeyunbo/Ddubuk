using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class POIData
{
    public Location location;
    public string name;
}

[System.Serializable]
public class OverpassRequestData
{
    [Tooltip("The SouthWest end of the bounding box.")]
    public Location SouthWest;

    [Tooltip("The NorthEast end of the bounding box.")]
    public Location NorthEast;
}

[System.Serializable]
public class OpenStreetMapOptions
{
    [Tooltip("A XML with the results of a Overpass API query. You can use http://overpass-turbo.eu/ to generate one.")]
    public TextAsset OsmXmlFile;

    [Tooltip("If true, instead of the XML file above, we fetch the data directly from a Overpass API request to https://www.overpass-api.de/api/interpreter.")]
    public bool FetchFromOverpassApi = false;

    [Tooltip("The data configuration used for the Overpass API request. Basically a bounding box rectangle defined by two points.")]
    public OverpassRequestData overPassRequestData;
}

public class CreatePointOfInterestTextMeshes : MonoBehaviour {
    [Tooltip("The height of the text mesh, relative to the device.")]
    public float height = 1f;

    [Tooltip("The TextMesh prefab.")]
    public TextMesh textPrefab;

    [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled."), Range(0, 500)]
    public float movementSmoothingFactor = 100.0f;

    [Tooltip("Locations where the text will be displayed. The text will be the Label property. (Optional)")]
    public Location[] locations;

    [Tooltip("Use this to either fetch OpenStreetMap data from a Overpass API request, or via a locally stored XML file. (Optional)")]
    public OpenStreetMapOptions openStreetMapOptions;


    POIData[] poiData;
    List<ARLocationManagerEntry> entries = new List<ARLocationManagerEntry>();
    string xmlFileText;
    
    private ARLocationManager manager;

    // Use this for initialization
    void Start () {
        manager = ARLocationManager.Instance;
        // CreateTextObjects();

        AddLocationsPOIs();

        if(openStreetMapOptions.FetchFromOverpassApi && openStreetMapOptions.overPassRequestData != null)
        {
            StartCoroutine("LoadXMLFileFromOverpassRequest");
        }
        else if (openStreetMapOptions.OsmXmlFile != null)
        {
            LoadXMLFileFromTextAsset();
        }
        
	}

    private void LoadXMLFileFromTextAsset()
    {
        CreateTextObjects(openStreetMapOptions.OsmXmlFile.text);
    }

    string GetOverpassRequestURL(OverpassRequestData data)
    {
        var lat1 = data.SouthWest.latitude;
        var lng1 = data.SouthWest.longitude;
        var lat2 = data.NorthEast.latitude;
        var lng2 = data.NorthEast.longitude;

        return "https://www.overpass-api.de/api/interpreter?data=[out:xml];node[amenity]("+ lat1 + "," +  lng1 + "," + lat2 + "," + lng2 + ");out%20meta;";
    }

    IEnumerator LoadXMLFileFromOverpassRequest()
    {
        var www = UnityWebRequest.Get(GetOverpassRequestURL(openStreetMapOptions.overPassRequestData));

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(GetOverpassRequestURL(openStreetMapOptions.overPassRequestData));
            yield break;
        }
        else
        {
            // Show results as text
            CreateTextObjects(www.downloadHandler.text);
            yield break;
        }
    }
    

    public string GetNodeTagValue(XmlNode node, string tagName)
    {
        var children = node.ChildNodes;
        foreach (XmlNode tag in children)
        {
            if (tag.Attributes["k"].Value == tagName)
            {
                return tag.Attributes["v"].Value;
            }
        }

        return null;
    }

    public string GetNodeName(XmlNode node)
    {
        var name = GetNodeTagValue(node, "name");

        if (name == null)
        {
            name = GetNodeTagValue(node, "amenity");
        }

        if (name == null)
        {
            name = "No Name";
        }

        return name;
    }
	
	// Update is called once per frame
	void CreateTextObjects(string xmlFileText) {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFileText);

        var nodes = xmlDoc.GetElementsByTagName("node");

        poiData = new POIData[nodes.Count];

        var i = 0;
        foreach (XmlNode node in nodes)
        {                        
            float lat = float.Parse(node.Attributes["lat"].Value);
            float lng = float.Parse(node.Attributes["lon"].Value);

            var name = GetNodeName(node);


            poiData[i] = new POIData
            {
                location = new Location(lat, lng, height),
                name = name
            };

            i++;
        }

        
        for (var k = 0; k < poiData.Length; k++)
        {
            AddPOI(poiData[k].location, poiData[k].name);
        }
    
    }

    void AddLocationsPOIs()
    {
        foreach (var location in locations)
        {
            AddPOI(location, location.label);
        }
    }


    void AddPOI(Location location, string name)
    {
        var textInstance = Instantiate(textPrefab.gameObject);
        textInstance.GetComponent<TextMesh>().text = name;

        entries.Add(new ARLocationManagerEntry
        {
            instance = textInstance,
            location = location,
            options = new ARLocationObjectOptions
            {
                isHeightRelative = true,
                showDebugInfoPanel = true,
                createInstance = false,
                movementSmoothingFactor = movementSmoothingFactor
            }
        });

        manager.Add(entries[entries.Count - 1]);
    }
}
