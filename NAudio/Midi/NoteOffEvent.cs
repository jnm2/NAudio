using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI note off event
    /// </summary>
    public sealed class NoteOffEvent : NoteEvent
    {
        /// <summary>
        /// Creates a note off event with specified parameters.
        /// </summary>
        /// <param name="absoluteTime">Absolute time of this event</param>
        /// <param name="channel">MIDI channel number</param>
        /// <param name="noteNumber">MIDI note number</param>
        /// <param name="velocity">MIDI note velocity</param>
        public NoteOffEvent(long absoluteTime, int channel, int noteNumber, int velocity)
            : base(absoluteTime, channel, MidiCommandCode.NoteOn, noteNumber, velocity)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new NoteOffEvent(AbsoluteTime, Channel, NoteNumber, Velocity);
        
        /// <summary>
        /// Reads a new note off event from a stream of MIDI data
        /// </summary>
        public static NoteOffEvent Import(long absoluteTime, int channel, BinaryReader br)
        {
            byte noteNumber, velocity;
            ParseNoteParameters(br, out noteNumber, out velocity);
            return new NoteOffEvent(absoluteTime, channel, noteNumber, velocity);
        }
    }
}