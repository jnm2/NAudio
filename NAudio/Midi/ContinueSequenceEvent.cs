namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI continue sequence event
    /// </summary>
    public sealed class ContinueSequenceEvent : MidiEvent
    {
        /// <summary>
        /// Creates a continue sequence event with specified parameters
        /// </summary>
        public ContinueSequenceEvent(long absoluteTime) : base(absoluteTime, 1, MidiCommandCode.ContinueSequence)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new ContinueSequenceEvent(AbsoluteTime);
    }
}