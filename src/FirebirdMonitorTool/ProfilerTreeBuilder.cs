using System;
using System.Collections;
using System.Collections.Generic;
using FirebirdMonitorTool.Attachment;
using FirebirdMonitorTool.Common;
using FirebirdMonitorTool.Function;
using FirebirdMonitorTool.Procedure;
using FirebirdMonitorTool.Statement;
using FirebirdMonitorTool.Transaction;
using FirebirdMonitorTool.Trigger;

namespace FirebirdMonitorTool
{
    public sealed class ProfilerTreeBuilder
    {
        public sealed class Node : IReadOnlyList<Node>
        {
            private readonly List<Node> m_Children;

            public ICommand Command { get; }
            public Node Parent { get; private set; }

            public Node(ICommand command)
            {
                m_Children = new List<Node>();
                Command = command;
            }

            public void AddChild(Node node)
            {
                node.Parent = this;
                m_Children.Add(node);
            }

            public override string ToString() => $"Command: {Command.GetType().Name}";

            public int Count => m_Children.Count;
            public Node this[int index] => m_Children[index];
            public IEnumerator<Node> GetEnumerator() => m_Children.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private readonly Dictionary<(long internalTraceId, long connectionId), Node> m_LiveNodes;
        private readonly Monitor m_Monitor;

        public event EventHandler<Node> OnNode;
        public event EventHandler<Exception> OnError
        {
            add { m_Monitor.OnError += value; }
            remove { m_Monitor.OnError -= value; }
        }

        public ProfilerTreeBuilder()
        {
            m_LiveNodes = new Dictionary<(long internalTraceId, long connectionId), Node>();
            m_Monitor = new Monitor();
            m_Monitor.OnCommand += (sender, command) =>
            {
                ProcessCommand(command);
            };
        }

        public void Process(string input)
        {
            m_Monitor.Process(input);
        }

        public void Flush()
        {
            m_Monitor.Flush();
        }

        public void LoadFile(string file)
        {
            m_Monitor.LoadFile(file);
        }

        private void ProcessCommand(ICommand command)
        {
            switch (command)
            {
                case not IAttachment:
                    // considering only attachment bound commands
                    break;

                case IAttachmentStart attachmentStart:
                    {
                        var root = new Node(null);
                        AddNextForRoot(attachmentStart, root);
                        m_LiveNodes.Add((attachmentStart.InternalTraceId, attachmentStart.ConnectionId), root);
                        break;
                    }
                case IAttachmentEnd attachmentEnd:
                    {
                        if (m_LiveNodes.Remove((attachmentEnd.InternalTraceId, attachmentEnd.ConnectionId), out var root))
                        {
                            AddNextForRoot(attachmentEnd, root);
                            OnNode?.Invoke(this, root);
                        }
                        break;
                    }

                case IAttachment attachmentCommand:
                    AddNext(attachmentCommand);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command.GetType().Name);
            }
        }

        private Node AddNext(IAttachment attachmentCommand)
        {
            if (!m_LiveNodes.TryGetValue((attachmentCommand.InternalTraceId, attachmentCommand.ConnectionId), out var root))
            {
                return null;
            }
            return AddNextForRoot(attachmentCommand, root);
        }

        private static Node AddNextForRoot(IAttachment attachmentCommand, Node root)
        {
            var node = new Node(attachmentCommand);
            GetCurrentNode(GetWorkingNode(root), attachmentCommand).AddChild(node);
            return node;
        }

        private static Node GetCurrentNode(Node node, ICommand command)
        {
            return command is IAttachmentEnd or ITransactionEnd or IStatementFinish or ITriggerEnd or IProcedureEnd or IFunctionEnd
                ? node.Parent
                : node;
        }

        private static Node GetWorkingNode(Node node)
        {
            if (node.Count == 0)
            {
                return node;
            }
            var lastNode = GetWorkingNode(node[^1]);
            return lastNode.Command is IAttachmentStart or ITransactionStart or IStatementStart or IFunctionStart or IProcedureStart or ITriggerStart
                ? lastNode
                : lastNode.Parent;
        }
    }
}
