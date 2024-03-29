﻿using FirebirdMonitorTool.Common;

namespace FirebirdMonitorTool.Attachment
{
	public interface IAttachment : ICommand
	{
		string DatabaseName { get; }
		long ConnectionId { get; }
		string User { get; }
		string Role { get; }
		string CharacterSet { get; }
		string RemoteProtocol { get; }
		string RemoteAddress { get; }
		string RemoteProcessName { get; }
		long? RemoteProcessId { get; }
	}
}
