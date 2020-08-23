# CMS50D+ CLI

```
Usage:
  CMS50DPlusCli [options] [command]

Options:
  --version         Show version information
  -?, -h, --help    Show help and usage information

Commands:
  ls-devices               List serial devices
  stream <devicePath>      Stream live data to stdout
  download <devicePath>    Download recorded data. This command may require prior device restart.
```

# Getting started

If you wish to manage dependencies yourself, just clone this repository and run `dotnet run -- --help` in the project root.

Otherwise install nix[[0]](https://nixos.org/) and run `nix-shell`[[1]](https://stackoverflow.com/a/44118856) in the project root.

# Prior art

[airikka/spo2cms50dplus](https://github.com/airikka/spo2cms50dplus)

[atbrask/CMS50Dplus](https://github.com/atbrask/CMS50Dplus) [(blogpost)](https://www.atbrask.dk/?p=244)

[jgerschler/CMS50DPlus-PulseOx](https://github.com/jgerschler/CMS50DPlus-PulseOx)

[firmware v4.6 protocol](https://gist.github.com/patrick-samy/df33e296885364f602f0c27f1eb139a8)
