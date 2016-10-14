using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI SMPTE offset event
    /// </summary>
    public sealed class SmpteOffsetEvent : MetaEvent
    {
        /// <summary>
        /// Creates a new SMPTE offset event
        /// </summary>
        public SmpteOffsetEvent(long absoluteTime, byte hours, byte minutes, byte seconds, byte frames, byte subFrames) : base(MetaEventType.SmpteOffset, absoluteTime)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Frames = frames;
            SubFrames = subFrames;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new SmpteOffsetEvent(AbsoluteTime, Hours, Minutes, Seconds, Frames, SubFrames);

        /// <summary>
        /// Hours
        /// </summary>
        public byte Hours { get; set; }

        /// <summary>
        /// Minutes
        /// </summary>
        public byte Minutes { get; set; }

        /// <summary>
        /// Seconds
        /// </summary>
        public byte Seconds { get; set; }

        /// <summary>
        /// Frames
        /// </summary>
        public byte Frames { get; set; }

        /// <summary>
        /// SubFrames
        /// </summary>
        public byte SubFrames { get; set; }


        /// <summary>
        /// Describes this SMPTE offset
        /// </summary>
        /// <returns>A string describing this event</returns>
        public override string ToString() => $"{base.ToString()} {Hours}:{Minutes}:{Seconds}:{Frames}:{SubFrames}";

        /// <summary>
        /// Reads a new SMPTE offset from a MIDI stream
        /// </summary>
        public static SmpteOffsetEvent Import(long absoluteTime, BinaryReader br, int length)
        {
            if (length != 5) throw new InvalidDataException($"Invalid SMPTE Offset length: Got {length}, expected 5");
            return new SmpteOffsetEvent(
                absoluteTime,
                hours: br.ReadByte(),
                minutes: br.ReadByte(),
                seconds: br.ReadByte(),
                frames: br.ReadByte(),
                subFrames: br.ReadByte());
        }

        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected override int ExportLength => 5;

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(Hours);
            writer.Write(Minutes);
            writer.Write(Seconds);
            writer.Write(Frames);
            writer.Write(SubFrames);
        }
    }
}

