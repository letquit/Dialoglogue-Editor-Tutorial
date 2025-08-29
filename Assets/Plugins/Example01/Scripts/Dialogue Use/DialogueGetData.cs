using UnityEngine;

/// <summary>
/// 对话数据获取类，用于从对话容器中检索节点数据
/// </summary>
public class DialogueGetData : MonoBehaviour
{
    [SerializeField] protected DialogueContainerSO dialogueContainer;
    
    /// <summary>
    /// 根据节点GUID获取对应的节点数据
    /// </summary>
    /// <param name="targetNodeGuid">目标节点的唯一标识符</param>
    /// <returns>找到的节点数据，如果未找到则返回null</returns>
    protected BaseNodeData GetNodeByGuid(string targetNodeGuid)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == targetNodeGuid);
    }
    
    /// <summary>
    /// 根据节点端口获取对应的节点数据
    /// </summary>
    /// <param name="nodePort">节点端口标识符</param>
    /// <returns>找到的节点数据，如果未找到则返回null</returns>
    protected BaseNodeData GetNodeByNodePort(string nodePort)
    {
        return dialogueContainer.AllNodes.Find(node => node.NodeGuid == nodePort);
    }
    
    /// <summary>
    /// 获取指定节点的下一个节点数据
    /// </summary>
    /// <param name="baseNodeData">基础节点数据，用于查找其连接的下一个节点</param>
    /// <returns>下一个节点的数据</returns>
    protected BaseNodeData GetNextNode(BaseNodeData baseNodeData)
    {
        NodeLinkData nodeLinkData =
            dialogueContainer.NodeLinkDatas.Find(edge => edge.BaseNodeGuid == baseNodeData.NodeGuid);
        return GetNodeByGuid(nodeLinkData.TargetNodeGuid);
    }
}

