using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TextNode {
    public TextDescription worldObject {get; set;}
    public List<TextNode> children {get; private set;}
    public TextNode parent {get; private set;}
    public string textName {get; set;}

    //given a name, give a list of my immediate children that have this name somewhere in their tree
    // public Dictionary<string,List<TextNode>> nameDict {get; private set;}

    public TextNode() {
        children = new List<TextNode>();
        // nameDict = new Dictionary<string, List<TextNode>>();
    }

    public void SetParent(TextNode newParent) {
        // Debug.Log("set parent of " + textName + " to " + newParent.textName);
        if (parent != null) {
            if (parent == newParent) return;
            parent.children.Remove(this);
        }
        parent = newParent;
        parent.children.Add(this);
    }

    public void DetatchFromTree() {
        if (parent != null) {
            parent.children.Remove(this);
        }
        parent = null;
    }

    public String ToJson() {
        String childrenJsons = String.Join(", ",children.Select(x => x.ToJson()).ToArray());
        var centerSize = worldObject.GetCenterAndSize();
        //return $"{{ name : \"{worldObject.GetName()}\", Description : {worldObject.GetDescription()}\", children : [{childrenJsons}]}}";
        return $"{{ name : {worldObject.GetName()}, description : {worldObject.GetDescription()}," +
            $" scripts : [{string.Join(", ", worldObject.GetScripts())}]," +
            $" center : {centerSize.Item1}" +
            $" scale : {centerSize.Item2}" +
            $"children : [{childrenJsons}]}}";
    }
    public string ToSimpleJson()
    {
        String childrenJsons = String.Join(", ", children.Select(x => x.ToSimpleJson()).ToArray());
        return $"{{ name : {worldObject.GetName()}," +
            $"children : [{childrenJsons}]}}";
    }
}


public class TextTree : Singleton<TextTree>
{
    private TextNode root = new TextNode();
    private Dictionary<string, List<TextNode>> nameToNodes = new Dictionary<string, List<TextNode>>();
    private List<(int,TextDescription)> allDescriptions = new List<(int, TextDescription)>();
    private Dictionary<TextDescription, TextNode> descriptionToNodes = new Dictionary<TextDescription, TextNode>();
    private List<TextNode> movingNodes = new List<TextNode>();
    [SerializeField] private float refreshTime;
    private float lastRefreshTime = 0;

    public void AddToDescriptionList(TextDescription description, int parentCount) {
        allDescriptions.Add((parentCount, description));
    }

    private bool setUp = false;
    private void Update() {
        if (setUp) return;
        allDescriptions.Sort((x,y) => x.Item1.CompareTo(y.Item1));
        foreach ((int _, TextDescription description) in allDescriptions)
        {
            description.AddToTree();
        }

        if (Time.time > lastRefreshTime + refreshTime) {
            lastRefreshTime = Time.time;
            Refresh();
        }
    }

    private TextNode BestParent(TextNode node,TextNode start) {
        // Debug.Log("testing " + node.textName + "with parent " + start.textName);
        foreach (TextNode child in start.children)
        {
            (Vector3 nodeCenter, Vector3 nodeSize) = node.worldObject.GetCenterAndSize();
            (Vector3 childCenter, Vector3 childSize) = child.worldObject.GetCenterAndSize();
            // Debug.Log("checking " + node.textName + "for parent " + child.textName + "\n node: "
            // + nodeCenter + "  " + nodeSize + "\n chil: " + childCenter + "  " + childSize);
            Vector3 v = nodeCenter - childCenter;
            if ((Mathf.Abs(v.x) <= childSize.x)
                && (Mathf.Abs(v.y) <= childSize.y)
                && (Mathf.Abs(v.z) <= childSize.z)
                && (nodeSize.x <= childSize.x)
                && (nodeSize.y <= childSize.y)
                && (nodeSize.z <= childSize.z)
                && PossibleParent(node.worldObject.movementLevel,child.worldObject.movementLevel)) {
                    return BestParent(node,child);
                }
        }
        return start;
    }

    private bool PossibleParent(MovementLevel child, MovementLevel parent) {
        switch (child) {
            case MovementLevel.Movable:
                return true;
            case MovementLevel.Stationary:
                return parent == MovementLevel.Stationary;
            case MovementLevel.Moving:
                return true;
        }
        return false;
    }

