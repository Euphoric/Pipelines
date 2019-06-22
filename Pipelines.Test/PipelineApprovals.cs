using ApprovalTests;
using ApprovalTests.Writers;

namespace Pipelines.Test
{
    public class PipelineApprovals
    {
        public static void Verify(IGraphNode input)
        {
            Approvals.Verify(WriterFactory.CreateTextWriter(DotGraph.DotGraph.FromPipeline(input), "dot"));
        }
    }
}