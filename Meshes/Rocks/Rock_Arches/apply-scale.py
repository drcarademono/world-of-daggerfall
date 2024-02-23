import bpy
import os

def apply_scale_and_save():
    # Apply scale to all objects
    for obj in bpy.context.scene.objects:
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.select_all(action='DESELECT')
        obj.select_set(True)
        if obj.type == 'MESH':
            bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)

    # Save the blend file
    bpy.ops.wm.save_mainfile()

if __name__ == "__main__":
    apply_scale_and_save()
