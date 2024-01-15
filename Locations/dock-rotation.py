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

# Read data from the original CSV, calculate new coordinates, and store them
updated_data = []
with open('WOD_Docks.csv', mode='r') as file:
    reader = csv.reader(file)
    headers = next(reader)
    rotYaxis_index = headers.index('rotYaxis')

    # If terrainX and terrainY columns do not exist, add them
    if 'terrainX' not in headers:
        headers.append('terrainX')
    if 'terrainY' not in headers:
        headers.append('terrainY')

    terrainX_index = headers.index('terrainX')
    terrainY_index = headers.index('terrainY')

    updated_data.append(headers)

    for row in reader:
        try:
            rotYaxis = float(row[rotYaxis_index])
        except ValueError:
            continue

        terrainX, terrainY = calculate_coordinates(rotYaxis)

        # Update or add terrainX and terrainY in the row
        if len(row) <= terrainX_index:
            row.append(terrainX)
        else:
            row[terrainX_index] = terrainX

        if len(row) <= terrainY_index:
            row.append(terrainY)
        else:
            row[terrainY_index] = terrainY

        updated_data.append(row)

# Write the updated data back to the original CSV file
with open('WOD_Docks.csv', mode='w', newline='') as file:
    writer = csv.writer(file)
    writer.writerows(updated_data)

print("Original CSV file has been updated with 'terrainX' and 'terrainY' values.")
