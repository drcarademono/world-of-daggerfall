import pandas as pd
from pathlib import Path

# Load the CSV file into a DataFrame
df = pd.read_csv('Locations.csv')

# Define the formulas for gisX and gisY
df['gisX'] = df['worldX'] + (df['terrainX'] / 128)
df['gisY'] = -(df['worldY']) - (1 - (df['terrainY'] / 128))

# Split the DataFrame by the 'name' column and save each subset into a new CSV file
output_directory = Path('.')
output_directory.mkdir(exist_ok=True)

for name, group in df.groupby('name'):
    filename = f"{name}.csv"
    group.to_csv(output_directory / filename, index=False)
    print(f"File saved: {filename}")

print("All files have been split and saved in the current directory.")

