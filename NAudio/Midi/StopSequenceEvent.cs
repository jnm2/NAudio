namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI stop sequence event
    /// </summary>
    public sealed class StopSequenceEvent : MidiEvent
    {
        /// <summary>
        /// Creates a stop sequence event with specified parameters
        /// </summary>
        public StopSequenceEvent(long absoluteTime) : base(absoluteTime, 1, MidiCommandCode.StopSequence)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new StopSequenceEvent(AbsoluteTime);
    }
}