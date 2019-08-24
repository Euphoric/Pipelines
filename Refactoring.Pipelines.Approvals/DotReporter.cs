﻿using System;
using System.IO;
using ApprovalTests.Reporters;

namespace Refactoring.Pipelines.Approvals
{
    public class DotReporter : GenericDiffReporter
    {
        public static readonly DotReporter INSTANCE = new DotReporter();

        public DotReporter() : base(new DiffInfo("{ProgramFiles}Microsoft VS Code\\bin\\Code.cmd", "-r {0}", () => new[] {"dot"})) { }

    }
}
