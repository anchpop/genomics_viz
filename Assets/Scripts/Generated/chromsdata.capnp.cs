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
                    Location = CapnpSerializable.Create<CapnpGen.Chromosome.BinRange>(reader.Location);
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

                public CapnpGen.Chromosome.BinRange Location
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
                    public CapnpGen.Chromosome.BinRange.READER Location => ctx.ReadStruct(0, CapnpGen.Chromosome.BinRange.READER.create);
                    public DeserializerState ExtraInfo => ctx.StructReadPointer(1);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public CapnpGen.Chromosome.BinRange.WRITER Location
                    {
                        get => BuildPointer<CapnpGen.Chromosome.BinRange.WRITER>(0);
                        set => Link(0, value);
                    }

                    public DynamicSerializerState ExtraInfo
                    {
                        get => BuildPointer<DynamicSerializerState>(1);
                        set => Link(1, value);
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
                Connections = CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.connections>(reader.Connections);
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                Description?.serialize(writer.Description);
                Connections?.serialize(writer.Connections);
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

            public CapnpGen.Chromosome.ConnectionSet.connections Connections
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
                public connections.READER Connections => new connections.READER(ctx);
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

                public connections.WRITER Connections
                {
                    get => Rewrap<connections.WRITER>();
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xa193d53375e36ee1UL)]
            public class connections : ICapnpSerializable
            {
                public const UInt64 typeId = 0xa193d53375e36ee1UL;
                public enum WHICH : ushort
                {
                    ChromatinInteractionPredictions = 0,
                    SignificantHiCInteractions = 1,
                    ChIAPetInteractions = 2,
                    CaptureCInteractions = 3,
                    EQtlLink = 4,
                    undefined = 65535
                }

                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    switch (reader.which)
                    {
                        case WHICH.ChromatinInteractionPredictions:
                            ChromatinInteractionPredictions = reader.ChromatinInteractionPredictions?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>>(_));
                            break;
                        case WHICH.SignificantHiCInteractions:
                            SignificantHiCInteractions = reader.SignificantHiCInteractions?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>>(_));
                            break;
                        case WHICH.ChIAPetInteractions:
                            ChIAPetInteractions = reader.ChIAPetInteractions?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>>(_));
                            break;
                        case WHICH.CaptureCInteractions:
                            CaptureCInteractions = reader.CaptureCInteractions?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>>(_));
                            break;
                        case WHICH.EQtlLink:
                            EQtlLink = reader.EQtlLink?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>>(_));
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
                            case WHICH.ChromatinInteractionPredictions:
                                _content = null;
                                break;
                            case WHICH.SignificantHiCInteractions:
                                _content = null;
                                break;
                            case WHICH.ChIAPetInteractions:
                                _content = null;
                                break;
                            case WHICH.CaptureCInteractions:
                                _content = null;
                                break;
                            case WHICH.EQtlLink:
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
                        case WHICH.ChromatinInteractionPredictions:
                            writer.ChromatinInteractionPredictions.Init(ChromatinInteractionPredictions, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.SignificantHiCInteractions:
                            writer.SignificantHiCInteractions.Init(SignificantHiCInteractions, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.ChIAPetInteractions:
                            writer.ChIAPetInteractions.Init(ChIAPetInteractions, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.CaptureCInteractions:
                            writer.CaptureCInteractions.Init(CaptureCInteractions, (_s1, _v1) => _v1?.serialize(_s1));
                            break;
                        case WHICH.EQtlLink:
                            writer.EQtlLink.Init(EQtlLink, (_s1, _v1) => _v1?.serialize(_s1));
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

                public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>> ChromatinInteractionPredictions
                {
                    get => _which == WHICH.ChromatinInteractionPredictions ? (IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>>)_content : null;
                    set
                    {
                        _which = WHICH.ChromatinInteractionPredictions;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>> SignificantHiCInteractions
                {
                    get => _which == WHICH.SignificantHiCInteractions ? (IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>>)_content : null;
                    set
                    {
                        _which = WHICH.SignificantHiCInteractions;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>> ChIAPetInteractions
                {
                    get => _which == WHICH.ChIAPetInteractions ? (IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>>)_content : null;
                    set
                    {
                        _which = WHICH.ChIAPetInteractions;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>> CaptureCInteractions
                {
                    get => _which == WHICH.CaptureCInteractions ? (IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>>)_content : null;
                    set
                    {
                        _which = WHICH.CaptureCInteractions;
                        _content = value;
                    }
                }

                public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>> EQtlLink
                {
                    get => _which == WHICH.EQtlLink ? (IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>>)_content : null;
                    set
                    {
                        _which = WHICH.EQtlLink;
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
                    public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>.READER> ChromatinInteractionPredictions => which == WHICH.ChromatinInteractionPredictions ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>.READER> SignificantHiCInteractions => which == WHICH.SignificantHiCInteractions ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>.READER> ChIAPetInteractions => which == WHICH.ChIAPetInteractions ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>.READER> CaptureCInteractions => which == WHICH.CaptureCInteractions ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>.READER.create) : default;
                    public IReadOnlyList<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>.READER> EQtlLink => which == WHICH.EQtlLink ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>.READER.create) : default;
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

                    public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>.WRITER> ChromatinInteractionPredictions
                    {
                        get => which == WHICH.ChromatinInteractionPredictions ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChromatinInteractionPredictions>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>.WRITER> SignificantHiCInteractions
                    {
                        get => which == WHICH.SignificantHiCInteractions ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.SignificantHiCInteractions>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>.WRITER> ChIAPetInteractions
                    {
                        get => which == WHICH.ChIAPetInteractions ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.ChIAPetInteractions>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>.WRITER> CaptureCInteractions
                    {
                        get => which == WHICH.CaptureCInteractions ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.CaptureCInteractions>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }

                    public ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>.WRITER> EQtlLink
                    {
                        get => which == WHICH.EQtlLink ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.ConnectionSet.Connection<CapnpGen.Chromosome.ConnectionSet.EQtlLink>.WRITER>>(1) : default;
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xb689b3a89610a72dUL)]
            public class Location : ICapnpSerializable
            {
                public const UInt64 typeId = 0xb689b3a89610a72dUL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Start = CapnpSerializable.Create<CapnpGen.Chromosome.BinRange>(reader.Start);
                    End = CapnpSerializable.Create<CapnpGen.Chromosome.BinRange>(reader.End);
                    applyDefaults();
                }

                public void serialize(WRITER writer)
                {
                    Start?.serialize(writer.Start);
                    End?.serialize(writer.End);
                }

                void ICapnpSerializable.Serialize(SerializerState arg_)
                {
                    serialize(arg_.Rewrap<WRITER>());
                }

                public void applyDefaults()
                {
                }

                public CapnpGen.Chromosome.BinRange Start
                {
                    get;
                    set;
                }

                public CapnpGen.Chromosome.BinRange End
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
                    public CapnpGen.Chromosome.BinRange.READER Start => ctx.ReadStruct(0, CapnpGen.Chromosome.BinRange.READER.create);
                    public CapnpGen.Chromosome.BinRange.READER End => ctx.ReadStruct(1, CapnpGen.Chromosome.BinRange.READER.create);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public CapnpGen.Chromosome.BinRange.WRITER Start
                    {
                        get => BuildPointer<CapnpGen.Chromosome.BinRange.WRITER>(0);
                        set => Link(0, value);
                    }

                    public CapnpGen.Chromosome.BinRange.WRITER End
                    {
                        get => BuildPointer<CapnpGen.Chromosome.BinRange.WRITER>(1);
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xc1ade6784ae88567UL)]
            public class Connection<TExtraInfo> : ICapnpSerializable where TExtraInfo : class
            {
                public const UInt64 typeId = 0xc1ade6784ae88567UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Location = CapnpSerializable.Create<CapnpGen.Chromosome.ConnectionSet.Location>(reader.Location);
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

                public CapnpGen.Chromosome.ConnectionSet.Location Location
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
                    public CapnpGen.Chromosome.ConnectionSet.Location.READER Location => ctx.ReadStruct(0, CapnpGen.Chromosome.ConnectionSet.Location.READER.create);
                    public DeserializerState ExtraInfo => ctx.StructReadPointer(1);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public CapnpGen.Chromosome.ConnectionSet.Location.WRITER Location
                    {
                        get => BuildPointer<CapnpGen.Chromosome.ConnectionSet.Location.WRITER>(0);
                        set => Link(0, value);
                    }

                    public DynamicSerializerState ExtraInfo
                    {
                        get => BuildPointer<DynamicSerializerState>(1);
                        set => Link(1, value);
                    }
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x8c7dd3ec0a8c037dUL)]
            public class ChromatinInteractionPredictions : ICapnpSerializable
            {
                public const UInt64 typeId = 0x8c7dd3ec0a8c037dUL;
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x8a3d693ae03aa4b7UL)]
            public class SignificantHiCInteractions : ICapnpSerializable
            {
                public const UInt64 typeId = 0x8a3d693ae03aa4b7UL;
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xba064b63c7e54053UL)]
            public class ChIAPetInteractions : ICapnpSerializable
            {
                public const UInt64 typeId = 0xba064b63c7e54053UL;
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xd78ea5051bb4609dUL)]
            public class CaptureCInteractions : ICapnpSerializable
            {
                public const UInt64 typeId = 0xd78ea5051bb4609dUL;
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

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xba628493b2076538UL)]
            public class EQtlLink : ICapnpSerializable
            {
                public const UInt64 typeId = 0xba628493b2076538UL;
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
                    ProteinBinding = 0,
                    ChromatinAccessibility = 1,
                    GeneticVariants = 2,
                    undefined = 65535
                }

                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    switch (reader.which)
                    {
                        case WHICH.ProteinBinding:
                            ProteinBinding = reader.ProteinBinding?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>>(_));
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
                            case WHICH.ProteinBinding:
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
                        case WHICH.ProteinBinding:
                            writer.ProteinBinding.Init(ProteinBinding, (_s1, _v1) => _v1?.serialize(_s1));
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

                public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>> ProteinBinding
                {
                    get => _which == WHICH.ProteinBinding ? (IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>>)_content : null;
                    set
                    {
                        _which = WHICH.ProteinBinding;
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
                    public IReadOnlyList<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.READER> ProteinBinding => which == WHICH.ProteinBinding ? ctx.ReadList(1).Cast(CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.READER.create) : default;
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

                    public ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.WRITER> ProteinBinding
                    {
                        get => which == WHICH.ProteinBinding ? BuildPointer<ListOfStructsSerializer<CapnpGen.Chromosome.SiteSet.Site<CapnpGen.Chromosome.SiteSet.ProteinBinding>.WRITER>>(1) : default;
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
                    Location = CapnpSerializable.Create<CapnpGen.Chromosome.BinRange>(reader.Location);
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

                public CapnpGen.Chromosome.BinRange Location
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
                    public CapnpGen.Chromosome.BinRange.READER Location => ctx.ReadStruct(0, CapnpGen.Chromosome.BinRange.READER.create);
                    public DeserializerState ExtraInfo => ctx.StructReadPointer(1);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(0, 2);
                    }

                    public CapnpGen.Chromosome.BinRange.WRITER Location
                    {
                        get => BuildPointer<CapnpGen.Chromosome.BinRange.WRITER>(0);
                        set => Link(0, value);
                    }

                    public DynamicSerializerState ExtraInfo
                    {
                        get => BuildPointer<DynamicSerializerState>(1);
                        set => Link(1, value);
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

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x88b5e7ad8566a9f0UL)]
        public class BinRange : ICapnpSerializable
        {
            public const UInt64 typeId = 0x88b5e7ad8566a9f0UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Lower = reader.Lower;
                Upper = reader.Upper;
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Lower = Lower;
                writer.Upper = Upper;
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public uint Lower
            {
                get;
                set;
            }

            public uint Upper
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
                public uint Lower => ctx.ReadDataUInt(0UL, 0U);
                public uint Upper => ctx.ReadDataUInt(32UL, 0U);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(1, 0);
                }

                public uint Lower
                {
                    get => this.ReadDataUInt(0UL, 0U);
                    set => this.WriteData(0UL, value, 0U);
                }

                public uint Upper
                {
                    get => this.ReadDataUInt(32UL, 0U);
                    set => this.WriteData(32UL, value, 0U);
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