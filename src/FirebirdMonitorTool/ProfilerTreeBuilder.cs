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
			readonly List<Node> _children;

			public ICommand Command { get; }
			public Node Parent { get; private set; }

			public Node(ICommand command)
			{
				_children = new List<Node>();
				Command = command;
			}

			public void AddChild(Node node)
			{
				node.Parent = this;
				_children.Add(node);
			}

			public override string ToString() => $"Command: {Command.GetType().Name}";

			public int Count => _children.Count;
			public Node this[int index] => _children[index];
			public IEnumerator<Node> GetEnumerator() => _children.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		readonly Dictionary<(long internalTraceId, long connectionId), Node> _liveNodes;
		readonly Monitor _monitor;

		public event EventHandler<Node> OnNode;
		public event EventHandler<Exception> OnError
		{
			add { _monitor.OnError += value; }
			remove { _monitor.OnError -= value; }
		}

		public ProfilerTreeBuilder()
		{
			_liveNodes = new Dictionary<(long internalTraceId, long connectionId), Node>();
			_monitor = new Monitor();
			_monitor.OnCommand += (sender, command) =>
			{
				ProcessCommand(command);
			};
		}

		public void Process(string input)
		{
			_monitor.Process(input);
		}

		public void Flush()
		{
			_monitor.Flush();
		}

		public void LoadFile(string file)
		{
			_monitor.LoadFile(file);
		}

		void ProcessCommand(ICommand command)
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
						_liveNodes.Add((attachmentStart.InternalTraceId, attachmentStart.ConnectionId), root);
						break;
					}
				case IAttachmentEnd attachmentEnd:
					{
						if (_liveNodes.Remove((attachmentEnd.InternalTraceId, attachmentEnd.ConnectionId), out var root))
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

		Node AddNext(IAttachment attachmentCommand)
		{
			if (!_liveNodes.TryGetValue((attachmentCommand.InternalTraceId, attachmentCommand.ConnectionId), out var root))
			{
				return null;
			}
			return AddNextForRoot(attachmentCommand, root);
		}

		static Node AddNextForRoot(IAttachment attachmentCommand, Node root)
		{
			var node = new Node(attachmentCommand);
			GetCurrentNode(GetWorkingNode(root), attachmentCommand).AddChild(node);
			return node;
		}

		static Node GetCurrentNode(Node node, ICommand command)
		{
			return command is IAttachmentEnd or ITransactionEnd or IStatementFinish or IProcedureEnd or IFunctionEnd or ITriggerEnd
				? node.Parent
				: node;
		}

		static Node GetWorkingNode(Node node)
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
