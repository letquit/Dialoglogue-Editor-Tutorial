using UnityEngine;

public class DialogueGetData : MonoBehaviour
{
    [SerializeField] protected DialogueContainerSO dialogueContainer;
    
    protected BaseNodeData GetNodeByGuid(string _targetNodeGuid)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _targetNodeGuid);
    }
    
    protected BaseNodeData GetNodeByNodePort(string _nodePort)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == _nodePort);
    }
    
    protected BaseNodeData GetNextNode(BaseNodeData _baseNodeData)
    {
        NodeLinkData nodeLinkData =
            dialogueContainer.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == _baseNodeData.NodeGuid);
        return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
    }
}
