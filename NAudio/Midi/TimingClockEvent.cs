namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI timing clock event
    /// </summary>
    public sealed class TimingClockEvent : MidiEvent
    {
        /// <summary>
        /// Creates a timing clock event with specified parameters
        /// </summary>
        public TimingClockEvent(long absoluteTime) : base(absoluteTime, 1, MidiCommandCode.TimingClock)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new TimingClockEvent(AbsoluteTime);
    }
}
