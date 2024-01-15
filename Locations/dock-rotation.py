import csv
import math

def calculate_coordinates(rotYaxis):
    # Handle special cases where the angle is a multiple of 45 degrees
    if rotYaxis % 45 == 0:
        if rotYaxis == 45 or rotYaxis == -315:
            return 127, 127  # Top right corner
        elif rotYaxis == 135 or rotYaxis == -225:
            return 0, 127   # Top left corner
        elif rotYaxis == 225 or rotYaxis == -135:
            return 0, 0     # Bottom left corner
        elif rotYaxis == 315 or rotYaxis == -45:
            return 127, 0   # Bottom right corner

    radius = 64
    center_x, center_y = 64, 64

    # Adjusting the angle since 0 degrees points downwards
    angle = math.radians(rotYaxis - 90)

    terrainX = round(center_x + radius * math.cos(angle))
    terrainY = round(center_y + radius * math.sin(angle))

    # Clamping values to stay within the 128x128 square
    terrainX = max(0, min(127, terrainX))
    terrainY = max(0, min(127, terrainY))

    return terrainX, terrainY

# Assuming the CSV file is named 'data.csv'
with open('WOD_Docks.csv', mode='r') as file:
    reader = csv.reader(file)
    headers = next(reader)  # Read the header row

    # Find the index of 'rotYaxis' in the header
    rotYaxis_index = headers.index('rotYaxis')

    for row in reader:
        try:
            # Extracting the 'rotYaxis' value using the found index
            rotYaxis = float(row[rotYaxis_index])
        except ValueError:
            # Skip rows with invalid 'rotYaxis' values
            continue

        terrainX, terrainY = calculate_coordinates(rotYaxis)
        print(f'rotYaxis: {rotYaxis}, terrainX: {terrainX}, terrainY: {terrainY}')

