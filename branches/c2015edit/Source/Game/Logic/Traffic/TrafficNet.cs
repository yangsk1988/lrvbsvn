using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Logic.Traffic
{
    /// <summary>
    ///  管理道路和路口
    /// </summary>
    public class TrafficNet
    {
        Dictionary<int, ITrafficComponment> trackTable;

        public TrafficNet()
        {
            this.trackTable = new Dictionary<int, ITrafficComponment>();
        }

        public void NotifyComponmentAdded(ITrafficComponment track)
        {
            trackTable.Add(track.Id, track);
        }

        public void NotifyComponmentRemoved(ITrafficComponment track)
        {
            trackTable.Remove(track.Id);
        }

        public ITrafficComponment GetComponment(int id)
        {
            return trackTable[id];
        }


        public void ParseNodes()
        {
            Dictionary<int, ITrafficComponment>.ValueCollection vals = trackTable.Values;

            foreach (ITrafficComponment c in vals)
            {
                //Console.Write("Node:");
                //Console.Write(c.Id);

                //Console.Write("(");
                //Console.Write(c.ToString());
                //Console.WriteLine(")");
                

                //List<TCConnectionInfo> info = c.GetConnections();

                //for (int i = 0; i < info.Count; i++)
                //{
                //    Console.Write(" Target:");

                //    Console.WriteLine(info[i].TargetComponmentId);
                //}

                c.ParseNodes();
            }
        }

        public int GenerateId(ITrafficComponment con)
        {
            int hash;

            do
            {
                hash = con.GetHashCode() + Randomizer.GetRandomInt(int.MaxValue);
            }
            while (trackTable.ContainsKey(hash));
           
            return hash;
        }
    }
}