    public void AddNode(TextDescription worldObject, string textName, bool moving = false) {
        // Debug.Log("adding node " + textName + "moving= " + moving);
        TextNode node = new TextNode {
            worldObject = worldObject,
            textName = textName
        };
        descriptionToNodes[worldObject] = node;
        PlaceNodeInTree(node);
        if (!nameToNodes.ContainsKey(worldObject.GetName())) nameToNodes[worldObject.GetName()] = new List<TextNode>();
        nameToNodes[worldObject.GetName()].Add(node);
        // Debug.Log("current json: " + ToJson());
        if (moving) {
            movingNodes.Add(node);
        }
    }

    private void Refresh() {
        foreach (TextNode movingNode in movingNodes) {
            movingNode.DetatchFromTree();
            PlaceNodeInTree(movingNode);
        }
    }

    public void UpdateNode(TextDescription worldObject) {
        //update all moving nodes first
        Refresh();
        //then update the actual node
        if (!descriptionToNodes.ContainsKey(worldObject)) return;
        TextNode node = descriptionToNodes[worldObject];
        // Debug.Log("Updating " + node.textName);
        node.DetatchFromTree();
        PlaceNodeInTree(node);
        // GM.Instance.AskQuestion();
    }

    public void PlaceNodeInTree(TextNode node) {
        TextNode p = BestParent(node,root);
        // Debug.Log("the node parent for " + node.textName + " is " + p.textName);
        var prevChildren = p.children.ToArray();
        node.SetParent(p);
        foreach (TextNode item in prevChildren)
        {
            item.DetatchFromTree();
            item.SetParent(BestParent(item,p));
        }
        // Debug.Log("current json: " + ToJson());
    }

    public GameObject StringToGameobject(string name) {
        
        // foreach (var k in nameToNodes.Keys) {
        //     Debug.Log(k);
        // }
        if (nameToNodes.ContainsKey(name)) {
            var nodes = nameToNodes[name];
            GameObject ret = nodes[0].worldObject.gameObject;
            return ret;
        }
        return null; //ask GPT to generalize
    }

    public TextDescription StringToDescription(string name) {
        if (nameToNodes.ContainsKey(name)) {
            return nameToNodes[name][0].worldObject;
        }
        return null;
    }

    public String ToJson() {
        Debug.Log($"simple Json: {{ \"root\": [{String.Join(", ", root.children.Select(x => x.ToSimpleJson()).ToArray())}] }}");
        return $"{{ \"root\": [{String.Join(", ",root.children.Select(x => x.ToJson()).ToArray())}] }}";
    }

    public string prettyJson() {
        string jsonString = ToJson();
        JObject json = JObject.Parse(jsonString);

        // Convert the JSON object back to a formatted JSON string
        string prettyJson = JsonConvert.SerializeObject(json, Formatting.Indented);

        return prettyJson;
    }

    public List<string> GetAllTextNodeNames()
    {
        List<string> names = new List<string>();
        GetAllTextNodeNamesRecursive(root, names);
        return names;
    }

    private void GetAllTextNodeNamesRecursive(TextNode node, List<string> names)
    {
        names.Add(node.worldObject.GetName().ToLower());
        foreach (var child in node.children)
        {
            GetAllTextNodeNamesRecursive(child, names);
        }
    }

    public List<TextNode> GetRelatedNodes(List<string> keywords)
    {
        HashSet<TextNode> relatedNodes = new HashSet<TextNode>();

        foreach (string keyword in keywords)
        {
            if (nameToNodes.ContainsKey(keyword))
            {
                foreach (TextNode node in nameToNodes[keyword])
                {
                    AddNodeAndAncestors(node, relatedNodes);
                }
            }
        }

        return relatedNodes.ToList();
    }

    private void AddNodeAndAncestors(TextNode node, HashSet<TextNode> relatedNodes)
    {
        if (!relatedNodes.Contains(node))
        {
            relatedNodes.Add(node);
            if (node.parent != null)
            {
                AddNodeAndAncestors(node.parent, relatedNodes);
            }
            foreach (TextNode child in node.children)
            {
                AddNodeAndAncestors(child, relatedNodes);
            }
        }
    }

    public string ToJsonRelated(List<string> keywords)
    {
        List<TextNode> relatedNodes = GetRelatedNodes(keywords);
        return $"{{ \"root\": [{String.Join(", ", relatedNodes.Select(x => x.ToJson()).ToArray())}] }}";
    }


}
