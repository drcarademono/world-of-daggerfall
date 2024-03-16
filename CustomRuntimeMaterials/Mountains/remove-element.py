import os
import json

# Function to remove the second element from each array in the given data
def remove_second_element(data):
    if isinstance(data, dict):
        for key, value in data.items():
            data[key] = remove_second_element(value)
    elif isinstance(data, list) and len(data) > 1:
        del data[1]
    return data

# Iterate over all files in the current directory
for filename in os.listdir('.'):
    # Check if the file is a JSON file
    if filename.endswith('.json'):
        # Read the JSON file
        with open(filename, 'r', encoding='utf-8') as file:
            data = json.load(file)
        
        # Modify the content by removing the second element from each array
        modified_data = remove_second_element(data)
        
        # Write the modified content back to the JSON file
        with open(filename, 'w', encoding='utf-8') as file:
            json.dump(modified_data, file, indent=4)

print("Processing complete.")

