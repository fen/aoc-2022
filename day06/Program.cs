if (args is not [var file] || !File.Exists(file)) {
    Console.WriteLine("day6 <input>");
    return;
}

var lines = await File.ReadAllLinesAsync(file);
foreach (var line in lines) {
    Console.WriteLine("Part One: {0}", FindMarker(line));
    Console.WriteLine("Part Two: {0}", FindMarker(line, 14));
}

int FindMarker(string line, int size = 4)
{
    for (int i = 0; i < line.Length; i++) {
        var possibleMarker = line[i..(i + size)];
        if (IsMarker(possibleMarker)) {
            return i + size;
        }
    }

    return default;

    bool IsMarker(string marker)
    {
        for (var i = 0; i < marker.Length; i++) {
            for (var j = i + 1; j < marker.Length; j++) {
                if (marker[i] == marker[j]) {
                    return false;
                }
            }
        }

        return true;
    }
}