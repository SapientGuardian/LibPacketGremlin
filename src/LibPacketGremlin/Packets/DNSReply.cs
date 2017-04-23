// -----------------------------------------------------------------------
//  <copyright file="DNSReply.cs" company="Outbreak Labs">
//     Copyright (c) Outbreak Labs. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using OutbreakLabs.LibPacketGremlin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DNS.Protocol;
using System.Linq;
using DNS.Protocol.ResourceRecords;

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    public class DNSReply : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 12;

        private readonly IList<Question> questions;
        private IList<IResourceRecord> answers;
        private IList<IResourceRecord> authority;
        private IList<IResourceRecord> additional;
        private Header header;

        public IPacket Payload => null;

        public IList<Question> Questions
        {
            get { return questions; }
        }

        public IList<IResourceRecord> AnswerRecords
        {
            get { return answers; }
        }

        public IList<IResourceRecord> AuthorityRecords
        {
            get { return authority; }
        }

        public IList<IResourceRecord> AdditionalRecords
        {
            get { return additional; }
        }

        public int Id
        {
            get { return header.Id; }
            set { header.Id = value; }
        }

        public bool RecursionAvailable
        {
            get { return header.RecursionAvailable; }
            set { header.RecursionAvailable = value; }
        }

        public bool AuthorativeServer
        {
            get { return header.AuthorativeServer; }
            set { header.AuthorativeServer = value; }
        }

        public bool Truncated
        {
            get { return header.Truncated; }
            set { header.Truncated = value; }
        }

        public OperationCode OperationCode
        {
            get { return header.OperationCode; }
            set { header.OperationCode = value; }
        }

        public ResponseCode ResponseCode
        {
            get { return header.ResponseCode; }
            set { header.ResponseCode = value; }
        }


        internal DNSReply()
        {
            this.header = new Header();
            this.questions = new List<Question>();
            this.answers = new List<IResourceRecord>();
            this.authority = new List<IResourceRecord>();
            this.additional = new List<IResourceRecord>();

            this.header.Response = true;
        }

        internal DNSReply(Header header, IList<Question> questions, IList<IResourceRecord> answers,
                IList<IResourceRecord> authority, IList<IResourceRecord> additional)
        {
            this.header = header;
            this.questions = questions;
            this.answers = answers;
            this.authority = authority;
            this.additional = additional;
        }

        public void CorrectFields()
        {
            header.QuestionCount = questions.Count;
            header.AnswerRecordCount = answers.Count;
            header.AuthorityRecordCount = authority.Count;
            header.AdditionalRecordCount = additional.Count;
        }

        public long Length()
        {
            return header.Size +
                    questions.Sum(q => q.Size) +
                    answers.Sum(a => a.Size) +
                    authority.Sum(a => a.Size) +
                    additional.Sum(a => a.Size);
        }

        public void SetContainer(IPacket container)
        {            
        }

        public void WriteToStream(Stream stream)
        {
            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                bw.Write(header.ToArray());
                foreach (var question in Questions)
                {
                    bw.Write(question.ToArray());
                }
                foreach (var answer in AnswerRecords)
                {
                    bw.Write(answer.ToArray());
                }
                foreach (var authority in AuthorityRecords)
                {
                    bw.Write(authority.ToArray());
                }
                foreach (var additional in AdditionalRecords)
                {
                    bw.Write(additional.ToArray());
                }
            }
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <param name="count">The length of the packet in bytes</param>
        /// <param name="index">The index into the buffer at which the packet begins</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(byte[] buffer, int index, int count, out DNSReply packet)
        {
            try
            {
                if (count < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                using (var ms = new MemoryStream(buffer, index, count, false))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        var messageBytes = br.ReadBytes(count);
                        var header = Header.FromArray(messageBytes);
                        int offset = header.Size;

                        if (!header.Response || header.QuestionCount == 0)
                        {
                            packet = null;
                            return false;
                        }

                        
                        if (header.Truncated)
                        {
                            packet = new DNSReply(header,
                                Question.GetAllFromArray(messageBytes, offset, header.QuestionCount),
                                new List<IResourceRecord>(),
                                new List<IResourceRecord>(),
                                new List<IResourceRecord>());
                        }
                        else
                        {
                            packet = new DNSReply(header,
                                    Question.GetAllFromArray(messageBytes, offset, header.QuestionCount, out offset),
                                    ResourceRecordFactory.GetAllFromArray(messageBytes, offset, header.AnswerRecordCount, out offset),
                                    ResourceRecordFactory.GetAllFromArray(messageBytes, offset, header.AuthorityRecordCount, out offset),
                                    ResourceRecordFactory.GetAllFromArray(messageBytes, offset, header.AdditionalRecordCount, out offset));
                        }
                        
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }
    }
}
