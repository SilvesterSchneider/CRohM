#!/bin/sh

# Change into tests folder
cd tests || exit

# Install npm dependencies
npm install

# Execute tests
npm test

# Return into main folder
cd .. || exit
