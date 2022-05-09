import sys
import os
import glob
import pandas as pd
import csv

usage = f"Usage: python {sys.argv[0]} <format> <output>" 

args = sys.argv[1:]
if not args:
    raise SystemExit(usage)

if len(args) < 2:
    raise SystemExit(usage)

if args[0] == "--help":
    raise SystemExit(usage)

pattern = args[0]
output = args[1]

print(f"Starting pattern {pattern}...")

path = os.path.join(os.getcwd(), "Locations", "**", pattern)

files = glob.glob(path, recursive=True)

csvs = []
for f in files:
    print(os.path.basename(f))
    csvs.append(pd.read_csv(f, quotechar='\0'))

combined_csv = pd.concat(csvs)
combined_csv.to_csv(output, index=False, encoding="utf-8-sig", quoting=csv.QUOTE_NONE)