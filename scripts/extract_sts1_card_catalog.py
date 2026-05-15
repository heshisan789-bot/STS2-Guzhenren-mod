from __future__ import annotations

import json
import re
from pathlib import Path


ROOT = Path("/Users/ryan/Documents/stsmods/guzhenren")
CARDS_DIR = ROOT / "other-lib/StS_Mod_GuZhenRen/src/main/java/GuZhenRen/cards"
CARD_JSON = ROOT / "other-lib/StS_Mod_GuZhenRen/src/main/resources/GuZhenRen/localization/zhs/CardStrings.json"
OUT_PATH = ROOT / "docs/sts1-card-effect-catalog.md"

DAO_ORDER = [
    ("GUANG_DAO", "光道"),
    ("YAN_DAO", "炎道"),
    ("LI_DAO", "力道"),
    ("JIN_DAO", "金道"),
    ("TOU_DAO", "偷道"),
    ("MU_DAO", "木道"),
    ("SHI_DAO", "食道"),
    ("SHA_DAO", "杀道"),
    ("GU_DAO", "骨道"),
    ("LU_DAO", "律道"),
    ("ZHI_DAO", "智道"),
    ("BIAN_HUA_DAO", "变化道"),
    ("YIN_YANG_DAO", "阴阳道"),
    ("JIAN_DAO", "剑道"),
    ("XUE_DAO", "血道"),
    ("YUN_DAO", "运道"),
    ("FENG_DAO", "风道"),
    ("ZHOU_DAO", "宙道"),
    ("NONE", "无道/特殊"),
]

DAO_MAP = dict(DAO_ORDER)
RARITY_MAP = {
    "BASIC": "基础",
    "COMMON": "普通",
    "UNCOMMON": "罕见",
    "RARE": "稀有",
    "SPECIAL": "特殊",
    "CURSE": "诅咒",
}
VALID_EXTENDS = {
    "AbstractGuZhenRenCard",
    "AbstractBenMingGuCard",
    "AbstractShaZhaoCard",
    "AbstractXuYingCard",
    "AbstractXianGuWuCard",
    "CustomCard",
}


def find_matching(text: str, start: int, open_ch: str, close_ch: str) -> int:
    depth = 0
    in_string: str | None = None
    escape = False
    for idx in range(start, len(text)):
        char = text[idx]
        if in_string:
            if escape:
                escape = False
            elif char == "\\":
                escape = True
            elif char == in_string:
                in_string = None
            continue

        if char in {"'", '"'}:
            in_string = char
        elif char == open_ch:
            depth += 1
        elif char == close_ch:
            depth -= 1
            if depth == 0:
                return idx
    raise ValueError(f"Unmatched {open_ch}{close_ch}")


def split_top_level(text: str) -> list[str]:
    parts: list[str] = []
    current: list[str] = []
    paren = bracket = brace = 0
    in_string: str | None = None
    escape = False

    for char in text:
        if in_string:
            current.append(char)
            if escape:
                escape = False
            elif char == "\\":
                escape = True
            elif char == in_string:
                in_string = None
            continue

        if char in {"'", '"'}:
            in_string = char
        elif char == "(":
            paren += 1
        elif char == ")":
            paren -= 1
        elif char == "[":
            bracket += 1
        elif char == "]":
            bracket -= 1
        elif char == "{":
            brace += 1
        elif char == "}":
            brace -= 1
        elif char == "," and paren == bracket == brace == 0:
            parts.append("".join(current).strip())
            current = []
            continue
        current.append(char)

    if current:
        parts.append("".join(current).strip())
    return parts


def parse_constants(text: str) -> dict[str, str]:
    return {
        name: value.strip()
        for name, value in re.findall(
            r"private\s+static\s+final\s+(?:int|float|boolean|String)\s+(\w+)\s*=\s*([^;]+);",
            text,
        )
    }


