using System.IO;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI time signature event
    /// </summary>
    public sealed class TimeSignatureEvent : MetaEvent 
    {
        /// <summary>
        /// Creates a new time signature event
        /// </summary>
        /// <param name="absoluteTime">Time at which to create this event</param>
        /// <param name="numerator">Numerator</param>
        /// <param name="denominator">Denominator</param>
        /// <param name="ticksInMetronomeClick">Ticks in Metronome Click</param>
        /// <param name="no32ndNotesInQuarterNote">No of 32nd Notes in Quarter Click</param>
        public TimeSignatureEvent(long absoluteTime, byte numerator, byte denominator, byte ticksInMetronomeClick, byte no32ndNotesInQuarterNote)
            : base(MetaEventType.TimeSignature, absoluteTime)
        {
            Numerator = numerator;
            Denominator = denominator;
            TicksInMetronomeClick = ticksInMetronomeClick;
            No32ndNotesInQuarterNote = no32ndNotesInQuarterNote;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new TimeSignatureEvent(AbsoluteTime, Numerator, Denominator, TicksInMetronomeClick, No32ndNotesInQuarterNote);

        /// <summary>
        /// Numerator (number of beats in a bar)
        /// </summary>
        public byte Numerator { get; set; }

        /// <summary>
        /// Denominator (Beat unit),
        /// 1 means 2, 2 means 4 (crochet), 3 means 8 (quaver), 4 means 16 and 5 means 32
        /// </summary>
        public byte Denominator { get; set; }

        /// <summary>
        /// Ticks in a metronome click
        /// </summary>
        public byte TicksInMetronomeClick { get; set; }

        /// <summary>
        /// Number of 32nd notes in a quarter note
        /// </summary>
        public byte No32ndNotesInQuarterNote { get; set; }

        /// <summary>
        /// The time signature
        /// </summary>
        public string TimeSignature => $"{Numerator}/{GetDenominatorDisplay(Denominator)}";

        private static string GetDenominatorDisplay(byte denominator)
        {
            switch (denominator)
            {
                case 1:
                    return "2";
                case 2:
                    return "4";
                case 3:
                    return "8";
                case 4:
                    return "16";
                case 5:
                    return "32";
                default:
                    return $"Unknown ({denominator})";
            }
        }
        
        /// <summary>
        /// Describes this time signature event
        /// </summary>
        /// <returns>A string describing this event</returns>
        public override string ToString() => $"{base.ToString()} {TimeSignature} TicksInClick:{TicksInMetronomeClick} 32ndsInQuarterNote:{No32ndNotesInQuarterNote}";

        /// <summary>
        /// Reads a new time signature event from a MIDI stream
        /// </summary>
        public static TimeSignatureEvent Import(long absoluteTime, BinaryReader br, int length)
        {
            if (length != 4) throw new InvalidDataException($"Invalid time signature length: Got {length}, expected 4");
            return new TimeSignatureEvent(
                absoluteTime,
                numerator: br.ReadByte(),
                denominator: br.ReadByte(),
                ticksInMetronomeClick: br.ReadByte(),
                no32ndNotesInQuarterNote: br.ReadByte());
        }


        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected override int ExportLength => 4;

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write(Numerator);
            writer.Write(Denominator);
            writer.Write(TicksInMetronomeClick);
            writer.Write(No32ndNotesInQuarterNote);
        }
    }
}