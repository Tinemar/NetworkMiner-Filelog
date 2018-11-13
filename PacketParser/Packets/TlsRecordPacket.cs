//  Copyright: Erik Hjelmvik, NETRESEC
//
//  NetworkMiner is free software; you can redistribute it and/or modify it
//  under the terms of the GNU General Public License
//

using System;
using System.Collections.Generic;
using System.Text;

namespace PacketParser.Packets {

    /// <summary>
    /// A Transport Layer Security (TLS) Record
    /// </summary>
    class TlsRecordPacket : AbstractPacket {
        //http://en.wikipedia.org/wiki/Transport_Layer_Security
        //http://tools.ietf.org/html/rfc2246

        internal enum ContentTypes : byte {
            ChangeCipherSpec=0x14,
            Alert=0x15,
            Handshake=0x16,
            Application=0x17,
        };

        private ContentTypes contentType;
        private byte versionMajor;//MSB
        private byte versionMinor;//LSB
        private ushort length;//MSB & LSB
        //private HandshakeProtocol handshakeProtocol;

        internal bool TlsRecordIsComplete { get { return PacketEndIndex-PacketStartIndex+1==5+this.length; } }
        internal ushort Length { get { return this.length; } }
        internal ContentTypes ContentType { get { return this.contentType; } }

        public static new bool TryParse(Frame parentFrame, int packetStartIndex, int packetEndIndex, out AbstractPacket result) {
            result = null;
            if(!Enum.IsDefined(typeof(ContentTypes), parentFrame.Data[packetStartIndex]))
                return false;

            //verify that the complete TLS record has been received
            ushort length = Utils.ByteConverter.ToUInt16(parentFrame.Data, packetStartIndex + 3);
            if(length+5 > packetEndIndex-packetStartIndex+1)
                return false;

            try {
                result = new TlsRecordPacket(parentFrame, packetStartIndex, packetEndIndex);
            }
            catch {
                result = null;
            }

            if(result == null)
                return false;
            else
                return true;
        }

        internal TlsRecordPacket(Frame parentFrame, int packetStartIndex, int packetEndIndex) : base(parentFrame, packetStartIndex, packetEndIndex, "TLS Record") {
            this.contentType=(ContentTypes)parentFrame.Data[packetStartIndex];
            this.versionMajor=parentFrame.Data[packetStartIndex+1];
            this.versionMinor=parentFrame.Data[packetStartIndex+2];
            this.length = Utils.ByteConverter.ToUInt16(parentFrame.Data, packetStartIndex + 3);
            this.PacketEndIndex=Math.Min(packetStartIndex+5+length-1, this.PacketEndIndex);

            if (!this.ParentFrame.QuickParse) {
                this.Attributes.Add("Content Type", "" + this.contentType);
                this.Attributes.Add("TLS Version major", "" + versionMajor);
                this.Attributes.Add("TLS Version minor", "" + versionMinor);
            }
        }

        public override IEnumerable<AbstractPacket> GetSubPackets(bool includeSelfReference) {
            if(includeSelfReference)
                yield return this;
        
            //I only care about the hadshake protocol
            if(this.contentType==ContentTypes.Handshake) {
                if (PacketStartIndex + 5 < PacketEndIndex)
                    yield return new RawPacket(ParentFrame, PacketStartIndex + 5, PacketEndIndex);//data in chunks, aka opaque fragment[TLSPlaintext.length] in RFC 5246
            }//end handshake
        }

        internal class HandshakePacket : AbstractPacket{

            internal enum MessageTypes : byte {
                HelloRequest=0x00,
                ClientHello=0x01,
                ServerHello=0x02,
                Certificate=0x0b,

                ServerKeyExchange=0x0c,
                CertificateRequest=0x0d,
                ServerHelloDone=0x0e,
                CertificateVerify=0x0f,

                ClientKeyExchange=0x10,
                Finished=0x14,
            };

            private MessageTypes messageType;
            private uint messageLength;//actually a 3-byte (uint24) long field

