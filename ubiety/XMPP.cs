// XMPP.cs
//
//Ubiety XMPP Library Copyright (C) 2006 - 2012 Dieter Lunn
//
//This library is free software; you can redistribute it and/or modify it under
//the terms of the GNU Lesser General Public License as published by the Free
//Software Foundation; either version 3 of the License, or (at your option)
//any later version.
//
//This library is distributed in the hope that it will be useful, but WITHOUT
//ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
//
//You should have received a copy of the GNU Lesser General Public License along
//with this library; if not, write to the Free Software Foundation, Inc., 59
//Temple Place, Suite 330, Boston, MA 02111-1307 USA

#region Usings

using System;
using ubiety.common;
using ubiety.common.logging;
using ubiety.net;
using ubiety.registries;
using ubiety.states;

#endregion

namespace ubiety
{
	/// <summary>
	/// Implements the XMPP(Jabber) Core and IM protocols
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Extensible Messaging and Presence Protocol (XMPP) is an open XML technology for real-time
	/// communications, which powers a wide range of applications including instant messaging, presence,
	/// media negotiation, whiteboarding, collaboration, lightweight middleware, content syndication, and
	/// generalized XML delivery.
	/// </para>
	/// <para>
	/// This library is an implementation of this protocol.  Those involved with the design and development
	/// of this library are as committed to open standards as the committees who created the original protocol.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// public class Test
	/// {
	///		public static Main()
	///		{
	///			// Create a new ID for authentication
	///			UbietySettings.ID = new JID("user@jabber.org/chat");
	///			UbietySettings.Password = "password";
	/// 
	///			// Create a new instance of the XMPP class
	///			XMPP ubiety = new XMPP();
	///			
	///         ubiety.Connect();
	///		}
	/// }
	/// </code>
	/// </example>
	public class XMPP
	{
		///<summary>
		///</summary>
		public static readonly string Version = typeof (XMPP).Assembly.GetName().Version.ToString();

		/// <summary>
		/// Initializes a new instance of the <see cref="XMPP"/> class.
		/// </summary>
		public XMPP()
		{
			TagRegistry.AddAssembly(typeof (XMPP).Assembly);
			Errors.OnError += OnError;
			ProtocolState.Socket = new AsyncSocket();
		}

		/// <summary>
		/// Connects this instance to an XMPP server.
		/// </summary>
		public void Connect()
		{
			// We need an XID and Password to connect to the server.
			if (String.IsNullOrEmpty(UbietySettings.Password))
			{
				Errors.SendError(this, ErrorType.MissingPassword,
				                 "Set the Password property of the Settings before connecting.", true);
				return;
			}

			if (String.IsNullOrEmpty(UbietySettings.Id))
			{
				Errors.SendError(this, ErrorType.MissingId, "Set the ID property of the Settings before connecting.", true);
				return;
			}

			// Set the current state to connecting and start the process.
			ProtocolState.State = new ConnectingState();
			ProtocolState.State.Execute();
		}

		/// <summary>
		/// Disconnects this instance from the server.
		/// </summary>
		public void Disconnect()
		{
			if ((ProtocolState.State is DisconnectState)) return;
			ProtocolState.State = new DisconnectState();
			ProtocolState.State.Execute();
		}

		private void OnError(object sender, ErrorEventArgs e)
		{
			Logger.ErrorFormat(this, "Error from {0}: {1}", sender, e.Message);
			if (e.Fatal)
			{
				Disconnect();
			}
		}

		#region Properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="XMPP"/> is connected to a server.
		/// </summary>
		/// <value>
		///   <c>true</c> if connected; otherwise, <c>false</c>.
		/// </value>
		public bool Connected
		{
			get { return ProtocolState.State is RunningState; }
		}

		#endregion
	}
}