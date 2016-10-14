using System;
using System.IO;

namespace NAudio.Midi 
{
    /// <summary>
    /// Represents an individual MIDI event
    /// </summary>
    public abstract class MidiEvent : ICloneable
    {
        /// <summary>The MIDI command code</summary>
        private int channel;
        private long absoluteTime;

        /// <summary>
        /// Creates a MidiEvent from a raw message received using
        /// the MME MIDI In APIs
        /// </summary>
        /// <param name="rawMessage">The short MIDI message</param>
        /// <returns>A new MIDI Event</returns>
        public static MidiEvent FromRawMessage(int rawMessage)
        {
            int b = rawMessage & 0xFF;
            int data1 = (rawMessage >> 8) & 0xFF;
            int data2 = (rawMessage >> 16) & 0xFF;
            MidiCommandCode commandCode;
            int channel = 1;

            if ((b & 0xF0) == 0xF0)
            {
                // both bytes are used for command code in this case
                commandCode = (MidiCommandCode)b;
            }
            else
            {
                commandCode = (MidiCommandCode)(b & 0xF0);
                channel = (b & 0x0F) + 1;
            }
            
            switch (commandCode)
            {
                case MidiCommandCode.NoteOn:
                    return new NoteOnEvent(0, channel, data1, data2, null);
                case MidiCommandCode.NoteOff:
                    return new NoteOffEvent(0, channel, data1, data2);
                case MidiCommandCode.KeyAftertouch:
                    return new KeyAftertouchEvent(0, channel, data1, data2);
                case MidiCommandCode.ControlChange:
                    return new ControlChangeEvent(0, channel, (MidiController)data1, data2);
                case MidiCommandCode.PatchChange:
                    return new PatchChangeEvent(0, channel, data1);
                case MidiCommandCode.ChannelAftertouch:
                    return new ChannelAftertouchEvent(0, channel, data1);
                case MidiCommandCode.PitchWheelChange:
                    return new PitchWheelChangeEvent(0, channel, data1 + (data2 << 7));
                case MidiCommandCode.TimingClock:
                    return new TimingClockEvent(0);
                case MidiCommandCode.StartSequence:
                    return new StartSequenceEvent(0);
                case MidiCommandCode.ContinueSequence:
                    return new ContinueSequenceEvent(0);
                case MidiCommandCode.StopSequence:
                    return new StopSequenceEvent(0);
                case MidiCommandCode.ActiveSensing:
                    return new ActiveSensingEvent(0);
                case MidiCommandCode.MetaEvent:
                case MidiCommandCode.Sysex:
                default:
                    throw new FormatException($"Unsupported MIDI Command Code for Raw Message {commandCode}");
            }
        }

        /// <summary>
        /// Constructs a MidiEvent from a BinaryStream
        /// </summary>
        /// <param name="br">The binary stream of MIDI data</param>
        /// <param name="trackPreviousEvent">The previous MIDI event (pass null for first event)</param>
        /// <returns>A new MidiEvent</returns>
        public static MidiEvent ReadNextEvent(BinaryReader br, MidiEvent trackPreviousEvent) 
        {
            var absoluteTime = (trackPreviousEvent?.AbsoluteTime ?? 0) + ReadVarInt(br);

            MidiCommandCode commandCode;
            var channel = 1;
            var b = br.ReadByte();
            if ((b & 0x80) == 0) 
            {
                if (trackPreviousEvent == null)
                {
                    throw new InvalidDataException("Expected full command for first event of the track.");
                    // The first event in each MTrk chunk must specify status.
                    // § Standard MIDI Files, p 6, ¶ 2
                    // The Complete MIDI 1.0 Detailed Specification, document version 96.1, third edition, MIDI Manufacturers Association
                    // https://www.midi.org/specifications/category/midi-1-0-detailed-specifications
                }

                // a running command - command & channel are same as previous
                commandCode = trackPreviousEvent.CommandCode;
                channel = trackPreviousEvent.Channel;
                br.BaseStream.Position--; // need to push this back
            }
            else 
            {
                if ((b & 0xF0) == 0xF0) 
                {
                    // both bytes are used for command code in this case
                    commandCode = (MidiCommandCode) b;
                }
                else 
                {
                    commandCode = (MidiCommandCode) (b & 0xF0);
                    channel = (b & 0x0F) + 1;
                }
            }
            
            switch (commandCode) 
            {
                case MidiCommandCode.NoteOn:
                    return NoteOnEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.NoteOff:
                    return NoteOffEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.KeyAftertouch:
                    return KeyAftertouchEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.ControlChange:
                    return ControlChangeEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.PatchChange:
                    return PatchChangeEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.ChannelAftertouch:
                    return ChannelAftertouchEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.PitchWheelChange:
                    return PitchWheelChangeEvent.Import(absoluteTime, channel, br);
                case MidiCommandCode.TimingClock:
                    return new TimingClockEvent(absoluteTime);
                case MidiCommandCode.StartSequence:
                    return new StartSequenceEvent(absoluteTime);
                case MidiCommandCode.ContinueSequence:
                    return new ContinueSequenceEvent(absoluteTime);
                case MidiCommandCode.StopSequence:
                    return new StopSequenceEvent(absoluteTime);
                case MidiCommandCode.ActiveSensing:
                    return new ActiveSensingEvent(absoluteTime);
                case MidiCommandCode.Sysex:
                    return SysexEvent.Import(absoluteTime, br);
                case MidiCommandCode.MetaEvent:
                    return MetaEvent.Import(absoluteTime, br);
                default:
                    throw new FormatException($"Unsupported MIDI Command Code {(byte)commandCode:X2}");
            }
        }

