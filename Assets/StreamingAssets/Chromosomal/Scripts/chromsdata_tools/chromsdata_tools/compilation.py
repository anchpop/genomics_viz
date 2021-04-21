from pathlib import Path
import capnp
from toolz import groupby

base_path = Path('../../')

schema_path = (base_path / Path('Schema/chromsdata.capnp')).resolve()
capnp.remove_import_hook()
chromosome_schema_capnp = capnp.load(str(schema_path))
SegmentSet = chromosome_schema_capnp.Chromosome.SegmentSet

def compile_text_to_binary():

    def get_coordinates():
        with open(base_path / Path("Data/chr1/5kb/points_GM.txt"), "r") as coordinates_file:
            def parse_line(line):
                [x, y, z] = map(float, line.split("\t"))
                
                vec3 = chromosome_schema_capnp.Chromosome.Point.Vec3.new_message()
                vec3.x = x
                vec3.y = y 
                vec3.z = z 

                return vec3
            return list(map(parse_line, coordinates_file))
        


    def get_bins():
        with open(base_path / Path("Data/chr1/5kb/coordinate_mapping.txt"), "r") as bins_file:
            def parse_line(line):
                [bin, index] = map(int, line.split("\t"))
                return bin
            return list(map(parse_line, bins_file))

    def make_point(d):
        (coordinate, bin) = d
        point = chromosome_schema_capnp.Chromosome.Point.new_message()
        point.coordinate = coordinate
        point.bin = bin
        return point

    def get_genes():
        with open(base_path / Path("Data/gene_annotation_V19.txt"), "r") as genes_file:
            genes_file.readline() # skip the first line - only specifies the 
            def parse_line(line):
                [_, chromosome, start_bin, end_bin, direction, stat, name, id] = line.split("\t")
                chromosome = chromosome[3:]
                start_bin = int(start_bin)
                end_bin = int(end_bin)
                ascending = direction == "+"

                return chromosome, start_bin, end_bin, ascending, stat.strip(), name.strip(), id.strip()
            def to_message(info):
                chromosome, start_bin, end_bin, ascending, stat, name, id = info 
                location = SegmentSet.Location.new_message()
                gene = SegmentSet.Gene.new_message()

                gene.ascending = ascending
                gene.stat = stat
                gene.name = name
                gene.id = id
                location.startBin = start_bin
                location.endBin = end_bin

                #segment = SegmentSet.Segment.new_message(location=location, extraInfo=gene)

                return chromosome, {'location': location, 'extraInfo': gene} # need to use a struct for generic types
                
            return {
                k: list(map(lambda segment: segment[1], v)) 
                for k, v 
                in groupby(
                    lambda segment: segment[0], 
                    map(
                        to_message, 
                        map(
                            parse_line, 
                            filter(
                                lambda line: line != "", 
                                genes_file)
                                )
                            )
                        ).items()
                    }


    index = 1
    coordinates = get_coordinates()
    bins = get_bins()
    segment_set = SegmentSet.new_message() 
    segment_set.segments.genes = get_genes()[str(index)]
    segment_set.description.name = "genes"

    assert len(coordinates) == len(bins)

    chromosome = chromosome_schema_capnp.Chromosome.new_message()
    chromosome.index.numbered = index
    chromosome.backbone = list(map(make_point, zip(coordinates, bins)))
    chromosome.segmentSets = [segment_set]
    chromosome.connectionSets = []


    chromosome_set = chromosome_schema_capnp.ChromosomeSet.new_message()
    chromosome_set.description.name = "Human chromosomes"
    chromosome_set.description.description = "From https://github.com/BDM-Lab/Hierarchical3DGenome/tree/master/output"
    chromosome_set.chromosomes = [chromosome]

    output = chromosome_set.to_bytes()
    with open(base_path / Path("Output/info.chromsdata"), "wb") as output_file:
        output_file.write(output)
