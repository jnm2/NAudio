using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI key aftertouch event
    /// </summary>
    public sealed class KeyAftertouchEvent : NoteEvent
    {
        /// <summary>
        /// Creates a key aftertouch event with specified parameters.
        /// </summary>
        /// <param name="absoluteTime">Absolute time of this event</param>
        /// <param name="channel">MIDI channel number</param>
        /// <param name="noteNumber">MIDI note number</param>
        /// <param name="velocity">MIDI note velocity</param>
        public KeyAftertouchEvent(long absoluteTime, int channel, int noteNumber, int velocity)
            : base(absoluteTime, channel, MidiCommandCode.KeyAftertouch, noteNumber, velocity)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new KeyAftertouchEvent(AbsoluteTime, Channel, NoteNumber, Velocity);

        /// <summary>
        /// Reads a new key aftertouch event from a stream of MIDI data
        /// </summary>
        public static KeyAftertouchEvent Import(long absoluteTime, int channel, BinaryReader br)
        {
            byte noteNumber, velocity;
            ParseNoteParameters(br, out noteNumber, out velocity);
            return new KeyAftertouchEvent(absoluteTime, channel, noteNumber, velocity);
        }
    }
}