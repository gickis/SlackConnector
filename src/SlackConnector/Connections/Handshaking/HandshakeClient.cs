﻿using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using SlackConnector.Connections.Messaging;
using SlackConnector.Connections.Responses;

namespace SlackConnector.Connections.Handshaking
{
    internal class HandshakeClient : IHandshakeClient
    {
        internal const string HANDSHAKE_PATH = "/api/rtm.start";
        private readonly IRestSharpFactory _restSharpFactory;
        private readonly IResponseVerifier _responseVerifier;

        public HandshakeClient(IRestSharpFactory restSharpFactory, IResponseVerifier responseVerifier)
        {
            _restSharpFactory = restSharpFactory;
            _responseVerifier = responseVerifier;
        }

        public async Task<HandshakeResponse> FirmShake(string slackKey)
        {
            var request = new RestRequest(HANDSHAKE_PATH);
            request.AddParameter("token", slackKey);

            IRestClient client = _restSharpFactory.CreateClient("https://slack.com");
            IRestResponse response = await client.ExecutePostTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException($"An error occured while attemping to handshake with Slack. HttpStatus: {response.StatusCode}", response.ErrorException);
            }

            return JsonConvert.DeserializeObject<HandshakeResponse>(response.Content);
        }
    }
}