using System;
using System.ComponentModel;
using System.IO;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents a MIDI text event
    /// </summary>
    public sealed class TextEvent : MetaEvent 
    {
        /// <summary>
        /// Creates a new text event
        /// </summary>
        /// <param name="metaEventType">MetaEvent type (must be one that is
        /// associated with text data)</param>
        /// <param name="absoluteTime">Absolute time of this event</param>
        /// <param name="text">The text in this type</param>
        public TextEvent(MetaEventType metaEventType, long absoluteTime, string text)
            : base(metaEventType, absoluteTime)
        {
            Text = text;
        }
        /// <summary>
        /// Deprecated: constructors should put type and time first.
        /// </summary>
        [Obsolete("Deprecated: constructors should put type and time first."), EditorBrowsable(EditorBrowsableState.Never)]
        public TextEvent(string text, MetaEventType metaEventType, long absoluteTime)
            : this(metaEventType, absoluteTime, text)
        {
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new TextEvent(MetaEventType, AbsoluteTime, Text);

        /// <summary>
        /// The contents of this text event
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Describes this MIDI text event
        /// </summary>
        /// <returns>A string describing this event</returns>
        public override string ToString() => $"{base.ToString()} {Text}";

        /// <summary>
        /// Reads a new text event from a MIDI stream
        /// </summary>
        public static TextEvent Import(MetaEventType metaEventType, long absoluteTime, BinaryReader br, int length)
        {
            return new TextEvent(metaEventType, absoluteTime, Utils.ByteEncoding.Instance.GetString(br.ReadBytes(length)));
        }

        /// <summary>
        /// The length of the meta event's exported bytes
        /// </summary>
        protected override int ExportLength => Text?.Length ?? 0;

        /// <summary>
        /// Calls base class export first, then exports the data 
        /// specific to this event
        /// <seealso cref="MidiEvent.Export">MidiEvent.Export</seealso>
        /// </summary>
        public override void Export(ref long absoluteTime, BinaryWriter writer)
        {
            base.Export(ref absoluteTime, writer);
            if (string.IsNullOrEmpty(Text)) return;
            base.Export(ref absoluteTime, writer);
            writer.Write(Utils.ByteEncoding.Instance.GetBytes(Text));
        }
    }
}