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
		# The bin is the lowest "base pair index" that this point in the 3D backbone corresponds to.  

		struct Vec3 {
			x @0 :Float32;
			y @1 :Float32;
			z @2 :Float32;
		}
	}

	segmentSets @4 :List(SegmentSet);
	# Used for storing things like genes, GWAS data, etc.

	struct SegmentSet {
		description @0 :Description;
		segments :union {
			genes @1 :List(Segment(Gene));
			others @2 :List(Segment(ChromatinState));
		}

		struct Segment(ExtraInfo) {
			location @0 :Location;
			extraInfo @1 :ExtraInfo;
		}
		
		struct Location {
			startBin @0 :UInt32;
			endBin @1 :UInt32;
		}
		
		struct Gene {
			ascending @0 :Bool;
			stat @1 :Text;
			name @2 :Text;
			id @3 :Text;
		}
		struct ChromatinState {
			info @0 :Text;
		}
	}


	connectionSets @5 :List(ConnectionSet);
	# Used for storing things like chromatid interaction predictions.

	struct ConnectionSet {
		description @0 :Description;
		connections @1 :List(Connection);
		
		struct Connection {
			startBinLower @0 :UInt32;
			startBinUpper @1 :UInt32;

			endBinLower @2 :UInt32;
			endBinUpper @3 :UInt32;
		} 
	}

	siteSets @6 :List(SiteSet);
	struct SiteSet {
		description @0 :Description;
		sites :union {
			proteinBinding @1 :List(Site(ProteinBinding));
			chromatinAccessibility @2 :List(Site(ChromatinAccessibility));
			geneticVariants @3 :List(Site(GeneticVariants));
		}
		
		struct Site(ExtraInfo) {
			location @0 :Location;
			extraInfo @1 :ExtraInfo;
		}

		struct Location {
			binLower @0 :UInt32;
			binUpper @1 :UInt32;
		} 

		struct ProteinBinding {

		}
		struct ChromatinAccessibility {

		}
		struct GeneticVariants {

		}
	}

}

struct ChromosomeSet {
	description @0 :Description;
	chromosomes @1 :List(Chromosome);
}

struct Description {
	name @0 :Text;
	description @1 :Text;
}