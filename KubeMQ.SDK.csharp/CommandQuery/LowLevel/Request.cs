﻿using Google.Protobuf;
using System.Threading;
using InnerRequest = KubeMQ.Grpc.Request;

namespace KubeMQ.SDK.csharp.CommandQuery.LowLevel
{
    /// <summary>
    ///  Represents the Request used in RequestReply to send information using the KubeMQ .
    /// </summary>
    public class Request
    {
        private static int _id = 0;

        #region Properties

        /// <summary>
        /// Represents a Request identifier.
        /// </summary>
        public string RequestID { get; set; }
        /// <summary>
        ///  Represents the type of request operation using KubeMQ.SDK.csharp.CommandQuery.RequestType.
        /// </summary>
        public RequestType RequestType { get; set; }
        /// <summary>
        /// Represents the sender ID that the Request will be send under.
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// Represents The channel name to send to using the KubeMQ
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// Represents The channel name to return response to .
        /// </summary>
        public string ReplyChannel { get; private set; }
        /// <summary>
        /// Represents text as System.String.
        /// </summary>
        public string Metadata { get; set; }
        /// <summary>
        /// Represents The content of the KubeMQ.SDK.csharp.RequestReply.LowLevel.Request.
        /// </summary>
        public byte[] Body { get; set; }
        /// <summary>
        /// Represents the limit for waiting for response (Milliseconds)
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// Represents if the request should be saved from Cache and under what "Key"(System.String) to save it.
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// Cache time to live : for how long does the request should be saved in Cache
        /// </summary>
        public int CacheTTL { get; set; }
        #endregion

        #region C'tor
        /// <summary>
        /// Initializes a new instance of the KubeMQ.SDK.csharp.RequestReply.LowLevel.Request .
        /// </summary>
        public Request() { }

        /// <summary>
        ///  Initializes a new instance of the KubeMQ.SDK.csharp.RequestReply.LowLevel.Request with a set of parameters .
        /// </summary>
        /// <param name="requestID">Represents a Request identifier.</param>
        /// <param name="requestType"> Represents the type of request operation using KubeMQ.SDK.csharp.CommandQuery.RequestType.</param>
        /// <param name="clientID">Represents the sender ID that the Request will be send under.</param>
        /// <param name="channel">Represents The channel name to send to using the KubeMQ.</param>
        /// <param name="replaychannel">Represents The channel name to return response to.</param>
        /// <param name="metadata">Represents text as System.String.</param>
        /// <param name="body">Represents The content of the KubeMQ.SDK.csharp.RequestReply.LowLevel.Request.</param>
        /// <param name="timeout">Represents the limit for waiting for response (Milliseconds)</param>
        /// <param name="cacheKey">Represents if the request should be saved from Cache and under what "Key"(System.String) to save it.</param>
        /// <param name="cacheTTL">Cache time to live : for how long does the request should be saved in Cache.</param>
        public Request(string requestID, RequestType requestType, string clientID, string channel, string replaychannel, string metadata, byte[] body, int timeout, string cacheKey, int cacheTTL)
        {
            RequestID = requestID;
            RequestType = requestType;
            ClientID = clientID;
            Channel = channel;
            ReplyChannel = replaychannel;
            Metadata = metadata;
            Body = body;
            Timeout = timeout;
            CacheKey = cacheKey;
            CacheTTL = cacheTTL;
        }
        #endregion

        internal Request(InnerRequest innerRequest)
        {
            RequestID = string.IsNullOrEmpty(innerRequest.RequestID) ? GetNextId().ToString() : innerRequest.RequestID;
            RequestType = (RequestType)innerRequest.RequestTypeData;
            ClientID = innerRequest.ClientID ?? string.Empty;
            Channel = innerRequest.Channel;
            Metadata = innerRequest.Metadata ?? string.Empty;
            Body = innerRequest.Body.ToByteArray();
            ReplyChannel = innerRequest.ReplyChannel;
            Timeout = innerRequest.Timeout;
            CacheKey = innerRequest.CacheKey ?? string.Empty;
            CacheTTL = innerRequest.CacheTTL;
        }

        /// <summary>
        /// Convert a Request to an InnerRequest
        /// </summary>
        /// <returns> An InnerRequest</returns>
        internal InnerRequest Convert()
        {
            return new InnerRequest()
            {
                RequestID = string.IsNullOrEmpty(this.RequestID) ? GetNextId().ToString() : this.RequestID,
                RequestTypeData = (InnerRequest.Types.RequestType)this.RequestType,
                ClientID = this.ClientID ?? string.Empty,
                Channel = this.Channel,
                //ReplyChannel - Set only by KubeMQ server
                Metadata = this.Metadata ?? string.Empty,
                Body = ByteString.CopyFrom(this.Body),
                Timeout = this.Timeout,
                CacheKey = this.CacheKey ?? string.Empty,
                CacheTTL = this.CacheTTL,
            };
        }

        /// <summary>
        /// Get an unique thread safety ID between 1 to 65535
        /// </summary>
        /// <returns></returns>
        private int GetNextId()
        {
            //return Interlocked.Increment(ref _id);

            int temp, temp2;

            do
            {
                temp = _id;
                temp2 = temp == ushort.MaxValue ? 1 : temp + 1;
            }
            while (Interlocked.CompareExchange(ref _id, temp2, temp) != temp);
            return _id;
        }
    }
}