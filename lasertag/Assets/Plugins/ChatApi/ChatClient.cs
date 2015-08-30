﻿// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

#if UNITY_3_5 || UNITY_4 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_0
#define UNITY
#endif

namespace ExitGames.Client.Photon.Chat
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    /// <summary>Central class of the Photon Chat API to connect, handle channels and messages.</summary>
    /// <remarks>
    /// This class must be instantiated with a IChatClientListener instance to get the callbacks.
    /// Integrate it into your game loop by calling Service regularly.
    /// Call Connect with an AppId that is setup as Photon Chat application. Note: Connect covers multiple
    /// messages between this client and the servers. A short workflow will connect you to a chat server.
    ///
    /// Each ChatClient resembles a user in chat (set in Connect). Each user automatically subscribes a channel
    /// for incoming private messages and can message any other user privately.
    /// Before you publish messages in any non-private channel, that channel must be subscribed.
    ///
    /// PublicChannels is a list of subscribed channels, containing messages and senders.
    /// PrivateChannels contains all incoming and sent private messages.
    /// </remarks>
    public class ChatClient : IPhotonPeerListener
    {
        /// <summary>The address of last connected Name Server.</summary>
        public string NameServerAddress { get; private set; }
        /// <summary>The address of the actual chat server assigned from NameServer. Public for read only.</summary>
        public string FrontendAddress { get; private set; }
        /// <summary>Region used to connect to. Currently all chat is done in EU. It can make sense to use only one region for the whole game.</summary>
        private string chatRegion = "EU";

        /// <summary>Settable only before you connect! Defaults to "EU".</summary>
        public string ChatRegion
        {
            get { return chatRegion; }
            set { chatRegion = value; }
        }

        /// <summary>Current state of the ChatClient. Also use CanChat.</summary>
        public ChatState State { get; private set; }
        public ChatDisconnectCause DisconnectedCause { get; private set; }
        public bool CanChat { get { return this.State == ChatState.ConnectedToFrontEnd && this.HasPeer; } }
        private bool HasPeer { get { return this.chatPeer != null; }  }

        /// <summary>The version of your client. A new version also creates a new "virtual app" to separate players from older client versions.</summary>
        public string AppVersion { get; private set; }

        /// <summary>The AppID as assigned from the Photon Cloud. If you host yourself, this is the "regular" Photon Server Application Name (most likely: "LoadBalancing").</summary>
        public string AppId { get; private set; }


        /// <summary>Settable only before you connect!</summary>
        public AuthenticationValues AuthValues { get; set; }

        /// <summary>The unique ID of a user/person, stored in AuthValues.UserId. Set it before you connect.</summary>
        /// <remarks>
        /// This value wraps AuthValues.UserId.
        /// It's not a nickname and we assume users with the same userID are the same person.</remarks>
        public string UserId
        {
            get
            {
                return (this.AuthValues != null) ? this.AuthValues.UserId : null;
            }
            private set
            {
                if (this.AuthValues == null)
                {
                    this.AuthValues = new AuthenticationValues();
                }
                this.AuthValues.UserId = value;
            }
        }

        public readonly Dictionary<string, ChatChannel> PublicChannels;
        public readonly Dictionary<string, ChatChannel> PrivateChannels;


        private readonly IChatClientListener listener = null;
        internal ChatPeer chatPeer = null;

        private bool didAuthenticate;
        private int msDeltaForServiceCalls = 50;
        private int msTimestampOfLastServiceCall;

        private const string ChatApppName = "chat";

        public ChatClient(IChatClientListener listener, ConnectionProtocol protocol =
#if UNITY_WEBGL
    ConnectionProtocol.WebSocketSecure
#else
    ConnectionProtocol.Udp
#endif
        )
        {
#if UNITY_WEBGL
	        if (protocol != ConnectionProtocol.WebSocket && protocol != ConnectionProtocol.WebSocketSecure) {
				UnityEngine.Debug.Log("WebGL only supports WebSocket protocol. Overriding ChatClient.Connect() 'protocol' parameter");
				protocol = ConnectionProtocol.WebSocketSecure;
			}
#endif

            this.listener = listener;
            this.State = ChatState.Uninitialized;

            this.chatPeer = new ChatPeer(this, protocol);

            this.PublicChannels = new Dictionary<string, ChatChannel>();
            this.PrivateChannels = new Dictionary<string, ChatChannel>();
        }

        /// <summary>
        /// Connects this client to the Photon Chat Cloud service, which will also authenticate the user (and set a UserId).
        /// </summary>
        /// <param name="appId">Get your Photon Chat AppId from the <a href="https://www.photonengine.com/en/Chat/Dashboard">Dashboard</a>.</param>
        /// <param name="appVersion">Any version string you make up. Used to separate users and variants of your clients, which might be incompatible.</param>
        /// <param name="authValues">Values for authentication. You can leave this null, if you set a UserId before. If you set authValues, they will override any UserId set before.</param>
        /// <returns></returns>
        public bool Connect(string appId, string appVersion, AuthenticationValues authValues)
		{
            this.chatPeer.TimePingInterval = 3000;
            this.DisconnectedCause = ChatDisconnectCause.None;

            if (authValues != null)
            {
                this.AuthValues = authValues;
            }
            this.AppId = appId;
            this.AppVersion = appVersion;
            this.didAuthenticate = false;
            this.msDeltaForServiceCalls = 100;
            this.chatPeer.QuickResendAttempts = 2;
            this.chatPeer.SentCountAllowance = 7;


            // clean all channels
            this.PublicChannels.Clear();
            this.PrivateChannels.Clear();

            this.NameServerAddress = this.chatPeer.NameServerAddress;
            bool isConnecting = this.chatPeer.Connect();
            if (isConnecting)
            {
                this.State = ChatState.ConnectingToNameServer;
            }
            return isConnecting;
        }

        /// <summary>
        /// Must be called regularly to keep connection between client and server alive and to process incoming messages.
        /// </summary>
        /// <remarks>
        /// This method limits the effort it does automatically using the private variable msDeltaForServiceCalls.
        /// That value is lower for connect and multiplied by 4 when chat-server connection is ready.
        /// </remarks>
        public void Service()
        {
            if (this.HasPeer && (Environment.TickCount - msTimestampOfLastServiceCall > msDeltaForServiceCalls || msTimestampOfLastServiceCall == 0))
            {
                msTimestampOfLastServiceCall = Environment.TickCount;
                this.chatPeer.Service();  //TODO: make sure to call service regularly. in best case it could be integrated into PhotonHandler.FallbackSendAckThread()!
            }
        }

        public void Disconnect()
        {
            if (this.HasPeer && this.chatPeer.PeerState != PeerStateValue.Disconnected)
            {
                this.chatPeer.Disconnect();
            }
        }

        public void StopThread()
        {
            if (this.HasPeer)
            {
                this.chatPeer.StopThread();
            }
        }

        /// <summary>Sends operation to subscribe to a list of channels by name.</summary>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels)
        {
            return this.Subscribe(channels, 0);
        }

        /// <summary>
        /// Sends operation to subscribe client to channels, optionally fetching a number of messages from the cache.
        /// </summary>
        /// <remarks>
        /// Subscribes channels will forward new messages to this user. Use PublishMessage to do so.
        /// The messages cache is limited but can be useful to get into ongoing conversations, if that's needed.
        /// </remarks>
        /// <param name="channels">List of channels to subscribe to. Avoid null or empty values.</param>
        /// <param name="messagesFromHistory">0: no history. 1 and higher: number of messages in history. -1: all available history.</param>
        /// <returns>If the operation could be sent at all (Example: Fails if not connected to Chat Server).</returns>
        public bool Subscribe(string[] channels, int messagesFromHistory)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            if (channels == null || channels.Length == 0)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "Subscribe can't be called for empty or null channels-list.");
                return false;
            }

            return this.SendChannelOperation(channels, (byte)ChatOperationCode.Subscribe, messagesFromHistory);
        }

        /// <summary>Unsubscribes from a list of channels, which stops getting messages from those.</summary>
        /// <remarks>
        /// The client will remove these channels from the PublicChannels dictionary once the server sent a response to this request.
        ///
        /// The request will be sent to the server and IChatClientListener.OnUnsubscribed gets called when the server
        /// actually removed the channel subscriptions.
        ///
        /// Unsubscribe will fail if you include null or empty channel names.
        /// </remarks>
        /// <param name="channels">Names of channels to unsubscribe.</param>
        /// <returns>False, if not connected to a chat server.</returns>
        public bool Unsubscribe(string[] channels)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            if (channels == null || channels.Length == 0)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "Unsubscribe can't be called for empty or null channels-list.");
                return false;
            }

            return SendChannelOperation(channels, ChatOperationCode.Unsubscribe, 0);
        }

        /// <summary>Sends a message to a public channel which this client subscribed to.</summary>
        /// <remarks>
        /// Before you publish to a channel, you have to subscribe it.
        /// Everyone in that channel will get the message.
        /// </remarks>
        /// <param name="channelName">Name of the channel to publish to.</param>
        /// <param name="message">Your message (string or any serializable data).</param>
        /// <returns>False if the client is not yet ready to send messages.</returns>
        public bool PublishMessage(string channelName, object message)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            if (string.IsNullOrEmpty(channelName) || message == null)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "PublishMessage parameters must be non-null and not empty.");
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { (byte)ChatParameterCode.Channel, channelName },
                    { (byte)ChatParameterCode.Message, message }
                };

            return this.chatPeer.OpCustom((byte)ChatOperationCode.Publish, parameters, true);
        }

        /// <summary>
        /// Sends a private message to a single target user. Calls OnPrivateMessage on the receiving client.
        /// </summary>
        /// <param name="target">Username to send this message to.</param>
        /// <param name="message">The message you want to send. Can be a simple string or anything serializable.</param>
        /// <returns>True if this clients can send the message to the server.</returns>
        public bool SendPrivateMessage(string target, object message)
        {
            return SendPrivateMessage(target, message, false);
        }

        /// <summary>
        /// Sends a private message to a single target user. Calls OnPrivateMessage on the receiving client.
        /// </summary>
        /// <param name="target">Username to send this message to.</param>
        /// <param name="message">The message you want to send. Can be a simple string or anything serializable.</param>
        /// <param name="encrypt">Optionally, private messages can be encrypted. Encryption is not end-to-end as the server decrypts the message.</param>
        /// <returns>True if this clients can send the message to the server.</returns>
        public bool SendPrivateMessage(string target, object message, bool encrypt)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            if (string.IsNullOrEmpty(target) || message == null)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "SendPrivateMessage parameters must be non-null and not empty.");
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.UserId, target },
                    { ChatParameterCode.Message, message }
                };

            bool sent = this.chatPeer.OpCustom((byte)ChatOperationCode.SendPrivate, parameters, true, 0, encrypt);
            return sent;
        }

        /// <summary>Sets the user's status (pre-defined or custom) and an optional message.</summary>
        /// <remarks>
        /// The predefined status values can be found in class ChatUserStatus.
        /// State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///
        /// You can set custom values in the status integer. Aside from the pre-configured ones,
        /// all states will be considered visible and online. Else, no one would see the custom state.
        ///
        /// The message object can be anything that Photon can serialize, including (but not limited to)
        /// Hashtable, object[] and string. This value is defined by your own conventions.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <param name="message">Optional string message or null.</param>
        /// <param name="skipMessage">If true, the message gets ignored. It can be null but won't replace any current message.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        private bool SetOnlineStatus(int status, object message, bool skipMessage)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.Status, status },
                };

            if (skipMessage)
            {
                parameters[ChatParameterCode.SkipMessage] = true;
            }
            else
            {
                parameters[ChatParameterCode.Message] = message;
            }
            return this.chatPeer.OpCustom(ChatOperationCode.UpdateStatus, parameters, true);
        }

        /// <summary>Sets the user's status without changing your status-message.</summary>
        /// <remarks>
        /// The predefined status values can be found in class ChatUserStatus.
        /// State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///
        /// You can set custom values in the status integer. Aside from the pre-configured ones,
        /// all states will be considered visible and online. Else, no one would see the custom state.
        ///
        /// This overload does not change the set message.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        public bool SetOnlineStatus(int status)
        {
            return SetOnlineStatus(status, null, true);
        }
        /// <summary>Sets the user's status without changing your status-message.</summary>
        /// <remarks>
        /// The predefined status values can be found in class ChatUserStatus.
        /// State ChatUserStatus.Invisible will make you offline for everyone and send no message.
        ///
        /// You can set custom values in the status integer. Aside from the pre-configured ones,
        /// all states will be considered visible and online. Else, no one would see the custom state.
        ///
        /// The message object can be anything that Photon can serialize, including (but not limited to)
        /// Hashtable, object[] and string. This value is defined by your own conventions.
        /// </remarks>
        /// <param name="status">Predefined states are in class ChatUserStatus. Other values can be used at will.</param>
        /// <param name="message">Also sets a status-message which your friends can get.</param>
        /// <returns>True if the operation gets called on the server.</returns>
        public bool SetOnlineStatus(int status, object message)
        {
            return SetOnlineStatus(status, message, false);
        }

        /// <summary>
        /// Adds friends to a list on the Chat Server which will send you status updates for those.
        /// </summary>
        /// <remarks>
        /// AddFriends and RemoveFriends enable clients to handle their friend list
        /// in the Photon Chat server. Having users on your friends list gives you access
        /// to their current online status (and whatever info your client sets in it).
        ///
        /// Each user can set an online status consisting of an integer and an arbitratry
        /// (serializable) object. The object can be null, Hashtable, object[] or anything
        /// else Photon can serialize.
        ///
        /// The status is published automatically to friends (anyone who set your user ID
        /// with AddFriends).
        ///
        /// Photon flushes friends-list when a chat client disconnects, so it has to be
        /// set each time. If your community API gives you access to online status already,
        /// you could filter and set online friends in AddFriends.
        ///
        /// Actual friend relations are not persistent and have to be stored outside
        /// of Photon.
        /// </remarks>
        /// <param name="friends">Array of friend userIds.</param>
        /// <returns>If the operation could be sent.</returns>
        public bool AddFriends(string[] friends)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            if (friends == null || friends.Length == 0)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "AddFriends can't be called for empty or null list.");
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.Friends, friends },
                };
            return this.chatPeer.OpCustom(ChatOperationCode.AddFriends, parameters, true);
        }

        /// <summary>
        /// Removes the provided entries from the list on the Chat Server and stops their status updates.
        /// </summary>
        /// <remarks>
        /// Photon flushes friends-list when a chat client disconnects. Unless you want to
        /// remove individual entries, you don't have to RemoveFriends.
        ///
        /// AddFriends and RemoveFriends enable clients to handle their friend list
        /// in the Photon Chat server. Having users on your friends list gives you access
        /// to their current online status (and whatever info your client sets in it).
        ///
        /// Each user can set an online status consisting of an integer and an arbitratry
        /// (serializable) object. The object can be null, Hashtable, object[] or anything
        /// else Photon can serialize.
        ///
        /// The status is published automatically to friends (anyone who set your user ID
        /// with AddFriends).
        ///
        /// Photon flushes friends-list when a chat client disconnects, so it has to be
        /// set each time. If your community API gives you access to online status already,
        /// you could filter and set online friends in AddFriends.
        ///
        /// Actual friend relations are not persistent and have to be stored outside
        /// of Photon.
        ///
        /// AddFriends and RemoveFriends enable clients to handle their friend list
        /// in the Photon Chat server. Having users on your friends list gives you access
        /// to their current online status (and whatever info your client sets in it).
        ///
        /// Each user can set an online status consisting of an integer and an arbitratry
        /// (serializable) object. The object can be null, Hashtable, object[] or anything
        /// else Photon can serialize.
        ///
        /// The status is published automatically to friends (anyone who set your user ID
        /// with AddFriends).
        ///
        ///
        /// Actual friend relations are not persistent and have to be stored outside
        /// of Photon.
        /// </remarks>
        /// <param name="friends">Array of friend userIds.</param>
        /// <returns>If the operation could be sent.</returns>
        public bool RemoveFriends(string[] friends)
        {
            if (!this.CanChat)
            {
                // TODO: log error
                return false;
            }

            if (friends == null || friends.Length == 0)
            {
                this.listener.DebugReturn(DebugLevel.WARNING, "RemoveFriends can't be called for empty or null list.");
                return false;
            }

            Dictionary<byte, object> parameters = new Dictionary<byte, object>
                {
                    { ChatParameterCode.Friends, friends },
                };
            return this.chatPeer.OpCustom(ChatOperationCode.RemoveFriends, parameters, true);
        }

        /// <summary>
        /// Get you the (locally used) channel name for the chat between this client and another user.
        /// </summary>
        /// <param name="userName">Remote user's name or UserId.</param>
        /// <returns>The (locally used) channel name for a private channel.</returns>
        public string GetPrivateChannelNameByUser(string userName)
        {
            return string.Format("{0}:{1}", this.UserId, userName);
        }

        /// <summary>
        /// Simplified access to either private or public channels by name.
        /// </summary>
        /// <param name="channelName">Name of the channel to get. For private channels, the channel-name is composed of both user's names.</param>
        /// <param name="isPrivate">Define if you expect a private or public channel.</param>
        /// <param name="channel">Out parameter gives you the found channel, if any.</param>
        /// <returns>True if the channel was found.</returns>
        public bool TryGetChannel(string channelName, bool isPrivate, out ChatChannel channel)
        {
            if (!isPrivate)
            {
               return this.PublicChannels.TryGetValue(channelName, out channel);
            }
            else
            {
                return this.PrivateChannels.TryGetValue(channelName, out channel);
            }
        }

        /// <summary>
        /// Simplified access to all channels by name. Checks public channels first, then private ones.
        /// </summary>
        /// <param name="channelName">Name of the channel to get.</param>
        /// <param name="channel">Out parameter gives you the found channel, if any.</param>
        /// <returns>True if the channel was found.</returns>
        public bool TryGetChannel(string channelName, out ChatChannel channel)
        {
            bool found = false;
            found = this.PublicChannels.TryGetValue(channelName, out channel);
            if (found) return true;

            found = this.PrivateChannels.TryGetValue(channelName, out channel);
            return found;
        }

        public void SendAcksOnly()
        {
            if (this.chatPeer != null) this.chatPeer.SendAcksOnly();
        }

        /// <summary>
        /// Sets the level (and amount) of debug output provided by the library.
        /// </summary>
        /// <remarks>
        /// This affects the callbacks to IChatClientListener.DebugReturn.
        /// Default Level: Error.
        /// </remarks>
        public DebugLevel DebugOut
        {
            set { this.chatPeer.DebugOut = value; }
            get { return this.chatPeer.DebugOut; }
        }

        #region Private methods area

        #region IPhotonPeerListener implementation

        void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
        {
            this.listener.DebugReturn(level, message);
        }

        void IPhotonPeerListener.OnEvent(EventData eventData)
        {
            switch (eventData.Code)
            {
                case ChatEventCode.ChatMessages:
                    this.HandleChatMessagesEvent(eventData);
                    break;
                case ChatEventCode.PrivateMessage:
                    this.HandlePrivateMessageEvent(eventData);
                    break;
                case ChatEventCode.StatusUpdate:
                    this.HandleStatusUpdate(eventData);
                    break;
                case ChatEventCode.Subscribe:
                    this.HandleSubscribeEvent(eventData);
                    break;
                case ChatEventCode.Unsubscribe:
                    this.HandleUnsubscribeEvent(eventData);
                    break;
            }
        }

        void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
        {
            switch (operationResponse.OperationCode)
            {
                case (byte)ChatOperationCode.Authenticate:
                    this.HandleAuthResponse(operationResponse);
                    break;

                // the following operations usually don't return useful data and no error.
                case (byte)ChatOperationCode.Subscribe:
                case (byte)ChatOperationCode.Unsubscribe:
                case (byte)ChatOperationCode.Publish:
                case (byte)ChatOperationCode.SendPrivate:
                default:
                    if (operationResponse.ReturnCode != 0)
                    {
                        this.listener.DebugReturn(DebugLevel.ERROR, string.Format("Chat Operation {0} failed (Code: {1}). Debug Message: {2}", operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage));
                    }
                    break;
            }
        }

        void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
	                if (!this.chatPeer.IsProtocolSecure) {
#if UNITY
						UnityEngine.Debug.Log("Establishing Encryption");
#endif
            	        this.chatPeer.EstablishEncryption();
        	        }
					else {
#if UNITY
						UnityEngine.Debug.Log("Skipping Encryption");
#endif
                        if (!this.didAuthenticate)
	                    {
                    	    this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
                	        if (!this.didAuthenticate)
            	            {
                                ((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
    	                    }
	                    }
					}

                    if (this.State == ChatState.ConnectingToNameServer)
                    {
                        this.State = ChatState.ConnectedToNameServer;
                        this.listener.OnChatStateChange(this.State);
                    }
                    else if (this.State == ChatState.ConnectingToFrontEnd)
                    {
                        this.AuthenticateOnFrontEnd();
                    }
                    break;
                case StatusCode.EncryptionEstablished:
                    // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                    if (!this.didAuthenticate)
                    {
                        this.didAuthenticate = this.chatPeer.AuthenticateOnNameServer(this.AppId, this.AppVersion, this.chatRegion, this.AuthValues);
                        if (!this.didAuthenticate)
                        {
                            ((IPhotonPeerListener)this).DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
                        }
                    }
                    break;
                case StatusCode.EncryptionFailedToEstablish:
                    this.State = ChatState.Disconnecting;
                    this.chatPeer.Disconnect();
                    break;
                case StatusCode.Disconnect:
                    if (this.State == ChatState.Authenticated)
                    {
                        this.ConnectToFrontEnd();
                    }
                    else
                    {
                        this.State = ChatState.Disconnected;
                        this.listener.OnChatStateChange(ChatState.Disconnected);
                        this.listener.OnDisconnected();
                    }
                    break;
            }
        }

