# -*- coding: utf-8 -*-
"""Génère Notes/GDD/data/GDD_balance.xlsx (stdlib only, pas openpyxl)."""
from __future__ import annotations

import csv
import zipfile
from pathlib import Path
from xml.sax.saxutils import escape

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / "Notes" / "GDD" / "data"
XLSX = DATA / "GDD_balance.xlsx"


def _inline(text: str) -> str:
    return f'<c t="inlineStr"><is><t xml:space="preserve">{escape(str(text))}</t></is></c>'


def _number(value: str) -> str:
    if value == "":
        return "<c/>"
    return f'<c t="n"><v>{escape(value)}</v></c>'


def _row(r: int, cells: list[str]) -> str:
    return f'<row r="{r}">{"".join(cells)}</row>'


def sheet_xml(rows: list[list[tuple[str, str]]]) -> str:
    """rows = list of (kind, value) with kind in {s, n}."""
    xml_rows = []
    for i, row in enumerate(rows, start=1):
        cells = []
        for kind, value in row:
            cells.append(_inline(value) if kind == "s" else _number(value))
        xml_rows.append(_row(i, cells))
    body = "".join(xml_rows)
    return (
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
        '<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">'
        f"<sheetData>{body}</sheetData></worksheet>"
    )


def load_csv(name: str) -> list[list[tuple[str, str]]]:
    path = DATA / name
    out: list[list[tuple[str, str]]] = []
    with path.open(encoding="utf-8-sig", newline="") as handle:
        reader = csv.reader(handle, delimiter=";")
        header = next(reader)
        out.append([("s", col) for col in header])
        numeric_idx = {
            i for i, col in enumerate(header) if col.endswith("Seconds") or col.endswith("Minutes")
            or col in ("harvestLeafMin", "harvestLeafMax", "harvestSeedMin", "harvestSeedMax",
                       "sellLeafGold", "seedBuyGold", "footprintCells", "stageEnum")
        }
        for raw in reader:
            row = []
            for i, col in enumerate(raw):
                if i in numeric_idx and col != "":
                    row.append(("n", col))
                else:
                    row.append(("s", col))
            out.append(row)
    return out


def main() -> None:
    DATA.mkdir(parents=True, exist_ok=True)
    sheets = [
        ("LIRE", load_csv("GDD_lire.csv")),
        ("retention", load_csv("GDD_retention.csv")),
        ("plantes", load_csv("GDD_plantes.csv")),
        ("stades_timers", load_csv("GDD_stades_timers.csv")),
        ("recolte", load_csv("GDD_recolte.csv")),
    ]

    ns_pkg = "http://schemas.openxmlformats.org/package/2006/relationships"
    ns_od = "http://schemas.openxmlformats.org/officeDocument/2006/relationships"
    ns_ct = "http://schemas.openxmlformats.org/package/2006/content-types"

    workbook_sheets = []
    workbook_rels = []
    content_overrides = [
        f'<Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>'
    ]
    files: dict[str, str] = {}

    for idx, (name, rows) in enumerate(sheets, start=1):
        files[f"xl/worksheets/sheet{idx}.xml"] = sheet_xml(rows)
        workbook_sheets.append(
            f'<sheet name="{escape(name)}" sheetId="{idx}" r:id="rId{idx}"/>'
        )
        workbook_rels.append(
            f'<Relationship Id="rId{idx}" Type="{ns_od}/worksheet" Target="worksheets/sheet{idx}.xml"/>'
        )
        content_overrides.append(
            f'<Override PartName="/xl/worksheets/sheet{idx}.xml" '
            'ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>'
        )

    files["[Content_Types].xml"] = (
        f'<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
        f'<Types xmlns="{ns_ct}">'
        '<Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>'
        '<Default Extension="xml" ContentType="application/xml"/>'
        + "".join(content_overrides)
        + "</Types>"
    )
    files["_rels/.rels"] = (
        f'<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
        f'<Relationships xmlns="{ns_pkg}">'
        f'<Relationship Id="rId1" Type="{ns_od}/officeDocument" Target="xl/workbook.xml"/>'
        "</Relationships>"
    )
    files["xl/workbook.xml"] = (
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
        '<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" '
        'xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">'
        f'<sheets>{"".join(workbook_sheets)}</sheets></workbook>'
    )
    files["xl/_rels/workbook.xml.rels"] = (
        f'<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
        f'<Relationships xmlns="{ns_pkg}">'
        + "".join(workbook_rels)
        + "</Relationships>"
    )

    if XLSX.exists():
        XLSX.unlink()
    with zipfile.ZipFile(XLSX, "w", compression=zipfile.ZIP_DEFLATED) as zf:
        for name, content in files.items():
            zf.writestr(name, content.encode("utf-8"))
    print(f"Wrote {XLSX}")


if __name__ == "__main__":
    main()
