﻿using System;
using System.Collections.Generic;
using System.Linq;
using Refactoring.Pipelines.ReflectionUtilities;

namespace Refactoring.Pipelines
{
    public static class JoinedPipes
    {
        public static JoinedPipes<TOutput1, TOutput2> JoinTo<TOutput1, TOutput2>(
            this ISender<TOutput1> sender1,
            ISender<TOutput2> sender2)
        {
            return new JoinedPipes<TOutput1, TOutput2>(sender1, sender2);
        }
    }

    public class JoinedPipes<TInput1, TInput2> : Sender<Tuple<TInput1, TInput2>>, IGraphNodeWithOutput
    {
        private readonly ForwardingListener<TInput1> _listener1;
        private readonly ForwardingListener<TInput2> _listener2;
        private readonly ISender<TInput1> _sender1;
        private readonly ISender<TInput2> _sender2;
        private readonly Queue<TInput1> _values1 = new Queue<TInput1>();
        private readonly Queue<TInput2> _values2 = new Queue<TInput2>();

        private JoinedPipes()
        {
            _listener1 = new ForwardingListener<TInput1>(this, OnMessage1);
            _listener2 = new ForwardingListener<TInput2>(this, OnMessage2);
        }

        public JoinedPipes(ISender<TInput1> sender1, ISender<TInput2> sender2) : this()
        {
            _sender1 = sender1;
            _sender2 = sender2;
            sender1.AddListener(_listener1);
            sender2.AddListener(_listener2);
        }

        public override string Name =>
            "Join";

        public override IEnumerable<IGraphNode> Parents =>
            new IGraphNode[] {_sender1, _sender2};

        IGraphNode IGraphNodeWithOutput.Output =>
            new OutputNode(this, OutputType.ToReadableString());

        private void OnMessage1(TInput1 value)
        {
            _values1.Enqueue(value);
            _SendIfReady();
        }

        private void OnMessage2(TInput2 value)
        {
            _values2.Enqueue(value);
            _SendIfReady();
        }

        private void _SendIfReady()
        {
            if (_values1.Any() && _values2.Any())
            {
                _Send(Tuple.Create(_values1.Dequeue(), _values2.Dequeue()));
            }
        }
    }
}