#if SDK_V4
        void IPhotonPeerListener.OnMessage(object msg)
        {
            // in v4 interface IPhotonPeerListener
            return;
        }
#endif

        #endregion

        private bool SendChannelOperation(string[] channels, byte operation, int historyLength)
        {
            Dictionary<byte, object> opParameters = new Dictionary<byte, object> { { (byte)ChatParameterCode.Channels, channels } };

            if (historyLength != 0)
            {
                opParameters.Add((byte)ChatParameterCode.HistoryLength, historyLength);
            }

            return this.chatPeer.OpCustom(operation, opParameters, true);
        }

        private void HandlePrivateMessageEvent(EventData eventData)
        {
            //Console.WriteLine(SupportClass.DictionaryToString(eventData.Parameters));

            var message = (object)eventData.Parameters[(byte)ChatParameterCode.Message];
            var sender = (string)eventData.Parameters[(byte)ChatParameterCode.Sender];

            string channelName;
            if (this.UserId != null && this.UserId.Equals(sender))
            {
                var target = (string)eventData.Parameters[(byte)ChatParameterCode.UserId];
                channelName = this.GetPrivateChannelNameByUser(target);
            }
            else
            {
                channelName = this.GetPrivateChannelNameByUser(sender);
            }

            ChatChannel channel;
            if (!this.PrivateChannels.TryGetValue(channelName, out channel))
            {
                channel = new ChatChannel(channelName);
                channel.IsPrivate = true;
                this.PrivateChannels.Add(channel.Name, channel);
            }

            channel.Add(sender, message);
            this.listener.OnPrivateMessage(sender, message, channelName);
        }

        private void HandleChatMessagesEvent(EventData eventData)
        {
            var messages = (object[])eventData.Parameters[(byte)ChatParameterCode.Messages];
            var senders = (string[])eventData.Parameters[(byte)ChatParameterCode.Senders];
            var channelName = (string)eventData.Parameters[(byte)ChatParameterCode.Channel];

            ChatChannel channel;
            if (!this.PublicChannels.TryGetValue(channelName, out channel))
            {
                // TODO: log that channel wasn't found
                return;
            }

            channel.Add(senders, messages);
            this.listener.OnGetMessages(channelName, senders, messages);
        }

        private void HandleSubscribeEvent(EventData eventData)
        {
            var channelsInResponse = (string[])eventData.Parameters[ChatParameterCode.Channels];
            var results = (bool[])eventData.Parameters[ChatParameterCode.SubscribeResults];

            for (int i = 0; i < channelsInResponse.Length; i++)
            {
                if (results[i])
                {
                    string channelName = channelsInResponse[i];
                    if (!this.PublicChannels.ContainsKey(channelName))
                    {
                        ChatChannel channel = new ChatChannel(channelName);
                        this.PublicChannels.Add(channel.Name, channel);
                    }
                }
            }

            this.listener.OnSubscribed(channelsInResponse, results);
        }

        private void HandleUnsubscribeEvent(EventData eventData)
        {
            var channelsInRequest = (string[])eventData[ChatParameterCode.Channels];
            for (var i = 0; i < channelsInRequest.Length; i++)
            {
                string channelName = channelsInRequest[i];
                this.PublicChannels.Remove(channelName);
            }

            this.listener.OnUnsubscribed(channelsInRequest);
        }

        private void HandleAuthResponse(OperationResponse operationResponse)
        {
            this.listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " on: " + this.chatPeer.NameServerAddress);
            if (operationResponse.ReturnCode == 0)
            {
                if (this.State == ChatState.ConnectedToNameServer)
                {
                    this.State = ChatState.Authenticated;
                    this.listener.OnChatStateChange(this.State);

                    if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
                    {
                        if (this.AuthValues == null)
                        {
                            this.AuthValues = new AuthenticationValues();
                        }
                        this.AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
                        this.FrontendAddress = (string) operationResponse[ParameterCode.Address];

                        // we disconnect and status handler starts to connect to front end
                        this.chatPeer.Disconnect();
                    }
                    else
                    {
                        //TODO: error reaction!
                    }
                }
                else if (this.State == ChatState.ConnectingToFrontEnd)
                {
                    this.msDeltaForServiceCalls = this.msDeltaForServiceCalls * 4;  // when we arrived on chat server: limit Service calls some more

                    this.State = ChatState.ConnectedToFrontEnd;
                    this.listener.OnChatStateChange(this.State);
                    this.listener.OnConnected();
                }
            }
            else
            {
                //this.listener.DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull() + " NS: " + this.NameServerAddress + " FrontEnd: " + this.frontEndAddress);

                switch (operationResponse.ReturnCode)
                {
                    case ErrorCode.InvalidAuthentication:
                        this.DisconnectedCause = ChatDisconnectCause.InvalidAuthentication;
                        break;
                    case ErrorCode.CustomAuthenticationFailed:
                        this.DisconnectedCause = ChatDisconnectCause.CustomAuthenticationFailed;
                        break;
                    case ErrorCode.InvalidRegion:
                        this.DisconnectedCause = ChatDisconnectCause.InvalidRegion;
                        break;
                    case ErrorCode.MaxCcuReached:
                        this.DisconnectedCause = ChatDisconnectCause.MaxCcuReached;
                        break;
                    case ErrorCode.OperationNotAllowedInCurrentState:
                        this.DisconnectedCause = ChatDisconnectCause.OperationNotAllowedInCurrentState;
                        break;
                }

                this.State = ChatState.Disconnecting;
                this.chatPeer.Disconnect();
            }
        }

        private void HandleStatusUpdate(EventData eventData)
        {
            var user = (string)eventData.Parameters[ChatParameterCode.Sender];
            var status = (int)eventData.Parameters[ChatParameterCode.Status];

            object message = null;
            bool gotMessage = eventData.Parameters.ContainsKey(ChatParameterCode.Message);
            if (gotMessage)
            {
                message = eventData.Parameters[ChatParameterCode.Message];
            }

            this.listener.OnStatusUpdate(user, status, gotMessage, message);
        }

        private void ConnectToFrontEnd()
        {
            this.State = ChatState.ConnectingToFrontEnd;
            this.listener.DebugReturn(DebugLevel.INFO, "Connecting to frontend " + this.FrontendAddress);
            this.chatPeer.Connect(this.FrontendAddress, ChatApppName);
        }

        private bool AuthenticateOnFrontEnd()
        {
            if (this.AuthValues != null)
            {
                var opParameters = new Dictionary<byte, object> {{(byte)ChatParameterCode.Secret, this.AuthValues.Token}};
                return this.chatPeer.OpCustom((byte)ChatOperationCode.Authenticate, opParameters, true);
            }
            else
            {
                Debug.WriteLine("Can't authenticate on front end server. CustomAuthValues is null");
            }
            return false;
        }

        #endregion
    }
}
