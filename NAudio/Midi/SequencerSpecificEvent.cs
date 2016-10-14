using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a sequencer-specific event
    /// </summary>
    public sealed class SequencerSpecificEvent : RawMetaEvent
    {
        /// <summary>
        /// Creates a new sequencer-specific event
        /// </summary>
        /// <param name="data">The sequencer-specific data</param>
        /// <param name="absoluteTime">Absolute time of this event</param>
        public SequencerSpecificEvent(long absoluteTime, byte[] data)
            : base(MetaEventType.SequencerSpecific, absoluteTime, data)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new SequencerSpecificEvent(AbsoluteTime, (byte[])Data.Clone());

        /// <summary>
        /// Reads a new sequencer-specific event from a MIDI stream
        /// </summary>
        public static SequencerSpecificEvent Import(long absoluteTime, BinaryReader br, int length)
        {
            var data = br.ReadBytes(length);
            if (data.Length != length) throw new InvalidDataException("Failed to read meta event's data fully");
            return new SequencerSpecificEvent(absoluteTime, data);
        }
    }
}