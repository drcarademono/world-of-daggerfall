import json
import os

def update_specific_entries(data):
    # Update "rainforest" materials
    if "rainforest" in data:
        for material_type in ["defaultMaterials", "winterMaterials"]:
            if material_type in data["rainforest"]:
                data["rainforest"][material_type] = [{"archive": 372, "record": 2, "frame": 0} for _ in data["rainforest"][material_type]]

    # Update "desert" materials to ensure there are two elements
    if "desert" in data:
        default_material = {"archive": 2, "record": 3, "frame": 0}
        data["desert"]["defaultMaterials"] = [default_material, default_material]  # Set exactly two elements
        if "winterMaterials" in data["desert"]:  # Check and update winterMaterials if present
            data["desert"]["winterMaterials"] = [default_material for _ in range(2)]  # Ensure two elements

def update_json_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        data = json.load(file)
    
    update_specific_entries(data)  # Update specific entries in the JSON data

    with open(file_path, 'w', encoding='utf-8') as file:
        json.dump(data, file, indent=2)

def update_directory(directory):
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith(".json"):
                update_json_file(os.path.join(root, file))

# Update current directory and all subdirectories
update_directory(".")

print("JSON files have been updated.")