            private System.Collections.Generic.List<byte[]> certificateList;//only for messageType=0x0b
            private string serverHostName = null;
            private List<Tuple<byte, byte>> supportedSslVersions;
            private List<string> applicationLayerProtocolNegotiationStrings;
            //internal byte VersionMajor { get; }//MSB
            //internal byte VersionMinor { get; }//LSB


            internal MessageTypes MessageType { get { return this.messageType; } }
            internal uint MessageLenght { get { return this.messageLength; } }
            internal System.Collections.Generic.List<byte[]> CertificateList { get { return this.certificateList; } }
            internal string ServerHostName { get { return this.serverHostName; } }

            internal Tuple<byte,byte>[] GetSupportedSslVersions() {
                return this.supportedSslVersions.ToArray();
            }
            internal string GetAlpnNextProtocolString() {
                return string.Join(", ", this.applicationLayerProtocolNegotiationStrings);
            }

            public static IEnumerable<HandshakePacket> GetHandshakes(IEnumerable<TlsRecordPacket> tlsRecordFragments) {
                using (System.IO.MemoryStream handshakeMessageData = new System.IO.MemoryStream()) {
                    Frame firstFrame = null;
                    foreach (TlsRecordPacket record in tlsRecordFragments) {
                        if (record.ContentType != TlsRecordPacket.ContentTypes.Handshake) {
                            yield break;
                        }
                        foreach (AbstractPacket recordData in record.GetSubPackets(false)) {
                            if (firstFrame == null)
                                firstFrame = recordData.ParentFrame;
                            handshakeMessageData.Write(recordData.ParentFrame.Data, recordData.PacketStartIndex, recordData.PacketLength);
                        }
                    }
                    if (handshakeMessageData.Length < 4) {//1 byte type, 3 bytes length
                        yield break;
                    }
                    handshakeMessageData.Position = 1;
                    byte[] lengthBytes = new byte[3];
                    uint messageLength = Utils.ByteConverter.ToUInt32(lengthBytes, 0, 3);
                    if (handshakeMessageData.Length < messageLength + 4) {
                        yield break;
                    }
                    handshakeMessageData.Position = 0;
                    Frame reassembledFrame = new Frame(firstFrame.Timestamp, handshakeMessageData.ToArray(), firstFrame.FrameNumber);

                    int nextHandshakeOffset = 0;
                    while (nextHandshakeOffset < reassembledFrame.Data.Length) {
                        HandshakePacket handshake;
                        try {
                            handshake = new HandshakePacket(reassembledFrame, nextHandshakeOffset, reassembledFrame.Data.Length - 1);
                            nextHandshakeOffset = handshake.PacketEndIndex + 1;
                        }
                        catch {
                            yield break;
                        }
                        yield return handshake;
                    }
                }
            }

