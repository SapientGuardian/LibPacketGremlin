// -----------------------------------------------------------------------
//  <copyright file="DNSQuery.cs" company="Outbreak Labs">
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

namespace OutbreakLabs.LibPacketGremlin.Packets
{
    public class DNSQuery : IPacket
    {
        /// <summary>
        ///     The minimum number of bytes required for a successful parse
        /// </summary>
        private const int MinimumParseableBytes = 12;

        private readonly IList<Question> questions;
        private Header header;

        public IPacket Payload => null;

        public IList<Question> Questions => questions;
        public int Id
        {
            get { return header.Id; }
            set { header.Id = value; }
        }

        public OperationCode OperationCode
        {
            get { return header.OperationCode; }
            set { header.OperationCode = value; }
        }
        public bool RecursionDesired
        {
            get { return header.RecursionDesired; }
            set { header.RecursionDesired = value; }
        }


        internal DNSQuery()
        {
            this.questions = new List<Question>();
            this.header = new Header();

            this.header.OperationCode = OperationCode.Query;
            this.header.Response = false;
        }

        internal DNSQuery(Header header, IList<Question> questions)
        {
            this.header = header;
            this.questions = questions;
        }

        public void CorrectFields()
        {
            header.QuestionCount = questions.Count;
        }

        public long Length()
        {
            return header.Size + questions.Sum(q => q.Size);
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
            }
        }

        /// <summary>
        ///     Attempts to parse raw data into a structured packet
        /// </summary>
        /// <param name="buffer">Raw data to parse</param>
        /// <param name="packet">Parsed packet</param>
        /// <returns>True if parsing was successful, false if it is not.</returns>
        internal static bool TryParse(ReadOnlySpan<byte> buffer, out DNSQuery packet)
        {
            try
            {
                if (buffer.Length < MinimumParseableBytes)
                {
                    packet = null;
                    return false;
                }

                var messageBytes = buffer.ToArray();
                
                var header = Header.FromArray(messageBytes);
                if (header.Response || header.QuestionCount == 0 ||
                    header.AdditionalRecordCount + header.AnswerRecordCount + header.AuthorityRecordCount > 0 ||
                    header.ResponseCode != ResponseCode.NoError)
                {

                    packet = null;
                    return false;
                }

                var questions = Question.GetAllFromArray(messageBytes, header.Size, header.QuestionCount);
                packet = new DNSQuery(header, questions);
                return true;

            }
            catch (Exception)
            {
                packet = null;
                return false;
            }
        }
    }
}
