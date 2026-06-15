## ADDED Requirements

### Requirement: Repository structure

The repository SHALL be structured as a valid pi package containing exactly one extension file.

#### Scenario: Repository contains required files

- **WHEN** the repository is cloned
- **THEN** it SHALL contain `package.json`, `README.md`, `LICENSE`, `.gitignore`, and an `extensions/` directory with `template-model-switch.ts`

### Requirement: Package manifest

The `package.json` SHALL declare the extension directory via the `pi` key and include the `pi-package` and `pi-extension` keywords.

#### Scenario: Package manifest is valid

- **WHEN** pi installs the package via `pi install git:github.com/ypyl/pi-template-model-switch`
- **THEN** pi SHALL discover and load the extension from the `extensions/` directory

#### Scenario: Package is discoverable on npm

- **WHEN** a user searches npm for `pi-package` keywords
- **THEN** this package SHALL appear in search results

### Requirement: Extension file

The extension file SHALL be the existing, working `template-model-switch.ts` from `~/.pi/agent/extensions/`, copied verbatim without modification.

#### Scenario: Extension loads successfully

- **WHEN** pi starts with the package installed
- **THEN** the extension SHALL register its `input`, `before_agent_start`, and `agent_end` event handlers without errors

#### Scenario: Extension switches model on template invocation

- **WHEN** a user invokes a prompt template with `model: anthropic/claude-sonnet-4-20250514` in its YAML frontmatter
- **THEN** the extension SHALL switch pi to that model for the duration of that single prompt execution
- **AND** restore the original model after the prompt completes

### Requirement: README documentation

The README SHALL document what the extension does, how to install it, how to use the `model:` frontmatter field, and provide a concrete example.

#### Scenario: User understands how to install

- **WHEN** a user reads the README
- **THEN** they SHALL find a one-line `pi install` command for git installation

#### Scenario: User understands how to use it

- **WHEN** a user reads the README
- **THEN** they SHALL find the `model:` frontmatter format (`provider/model-id`) and a complete example prompt template

### Requirement: License

The repository SHALL be licensed under MIT.

#### Scenario: License file present

- **WHEN** the repository is viewed on GitHub
- **THEN** a `LICENSE` file with the MIT license text SHALL be present
