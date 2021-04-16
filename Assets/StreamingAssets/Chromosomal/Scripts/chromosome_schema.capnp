@0xd979abd3bf554e38;

struct Chromosome {
	# Contains all the relevant information about a single chromosome.

	backbone @0 :Backbone;
	struct Backbone {
		description @0 :Text;

		points @1 :List(Point);

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

	segmentSets @1 :List(SegmentSet);
	# Used for storing things like genes, GWAS data, etc.

	struct SegmentSet {
		name @0 :Text;
		description @1 :Text;
		kind :union {
    		genes @2 :Void;
			mutations @3 :Void;
		}

		segments @4 :List(Segment);
	}
	struct Segment {
		info @0 :Text;
		startBin @1 :UInt32;
		endBin @2 :UInt32;
	}

	connectionSets @2 :List(ConnectionSet);
	struct ConnectionSet {
		name @0 :Text;
		description @1 :Text;
		connections @2 :List(Connection);
	}
	struct Connection {
		startBinLower @0 :UInt32;
		startBinUpper @1 :UInt32;

		endBinLower @2 :UInt32;
		endBinUpper @3 :UInt32;
	} 
}

struct ChromosomeSet {
	name @0 :Text;
	description @1 :Text;
	chromosomes @2 :List(Chromosome);
}
