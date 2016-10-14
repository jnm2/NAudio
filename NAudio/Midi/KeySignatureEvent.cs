using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI key signature event event
    /// </summary>
    public sealed class KeySignatureEvent : MetaEvent
    {
        /// <summary>
        /// Creates a new key signature event with the specified data
        /// </summary>
        public KeySignatureEvent(long absoluteTime, byte sharpsFlats, byte majorMinor)
            : base(MetaEventType.KeySignature, absoluteTime)
        {
            SharpsFlats = sharpsFlats;
            MajorMinor = majorMinor;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new KeySignatureEvent(AbsoluteTime, SharpsFlats, MajorMinor);

        /// <summary>
        /// Number of sharps or flats (-7=7 flats, 0=key of C,7=7 sharps)
        /// </summary>
        public byte SharpsFlats { get; set; }

        /// <summary>
        /// Major or Minor key (0=major, 1=minor)
        /// </summary>
        public byte MajorMinor { get; set; }

        /// <summary>
        /// Describes this event
        /// </summary>
        public override string ToString() => $"{base.ToString()} {SharpsFlats} {MajorMinor}";

        /// <summary>
        /// Reads a new key signature event from a MIDI stream
        /// </summary>
        public static KeySignatureEvent Import(long absoluteTime, BinaryReader br, int length)
        {
            if (length != 2) throw new InvalidDataException("Invalid key signature length");
            return new KeySignatureEvent(
                absoluteTime,
                sharpsFlats: br.ReadByte(),
                majorMinor: br.ReadByte());
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
            writer.Write(SharpsFlats);
            writer.Write(MajorMinor);
        }
    }
}



