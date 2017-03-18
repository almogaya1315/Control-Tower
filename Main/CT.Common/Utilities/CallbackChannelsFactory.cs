using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.Utilities
{
    public class CallbackChannelFactory<ICallback>
    {
        #region props and ctor
        Dictionary<string, ICallback> channelSessions { get; set; }

        public CallbackChannelFactory()
        {
            channelSessions = new Dictionary<string, ICallback>();
        }
        #endregion

        #region methods
        public ICallback CreateChannel(OperationContext current, string sessionId)
        {
            ICallback channel = current.GetCallbackChannel<ICallback>();
            if (!channelSessions.Values.Contains(channel))
                channelSessions[sessionId] = channel;
            return channel;
        }

        public ICollection<ICallback> GetOpenChannels(ICallback currentChannel)
        {
            if (currentChannel == null) return channelSessions.Values.ToList();
            ICollection<ICallback> openChannles = new List<ICallback>();
            foreach (var channel in channelSessions.Values)
            {
                if (!channel.Equals(currentChannel))
                    openChannles.Add(channel);
            }
            return openChannles;
        }

        public Dictionary<string, ICallback> GetChannelSessions()
        {
            return channelSessions;
        }

        public ICallback GetChannel(string sessionId)
        {
            foreach (var session in channelSessions.Keys)
            {
                if (session == sessionId)
                    return channelSessions[sessionId];
            }
            return default(ICallback);
        }

        public bool RemoveChannel(string sessionId)
        {
            foreach (var session in channelSessions.Keys)
            {
                if (session.Equals(sessionId))
                {
                    channelSessions.Remove(sessionId);
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
