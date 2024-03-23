import pandas as pd
import glob

# Pattern to match all files ending with 'Rocks.csv' in current and subdirectories
pattern = '**/*Rocks.csv'

# Use glob to find all files matching the pattern, including in subdirectories
files = glob.glob(pattern, recursive=True)

for file in files:
    # Load the CSV file
    df = pd.read_csv(file)
    
    # Change every value in the sink column to 1.0
    df['sink'] = 0.5
    
    # Save the modified DataFrame back to CSV
    df.to_csv(file, index=False)

    print(f"Modified file saved as: {file}")

