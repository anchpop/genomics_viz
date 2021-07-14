from pathlib import Path
import capnp
from toolz import groupby
import pandas as pd

base_path = Path('../../')

schema_path = (base_path / Path('Schema/chromsdata.capnp')).resolve()
capnp.remove_import_hook()
chromosome_schema_capnp = capnp.load(str(schema_path))
SegmentSet = chromosome_schema_capnp.Chromosome.SegmentSet

def fst(x): return x[0]
def snd(x): return x[1]

def compile_text_to_binary():
    def get_coordinates(i):
        with open(base_path / Path(f"Data/chromosomes/chr{i}/5kb/points_GM.txt"), "r") as coordinates_file:
            def parse_line(line):
                [x, y, z] = map(float, line.split("\t"))
                
                vec3 = chromosome_schema_capnp.Chromosome.Point.Vec3.new_message()
                vec3.x = x
                vec3.y = y 
                vec3.z = z 

                return vec3
            return list(map(parse_line, coordinates_file))
        


    def get_bins(i):
        with open(base_path / Path(f"Data/chromosomes/chr{i}/5kb/coordinate_mapping.txt"), "r") as bins_file:
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
            genes_file.readline() # skip the first line
            def parse_line(line):
                [_, chromosome, start_bin, end_bin, direction, stat, name, id] = line.split("\t")
                chromosome = chromosome[3:]
                start_bin = int(start_bin)
                end_bin = int(end_bin)
                ascending = direction == "+"

                return chromosome, start_bin, end_bin, ascending, stat.strip(), name.strip(), id.strip()
            def to_message(info):
                chromosome, start_bin, end_bin, ascending, stat, name, id = info 
                location = chromosome_schema_capnp.Chromosome.BinRange.new_message()
                gene = SegmentSet.Gene.new_message()

                gene.ascending = ascending
                gene.stat = stat
                gene.name = name
                gene.id = id
                location.lower = start_bin
                location.upper = end_bin
            
                return chromosome, {'location': location, 'extraInfo': gene} # need to use a dictionary for generic types
            
            return {
                k: list(map(snd, v)) 
                for k, v 
                in groupby(
                    fst, 
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


    def get_site_sets():
        def read_bed_file(path):
            def parse_line(line):
                info = line.split('\t')
                chromosome = info[0]
                lower = int(info[1])
                upper = int(info[2])

                protein_binding = chromosome_schema_capnp.Chromosome.SiteSet.ProteinBinding.new_message()
                location = chromosome_schema_capnp.Chromosome.BinRange.new_message()
                location.lower = lower
                location.upper = upper

                return chromosome, {'location': location, 'extraInfo': protein_binding}

            with open(path, 'r') as bed:
                return {
                    k: list(map(snd, v)) 
                    for k, v 
                    in groupby(
                        fst, map(parse_line, bed)
                    ).items()
                }
        def get_set(bed): 
            path = base_path / Path("Data/Sites") / Path(bed + ".txt")
            data = read_bed_file(path)
            return data
        
        def make_set(chromosome, bed_name, data): 
            site_set = chromosome_schema_capnp.Chromosome.SiteSet.new_message()
            site_set.description.name = bed_name
            site_set.sites.proteinBinding = data[chromosome]

            return site_set 

        beds = {bed: get_set(bed) for bed in ['CTCF', 'GATA1', 'IDR_final_optimal']}
        chromosomes = set(chromosome for _, data in beds.items() for chromosome, _ in data.items())
        return {chromosome: [make_set(chromosome, bed_name, data) for bed_name, data in beds.items()] for chromosome in chromosomes}

    
    def get_connection_sets():
        chromatin_interaction_predictions_name = 'gm12878_imputed_dnase_rnaseq'
        chromatin_interaction_predictions_filename = f'{chromatin_interaction_predictions_name}.txt'
        def process_chromatin_interaction_predictions(data): 
            connection_set = chromosome_schema_capnp.Chromosome.ConnectionSet.new_message()

            def process_row(row): 
                start_range = chromosome_schema_capnp.Chromosome.BinRange.new_message()
                end_range = chromosome_schema_capnp.Chromosome.BinRange.new_message()

                start_range.lower = row['start_lower_bin']
                start_range.upper = row['start_upper_bin']
                end_range.lower = row['end_lower_bin']
                end_range.upper = row['end_upper_bin']

                location = chromosome_schema_capnp.Chromosome.ConnectionSet.Location.new_message()
                location.start = start_range
                location.end = end_range

                extra_info = chromosome_schema_capnp.Chromosome.ConnectionSet.ChromatinInteractionPredictions.new_message()

                return {'location': location, 'extraInfo': extra_info}

            rows = [process_row(row) for (index, row) in data.iterrows()]
            connection_set.connections.chromatinInteractionPredictions = rows
            connection_set.description.name = chromatin_interaction_predictions_name
            return connection_set

        path = base_path / Path("Data/Connections") / chromatin_interaction_predictions_filename
        chromatin_interaction_predictions = pd.read_csv(path, sep="\t")
        chromatin_interaction_predictions.columns = ["chromosome", "start_lower_bin", "start_upper_bin", "idk", "idk2", "end_lower_bin", "end_upper_bin", "idk3", "idk4", "idk5"]

        chromosome_predictions = chromatin_interaction_predictions.groupby(['chromosome'])
        
        return {chromosome: [process_chromatin_interaction_predictions(data)] for (chromosome, data) in chromosome_predictions}


    siteSets = get_site_sets()
    connectionSets = get_connection_sets()

    chromosome_set = chromosome_schema_capnp.ChromosomeSet.new_message()
    chromosome_set.description.name = "Human chromosomes"
    chromosome_set.description.description = "From https://github.com/BDM-Lab/Hierarchical3DGenome/tree/master/output"

    index = 1

    def createChromosome(index):
        coordinates = get_coordinates(index)
        bins = get_bins(index)
        segment_set = SegmentSet.new_message() 
        segment_set.segments.genes = get_genes()[str(index)]
        segment_set.description.name = "genes"

        assert len(coordinates) == len(bins)

        chromosome = chromosome_schema_capnp.Chromosome.new_message()
        chromosome.index.numbered = index
        chromosome.backbone = list(map(make_point, zip(coordinates, bins)))
        chromosome.segmentSets = [segment_set]
        chromosome.connectionSets = []
        chromosome.siteSets = siteSets[f'chr{index}']
        chromosome.connectionSets = connectionSets[f'chr{index}']

        return chromosome
    

    chromosome_set.chromosomes = [createChromosome(i) for i in range(1, 3)]

    output = chromosome_set.to_bytes()
    with open(base_path / Path("Output/info.chromsdata"), "wb") as output_file:
        output_file.write(output)
