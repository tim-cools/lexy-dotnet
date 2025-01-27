# Setup

## lexy-typescript dependency
At the moment the lexy-compiler is added as local npm package. Run 'yarn update-lexy' to update the package from source. 
Ensure lexy-typescript compiles correctly before running the command. Repositories: lexy-typescript and lexy-editor should be in the same parent folder for the command to work.

## Run locally

Run editor locally
`yarn start`


## Known Todo's

- [ ] Publish lexy-typescript as npm package 'lexy-compiler' and include it from the npm repository
- [ ] Check how state management can be improved. It feels a bit messy.
- [ ] Develop a backend (preferably in node.js or dotnet) so it can run locally as an editor and publicly as a playground application.