def resolve_token(token: str, constants: dict[str, str]) -> str:
    token = token.strip()
    if token in constants:
        return resolve_token(constants[token], constants)
    if token.startswith("CardRarity."):
        return RARITY_MAP.get(token.split(".")[-1], token)
    if token.startswith("Dao."):
        return DAO_MAP.get(token.split(".")[-1], token)
    if token.startswith("GuZhenRenTags."):
        suffix = token.split(".")[-1]
        if suffix.endswith("_DAO"):
            return DAO_MAP.get(suffix, suffix)
    if token.startswith('"') and token.endswith('"'):
        return token[1:-1]
    if re.fullmatch(r"-?\d+", token):
        return token
    return token


def normalize_effect(text: str) -> str:
    text = text.replace(" NL ", "<br>").replace(" NL", "<br>").replace("NL ", "<br>")
    text = text.replace("guzhenren:", "").replace("stslib:", "")
    text = re.sub(r"\s+", " ", text).strip()
    return text


def iter_classes(file_text: str):
    pattern = re.compile(
        r"(?m)^(\s*)(public\s+)?(static\s+)?(abstract\s+)?class\s+(\w+)\s+extends\s+(\w+)"
    )
    for match in pattern.finditer(file_text):
        name = match.group(5)
        extends = match.group(6)
        brace_start = file_text.find("{", match.end())
        brace_end = find_matching(file_text, brace_start, "{", "}")
        yield name, extends, match.group(4) is not None, file_text[brace_start + 1 : brace_end]


def parse_constructor(name: str, extends: str, body: str, file_constants: dict[str, str]) -> tuple[str, str]:
    constants = dict(file_constants)
    constants.update(parse_constants(body))
    ctor_match = re.search(r"public\s+" + re.escape(name) + r"\s*\([^)]*\)\s*\{", body)
    if not ctor_match:
        return "-", "-"

    ctor_start = ctor_match.end() - 1
    ctor_end = find_matching(body, ctor_start, "{", "}")
    ctor_body = body[ctor_start + 1 : ctor_end]
    super_index = ctor_body.find("super(")
    if super_index == -1:
        return "-", "-"
    paren_start = ctor_body.find("(", super_index)
    paren_end = find_matching(ctor_body, paren_start, "(", ")")
    args = split_top_level(ctor_body[paren_start + 1 : paren_end])

    if extends in {"AbstractGuZhenRenCard", "AbstractBenMingGuCard"} and len(args) >= 8:
        return resolve_token(args[3], constants), resolve_token(args[7], constants)
    if extends == "AbstractShaZhaoCard" and len(args) >= 4:
        return resolve_token(args[3], constants), "特殊"
    if extends == "AbstractXuYingCard" and len(args) >= 4:
        return resolve_token(args[3], constants), "特殊"
    if extends == "CustomCard" and len(args) >= 8:
        return resolve_token(args[3], constants), resolve_token(args[7], constants)
    return "-", "-"


