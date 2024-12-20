import json
import os

# Use the current directory
folder_path = os.getcwd()
mountain_1_path = os.path.join(folder_path, "Mountain_01.json")

# Load Mountain_1.json
def load_json(file_path):
    with open(file_path, 'r') as file:
        return json.load(file)

def save_json(data, file_path):
    with open(file_path, 'w') as file:
        json.dump(data, file, indent=4)

mountain_1_data = load_json(mountain_1_path)

# Iterate through all JSON files in the folder
for filename in os.listdir(folder_path):
    if filename.endswith(".json") and filename != "Mountain_1.json":
        file_path = os.path.join(folder_path, filename)

        # Load the current JSON file
        current_data = load_json(file_path)

        # Update the JSON file with second elements from Mountain_1.json
        for key, value in mountain_1_data.items():
            if key in current_data:
                for material_type in ["defaultMaterials", "winterMaterials"]:
                    if material_type in value:
                        # Ensure current_data has the material_type
                        if material_type not in current_data[key]:
                            current_data[key][material_type] = []

                        # Append second element from Mountain_1.json if it exists
                        if len(value[material_type]) > 1:
                            second_element = value[material_type][1]

                            # Ensure current_data has at least one element
                            if len(current_data[key][material_type]) == 0:
                                current_data[key][material_type].append(second_element)
                            elif len(current_data[key][material_type]) == 1:
                                current_data[key][material_type].append(second_element)
                            else:
                                # Replace the second element if it already exists
                                current_data[key][material_type][1] = second_element

        # Save the updated JSON file
        save_json(current_data, file_path)

print("All JSON files updated successfully.")

