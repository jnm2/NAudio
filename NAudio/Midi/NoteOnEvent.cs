using System;
using System.IO;

namespace NAudio.Midi
{
    /// <summary>
    /// Represents a MIDI note on event
    /// </summary>
    public sealed class NoteOnEvent : NoteEvent
    {
        private NoteEvent offEvent;
        
        /// <summary>
        /// Creates a note on event with specified parameters and optionally creates an associated note off event a given duration of time ahead.
        /// </summary>
        /// <param name="absoluteTime">Absolute time of this event</param>
        /// <param name="channel">MIDI channel number</param>
        /// <param name="noteNumber">MIDI note number</param>
        /// <param name="velocity">MIDI note velocity</param>
        /// <param name="duration">If set, specifies where to place the created note off event.</param>
        public NoteOnEvent(long absoluteTime, int channel, int noteNumber, int velocity, int? duration)
            : base(absoluteTime, channel, MidiCommandCode.NoteOn, noteNumber, velocity)
        {
            if (duration != null)
                OffEvent = new NoteOffEvent(absoluteTime + duration.Value, channel, noteNumber, 0);
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public override MidiEvent Clone() => new NoteOnEvent(AbsoluteTime, Channel, NoteNumber, Velocity, NoteLength);

        /// <summary>
        /// The associated note off event
        /// </summary>
        public NoteEvent OffEvent
        {
            get
            {
                return offEvent;
            }
            set
            {
                if (!MidiEvent.IsNoteOff(value))
                {
                    throw new ArgumentException("OffEvent must be a valid MIDI note off event");
                }
                if (value.NoteNumber != this.NoteNumber)
                {
                    throw new ArgumentException("Note Off Event must be for the same note number");
                }
                if (value.Channel != this.Channel)
                {
                    throw new ArgumentException("Note Off Event must be for the same channel");
                }
                offEvent = value;
            }
        }

        /// <summary>
        /// Get or set the Note Number, updating the off event at the same time
        /// </summary>
        public override int NoteNumber
        {
            get
            {
                return base.NoteNumber;
            }
            set
            {
                base.NoteNumber = value;
                if (OffEvent != null)
                {
                    OffEvent.NoteNumber = NoteNumber;
                }
            }
        }

        /// <summary>
        /// Get or set the channel, updating the off event at the same time
        /// </summary>
        public override int Channel
        {
            get
            {
                return base.Channel;
            }
            set
            {
                base.Channel = value;
                if (OffEvent != null)
                {
                    OffEvent.Channel = Channel;
                }
            }
        }

        /// <summary>
        /// The duration of this note
        /// </summary>
        public int? NoteLength
        {
            get
            {
                return offEvent == null ? (int?)null : (int)(offEvent.AbsoluteTime - AbsoluteTime);
            }
            set
            {
                if (value == null)
                {
                    if (OffEvent != null) throw new InvalidOperationException($"If {nameof(OffEvent)} is not null, the note length cannot be null.");
                }
                else
                {
                    if (OffEvent == null) throw new InvalidOperationException($"If {nameof(OffEvent)} is null, the note length must be null.");
                    if (value.Value < 0) throw new ArgumentOutOfRangeException(nameof(value), value.Value, "Note length must be greater than or equal to zero.");
                    offEvent.AbsoluteTime = AbsoluteTime + value.Value;
                }
            }
        }

        /// <summary>
        /// The absolute time for this event
        /// </summary>
        public override long AbsoluteTime
        {
            get { return base.AbsoluteTime; }
            set
            {
                base.AbsoluteTime = value;
                NoteLength = NoteLength;
            }
        }

        /// <summary>
        /// Reads a new note on event from a stream of MIDI data
        /// </summary>
        public static NoteOnEvent Import(long absoluteTime, int channel, BinaryReader br)
        {
            byte noteNumber, velocity;
            ParseNoteParameters(br, out noteNumber, out velocity);
            return new NoteOnEvent(absoluteTime, channel, noteNumber, velocity, null);
        }

        /// <summary>
        /// Describes the note Event
        /// </summary>
        public override string ToString()
        {
            if ((this.Velocity == 0) && (OffEvent == null))
            {
                return String.Format("{0} (Note Off)",
                    base.ToString());
            }
            return String.Format("{0} Len: {1}",
                base.ToString(),
                (this.OffEvent == null) ? "?" : this.NoteLength.ToString());
        }
    }
}