﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipelines
{
    public class NodeMetadata
    {
        public int count;
        public string Name;
        public ILabeledNode Node;
    }

    public static class DotGraph
    {
        public static string Quoted(string value)
        {
            return $@"""{value}""";
        }

        public static HashSet<NodeMetadata> metadata;

        public static string FromPipeline<T>(InputPipe<T> root)
        {
            DotGraph.metadata = new HashSet<NodeMetadata>();

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


        public static StringBuilder ProcessTree(ILabeledNode node, StringBuilder result, Action<ILabeledNode, StringBuilder> processNode, Action<ILabeledNode, ILabeledNode, StringBuilder> processChild, HashSet<NodeMetadata> metadata)
        {
            processNode(node, result);

            foreach (var listener in node.Listeners)
            {
                processChild(node, listener, result);
                ProcessTree(listener, result, processNode, processChild, metadata);
            }
            return result;
        }

        internal static NodeMetadata CheckNameUnique(ILabeledNode node)
        {
            var existing = DotGraph.metadata.FirstOrDefault(_ => _.Node.Equals(node));
            if (existing != null)
            {
                return existing;
            }

            var name = node.Name;
            bool any = DotGraph.metadata.Any(_ => _.Node.Name == name);
            if (any)
            {
                var maxCount = DotGraph.metadata.Where(_ => _.Node.Name == name).Max(_ => _.count);
                maxCount++;
                var newMetadata = new NodeMetadata
                {
                    count = maxCount,
                    Name = Quoted(name + ' ' + maxCount),
                    Node = node,
                };
                DotGraph.metadata.Add(newMetadata);
                return newMetadata;
            }
            else
            {
                var newMetadata = new NodeMetadata
                {
                    count = 0,
                    Name = Quoted(name),
                    Node = node,
                };
                DotGraph.metadata.Add(newMetadata);
                return newMetadata;
            }
        }
    }
}