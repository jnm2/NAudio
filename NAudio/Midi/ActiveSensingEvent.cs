namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI active sensing event
    /// </summary>
    public sealed class ActiveSensingEvent : MidiEvent
    {
        /// <summary>
        /// Creates an active sensing event with specified parameters
        /// </summary>
        public ActiveSensingEvent(long absoluteTime) : base(absoluteTime, 1, MidiCommandCode.ActiveSensing)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new ActiveSensingEvent(AbsoluteTime);
    }
}