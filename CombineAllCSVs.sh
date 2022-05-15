mkdir -p CombinedCSVs

python CombineCSVs.py *_Bandits.csv CombinedCSVs/Bandits.csv
python CombineCSVs.py *_Bandits_Roadside.csv CombinedCSVs/Bandits_Roadside.csv
python CombineCSVs.py *_Border_Towers.csv CombinedCSVs/Border_Towers.csv
python CombineCSVs.py *_Guardtowers_Roadside.csv CombinedCSVs/Guardtowers_Roadside.csv
python CombineCSVs.py *_Rocks.csv CombinedCSVs/Rocks.csv
python CombineCSVs.py *_Ruins.csv CombinedCSVs/Ruins.csv
python CombineCSVs.py *_Ruins_Roadside.csv CombinedCSVs/Ruins_Roadside.csv
python CombineCSVs.py *_Shrines.csv CombinedCSVs/Shrines.csv
python CombineCSVs.py *_Shrines_Roadside.csv CombinedCSVs/Shrines_Roadside.csv
