import bpy

# Override color management settings
bpy.context.scene.view_settings.view_transform = 'Standard'
bpy.context.scene.view_settings.look = 'None'

# Iterate through selected objects
for obj in bpy.context.selected_objects:
    # Ensure the object is a mesh
    if obj.type == 'MESH':
        # Iterate through the UV maps
        for uvmap in obj.data.uv_layers:
            # Iterate through the UV map data
            for data in uvmap.data:
                # Scale each UV coordinate
                data.uv[0] *= 2  # Scale U coordinate
                data.uv[1] *= 2  # Scale V coordinate

# Save the modified file
bpy.ops.wm.save_as_mainfile(filepath=bpy.data.filepath)

