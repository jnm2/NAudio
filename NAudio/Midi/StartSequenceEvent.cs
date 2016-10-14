namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI start sequence event
    /// </summary>
    public sealed class StartSequenceEvent : MidiEvent
    {
        /// <summary>
        /// Creates a start sequence event with specified parameters
        /// </summary>
        public StartSequenceEvent(long absoluteTime) : base(absoluteTime, 1, MidiCommandCode.StartSequence)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new StartSequenceEvent(AbsoluteTime);
    }
}