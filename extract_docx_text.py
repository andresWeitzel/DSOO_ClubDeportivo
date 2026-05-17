import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path

path = Path('doc/definiciones_club_deportivo/analisis_club_deportivo.docx')
with zipfile.ZipFile(path) as z:
    xml = z.read('word/document.xml')
root = ET.fromstring(xml)
ns = {'w': 'http://schemas.openxmlformats.org/wordprocessingml/2006/main'}
lines = []
for p in root.iterfind('.//w:p', ns):
    text = ''.join(node.text or '' for node in p.iterfind('.//w:t', ns))
    if text.strip():
        lines.append(text.strip())
print('\n'.join(lines[:200]))
