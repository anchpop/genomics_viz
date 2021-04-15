@0xd979abd3bf554e38;

struct ChromosomeInfo {
  	category @0 :Text;
	index @1 :UInt32;
	backbone @2 :Backbone;

	struct Backbone {
		points @0 :List(Point);

		struct Point {
			coordinate @0 :Vec3;
			bin @1 :UInt32;

			struct Vec3 {
				x @0 :Float32;
				y @1 :Float32;
				z @2 :Float32;
			}
		}
	}
}

struct ForRendering {
	infos @0 :List(ChromosomeInfo);
}

capnproto 