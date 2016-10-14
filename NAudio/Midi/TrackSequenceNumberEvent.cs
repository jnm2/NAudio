using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI track sequence number event
    /// </summary>
    public sealed class TrackSequenceNumberEvent : MetaEvent
    {
        /// <summary>
        /// The MIDI track sequence number
        /// </summary>
        public ushort SequenceNumber { get; set; }

        /// <summary>
        /// Creates a new track sequence number event
        /// </summary>
        public TrackSequenceNumberEvent(long absoluteTime, ushort sequenceNumber) : base(MetaEventType.TrackSequenceNumber, absoluteTime)
        {
            SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new TrackSequenceNumberEvent(AbsoluteTime, SequenceNumber);

        /// <summary>
        /// Describes this event
        /// </summary>
        /// <returns>String describing the event</returns>
        public override string ToString() => $"{base.ToString()} {SequenceNumber}";

        /// <summary>
        /// Reads a new track sequence number event from a MIDI stream
        /// </summary>
        public static TrackSequenceNumberEvent Import(long absoluteTime, BinaryReader br, int length)
        {
            // TODO: there is a form of the TrackSequenceNumberEvent that has a length of zero
            if (length != 2) throw new InvalidDataException("Invalid sequence number length");
            return new TrackSequenceNumberEvent(absoluteTime, (ushort)((br.ReadByte() << 8) + br.ReadByte()));
        }


        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected override int ExportLength => 2;

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)((SequenceNumber >> 8) & 0xFF));
            writer.Write((byte)( SequenceNumber       & 0xFF));
        }
    }
}
