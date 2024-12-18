import json

# Input JSON file and output file paths
input_file = "WorldOfDaggerfall.dfmod.json"  # Replace with the actual file path
output_file = "WorldOfDaggerfall.dfmod.json"

# Load JSON data
with open(input_file, 'r') as file:
    data = json.load(file)

# Filter lines that do not contain "LoggingCamp"
filtered_files = [item for item in data["Files"] if "LoggingCamp" not in item]

# Replace the "Files" key with the filtered list
data["Files"] = filtered_files

# Write back the modified JSON
with open(output_file, 'w') as file:
    json.dump(data, file, indent=4)

print(f"Filtered JSON saved to {output_file}")

