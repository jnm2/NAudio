using System.IO;
using NAudio.Midi;
using NUnit.Framework;

namespace NAudioTests.Midi
{
    [TestFixture]
    public class MidiFileTests
    {
        private static MidiEventCollection CreateSampleCollection(int type)
        {
            var events = new MidiEventCollection(type, 120);

            events.AddTrack(new MidiEvent[]
            {
                new NoteOnEvent(0, 1, 40, 40, null),
                new NoteOffEvent(120, 1, 40, 0)
            });

            if (type != 0)
            {
                events.AddTrack(new MidiEvent[]
                {
                    new NoteOnEvent(0, 2, 40, 40, null),
                    new NoteOffEvent(120, 2, 40, 0)
                });
            }

            events.PrepareForExport();

            return events;
        }

        private static Stream WriteCollectionStream(MidiEventCollection collection)
        {
            using (var stream = new MemoryStream())
            {
                MidiFile.Export(new AssertNotClosedStream(stream), collection);
                return new MemoryStream(stream.ToArray(), false);
            }
        }

        [Test]
        public void WriteAndReadType0()
        {
            var collection = CreateSampleCollection(0);
            using (var stream = WriteCollectionStream(collection))
                AssertThatMidiCollectionIsEqualTo(new MidiFile(stream).Events, collection);
        }

        [Test]
        public void WriteAndReadType1()
        {
            var collection = CreateSampleCollection(1);
            using (var stream = WriteCollectionStream(collection))
                AssertThatMidiCollectionIsEqualTo(new MidiFile(stream).Events, collection);
        }

        private static void AssertThatMidiCollectionIsEqualTo(MidiEventCollection actual, MidiEventCollection expected)
        {
            if (actual == expected) return;
            if (actual == null) Assert.Fail("Expected: null; Actual: non-null");
            if (expected == null) Assert.Fail("Expected: non-null: Actual: null");

            if (actual.MidiFileType != expected.MidiFileType)
                Assert.Fail($"Expected {nameof(expected.MidiFileType)}: {expected.MidiFileType}; Actual: {actual.MidiFileType}");

            if (actual.DeltaTicksPerQuarterNote != expected.DeltaTicksPerQuarterNote)
                Assert.Fail($"Expected {nameof(expected.DeltaTicksPerQuarterNote)}: {expected.DeltaTicksPerQuarterNote}; Actual: {actual.DeltaTicksPerQuarterNote}");

            if (actual.StartAbsoluteTime != expected.StartAbsoluteTime)
                Assert.Fail($"Expected {nameof(expected.StartAbsoluteTime)}: {expected.StartAbsoluteTime}; Actual: {actual.StartAbsoluteTime}");

            if (actual.Tracks != expected.Tracks)
                Assert.Fail($"Expected {nameof(expected.Tracks)}: {expected.Tracks}; Actual: {actual.Tracks}");
            
            for (var track = 0; track < actual.Tracks; track++)
            {
                var actualTrack = actual[track];
                var expectedTrack = expected[track];

                if (actualTrack.Count != expectedTrack.Count)
                    Assert.Fail(
                        $"Expected track {track} {nameof(expectedTrack.Count)}: {expectedTrack.Count}; Actual: {expectedTrack.Count}");

                for (var i = 0; i < actualTrack.Count; i++)
                {
                    var actualEvent = actualTrack[i];
                    var expectedEvent = expectedTrack[i];

                    if (actualEvent.CommandCode != expectedEvent.CommandCode)
                        Assert.Fail($"Expected track {track} event {i} {nameof(expectedEvent.CommandCode)}: {expectedEvent.CommandCode}; Actual: {actualEvent.CommandCode}");

                    if (actualEvent.Channel != expectedEvent.Channel)
                        Assert.Fail($"Expected track {track} event {i} {nameof(expectedEvent.Channel)}: {expectedEvent.Channel}; Actual: {actualEvent.Channel}");

                    if (actualEvent.AbsoluteTime != expectedEvent.AbsoluteTime)
                        Assert.Fail($"Expected track {track} event {i} {nameof(expectedEvent.AbsoluteTime)}: {expectedEvent.AbsoluteTime}; Actual: {actualEvent.AbsoluteTime}");
                        
                    // Other properties not tested
                }
            }
        }

        [Test]
        public void ExportDoesNotCloseStream()
        {
            using (var stream = new MemoryStream())
                MidiFile.Export(new AssertNotClosedStream(stream), CreateSampleCollection(1));
        }

        [Test]
        public void ImportDoesNotCloseStream()
        {
            using (var stream = WriteCollectionStream(CreateSampleCollection(1)))
                new MidiFile(new AssertNotClosedStream(stream));
        }

        private sealed class AssertNotClosedStream : Stream
        {
            private readonly Stream innerStream;

            public AssertNotClosedStream(Stream innerStream)
            {
                this.innerStream = innerStream;
            }

            public override void Close()
            {
                Assert.Fail("The stream was closed.");
            }

            public override void Flush()
            {
                innerStream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);

            public override void SetLength(long value) => innerStream.SetLength(value);

            public override int Read(byte[] buffer, int offset, int count) => innerStream.Read(buffer, offset, count);

            public override void Write(byte[] buffer, int offset, int count) => innerStream.Write(buffer, offset, count);

            public override bool CanRead => innerStream.CanRead;
            public override bool CanSeek => innerStream.CanRead;
            public override bool CanWrite => innerStream.CanRead;
            
            public override long Length => innerStream.Length;

            public override long Position
            {
                get { return innerStream.Position; }
                set { innerStream.Position = value; }
            }
        }
    }
}
