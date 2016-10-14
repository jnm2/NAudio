using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI sysex message
    /// </summary>
    public sealed class SysexEvent : MidiEvent 
    {
        /// <summary>
        /// The raw data sent in the sysex message
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Creates a MIDI sysex message with specified parameters
        /// </summary>
        public SysexEvent(long absoluteTime, byte[] data) : base(absoluteTime, 1, MidiCommandCode.Sysex)
        {
            Data = data;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new SysexEvent(AbsoluteTime, (byte[])Data?.Clone());

        /// <summary>
        /// Describes this sysex message
        /// </summary>
        /// <returns>A string describing the sysex message</returns>
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in Data)
            {
                sb.AppendFormat("{0:X2} ", b);
            }
            return String.Format("{0} Sysex: {1} bytes\r\n{2}",this.AbsoluteTime,Data.Length,sb.ToString());
        }

        /// <summary>
        /// Reads a sysex message from a MIDI stream
        /// </summary>
        public static SysexEvent Import(long absoluteTime, BinaryReader br)
        {
            var sysexData = new List<byte>();
            while (true)
            {
                var b = br.ReadByte();
                if (b == 0xF7) break;
                sysexData.Add(b);
            }
            return new SysexEvent(absoluteTime, sysexData.ToArray());
        }

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            if (Data != null) writer.Write(Data, 0, Data.Length);
            writer.Write((byte)0xF7);
        }
    }
}