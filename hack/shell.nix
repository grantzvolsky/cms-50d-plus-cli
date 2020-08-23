{ pkgs ? import <nixpkgs> {}
, name ? "wx-env"
}: with pkgs;

let
  my-pkgs = python-packages: with python-packages; [
    pyserial
    pylint
  ]; 
  my-python = python3.withPackages my-pkgs;
in pkgs.mkShell {
  inherit name;
  nativeBuildInputs = [ # these are executed on the build machine
    my-python
    less
  ];

  buildInputs = []; # these are cross-compiled and executed on the host machine
}
