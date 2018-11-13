﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PacketParser.Packets {
    public interface IIPPacket : IPacket {
        System.Net.IPAddress SourceIPAddress { get; }
        System.Net.IPAddress DestinationIPAddress { get; }
        int PayloadLength { get; }
        byte HeaderLength { get; }
        /// <summary>
        /// Time To Live (TTL) measured in Hops
        /// </summary>
        byte HopLimit { get; }
        byte NextRFC1700Protocol { get; }
    }
}
