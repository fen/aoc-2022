if (args is not [var file] || !System.IO.File.Exists(file)) {
    Console.WriteLine("day7 <input>");
    return;
}

var root = await ParseCommandLineAsync(file);
Console.WriteLine(
    "Part One: {0}",
    root.GetAllSubDirectories()
        .Where(d => d.GetSize() <= 100000)
        .Sum(d => d.GetSize())
);

var freeSpace = 70000000 - root.GetSize();
var neededSpace = 30000000 - freeSpace;
Console.WriteLine(
    "Part Two: {0}",
    root.GetAllSubDirectories()
        .Where(d => d.GetSize() >= neededSpace)
        .Min(d => d.GetSize())
);

async Task<Directory> ParseCommandLineAsync(string file)
{
    var lines = (await System.IO.File.ReadAllLinesAsync(file)).ToList();

    Directory? currentDirectory = null;
    for (var i = 0; i < lines.Count; i++) {
        var line = lines[i];
        var split = line.Split(' ');

        switch (split[1]) {
        case "cd":
            ChangeDirectory(split, ref currentDirectory);
            break;
        case "ls":
            ListFiles(ref i, lines, currentDirectory);
            break;
        }
    }

    return currentDirectory!.GetRoot();

    static void ChangeDirectory(string[] strings, ref Directory? directory)
    {
        if (strings[2] == "..") {
            directory = directory?.Parent;
        } else {
            var tmp = directory;
            directory = new(strings[2], directory, new(), new());
            tmp?.SubDirectories?.Add(directory);
        }
    }

    static void ListFiles(ref int i, List<string> lines, Directory? directory)
    {
        i += 1;
        for (; i < lines.Count; i++) {
            var line = lines[i];
            if (line[0] == '$') {
                i--;
                break;
            }

            var split = line.Split(' ');
            if (split[0] == "dir") {
                continue;
            }

            var (size, name) = split;
            directory?.Files?.Add(new(name, int.Parse(size)));
        }
    }
}

record Directory(string Name, Directory? Parent, List<Directory> SubDirectories, List<File> Files)
{
    public Directory GetRoot()
    {
        var dir = this;
        while (dir.Parent is not null) {
            dir = dir.Parent;
        }

        return dir;
    }

    public IEnumerable<Directory> GetAllSubDirectories()
    {
        foreach (var subDirectory in SubDirectories) {
            yield return subDirectory;
            foreach (var directory in subDirectory.GetAllSubDirectories()) {
                yield return directory;
            }
        }
    }

    public int GetSize() => Files.Sum(f => f.Size) + SubDirectories.Sum(d => d.GetSize());
}

record File(string Name, int Size);