def main() -> None:
    card_strings = json.loads(CARD_JSON.read_text())
    string_names = {key.split(":", 1)[1] for key in card_strings}
    entries: list[dict[str, object]] = []
    seen_classes: set[str] = set()

    for path in sorted(CARDS_DIR.glob("*.java")):
        file_text = path.read_text()
        file_constants = parse_constants(file_text)
        for name, extends, is_abstract, body in iter_classes(file_text):
            if is_abstract or name.startswith("Abstract") or extends not in VALID_EXTENDS:
                continue
            if name not in string_names:
                continue

            seen_classes.add(name)
            data = card_strings[f"GuZhenRen:{name}"]
            cost, rarity = parse_constructor(name, extends, body, file_constants)

            constants = dict(file_constants)
            constants.update(parse_constants(body))
            dao = "无道/特殊"
            dao_match = re.search(r"setDao\(Dao\.(\w+)\)", body)
            if dao_match:
                dao = DAO_MAP.get(dao_match.group(1), dao_match.group(1))
            else:
                tag_match = re.search(r"tags\.add\(GuZhenRenTags\.(\w+_DAO)\)", body)
                if tag_match:
                    dao = DAO_MAP.get(tag_match.group(1), tag_match.group(1))
                elif extends == "AbstractXuYingCard":
                    dao = "力道"

            rank = "0"
            rank_match = re.search(r"setRank\(([^)]+)\)", body)
            if rank_match:
                rank = resolve_token(rank_match.group(1), constants)

            benming = extends == "AbstractBenMingGuCard"
            shazhao = extends == "AbstractShaZhaoCard"

            display_name = data["NAME"]
            if benming:
                display_name += "（本命蛊）"
            if shazhao:
                display_name += "（杀招）"

            entries.append(
                {
                    "name": display_name,
                    "raw_name": data["NAME"],
                    "rank": rank,
                    "dao": dao,
                    "cost": cost,
                    "rarity": rarity,
                    "effect": normalize_effect(data.get("DESCRIPTION", "")),
                    "shazhao": shazhao,
                }
            )

    order_index = {name: idx for idx, (_, name) in enumerate(DAO_ORDER)}
    entries.sort(key=lambda item: (order_index.get(item["dao"], 999), item["raw_name"]))
    shazhao_entries = [item for item in entries if item["shazhao"]]
    missing_entries = sorted(string_names - seen_classes)

    OUT_PATH.parent.mkdir(parents=True, exist_ok=True)
    lines: list[str] = [
        "# STS1 蛊真人卡牌效果总表",
        "",
        "- 统计来源：`other-lib/StS_Mod_GuZhenRen` 的塔一源码与 `CardStrings.json` 中文文本",
        f"- 统计范围：共 {len(entries)} 张可定位到源码/内嵌类定义的卡牌，按“道”分组",
        "- 列顺序：名称、转、道、费用、稀有度、效果",
        "- 标记规则：本命蛊直接标在名称后；杀招也在名称后标明",
        "",
    ]

    for _, dao_name in DAO_ORDER:
        group = [item for item in entries if item["dao"] == dao_name]
        if not group:
            continue
        lines.extend(
            [
                f"## {dao_name}",
                "",
                "| 名称 | 转 | 道 | 费用 | 稀有度 | 效果 |",
                "| --- | --- | --- | --- | --- | --- |",
            ]
        )
        for item in group:
            effect = str(item["effect"]).replace("|", "\\|")
            lines.append(
                f"| {item['name']} | {item['rank']} | {item['dao']} | {item['cost']} | {item['rarity']} | {effect} |"
            )
        lines.append("")

    lines.extend(
        [
            "## 杀招总表",
            "",
            f"- 共 {len(shazhao_entries)} 张杀招。",
            "",
            "| 名称 | 转 | 道 | 费用 | 稀有度 | 效果 |",
            "| --- | --- | --- | --- | --- | --- |",
        ]
    )
    for item in shazhao_entries:
        effect = str(item["effect"]).replace("|", "\\|")
        lines.append(
            f"| {item['name']} | {item['rank']} | {item['dao']} | {item['cost']} | {item['rarity']} | {effect} |"
        )
    lines.extend(
        [
            "",
            "## 天劫地灾",
            "",
            "- 在塔一源码与中文本地化中，未检索到已实装的“天劫”“地灾”卡牌、Power、Action 或独立机制效果。",
            "- 目前只发现与“劫云”相关的风味文本引用，未形成可统计的机制表。",
            "",
        ]
    )

    if missing_entries:
        lines.extend(
            [
                "## 仅见本地化、未在 cards 目录定位到源码定义的条目",
                "",
                "- 这些条目仍保留在 `CardStrings.json` 中，但未在 `src/main/java/GuZhenRen/cards` 下检测到对应类定义，可能是旧内容、已删除实现或在其他位置生成。",
                "",
            ]
        )
        for name in missing_entries:
            lines.append(f"- {name}")
        lines.append("")

    OUT_PATH.write_text("\n".join(lines))
    print(f"generated {OUT_PATH}")
    print(f"entries={len(entries)} shazhao={len(shazhao_entries)} missing={len(missing_entries)}")


if __name__ == "__main__":
    main()
