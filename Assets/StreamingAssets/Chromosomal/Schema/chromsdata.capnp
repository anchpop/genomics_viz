@0xc14c1fa98840cbb7;

struct Chromosome {
	# Contains all the relevant information about a single chromosome.
	index :union {
		numbered @0 :UInt32;
		x @1 :Void;
		y @2 :Void;
	}

	backbone @3 :List(Point);

	struct Point {
		coordinate @0 :Vec3;
		bin @1 :UInt32;

		struct Vec3 {
			x @0 :Float32;
			y @1 :Float32;
			z @2 :Float32;
		}
	}

	segmentSets @4 :List(SegmentSet);
	# Used for storing things like genes, GWAS data, etc.

	struct SegmentSet {
		name @0 :Text;
		description @1 :Text;
		segments :union {
			geneSegments @2 :List(GeneSegment);
			otherSegments @3 :List(OtherSegment);
		}

		struct GeneSegment {
			ascending @0 :Bool;
			stat @1 :Text;
			name @2 :Text;
			id @3 :Text;
			segmentInfo @4 :SegmentInfo;
		}
		struct OtherSegment {
			info @0 :Text;
			segmentInfo @1 :SegmentInfo;
		}
		struct SegmentInfo {
			startBin @0 :UInt32;
			endBin @1 :UInt32;
		}
	}

	connectionSets @5 :List(ConnectionSet);
	# Used for storing things like chromatid interaction predictions.

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
