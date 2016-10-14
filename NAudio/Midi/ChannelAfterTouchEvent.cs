using System;
using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI channel aftertouch event.
    /// </summary>
    public sealed class ChannelAftertouchEvent : MidiEvent
    {
        private byte aftertouchPressure;

        /// <summary>
        /// Creates a new channel aftertouch event
        /// </summary>
        /// <param name="absoluteTime">Absolute time</param>
        /// <param name="channel">Channel</param>
        /// <param name="aftertouchPressure">Aftertouch pressure</param>
        public ChannelAftertouchEvent(long absoluteTime, int channel, int aftertouchPressure)
            : base(absoluteTime, channel, MidiCommandCode.ChannelAftertouch)
        {
            AftertouchPressure = aftertouchPressure;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new ChannelAftertouchEvent(AbsoluteTime, Channel, aftertouchPressure);

        /// <summary>
        /// Reads a channel aftertouch event from a MIDI stream
        /// </summary>
        public static ChannelAftertouchEvent Import(long absoluteTime, int channel, BinaryReader br)
        {
            var aftertouchPressure = br.ReadByte();
            if ((aftertouchPressure & 0x80) != 0)
            {
                throw new InvalidDataException("Invalid aftertouchPressure");
                // TODO: might be a follow-on
            }
            return new ChannelAftertouchEvent(absoluteTime, channel, aftertouchPressure);
        }

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(aftertouchPressure);
        }

        /// <summary>
        /// The aftertouch pressure value
        /// </summary>
        public int AftertouchPressure
        {
            get { return aftertouchPressure; }
            set
            {
                if (value < 0 || value > 127)
                {
                    throw new ArgumentOutOfRangeException("value", "After touch pressure must be in the range 0-127");
                }
                aftertouchPressure = (byte) value;
            }
        }
    }
}
