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
            Index = CapnpSerializable.Create<CapnpGen.Chromosome.index>(reader.Index);
            Backbone = reader.Backbone?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.Point>(_));
            SegmentSets = reader.SegmentSets?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SegmentSet>(_));
            ConnectionSets = reader.ConnectionSets?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet>(_));
            SiteSets = reader.SiteSets?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Index?.serialize(writer.Index);
            writer.Backbone.Init(Backbone, (_s1, _v1) => _v1?.serialize(_s1));
            writer.SegmentSets.Init(SegmentSets, (_s1, _v1) => _v1?.serialize(_s1));
            writer.ConnectionSets.Init(ConnectionSets, (_s1, _v1) => _v1?.serialize(_s1));
            writer.SiteSets.Init(SiteSets, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.Chromosome.index Index
        {
            get;
            set;
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

        public IReadOnlyList<CapnpGen.Chromosome.SiteSet> SiteSets
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
            public index.READER Index => new index.READER(ctx);
            public IReadOnlyList<CapnpGen.Chromosome.Point.READER> Backbone => ctx.ReadList(0).Cast(CapnpGen.Chromosome.Point.READER.create);
            public IReadOnlyList<CapnpGen.Chromosome.SegmentSet.READER> SegmentSets => ctx.ReadList(1).Cast(CapnpGen.Chromosome.SegmentSet.READER.create);
            public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.READER> ConnectionSets => ctx.ReadList(2).Cast(CapnpGen.Chromosome.ConnectionSet.READER.create);
            public IReadOnlyList<CapnpGen.Chromosome.SiteSet.READER> SiteSets => ctx.ReadList(3).Cast(CapnpGen.Chromosome.SiteSet.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 4);
            }

            public index.WRITER Index
            {
                get => Rewrap<index.WRITER>();
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

            public ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.WRITER> SiteSets
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.WRITER>>(3);
                set => Link(3, value);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xfcb97c476c6fb2d2UL)]
        public class index : ICapnpSerializable
        {
            public const UInt64 typeId = 0xfcb97c476c6fb2d2UL;
            public enum WHICH : ushort
            {
                Numbered = 0,
                X = 1,
                Y = 2,
                undefined = 65535
            }

            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                switch (reader.which)
                {
                    case WHICH.Numbered:
                        Numbered = reader.Numbered;
                        break;
                    case WHICH.X:
                        which = reader.which;
                        break;
                    case WHICH.Y:
                        which = reader.which;
                        break;
                }

                applyDefaults();
            }

            private WHICH _which = WHICH.undefined;
            private object _content;
            public WHICH which
            {
                get => _which;
                set
                {
                    if (value == _which)
                        return;
                    _which = value;
                    switch (value)
                    {
                        case WHICH.Numbered:
                            _content = 0;
                            break;
                        case WHICH.X:
                            break;
                        case WHICH.Y:
                            break;
                    }
                }
            }

            public void serialize(WRITER writer)
            {
                writer.which = which;
                switch (which)
                {
                    case WHICH.Numbered:
                        writer.Numbered = Numbered.Value;
                        break;
                    case WHICH.X:
                        break;
                    case WHICH.Y:
                        break;
                }
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint? Numbered
            {
                get => _which == WHICH.Numbered ? (uint? )_content : null;
                set
                {
                    _which = WHICH.Numbered;
                    _content = value;
                }
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
                public WHICH which => (WHICH)ctx.ReadDataUShort(32U, (ushort)0);
                public uint Numbered => which == WHICH.Numbered ? ctx.ReadDataUInt(0UL, 0U) : default;
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                }

                public WHICH which
                {
                    get => (WHICH)this.ReadDataUShort(32U, (ushort)0);
                    set => this.WriteData(32U, (ushort)value, (ushort)0);
                }

                public uint Numbered
                {
                    get => which == WHICH.Numbered ? this.ReadDataUInt(0UL, 0U) : default;
                    set => this.WriteData(0UL, value, 0U);
                }
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
                Description = CapnpSerializable.Create<CapnpGen.Description>(reader.Description);
                Segments = CapnpSerializable.Create<CapnpGen.Chromosome.SegmentSet.segments>(reader.Segments);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Description?.serialize(writer.Description);
                Segments?.serialize(writer.Segments);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public CapnpGen.Description Description
            {
                get;
                set;
            }

            public CapnpGen.Chromosome.SegmentSet.segments Segments
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
                public CapnpGen.Description.READER Description => ctx.ReadStruct(0, CapnpGen.Description.READER.create);
                public segments.READER Segments => new segments.READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 2);
                }

                public CapnpGen.Description.WRITER Description
                {
                    get => BuildPointer<CapnpGen.Description.WRITER>(0);
                    set => Link(0, value);
                }

                public segments.WRITER Segments
                {
                    get => Rewrap<segments.WRITER>();
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xab9c3a6431928a08UL)]
            public class segments : ICapnpSerializable
            {
                public const UInt64 typeId = 0xab9c3a6431928a08UL;
                public enum WHICH : ushort
                {
                    Genes = 0,
                    Others = 1,
                    undefined = 65535
                }

                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    switch (reader.which)
                    {
                        case WHICH.Genes:
                            Genes = reader.Genes?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>(_));
                            break;
                        case WHICH.Others:
                            Others = reader.Others?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>(_));
                            break;
                    }

                    applyDefaults();
                }

                private WHICH _which = WHICH.undefined;
                private object _content;
                public WHICH which
                {
                    get => _which;
                    set
                    {
                        if (value == _which)
                            return;
                        _which = value;
                        switch (value)
                        {
                            case WHICH.Genes:
                                _content = null;
                                break;
                            case WHICH.Others:
                                _content = null;
                                break;
                        }
                    }
                }

                public void serialize(WRITER writer)
                {
                    writer.which = which;
                    switch (which)
                    {
                        case WHICH.Genes:
                            writer.Genes.Init(Genes, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.Others:
                            writer.Others.Init(Others, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                    }
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public IReadOnlyList<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>> Genes
                {
                    get => _which == WHICH.Genes ? (IReadOnlyList<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>>)_content : null;
                    set
                    {
                        _which = WHICH.Genes;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>> Others
                {
                    get => _which == WHICH.Others ? (IReadOnlyList<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>>)_content : null;
                    set
                    {
                        _which = WHICH.Others;
                        _content = value;
                    }
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
                    public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
                    public IReadOnlyList<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>.READER> Genes => which == WHICH.Genes ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>.READER> Others => which == WHICH.Others ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>.READER.create) : default;
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public WHICH which
                    {
                        get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                        set => this.WriteData(0U, (ushort)value, (ushort)0);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>.WRITER> Genes
                    {
                        get => which == WHICH.Genes ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.Gene>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>.WRITER> Others
                    {
                        get => which == WHICH.Others ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SegmentSet.Segment<CapnpGen.Chromosome.SegmentSet.ChromatinState>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9cfa2265064f7566UL)]
            public class Segment<TExtraInfo> : ICapnpSerializable where TExtraInfo : class
            {
                public const UInt64 typeId = 0x9cfa2265064f7566UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Location = CapnpSerializable.Create<CapnpGen.Chromosome.SegmentSet.Location>(reader.Location);
                    ExtraInfo = CapnpSerializable.Create<TExtraInfo>(reader.ExtraInfo);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    Location?.serialize(writer.Location);
                    writer.ExtraInfo.SetObject(ExtraInfo);
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public CapnpGen.Chromosome.SegmentSet.Location Location
                {
                    get;
                    set;
                }

                public TExtraInfo ExtraInfo
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
                    public CapnpGen.Chromosome.SegmentSet.Location.READER Location => ctx.ReadStruct(0, CapnpGen.Chromosome.SegmentSet.Location.READER.create);
                    public DeserializerState ExtraInfo => ctx.StructReadPointer(1);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public CapnpGen.Chromosome.SegmentSet.Location.WRITER Location
                    {
                        get => BuildPointer<CapnpGen.Chromosome.SegmentSet.Location.WRITER>(0);
                        set => Link(0, value);
                    }

                    public DynamicSerializerState ExtraInfo
                    {
                        get => BuildPointer<DynamicSerializerState>(1);
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xfb23a1504b7ceb60UL)]
            public class Location : ICapnpSerializable
            {
                public const UInt64 typeId = 0xfb23a1504b7ceb60UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    StartBin = reader.StartBin;
                    EndBin = reader.EndBin;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
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
                    public uint StartBin => ctx.ReadDataUInt(0UL, 0U);
                    public uint EndBin => ctx.ReadDataUInt(32UL, 0U);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(1, 0);
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc925f8c38ceb448bUL)]
            public class Gene : ICapnpSerializable
            {
                public const UInt64 typeId = 0xc925f8c38ceb448bUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Ascending = reader.Ascending;
                    Stat = reader.Stat;
                    Name = reader.Name;
                    Id = reader.Id;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Ascending = Ascending;
                    writer.Stat = Stat;
                    writer.Name = Name;
                    writer.Id = Id;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public bool Ascending
                {
                    get;
                    set;
                }

                public string Stat
                {
                    get;
                    set;
                }

                public string Name
                {
                    get;
                    set;
                }

                public string Id
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
                    public bool Ascending => ctx.ReadDataBool(0UL, false);
                    public string Stat => ctx.ReadText(0, null);
                    public string Name => ctx.ReadText(1, null);
                    public string Id => ctx.ReadText(2, null);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(1, 3);
                    }

                    public bool Ascending
                    {
                        get => this.ReadDataBool(0UL, false);
                        set => this.WriteData(0UL, value, false);
                    }

                    public string Stat
                    {
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }

                    public string Name
                    {
                        get => this.ReadText(1, null);
                        set => this.WriteText(1, value, null);
                    }

                    public string Id
                    {
                        get => this.ReadText(2, null);
                        set => this.WriteText(2, value, null);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x86703710594224e9UL)]
            public class ChromatinState : ICapnpSerializable
            {
                public const UInt64 typeId = 0x86703710594224e9UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Info = reader.Info;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Info = Info;
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
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 1);
                    }

                    public string Info
                    {
                        get => this.ReadText(0, null);
                        set => this.WriteText(0, value, null);
                    }
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
                Description = CapnpSerializable.Create<CapnpGen.Description>(reader.Description);
                Connections = reader.Connections?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Connection>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Description?.serialize(writer.Description);
                writer.Connections.Init(Connections, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public CapnpGen.Description Description
            {
                get;
                set;
            }

            public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection> Connections
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
                public CapnpGen.Description.READER Description => ctx.ReadStruct(0, CapnpGen.Description.READER.create);
                public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection.READER> Connections => ctx.ReadList(1).Cast(CapnpGen.Chromosome.ConnectionSet.Connection.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 2);
                }

                public CapnpGen.Description.WRITER Description
                {
                    get => BuildPointer<CapnpGen.Description.WRITER>(0);
                    set => Link(0, value);
                }

                public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection.WRITER> Connections
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection.WRITER>>(1);
                    set => Link(1, value);
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc1ade6784ae88567UL)]
            public class Connection : ICapnpSerializable
            {
                public const UInt64 typeId = 0xc1ade6784ae88567UL;
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

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb5e47c0e74a7880aUL)]
        public class SiteSet : ICapnpSerializable
        {
            public const UInt64 typeId = 0xb5e47c0e74a7880aUL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Description = CapnpSerializable.Create<CapnpGen.Description>(reader.Description);
                Sites = CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet.sites>(reader.Sites);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Description?.serialize(writer.Description);
                Sites?.serialize(writer.Sites);
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public CapnpGen.Description Description
            {
                get;
                set;
            }

            public CapnpGen.Chromosome.SiteSet.sites Sites
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
                public CapnpGen.Description.READER Description => ctx.ReadStruct(0, CapnpGen.Description.READER.create);
                public sites.READER Sites => new sites.READER(ctx);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 2);
                }

                public CapnpGen.Description.WRITER Description
                {
                    get => BuildPointer<CapnpGen.Description.WRITER>(0);
                    set => Link(0, value);
                }

                public sites.WRITER Sites
                {
                    get => Rewrap<sites.WRITER>();
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xfd0d3e78a737b056UL)]
            public class sites : ICapnpSerializable
            {
                public const UInt64 typeId = 0xfd0d3e78a737b056UL;
                public enum WHICH : ushort
                {
                    ProteinBindingSites = 0,
                    ChromatinAccessibility = 1,
                    GeneticVariants = 2,
                    undefined = 65535
                }

                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    switch (reader.which)
                    {
                        case WHICH.ProteinBindingSites:
                            ProteinBindingSites = reader.ProteinBindingSites?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>>(_));
                            break;
                        case WHICH.ChromatinAccessibility:
                            ChromatinAccessibility = reader.ChromatinAccessibility?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>>(_));
                            break;
                        case WHICH.GeneticVariants:
                            GeneticVariants = reader.GeneticVariants?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>>(_));
                            break;
                    }

                    applyDefaults();
                }

                private WHICH _which = WHICH.undefined;
                private object _content;
                public WHICH which
                {
                    get => _which;
                    set
                    {
                        if (value == _which)
                            return;
                        _which = value;
                        switch (value)
                        {
                            case WHICH.ProteinBindingSites:
                                _content = null;
                                break;
                            case WHICH.ChromatinAccessibility:
                                _content = null;
                                break;
                            case WHICH.GeneticVariants:
                                _content = null;
                                break;
                        }
                    }
                }

                public void serialize(WRITER writer)
                {
                    writer.which = which;
                    switch (which)
                    {
                        case WHICH.ProteinBindingSites:
                            writer.ProteinBindingSites.Init(ProteinBindingSites, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.ChromatinAccessibility:
                            writer.ChromatinAccessibility.Init(ChromatinAccessibility, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.GeneticVariants:
                            writer.GeneticVariants.Init(GeneticVariants, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                    }
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>> ProteinBindingSites
                {
                    get => _which == WHICH.ProteinBindingSites ? (IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>>)_content : null;
                    set
                    {
                        _which = WHICH.ProteinBindingSites;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>> ChromatinAccessibility
                {
                    get => _which == WHICH.ChromatinAccessibility ? (IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>>)_content : null;
                    set
                    {
                        _which = WHICH.ChromatinAccessibility;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>> GeneticVariants
                {
                    get => _which == WHICH.GeneticVariants ? (IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>>)_content : null;
                    set
                    {
                        _which = WHICH.GeneticVariants;
                        _content = value;
                    }
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
                    public WHICH which => (WHICH)ctx.ReadDataUShort(0U, (ushort)0);
                    public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.READER> ProteinBindingSites => which == WHICH.ProteinBindingSites ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>.READER> ChromatinAccessibility => which == WHICH.ChromatinAccessibility ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>.READER> GeneticVariants => which == WHICH.GeneticVariants ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>.READER.create) : default;
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                    }

                    public WHICH which
                    {
                        get => (WHICH)this.ReadDataUShort(0U, (ushort)0);
                        set => this.WriteData(0U, (ushort)value, (ushort)0);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.WRITER> ProteinBindingSites
                    {
                        get => which == WHICH.ProteinBindingSites ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>.WRITER> ChromatinAccessibility
                    {
                        get => which == WHICH.ChromatinAccessibility ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ChromatinAccessibility>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>.WRITER> GeneticVariants
                    {
                        get => which == WHICH.GeneticVariants ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.GeneticVariants>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb1bf4ee9dc45859bUL)]
            public class Site<TExtraInfo> : ICapnpSerializable where TExtraInfo : class
            {
                public const UInt64 typeId = 0xb1bf4ee9dc45859bUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Location = CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet.Location>(reader.Location);
                    ExtraInfo = CapnpSerializable.Create<TExtraInfo>(reader.ExtraInfo);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    Location?.serialize(writer.Location);
                    writer.ExtraInfo.SetObject(ExtraInfo);
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public CapnpGen.Chromosome.SiteSet.Location Location
                {
                    get;
                    set;
                }

                public TExtraInfo ExtraInfo
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
                    public CapnpGen.Chromosome.SiteSet.Location.READER Location => ctx.ReadStruct(0, CapnpGen.Chromosome.SiteSet.Location.READER.create);
                    public DeserializerState ExtraInfo => ctx.StructReadPointer(1);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public CapnpGen.Chromosome.SiteSet.Location.WRITER Location
                    {
                        get => BuildPointer<CapnpGen.Chromosome.SiteSet.Location.WRITER>(0);
                        set => Link(0, value);
                    }

                    public DynamicSerializerState ExtraInfo
                    {
                        get => BuildPointer<DynamicSerializerState>(1);
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xfde6fb7b158dc800UL)]
            public class Location : ICapnpSerializable
            {
                public const UInt64 typeId = 0xfde6fb7b158dc800UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Bin = reader.Bin;
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    writer.Bin = Bin;
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
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
                    public uint Bin => ctx.ReadDataUInt(0UL, 0U);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(1, 0);
                    }

                    public uint Bin
                    {
                        get => this.ReadDataUInt(0UL, 0U);
                        set => this.WriteData(0UL, value, 0U);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xdaa7016f679bd778UL)]
            public class ProteinBinding : ICapnpSerializable
            {
                public const UInt64 typeId = 0xdaa7016f679bd778UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
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
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 0);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd173cb11b2c565d7UL)]
            public class ChromatinAccessibility : ICapnpSerializable
            {
                public const UInt64 typeId = 0xd173cb11b2c565d7UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
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
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 0);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xda00f003e11f857aUL)]
            public class GeneticVariants : ICapnpSerializable
            {
                public const UInt64 typeId = 0xda00f003e11f857aUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
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
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 0);
                    }
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
            Description = CapnpSerializable.Create<CapnpGen.Description>(reader.Description);
            Chromosomes = reader.Chromosomes?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            Description?.serialize(writer.Description);
            writer.Chromosomes.Init(Chromosomes, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public CapnpGen.Description Description
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
            public CapnpGen.Description.READER Description => ctx.ReadStruct(0, CapnpGen.Description.READER.create);
            public IReadOnlyList<CapnpGen.Chromosome.READER> Chromosomes => ctx.ReadList(1).Cast(CapnpGen.Chromosome.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public CapnpGen.Description.WRITER Description
            {
                get => BuildPointer<CapnpGen.Description.WRITER>(0);
                set => Link(0, value);
            }

            public ListOfStructsSerializer<CapnpGen.Chromosome.WRITER> Chromosomes
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.WRITER>>(1);
                set => Link(1, value);
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x9ccea4605441fb2fUL)]
    public class Description : ICapnpSerializable
    {
        public const UInt64 typeId = 0x9ccea4605441fb2fUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Name = reader.Name;
            TheDescription = reader.TheDescription;
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Name = Name;
            writer.TheDescription = TheDescription;
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

        public string TheDescription
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
            public string TheDescription => ctx.ReadText(1, null);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 2);
            }

            public string Name
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public string TheDescription
            {
                get => this.ReadText(1, null);
                set => this.WriteText(1, value, null);
            }
        }
    }
}