            internal HandshakePacket(Frame parentFrame, int packetStartIndex, int packetEndIndex)
                : base(parentFrame, packetStartIndex, packetEndIndex, "TLS Handshake Protocol") {
                this.certificateList=new List<byte[]>();
                this.supportedSslVersions = new List<Tuple<byte, byte>>();
                this.applicationLayerProtocolNegotiationStrings = new List<string>();

                this.messageType=(MessageTypes)parentFrame.Data[packetStartIndex];
                if (!this.ParentFrame.QuickParse)
                    this.Attributes.Add("Message Type", ""+messageType);
                this.messageLength = Utils.ByteConverter.ToUInt32(parentFrame.Data, packetStartIndex + 1, 3);
                this.PacketEndIndex=(int)(packetStartIndex+4+messageLength-1);

                if (this.messageType == MessageTypes.ClientHello) {
                    //this.VersionMajor = parentFrame.Data[PacketStartIndex + 4];
                    //this.VersionMinor = parentFrame.Data[PacketStartIndex + 5];
                    this.supportedSslVersions.Add(new Tuple<byte, byte>(parentFrame.Data[PacketStartIndex + 4], parentFrame.Data[PacketStartIndex + 5]));
                    byte sessionIdLength = parentFrame.Data[PacketStartIndex + 38];
                    ushort cipherSuiteLength = Utils.ByteConverter.ToUInt16(parentFrame.Data, PacketStartIndex + 39 + sessionIdLength);
                    byte compressionMethodsLength = parentFrame.Data[PacketStartIndex + 41 + cipherSuiteLength];
                    ushort extensionsLength = Utils.ByteConverter.ToUInt16(parentFrame.Data, PacketStartIndex + 42 + cipherSuiteLength + compressionMethodsLength);
                    int extensionIndex = PacketStartIndex + 44 + cipherSuiteLength + compressionMethodsLength;
                    while(extensionIndex < this.PacketEndIndex && extensionIndex < PacketStartIndex + 44 + cipherSuiteLength + compressionMethodsLength + extensionsLength) {
                        ushort extensionType = Utils.ByteConverter.ToUInt16(parentFrame.Data, extensionIndex);
                        ushort extensionLength = Utils.ByteConverter.ToUInt16(parentFrame.Data, extensionIndex +2);
                        if (extensionType == 0) {//Server Name Indication rfc6066
                            ushort serverNameListLength = Utils.ByteConverter.ToUInt16(parentFrame.Data, extensionIndex + 4);
                            int offset = 6;
                            while (offset < serverNameListLength) {
                                byte serverNameType = parentFrame.Data[extensionIndex + offset];
                                ushort serverNameLength = Utils.ByteConverter.ToUInt16(parentFrame.Data, extensionIndex + offset + 1);
                                if (serverNameLength == 0)
                                    break;
                                else {
                                    if (serverNameType == 0) {//host_name(0)
                                        this.serverHostName = Utils.ByteConverter.ReadString(parentFrame.Data, extensionIndex + offset + 3, serverNameLength);
                                    }
                                    offset += serverNameLength;
                                }
                            }

                        }
                        else if (extensionType == 16) {//ALPN
                            int index = extensionIndex + 6;
                            while (index < extensionIndex + extensionLength + 4) {
                                this.applicationLayerProtocolNegotiationStrings.Add(Utils.ByteConverter.ReadLengthValueString(parentFrame.Data, ref index, 1));
                            }
                        }
                        else if(extensionType == 43) {//Supported versions
                            for(int offset = 5; offset < extensionLength + 4; offset+=2) {
                                this.supportedSslVersions.Add(new Tuple<byte, byte>(parentFrame.Data[extensionIndex + offset], parentFrame.Data[extensionIndex + offset + 1]));
                            }
                        }
                        extensionIndex += 4 + extensionLength;
                    }
                }
                else if(this.messageType == MessageTypes.ServerHello) {
                    //this.VersionMajor = parentFrame.Data[PacketStartIndex + 4];
                    //this.VersionMinor = parentFrame.Data[PacketStartIndex + 5];
                    this.supportedSslVersions.Add(new Tuple<byte, byte>(parentFrame.Data[PacketStartIndex + 4], parentFrame.Data[PacketStartIndex + 5]));

                }
                else if(this.messageType==MessageTypes.Certificate) {
                    uint certificatesLenght = Utils.ByteConverter.ToUInt32(parentFrame.Data, packetStartIndex + 4, 3);
                    int certificateIndexBase=packetStartIndex+7;
                    int certificateIndexOffset=0;
                    while(certificateIndexOffset<certificatesLenght) {
                        //read 3 byte length
                        uint certificateLenght = Utils.ByteConverter.ToUInt32(parentFrame.Data, certificateIndexBase + certificateIndexOffset, 3);
                        certificateIndexOffset+=3;
                        //rest is a certificate
                        byte[] certificate=new byte[certificateLenght];
                        Array.Copy(parentFrame.Data, certificateIndexBase+certificateIndexOffset, certificate, 0, certificate.Length);
                        this.certificateList.Add(certificate);
                        certificateIndexOffset+=certificate.Length;
                    }
                }
            }
            //Server Certificate: http://tools.ietf.org/html/rfc2246 7.4.2


            public override IEnumerable<AbstractPacket> GetSubPackets(bool includeSelfReference) {
                if(includeSelfReference)
                    yield return this;
                yield break;//no sub packets
            }


        }



    }
}
