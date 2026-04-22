# Questionable JSON Builder

Questionable JSON Builder is a Dalamud plugin for Final Fantasy XIV that helps contributors create **Questionable-compatible quest JSON files** without manually writing the JSON by hand.

It loads quest data from the game, compares it against the Questionable repository, shows which quests are already implemented, and guides the user through building valid quest steps and sequences.

## Features

- Quest search and selection from in-game data
- Implemented vs missing quest detection
- Guided step builder for common quest actions
- Auto-generated Sequence `0` and `255`
- JSON preview before export
- Export to a user-selected directory
- Fixed filename generation for Questionable compatibility
- Help window and contributor-focused UI guidance

## Supported step types

The builder supports common Questionable workflow steps such as:

- Talk / interact
- Hand over
- Use item
- Say / chat message
- Emote
- Action
- Walk to location
- Combat
- Craft
- Gather
- Switch class
- Manual progress / instruction steps

## Important notes

- The plugin is designed to keep the exported filename in the correct format for Questionable.
- Only change the **export directory**, not the generated filename.
- Sequence `0` and `255` are created automatically.
- Some advanced Questionable fields are exposed as structured or JSON-backed inputs for compatibility.
- If you need help or encounter any errors please reach out on the official Discord: https://puni.sh/

## Credits

- Created by epinephren.
- Built for contributors working with the Questionable project.