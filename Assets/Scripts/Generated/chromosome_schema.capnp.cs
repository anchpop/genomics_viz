using Capnp;
using Capnp.Rpc;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CapnpGen
{
    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xebf0df3e5737c601UL)]
    public class ChromosomeInfo : ICapnpSerializable
    {
        public const UInt64 typeId = 0xebf0df3e5737c601UL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Category = reader.Category;
            Index = reader.Index;
            TheBackbone = CapnpSerializable.Create<CapnpGen.ChromosomeInfo.Backbone>(reader.TheBackbone);
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Category = Category;
            writer.Index = Index;
            TheBackbone?.serialize(writer.TheBackbone);
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public string Category
        {
            get;
            set;
        }

        public uint Index
        {
            get;
            set;
        }

        public CapnpGen.ChromosomeInfo.Backbone TheBackbone
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
            public string Category => ctx.ReadText(0, null);
            public uint Index => ctx.ReadDataUInt(0UL, 0U);
            public CapnpGen.ChromosomeInfo.Backbone.READER TheBackbone => ctx.ReadStruct(1, CapnpGen.ChromosomeInfo.Backbone.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(1, 2);
            }

            public string Category
            {
                get => this.ReadText(0, null);
                set => this.WriteText(0, value, null);
            }

            public uint Index
            {
                get => this.ReadDataUInt(0UL, 0U);
                set => this.WriteData(0UL, value, 0U);
            }

            public CapnpGen.ChromosomeInfo.Backbone.WRITER TheBackbone
            {
                get => BuildPointer<CapnpGen.ChromosomeInfo.Backbone.WRITER>(1);
                set => Link(1, value);
            }
        }

        [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xad5aaaf32b4a1a53UL)]
        public class Backbone : ICapnpSerializable
        {
            public const UInt64 typeId = 0xad5aaaf32b4a1a53UL;
            void ICapnpSerializable.Deserialize(DeserializerState arg_)
            {
                var reader = READER.create(arg_);
                Points = reader.Points?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.ChromosomeInfo.Backbone.Point>(_));
                applyDefaults();
            }

            public void serialize(WRITER writer)
            {
                writer.Points.Init(Points, (_s1, _v1) => _v1?.serialize(_s1));
            }

            void ICapnpSerializable.Serialize(SerializerState arg_)
            {
                serialize(arg_.Rewrap<WRITER>());
            }

            public void applyDefaults()
            {
            }

            public IReadOnlyList<CapnpGen.ChromosomeInfo.Backbone.Point> Points
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
                public IReadOnlyList<CapnpGen.ChromosomeInfo.Backbone.Point.READER> Points => ctx.ReadList(0).Cast(CapnpGen.ChromosomeInfo.Backbone.Point.READER.create);
            }

            public class WRITER : SerializerState
            {
                public WRITER()
                {
                    this.SetStruct(0, 1);
                }

                public ListOfStructsSerializer<CapnpGen.ChromosomeInfo.Backbone.Point.WRITER> Points
                {
                    get => BuildPointer<ListOfStructsSerializer<CapnpGen.ChromosomeInfo.Backbone.Point.WRITER>>(0);
                    set => Link(0, value);
                }
            }

            [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xe5e44b92f3a86f52UL)]
            public class Point : ICapnpSerializable
            {
                public const UInt64 typeId = 0xe5e44b92f3a86f52UL;
                void ICapnpSerializable.Deserialize(DeserializerState arg_)
                {
                    var reader = READER.create(arg_);
                    Coordinate = CapnpSerializable.Create<CapnpGen.ChromosomeInfo.Backbone.Point.Vec3>(reader.Coordinate);
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

                public CapnpGen.ChromosomeInfo.Backbone.Point.Vec3 Coordinate
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
                    public CapnpGen.ChromosomeInfo.Backbone.Point.Vec3.READER Coordinate => ctx.ReadStruct(0, CapnpGen.ChromosomeInfo.Backbone.Point.Vec3.READER.create);
                    public uint Bin => ctx.ReadDataUInt(0UL, 0U);
                }

                public class WRITER : SerializerState
                {
                    public WRITER()
                    {
                        this.SetStruct(1, 1);
                    }

                    public CapnpGen.ChromosomeInfo.Backbone.Point.Vec3.WRITER Coordinate
                    {
                        get => BuildPointer<CapnpGen.ChromosomeInfo.Backbone.Point.Vec3.WRITER>(0);
                        set => Link(0, value);
                    }

                    public uint Bin
                    {
                        get => this.ReadDataUInt(0UL, 0U);
                        set => this.WriteData(0UL, value, 0U);
                    }
                }

                [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0x885550384ae709e9UL)]
                public class Vec3 : ICapnpSerializable
                {
                    public const UInt64 typeId = 0x885550384ae709e9UL;
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
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("capnpc-csharp", "1.3.0.0"), TypeId(0xa0794aba009bb6cdUL)]
    public class ForRendering : ICapnpSerializable
    {
        public const UInt64 typeId = 0xa0794aba009bb6cdUL;
        void ICapnpSerializable.Deserialize(DeserializerState arg_)
        {
            var reader = READER.create(arg_);
            Infos = reader.Infos?.ToReadOnlyList(_ => CapnpSerializable.Create<CapnpGen.ChromosomeInfo>(_));
            applyDefaults();
        }

        public void serialize(WRITER writer)
        {
            writer.Infos.Init(Infos, (_s1, _v1) => _v1?.serialize(_s1));
        }

        void ICapnpSerializable.Serialize(SerializerState arg_)
        {
            serialize(arg_.Rewrap<WRITER>());
        }

        public void applyDefaults()
        {
        }

        public IReadOnlyList<CapnpGen.ChromosomeInfo> Infos
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
            public IReadOnlyList<CapnpGen.ChromosomeInfo.READER> Infos => ctx.ReadList(0).Cast(CapnpGen.ChromosomeInfo.READER.create);
        }

        public class WRITER : SerializerState
        {
            public WRITER()
            {
                this.SetStruct(0, 1);
            }

            public ListOfStructsSerializer<CapnpGen.ChromosomeInfo.WRITER> Infos
            {
                get => BuildPointer<ListOfStructsSerializer<CapnpGen.ChromosomeInfo.WRITER>>(0);
                set => Link(0, value);
            }
        }
    }
}