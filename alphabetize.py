import re
import json
import sys
import os

# Check if the JSON file name is provided as a command-line argument
if len(sys.argv) < 2:
    print("Please provide the JSON file name as a command-line argument.")
    sys.exit(1)

# Read the JSON data from the file
json_file = sys.argv[1]
with open(json_file, 'r') as f:
    data = json.load(f)

# Extracting the file paths
files = data['Files']

# Function to create a sorting key: a tuple of the text part and the numeric part of the filename
def sorting_key(file_path):
    filename = os.path.basename(file_path)  # Extracts the filename from the path
    parts = re.split(r'(\d+)', filename)
    numeric_parts = [int(part) if part.isdigit() else part.lower() for part in parts]
    return numeric_parts

# Sorting the files using the custom sorting key
files.sort(key=sorting_key)

# Writing the rearranged files back to the JSON
data['Files'] = files

# Writing the updated data back to the JSON file
with open(json_file, 'w') as f:
    json.dump(data, f, indent=4)

print(f"Files in {json_file} have been rearranged in alphabetical and numeric order by filename.")

