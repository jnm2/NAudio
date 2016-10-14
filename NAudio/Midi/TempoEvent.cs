using System.IO;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI tempo event
    /// </summary>
    public sealed class TempoEvent : MetaEvent 
    {
        /// <summary>
        /// Creates a new tempo event with specified settings
        /// </summary>
        /// <param name="absoluteTime">Absolute time</param>
        /// <param name="microsecondsPerQuarterNote">Microseconds per quarter note</param>
        public TempoEvent(long absoluteTime, int microsecondsPerQuarterNote) : base(MetaEventType.SetTempo, absoluteTime)
        {
            MicrosecondsPerQuarterNote = microsecondsPerQuarterNote;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new TempoEvent(AbsoluteTime, MicrosecondsPerQuarterNote);

        /// <summary>
        /// Describes this tempo event
        /// </summary>
        /// <returns>String describing the tempo event</returns>
        public override string ToString() 
        {
            return $"{base.ToString()} {60000000 / MicrosecondsPerQuarterNote}bpm ({MicrosecondsPerQuarterNote})";
        }

        /// <summary>
        /// Microseconds per quarter note
        /// </summary>
        public int MicrosecondsPerQuarterNote { get; set; }

        /// <summary>
        /// Tempo
        /// </summary>
        public double Tempo
        {
            get { return 60000000.0 / MicrosecondsPerQuarterNote; }
            set { MicrosecondsPerQuarterNote = (int)(60000000.0 / value); }
        }

        /// <summary>
        /// Reads a new tempo event from a MIDI stream
        /// </summary>
        public static TempoEvent Import(long absoluteTime, BinaryReader br, int length)
        {
            if (length != 3) throw new InvalidDataException("Invalid tempo length");
            return new TempoEvent(absoluteTime, (br.ReadByte() << 16) + (br.ReadByte() << 8) + br.ReadByte());
        }

        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected override int ExportLength => 3;

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)((MicrosecondsPerQuarterNote >> 16) & 0xFF));
            writer.Write((byte)((MicrosecondsPerQuarterNote >>  8) & 0xFF));
            writer.Write((byte)( MicrosecondsPerQuarterNote        & 0xFF));
        }
    }
}