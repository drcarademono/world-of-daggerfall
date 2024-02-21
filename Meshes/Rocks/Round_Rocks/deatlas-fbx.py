import bpy
import sys
import bmesh  # Make sure to include this import

# Function to clear existing data
def reset_blend():
    bpy.ops.wm.read_factory_settings(use_empty=True)

# Function to load an FBX file
def load_fbx(fbx_path):
    bpy.ops.import_scene.fbx(filepath=fbx_path)

def de_atlas_object(obj):
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode='EDIT')
    mesh = bmesh.from_edit_mesh(obj.data)
    uv_layer = mesh.loops.layers.uv.active

    # Dictionary to keep track of UV islands and the faces belonging to them
    islands = {}

    # Function to check if a face is part of an existing island
    def face_in_island(face, island):
        for loop in face.loops:
            uv = loop[uv_layer].uv
            for other_face in island:
                for other_loop in other_face.loops:
                    other_uv = other_loop[uv_layer].uv
                    if uv == other_uv:
                        return True
        return False

    # Identify UV islands
    for face in mesh.faces:
        added = False
        for island in islands.values():
            if face_in_island(face, island):
                island.append(face)
                added = True
                break
        if not added:
            islands[len(islands)] = [face]

    # Create new materials and assign them to faces based on islands
    for island_index, island_faces in islands.items():
        mat_name = f"Material_{island_index}"
        if mat_name not in bpy.data.materials:
            mat = bpy.data.materials.new(name=mat_name)
        else:
            mat = bpy.data.materials[mat_name]
        obj.data.materials.append(mat)
        for face in island_faces:
            face.material_index = island_index

    bmesh.update_edit_mesh(obj.data)
    bpy.ops.object.mode_set(mode='OBJECT')

# Main logic
if __name__ == "__main__":
    # Parse the FBX file path from the arguments
    fbx_path = sys.argv[-1]

    # Reset Blender to a clean state
    reset_blend()

    # Load the FBX file
    load_fbx(fbx_path)

    # Assuming the imported object is the active object, adjust as necessary
    obj = bpy.context.selected_objects[0]

    # Apply the de-atlas process
    de_atlas_object(obj)

    # Save the modified object back to an FBX file
    # You might want to modify the file path to avoid overwriting the original file
    bpy.ops.export_scene.fbx(filepath=fbx_path.replace('.fbx', '.fbx'))

