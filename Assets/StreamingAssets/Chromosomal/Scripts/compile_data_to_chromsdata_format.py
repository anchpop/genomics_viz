import capnp
import chromosome_schema_capnp

def get_coordinates():
    with open("./Data/chr1/5kb/points_GM.txt", "r") as coordinates_file:
        def parse_line(line):
            [x, y, z] = map(float, line.split("\t"))
            
            vec3 = chromosome_schema_capnp.Chromosome.Backbone.Point.Vec3.new_message()
            vec3.x = x
            vec3.y = y 
            vec3.z = z 

            return vec3
        return list(map(parse_line, coordinates_file))
    


def get_bins():
    with open("./Data/chr1/5kb/coordinate_mapping.txt", "r") as bins_file:
        def parse_line(line):
            [bin, index] = map(int, line.split("\t"))
            return bin
        return list(map(parse_line, bins_file))

def make_point(d):
    (coordinate, bin) = d
    point = chromosome_schema_capnp.Chromosome.Backbone.Point.new_message()
    point.coordinate = coordinate
    point.bin = bin
    return point

coordinates = get_coordinates()
bins = get_bins()

assert len(coordinates) == len(bins)

backbone = chromosome_schema_capnp.Chromosome.Backbone.new_message()
backbone.points = list(map(make_point, zip(coordinates, bins)))


chromosome_info = chromosome_schema_capnp.Chromosome.new_message()
chromosome_info.backbone = backbone


chromosome_set = chromosome_schema_capnp.ChromosomeSet.new_message()
chromosome_set.name = "Human chromosomes"
chromosome_set.name = "From https://github.com/BDM-Lab/Hierarchical3DGenome/tree/master/output"
chromosome_set.chromosomes = [chromosome_info]

output = chromosome_set.to_bytes()
with open("./Output/info.chromsdata", "wb") as output_file:
    output_file.write(output)

"""
temp.value = 100
temp.unit = 'c'
print(temp)

temp_bytes = temp.to_bytes()
print(temp_bytes)

restored_temp = chromosome_schema_capnp.Temperature.from_bytes(temp_bytes)
print(restored_temp)
"""