        /// <summary>
        /// Converts this MIDI event to a short message (32 bit integer) that
        /// can be sent by the Windows MIDI out short message APIs
        /// Cannot be implemented for all MIDI messages
        /// </summary>
        /// <returns>A short message</returns>
        public virtual int GetAsShortMessage()
        {
            return (channel - 1) + (int)CommandCode;
        }

        /// <summary>
        /// Creates a MIDI event with specified parameters
        /// </summary>
        /// <param name="absoluteTime">Absolute time of this event</param>
        /// <param name="channel">MIDI channel number</param>
        /// <param name="commandCode">MIDI command code</param>
        protected MidiEvent(long absoluteTime, int channel, MidiCommandCode commandCode)
        {
            this.absoluteTime = absoluteTime;
            this.Channel = channel;
            CommandCode = commandCode;
        }

        /// <summary>
        /// Creates a deep clone of this MIDI event.
        /// </summary>
        public abstract MidiEvent Clone();

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// The MIDI Channel Number for this event (1-16)
        /// </summary>
        public virtual int Channel 
        {
            get 
            {
                return channel;
            }
            set
            {
                if ((value < 1) || (value > 16))
                {
                    throw new ArgumentOutOfRangeException("value", value,
                        String.Format("Channel must be 1-16 (Got {0})",value));
                }
                channel = value;
            }
        }
        
        /// <summary>
        /// The absolute time for this event
        /// </summary>
        public virtual long AbsoluteTime 
        {
            get 
            {
                return absoluteTime;
            }
            set 
            {
                absoluteTime = value;
            }
        }
        
        /// <summary>
        /// The command code for this event
        /// </summary>
        public MidiCommandCode CommandCode { get; }

        /// <summary>
        /// Whether this is a note off event
        /// </summary>
        public static bool IsNoteOff(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent ne = (NoteEvent)midiEvent;
                    return (ne.Velocity == 0);
                }
                return (midiEvent.CommandCode == MidiCommandCode.NoteOff);
            }
            return false;
        }

        /// <summary>
        /// Whether this is a note on event
        /// </summary>
        public static bool IsNoteOn(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                {
                    NoteEvent ne = (NoteEvent)midiEvent;
                    return (ne.Velocity > 0);
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if this is an end track event
        /// </summary>
        public static bool IsEndTrack(MidiEvent midiEvent)
        {
            if (midiEvent != null)
            {
                MetaEvent me = midiEvent as MetaEvent;
                if (me != null)
                {
                    return me.MetaEventType == MetaEventType.EndTrack;
                }
            }
            return false;
        }

        
        /// <summary>
        /// Displays a summary of the MIDI event
        /// </summary>
        /// <returns>A string containing a brief description of this MIDI event</returns>
        public override string ToString() 
        {
            if(CommandCode >= MidiCommandCode.Sysex)
                return String.Format("{0} {1}",absoluteTime,CommandCode);
            else
                return String.Format("{0} {1} Ch: {2}", absoluteTime, CommandCode, channel);
        }
        
        /// <summary>
        /// Utility function that can read a variable length integer from a binary stream
        /// </summary>
        /// <param name="br">The binary stream</param>
        /// <returns>The integer read</returns>
        public static int ReadVarInt(BinaryReader br) 
        {
            int value = 0;
            byte b;
            for(int n = 0; n < 4; n++) 
            {
                b = br.ReadByte();
                value <<= 7;
                value += (b & 0x7F);
                if((b & 0x80) == 0) 
                {
                    return value;
                }
            }
            throw new FormatException("Invalid Var Int");
        }

        /// <summary>
        /// Writes a variable length integer to a binary stream
        /// </summary>
        /// <param name="writer">Binary stream</param>
        /// <param name="value">The value to write</param>
        public static void WriteVarInt(BinaryWriter writer, int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "Cannot write a negative Var Int");
            }
            if (value > 0x0FFFFFFF)
            {
                throw new ArgumentOutOfRangeException("value", value, "Maximum allowed Var Int is 0x0FFFFFFF");
            }

            int n = 0;
            byte[] buffer = new byte[4];
            do
            {
                buffer[n++] = (byte)(value & 0x7F);
                value >>= 7;
            } while (value > 0);
            
            while (n > 0)
            {
                n--;
                if(n > 0)
                    writer.Write((byte) (buffer[n] | 0x80));
                else 
                    writer.Write(buffer[n]);
            }
        }

        /// <summary>
        /// Exports this MIDI event's data
        /// Overriden in derived classes, but they should call this version
        /// </summary>
        /// <param name="absoluteTime">Absolute time used to calculate delta. 
        /// Is updated ready for the next delta calculation</param>
        /// <param name="writer">Stream to write to</param>
        public virtual void Export(ref long absoluteTime, BinaryWriter writer)
        {
            if (this.absoluteTime < absoluteTime)
            {
                throw new FormatException("Can't export unsorted MIDI events");
            }
            WriteVarInt(writer,(int) (this.absoluteTime - absoluteTime));
            absoluteTime = this.absoluteTime;
            int output = (int) CommandCode;
            if (CommandCode != MidiCommandCode.MetaEvent)
            {
                output += (channel - 1);
            }
            writer.Write((byte)output);
        }
    }
}