import json
import os

def update_json_structure(data):
    """
    Recursively search and update the JSON data.
    """
    if isinstance(data, dict):
        for key, value in data.items():
            if isinstance(value, dict) or isinstance(value, list):
                update_json_structure(value)
    elif isinstance(data, list):
        for i, item in enumerate(data):
            # Check if the item matches the target structure
            if item == {"archive": 372, "record": 2, "frame": 0}:
                data[i] = {"archive": 2003, "record": 4, "frame": 0}
            else:
                update_json_structure(item)

def update_json_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        data = json.load(file)
    
    update_json_structure(data)  # Recursively search and update the JSON data

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

