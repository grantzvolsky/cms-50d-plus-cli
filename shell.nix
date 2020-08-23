# Currently there is no easy way to build .NET with nix[0] so let's just use a shell
# [0]: https://sgt.hootr.club/molten-matter/dotnet-on-nix/

{ pkgs ? import <nixpkgs> {}
, name ? "cms-50d-plus-cli"
}: with pkgs;

pkgs.mkShell {
  inherit name;

  DOTNET_ROOT=pkgs.dotnet-sdk_3; # down the line mkDerivation exports its attrset, including this attribute, as env variables

  nativeBuildInputs = [
    dotnet-sdk_3
  ];

  shellHook = ''
    dotnet publish -c Release -o out
  '';
}
