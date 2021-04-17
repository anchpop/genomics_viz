using Capnp;
using Capnp.Rpc;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb14e4bccbe1ea87dUL)]
    public class Chromosome : ICapnpSerializable
    {
        public const UInt64 typeId = 0xb14e4bccbe1ea87dUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Backbone = reader.Backbone?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.Point>(_));
            SegmentSets = reader.SegmentSets?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SegmentSet>(_));
            ConnectionSets = reader.ConnectionSets?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Backbone.Init(Backbone, (_s1, _v1) => _v1?.serialize(_s1));
            writer.SegmentSets.Init(SegmentSets, (_s1, _v1) => _v1?.serialize(_s1));
            writer.ConnectionSets.Init(ConnectionSets, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<CapnpGen.Chromosome.Point> Backbone
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpGen.Chromosome.SegmentSet> SegmentSets
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet> ConnectionSets
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public IReadOnlyList<CapnpGen.Chromosome.Point.READER> Backbone => ctx.ReadList(0).Cast(CapnpGen.Chromosome.Point.READER.create);
            public IReadOnlyList<CapnpGen.Chromosome.SegmentSet.READER> SegmentSets => ctx.ReadList(1).Cast(CapnpGen.Chromosome.SegmentSet.READER.create);
            public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.READER> ConnectionSets => ctx.ReadList(2).Cast(CapnpGen.Chromosome.ConnectionSet.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 3);
            }

            public ListOfStructsSerializer<CapnpGen.Chromosome.Point.WRITER> Backbone
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.Point.WRITER>>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<CapnpGen.Chromosome.SegmentSet.WRITER> SegmentSets
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SegmentSet.WRITER>>(1);
                set => Link(1, value);
            }

            public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.WRITER> ConnectionSets
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.WRITER>>(2);
                set => Link(2, value);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x813540d3e155e671UL)]
        public class Point : ICapnpSerializable
        {
            public const UInt64 typeId = 0x813540d3e155e671UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Coordinate = CapnpSerializable.Create<CapnpGen.Chromosome.Point.Vec3>(reader.Coordinate);
                Bin = reader.Bin;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Coordinate?.serialize(writer.Coordinate);
                writer.Bin = Bin;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public CapnpGen.Chromosome.Point.Vec3 Coordinate
            {
                get;
                set;
            }

            public uint Bin
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public CapnpGen.Chromosome.Point.Vec3.READER Coordinate => ctx.ReadStruct(0, CapnpGen.Chromosome.Point.Vec3.READER.create);
                public uint Bin => ctx.ReadDataUInt(0UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public CapnpGen.Chromosome.Point.Vec3.WRITER Coordinate
                {
                    get => BuildPointer<CapnpGen.Chromosome.Point.Vec3.WRITER>(0);
                    set => Link(0, value);
                }

                public uint Bin
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xa77ac4ce72d8f055UL)]
            public class Vec3 : ICapnpSerializable
            {
                public const UInt64 typeId = 0xa77ac4ce72d8f055UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    X = reader.X;
                    Y = reader.Y;
                    Z = reader.Z;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.X = X;
                    writer.Y = Y;
                    writer.Z = Z;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public float X
                {
                    get;
                    set;
                }

                public float Y
                {
                    get;
                    set;
                }

                public float Z
                {
                    get;
                    set;
                }

                public struct READER
                {
                    readonly DeserializerState ctx;
                    public READER(DeserializerState ctx)
                    {
                        this.ctx = ctx;
                    }

                    public static READER create(DeserializerState ctx) => new READER(ctx);
                    public static implicit operator DeserializerState(READER reader) => reader.ctx;
                    public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                    public float X => ctx.ReadDataFloat(0UL, 0F);
                    public float Y => ctx.ReadDataFloat(32UL, 0F);
                    public float Z => ctx.ReadDataFloat(64UL, 0F);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(2, 0);
                    }

                    public float X
                    {
                        get => this.ReadDataFloat(0UL, 0F);
                        set => this.WriteData(0UL, value, 0F);
                    }

                    public float Y
                    {
                        get => this.ReadDataFloat(32UL, 0F);
                        set => this.WriteData(32UL, value, 0F);
                    }

                    public float Z
                    {
                        get => this.ReadDataFloat(64UL, 0F);
                        set => this.WriteData(64UL, value, 0F);
                    }
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd0f05383c40349c6UL)]
        public class SegmentSet : ICapnpSerializable
        {
            public const UInt64 typeId = 0xd0f05383c40349c6UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Name = reader.Name;
                Description = reader.Description;
                Segments = reader.Segments?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.Segment>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Name = Name;
                writer.Description = Description;
                writer.Segments.Init(Segments, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Name
            {
                get;
                set;
            }

            public string Description
            {
                get;
                set;
            }

            public IReadOnlyList<CapnpGen.Chromosome.Segment> Segments
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Name => ctx.ReadText(0, null);
                public string Description => ctx.ReadText(1, null);
                public IReadOnlyList<CapnpGen.Chromosome.Segment.READER> Segments => ctx.ReadList(2).Cast(CapnpGen.Chromosome.Segment.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 3);
                }

                public string Name
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public string Description
                {
                    get => this.ReadText(1, null);
                    set => this.WriteText(1, value, null);
                }

                public ListOfStructsSerializer<CapnpGen.Chromosome.Segment.WRITER> Segments
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.Segment.WRITER>>(2);
                    set => Link(2, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xa620ac8b9742056dUL)]
        public class Segment : ICapnpSerializable
        {
            public const UInt64 typeId = 0xa620ac8b9742056dUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Info = reader.Info;
                StartBin = reader.StartBin;
                EndBin = reader.EndBin;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Info = Info;
                writer.StartBin = StartBin;
                writer.EndBin = EndBin;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Info
            {
                get;
                set;
            }

            public uint StartBin
            {
                get;
                set;
            }

            public uint EndBin
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Info => ctx.ReadText(0, null);
                public uint StartBin => ctx.ReadDataUInt(0UL, 0U);
                public uint EndBin => ctx.ReadDataUInt(32UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 1);
                }

                public string Info
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public uint StartBin
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public uint EndBin
                {
                    get => this.ReadDataUInt(32UL, 0U);
                    set => this.WriteData(32UL, value, 0U);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9e37cd7feb33c53eUL)]
        public class ConnectionSet : ICapnpSerializable
        {
            public const UInt64 typeId = 0x9e37cd7feb33c53eUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Name = reader.Name;
                Description = reader.Description;
                Connections = reader.Connections?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.Connection>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Name = Name;
                writer.Description = Description;
                writer.Connections.Init(Connections, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public string Name
            {
                get;
                set;
            }

            public string Description
            {
                get;
                set;
            }

            public IReadOnlyList<CapnpGen.Chromosome.Connection> Connections
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public string Name => ctx.ReadText(0, null);
                public string Description => ctx.ReadText(1, null);
                public IReadOnlyList<CapnpGen.Chromosome.Connection.READER> Connections => ctx.ReadList(2).Cast(CapnpGen.Chromosome.Connection.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 3);
                }

                public string Name
                {
                    get => this.ReadText(0, null);
                    set => this.WriteText(0, value, null);
                }

                public string Description
                {
                    get => this.ReadText(1, null);
                    set => this.WriteText(1, value, null);
                }

                public ListOfStructsSerializer<CapnpGen.Chromosome.Connection.WRITER> Connections
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.Connection.WRITER>>(2);
                    set => Link(2, value);
                }
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xfb40cd528956dab1UL)]
        public class Connection : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfb40cd528956dab1UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                StartBinLower = reader.StartBinLower;
                StartBinUpper = reader.StartBinUpper;
                EndBinLower = reader.EndBinLower;
                EndBinUpper = reader.EndBinUpper;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.StartBinLower = StartBinLower;
                writer.StartBinUpper = StartBinUpper;
                writer.EndBinLower = EndBinLower;
                writer.EndBinUpper = EndBinUpper;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint StartBinLower
            {
                get;
                set;
            }

            public uint StartBinUpper
            {
                get;
                set;
            }

            public uint EndBinLower
            {
                get;
                set;
            }

            public uint EndBinUpper
            {
                get;
                set;
            }

            public struct READER
            {
                readonly DeserializerState ctx;
                public READER(DeserializerState ctx)
                {
                    this.ctx = ctx;
                }

                public static READER create(DeserializerState ctx) => new READER(ctx);
                public static implicit operator DeserializerState(READER reader) => reader.ctx;
                public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
                public uint StartBinLower => ctx.ReadDataUInt(0UL, 0U);
                public uint StartBinUpper => ctx.ReadDataUInt(32UL, 0U);
                public uint EndBinLower => ctx.ReadDataUInt(64UL, 0U);
                public uint EndBinUpper => ctx.ReadDataUInt(96UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(2, 0);
                }

                public uint StartBinLower
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public uint StartBinUpper
                {
                    get => this.ReadDataUInt(32UL, 0U);
                    set => this.WriteData(32UL, value, 0U);
                }

                public uint EndBinLower
                {
                    get => this.ReadDataUInt(64UL, 0U);
                    set => this.WriteData(64UL, value, 0U);
                }

                public uint EndBinUpper
                {
                    get => this.ReadDataUInt(96UL, 0U);
                    set => this.WriteData(96UL, value, 0U);
                }
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe4237fce752a6e72UL)]
    public class ChromosomeSet : ICapnpSerializable
    {
        public const UInt64 typeId = 0xe4237fce752a6e72UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            Description = reader.Description;
            Chromosomes = reader.Chromosomes?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Name = Name;
            writer.Description = Description;
            writer.Chromosomes.Init(Chromosomes, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public IReadOnlyList<CapnpGen.Chromosome> Chromosomes
        {
            get;
            set;
        }

        public struct READER
        {
            readonly DeserializerState ctx;
            public READER(DeserializerState ctx)
            {
                this.ctx = ctx;
            }

            public static READER create(DeserializerState ctx) => new READER(ctx);
            public static implicit operator DeserializerState(READER reader) => reader.ctx;
            public static implicit operator READER(DeserializerState ctx) => new READER(ctx);
            public string Name => ctx.ReadText(0, null);
            public string Description => ctx.ReadText(1, null);
            public IReadOnlyList<CapnpGen.Chromosome.READER> Chromosomes => ctx.ReadList(2).Cast(CapnpGen.Chromosome.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 3);
            }

            public string Name
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public string Description
            {
                get => this.ReadText(1, null);
                set => this.WriteText(1, value, null);
            }

            public ListOfStructsSerializer<CapnpGen.Chromosome.WRITER> Chromosomes
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.WRITER>>(2);
                set => Link(2, value);
            }
        }
    }
}