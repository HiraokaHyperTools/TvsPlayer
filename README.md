# TvsPlayer

Quick links: [Online demo (WebAssembly)](https://hiraokahypertools.github.io/TvsPlayer/)

A sample representation of a screen frame player, for a `.tvs` session file recorded by the TeamViewer remote support and control software.

This is done by reverse engineering of the file format, and is not based on any official documentation. The implementation is a work in progress, and may not be complete or accurate.

Notes: data is basically formatted in little-endian (Intel byte order) based.

## Birds eye of a TVS file structe

```
┌ Header
├┬ KEY blobs (zlib compressed)
│└┬ KEY records (has a timestamp, and a record type)
│ └─ KEY tags (has a tag byte, and a blob)
└ Footer
```

## Some record types

| RecordType | Usage |
|------------|-------|
| `0x1C01` | Session initialization?
| `0x0219` | ?
| `0x0402` | Fills
| `0x0502` | RLEs
| `0x0602` | JPEGs
| `0x0702` | Key frame?
| `0x070C` | ?
| `0x0802` | Screen update?

## Some tag byte types

The interpretation of the tag byte seems to depend on the record type.

### RecordType `0x1C01` tags

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 4 | Screen width (LE int32. e.g., `0x780` = 1920) |
| `0x01` | 4 | Screen height (LE int32. e.g., `0x438` = 1080) |
| `0x02` | 4 | Color depth indicator? (LE int32. `8` = 16bit palette mode, `32` = 24bit BGR mode. Linked with Tag 0x06) |
| `0x03` | 4 | ? |
| `0x04` | 1 | ? |
| `0x05` | 4 | ? |
| `0x06` | 4 | Palette entry size specification? `0x3C`=2 bytes/16bit, `0x50`=3 bytes/24bit |
| `0x07` | 4 | ? |
| `0x09` | 4 | ? |
| `0x0A` | 4 | ? |
| `0x0C` | 4 | Tile size? (LE int32. `64` = 64 × 64 pixels) |
| `0x0D` | 8 | ? |
| `0x10` | 4 | ? |
| `0x11` | 1 | Version flag? (value `0x01`) |
| `0x12` | 1 | ? |
| `0x15` | 1 | ? |
| `0x16` | 1 | ? |
| `0x18` | 34 | Screen information string? (**UTF-16 LE** encoding, null-terminated. e.g., `"1920X1080X32X32;\0"`) |
| `0x1B` | 4 | ? |
| `0x1C` | 1 | ? |
| `0x1D` | 8 | 8-byte structure (`04 00 00 00 5C 00 30 00`, meaning unknown) |
| `0x1E` | 1 | ? |
| `0x1F` | 4 | ? |
| `0xF6` | 4 | Protocol version identifier? (`0x13` = 19) |
| `0xF8` | 8 | Session identifier? |
| `0xFC` | 4 | Session-specific hash value? |
| `0xFE` | 1 | Flag value? |
| `0xFF` | 4 | Checksum? / hash value? |

### RecordType `0x0219` tags

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 1 | ? |
| `0xFE` | 1 | Flag value? |

### RecordType `0x0402` tags

### RecordType `0x0502` tags

### RecordType `0x0602` tags

### RecordType `0x070C` tags

| Tag | Len | What |
|-----|-----|------|
| `0x06` | 4 | ? |
| `0x24` | 8 | ? |
| `0xF6` | 4 | ? |
| `0xF8` | 8 | ? |
| `0xFC` | 4 | ? |
| `0xFE` | 1 | Flag value? |
| `0xFF` | 4 | ? |

## RecordType 0x??02 tags

These RecordTypes seem to be screen update records.

Check the single byte of a tag 0x00.
It helps to decide an image format.

| DataType | Description |
|----------|-------------|
| 0x09 | RLE09 |
| 0x0A | Block fill |
| 0x0B | 1-bpp bitmaps |
| 0x0C | RLE0C |
| 0x0D | JPEG |

### RLE09

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 1 | DataType 0x09 |
| `0x01` | 16 | RECT (x, y, cx, cy) |
| `0x05` | (varlen) | RLE09 blob |
| `0xFE` | 1 | Flag value? |

### Block fill

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 1 | DataType 0x0A |
| `0x19` | 2 | The sequential block index |
| `0x1C` | 2 or 3 | Fill color |
| `0xFE` | 1 | Flag value? |

### 1-bpp bitmaps

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 1 | DataType 0x0B |
| `0x19` | 2 | The sequential block index |
| `0x1B` | absent, 1 or more | Tiles configuration in bit masks: 0 skip, 1 draw |
| `0x1C` | (varlen) | 2 colors palette data (incremental palette) |
| `0x1D` | (varlen) | Packed 1-bpp bitmaps, LSB first |
| `0xFE` | 1 | Flag value? |

### RLE0C

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 1 | DataType 0x0C |
| `0x19` | 2 | The sequential block index |
| `0x1B` | absent, 1 or more | Tiles configuration in bit masks: 0 skip, 1 draw |
| `0x1C` | (varlen) | Palette data (incremental palette) |
| `0x1D` | (varlen) | Packed RLE0C blobs |
| `0xFE` | 1 | Flag value? |

### JPEG

| Tag | Len | What |
|-----|-----|------|
| `0x00` | 1 | DataType 0x0D |
| `0x11` | (varlen) | The JPEG header part |
| `0x12` | 4 | ? |
| `0x19` | 2 | The sequential block index |
| `0x1D` | (varlen) | The JPEG data blobs for appending |
| `0xFE` | 1 | Flag value? |
