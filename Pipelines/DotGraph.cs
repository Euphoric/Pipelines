﻿using System;
using System.Text;

namespace Pipelines
{
    public static class DotGraph
    {
        public static string Quoted(string value)
        {
            return $@"""{value}""";
        }


        public static string FromPipeline<T>(InputPipe<T> root)
        {
            var metadata = new NodeMetadata(root);

            return $@"
digraph G {{ node [style=filled, shape=rec]

# Nodes
{DotGraphNodes.AppendNodeAndChildren(root, metadata)}

# Formatting
{DotGraphFormatting.AppendFormatting(root, metadata)}
{DotGraphRanking.AppendRankings(root, metadata)}

}}
".Trim();
        }


        public static StringBuilder ProcessTree(ILabeledNode node, StringBuilder result, Action<ILabeledNode, NodeMetadata, StringBuilder> processNode, Action<ILabeledNode, ILabeledNode, NodeMetadata, StringBuilder> processChild, NodeMetadata metadata)
        {
            processNode(node, metadata, result);

            foreach (var listener in node.Listeners)
            {
                processChild(node, listener, metadata, result);
                ProcessTree(listener, result, processNode, processChild, metadata);
            }
            return result;
        }
    }
}