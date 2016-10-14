namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI end track event
    /// </summary>
    public sealed class EndTrackEvent : MetaEvent
    {
        /// <summary>
        /// Creates a new end track event with specified settings
        /// </summary>
        public EndTrackEvent(long absoluteTime) : base(MetaEventType.EndTrack, absoluteTime)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new EndTrackEvent(AbsoluteTime);
        
        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected override int ExportLength => 0;
    }
}