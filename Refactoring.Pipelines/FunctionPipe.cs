﻿using System;
using System.Collections.Generic;

namespace Refactoring.Pipelines
{
    public class FunctionPipe<TInput, TOutput> : Sender<TOutput>, IListener<TInput>, IFunctionPipe
    {
        private readonly Func<TInput, TOutput> _func;
        private readonly ISender<TInput> _predecessor;

        public FunctionPipe(Func<TInput, TOutput> func, ISender<TInput> predecessor)
        {
            _func = func;
            _predecessor = predecessor;
            predecessor.AddListener(this);
        }

        IGraphNode IFunctionPipe.Predecessor =>
            _predecessor;

        IGraphNode IGraphNodeWithOutput.Output =>
            new OutputNode(this, OutputType.ToReadableString());
        IEnumerable<IGraphNode> ISender.Children =>
            Listeners;

        public void OnMessage(TInput input)
        {
            var result = _func(input);
            _Send(result);
        }

        public override string Name =>
            $@"{_func.Method.DeclaringType.ToReadableString()}.{_func.Method.Name}()";

        public override IEnumerable<IGraphNode> Parents =>
            new[] {_predecessor};
    }

    internal class OutputNode : IGraphNode
    {
        private readonly string _name;
        private readonly IGraphNode _owner;

        public OutputNode(IGraphNode owner, string name)
        {
            _owner = owner;
            _name = name;
        }

        string IGraphNode.Name =>
            _name;

        IEnumerable<IGraphNode> IGraphNode.Parents =>
            new[] {_owner};

        public override bool Equals(object other)
        {
            return other is OutputNode that && _owner == that._owner;
        }

        public override int GetHashCode() { return 0; }
    }
}
