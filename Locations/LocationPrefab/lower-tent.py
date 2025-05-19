#!/usr/bin/env python3
import xml.etree.ElementTree as ET
import glob
import os

DELTA = 0.558

for path in glob.glob("*.txt"):
    tree = ET.parse(path)
    root = tree.getroot()
    updated = False

    # find all <object> elements
    for obj in root.findall("object"):
        oid = obj.find("name")
        if oid is not None and oid.text.strip() == "41606":
            posY = obj.find("posY")
            if posY is not None:
                old = float(posY.text)
                new = old - DELTA
                posY.text = f"{new:.6f}"
                updated = True

    if updated:
        # backup original
        os.rename(path, path + ".bak")
        # write updated XML (no declaration, same encoding)
        tree.write(path, encoding="utf-8", xml_declaration=False)
        print(f"→ {path} updated (backup saved as {path}.bak)")

