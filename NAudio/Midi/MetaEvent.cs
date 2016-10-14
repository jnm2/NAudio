using System;
using System.IO;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI meta event
    /// </summary>
    public abstract class MetaEvent : MidiEvent 
    {
        /// <summary>
        /// Gets the type of this meta event
        /// </summary>
        public MetaEventType MetaEventType { get; }
        

        /// <summary>
        /// Custom constructor for use by derived types, who will manage the data themselves
        /// </summary>
        /// <param name="metaEventType">Meta event type</param>
        /// <param name="absoluteTime">Absolute time</param>
        protected MetaEvent(MetaEventType metaEventType, long absoluteTime)
            : base(absoluteTime, 1, MidiCommandCode.MetaEvent)
        {
            MetaEventType = metaEventType;
        }

        /// <summary>
        /// Reads a meta event from a stream
        /// </summary>
        public static MetaEvent Import(long absoluteTime, BinaryReader br) 
        {
            var metaEventType = (MetaEventType)br.ReadByte();
            var length = ReadVarInt(br);
            
            switch (metaEventType) 
            {
                case MetaEventType.TrackSequenceNumber: // Sets the track's sequence number.
                    return TrackSequenceNumberEvent.Import(absoluteTime, br, length);
                case MetaEventType.TextEvent: // Text event
                case MetaEventType.Copyright: // Copyright
                case MetaEventType.SequenceTrackName: // Sequence / Track Name
                case MetaEventType.TrackInstrumentName: // Track instrument name
                case MetaEventType.Lyric: // lyric
                case MetaEventType.Marker: // marker
                case MetaEventType.CuePoint: // cue point
                case MetaEventType.ProgramName:
                case MetaEventType.DeviceName:
                    return TextEvent.Import(metaEventType, absoluteTime, br, length);
                case MetaEventType.EndTrack: // This event must come at the end of each track
                    if (length != 0) throw new InvalidDataException("End track length");
                    return new EndTrackEvent(absoluteTime);
                case MetaEventType.SetTempo: // Set tempo
                    return TempoEvent.Import(absoluteTime, br, length);
                case MetaEventType.TimeSignature: // Time signature
                    return TimeSignatureEvent.Import(absoluteTime, br, length);
                case MetaEventType.KeySignature: // Key signature
                    return KeySignatureEvent.Import(absoluteTime, br, length);
                case MetaEventType.SequencerSpecific: // Sequencer specific information
                    return SequencerSpecificEvent.Import(absoluteTime, br, length);
                case MetaEventType.SmpteOffset:
                    return SmpteOffsetEvent.Import(absoluteTime, br, length);
                default:
                    var data = br.ReadBytes(length);
                    if (data.Length != length) throw new InvalidDataException("Failed to read meta event's data fully");
                    return new RawMetaEvent(metaEventType, absoluteTime, data);
            }
        }

        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected abstract int ExportLength { get; }

        /// <summary>
        /// <see cref="MidiEvent.Export"/>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            writer.Write((byte)MetaEventType);
            WriteVarInt(writer, ExportLength);
        }

        /// <summary>
        /// Describes this meta event
        /// </summary>
        public override string ToString() => $"{AbsoluteTime} {MetaEventType}";
    }
}