import os
import json

# Function to recursively reverse arrays in the JSON
def reverse_arrays(data):
    if isinstance(data, dict):
        for key, value in data.items():
            data[key] = reverse_arrays(value)
    elif isinstance(data, list):
        return list(reversed([reverse_arrays(item) for item in data]))
    return data

# Get the current directory
current_folder = os.getcwd()

# Loop through all files in the current directory
for filename in os.listdir(current_folder):
    # Check if the file is a JSON file
    if filename.endswith(".json"):
        filepath = os.path.join(current_folder, filename)
        
        # Open and load the JSON file
        with open(filepath, 'r') as file:
            data = json.load(file)
        
        # Reverse the arrays
        modified_data = reverse_arrays(data)
        
        # Save the modified JSON back to the file
        with open(filepath, 'w') as file:
            json.dump(modified_data, file, indent=4)

print("Finished processing all JSON files.")

